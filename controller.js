// Copyright (c) 2018 ml5
//
// This software is released under the MIT License.
// https://opensource.org/licenses/MIT

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

// Function that p5 calls initially to set up graphics
function setup() {
  var canvas = createCanvas(640, 480);
  canvas.parent('videoContainer');
  video = createCapture(VIDEO);
  video.size(width, height);
  video.parent('videoContainer')

  pixelDensity(1);
  pg = createGraphics(width, height);
  pg.parent('videoContainer');

  // Create a new poseNet method with a single detection
  poseNet = ml5.poseNet(video, function() {});

  poseNet.on('pose', function(results) {
    poses = results;
  });

  // Show graphics
  pg.show();

  jumpCooldown = 0;

  // Set defaults
  mode = "active";
  delay = 7;
  threshold = 150;
  sensitivity = 40;
}

// Function that p5 calls repeatedly to render graphics
function draw() {
  findNose();
  updateThreshold();
}

function findNose() {
  jumpCooldown = max(0, jumpCooldown - 1);
  var trigger = 0; // By default don't register movement
  if(poses.length > 0) {
    // Only detect nose keypoint of first pose
    let nose = poses[0].pose.keypoints[0];
    // Only recognize if probability is bigger than 0.2
    if (nose.score > 0.2) {
      noseX = nose.position.x;
      noseY = nose.position.y;

      // Active zone
      if(mode == "active") {
        trigger = pNoseY > threshold && noseY < threshold;
      }
      // Difference
      else {
        console.log(pNoseY);
        console.log(noseY);
        console.log(sensitivity);
        trigger = (pNoseY - noseY) > sensitivity; 
        console.log(trigger);
        magnitude = pNoseY - noseY;
      }

      pNoseX = noseX;
      pNoseY = noseY;
    }
  }

  if (jumpCooldown == 0 && trigger) {
    jumpCooldown = delay; // Reset jump cooldown
    console.log("JUMP");
    gameInstance.SendMessage("Player", "JumpDiscrete");
    // TODO: Make sure this hook-in works properly; use Magnitude (but ignore depending on game settings) in velocity mode
    // if (mode == "active") {
    //   gameInstance.SendMessage("Player", "JumpDiscrete");
    // }
    // else {
    //   gameInstance.SendMessage("Player", "JumpWithMagnitude", magnitude);
    // }
  }
}

// Visually update the threshold value
function updateThreshold() {
  pg.clear();
  // Only render line in active mode. 
  if(mode == "active") { 
    pg.stroke(230, 80, 0);
    pg.strokeWeight(1);
    pg.line(0, threshold, width, threshold);
  }
}