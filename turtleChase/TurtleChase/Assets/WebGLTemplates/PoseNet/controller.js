let video;
let poseNet;
let poses = [];

let pg;

let noseX;
let noseY;

let pNoseX;
let pNoseY;

let magnitude;

let jumpCooldown;
let threshold;
let mode;

let ticks;
let start;

// Function that p5 calls initially to set up graphics
function setup() {
  let canvas = createCanvas(640, 480);
  canvas.parent('videoContainer');
  video = createCapture(VIDEO);
  video.size(width, height);
  video.parent('videoContainer')

  pixelDensity(1);
  pg = createGraphics(width, height);
  pg.parent('videoContainer');

  let options = { 
    // imageScaleFactor: 0.3,
    // outputStride: 16,
    flipHorizontal: false,
    minConfidence: 0,
    maxPoseDetections: 1,
    // scoreThreshold: 0.5,
    scoreThreshold: 2,
    nmsRadius: 20,
    detectionType: 'single',
    // multiplier: 0.75,
  }
  // Create a new poseNet method with a single detection
  poseNet = ml5.poseNet(video, options, function() {});

  poseNet.on('pose', function(results) {
    poses = results;
    // console.log(poses);
    // ticks++;
    // console.log(ticks/(Date.now()-start));
  });

  // Show graphics
  pg.show();

  jumpCooldown = 0;

  // Set defaults
  mode = "active";
  delay = 7;
  threshold = 150;
  sensitivity = 40;

  // Benchmarking code
  ticks = 0;
  start = Date.now();
}

// Function that p5 calls repeatedly to render graphics
function draw() {
  pg.clear();
  findNose();
  updateThreshold();
}

function findNose() {
  jumpCooldown = max(0, jumpCooldown - 1);
  let trigger = 0; // By default don't register movement
  if(poses.length > 0) {
    // Only detect nose keypoint of first pose
    let nose = poses[0].pose.keypoints[0];
    noseX = nose.position.x;
    noseY = nose.position.y;

    switch(mode) {
      case "active":
        trigger = pNoseY > threshold != noseY > threshold;
        on = noseY < threshold;
        break;
      case "magnitude":
        magnitude = pNoseY - noseY; // Drop through
      case "velocity":
        trigger = (pNoseY - noseY) > sensitivity; 
        break;
      default:
    }

    pNoseX = noseX;
    pNoseY = noseY;

    // Render nose dot
    pg.stroke(0, 225, 0);
    pg.strokeWeight(5);
    pg.ellipse(noseX, noseY, 1, 1);

    // Benchmarking code
    // ticks++;
    // console.log(ticks/(Date.now()-start));
  }

  if (jumpCooldown == 0 && trigger) {
    jumpCooldown = delay; // Reset jump cooldown
    switch(mode) {
      case "active":
        if(on) {
          console.log("JUMP (discrete)")
          // gameInstance.SendMessage("Player", "JumpEnter");
        }
        else {
          console.log("JUMP (off)")
          // gameInstance.SendMessage("Player", "JumpExit");
        }
        break;
      case "velocity":
        console.log("JUMP (discrete)");
        // gameInstance.SendMessage("Player", "JumpEnter");
        break;
      case "magnitude":
        console.log("JUMP (magnitude "+magnitude+")");
        // gameInstance.SendMessage("Player", "JumpEnter", magnitude);
        break;
      default:
    }
  }
}

// Visually update the threshold value
function updateThreshold() {
  // Only render line in active mode. 
  if(mode == "active") { 
    pg.stroke(230, 80, 0);
    pg.strokeWeight(1);
    pg.line(0, threshold, width, threshold);
  }
}