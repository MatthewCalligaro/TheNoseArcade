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

// Benchmark variables
let ticks;
let start;

// Canvas size
let vidWidth = 640;
let vidHeight = 480;

// Set defaults
let mode = "active";
let delay = 7;
let threshold = vidHeight / 2;
let sensitivity = vidHeight / 12;

// Function that p5 calls initially to set up graphics
function setup() {
  // let canvas = createCanvas(vidWidth, vidHeight);
  // canvas.parent('videoContainer');
  video = createCapture(VIDEO);
  video.size(vidWidth, vidHeight);
  // video.size(width, height);
  // video.parent('videoContainer')

  pixelDensity(1);
  pg = createGraphics(vidWidth, vidHeight);
  // pg = createGraphics(width, height);
  pg.parent('videoContainer');

  let options = { 
    // imageScaleFactor: 0.3,
    // outputStride: 16,
    flipHorizontal: true,
    // flipHorizontal: false,
    minConfidence: 0.2,
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

  // Hide the video so we can render it flipped in the draw loop. 
  video.hide();

  // Show graphics
  pg.show();

  jumpCooldown = 0;

  // Benchmarking code
  ticks = 0;
  start = Date.now();
}

// Function that p5 calls repeatedly to render graphics
function draw() {
  pg.clear();

  // Reverse and render video
  // translate(capture.width,0);
  // scale(-1.0,1.0);
  // pg.image(video, 0, 0);
  // scale(1.0,-1.0);

  // push();
  pg.translate(vidWidth,0);
  pg.scale(-1.0, 1.0);
  pg.image(video, 0, 0);
  // pop();

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
          console.log("active: JumpEnter")
          // gameInstance.SendMessage("Player", "JumpEnter", 1);
        }
        else {
          console.log("active: JumpExit")
          // gameInstance.SendMessage("Player", "JumpExit");
        }
        break;
      case "velocity":
        console.log("velocity: JumpEnter");
        // gameInstance.SendMessage("Player", "JumpEnter", 1);
        break;
      case "magnitude":
        console.log("magnitude: JumpEnter ("+magnitude+")");
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
    pg.line(0, threshold, vidWidth, threshold);
  }
}