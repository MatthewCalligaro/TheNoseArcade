// Copyright (c) 2018 ml5
//
// This software is released under the MIT License.
// https://opensource.org/licenses/MIT

let video;
let poseNet;
let poses = [];

let noseX;
let noseY;

let pNoseX;
let pNoseY;

let jumpCooldown;

function setup() {
  createCanvas(640, 480);
  video = createCapture(VIDEO);
  video.size(width, height);

  // Create a new poseNet method with a single detection
  poseNet = ml5.poseNet(video, function() {});

  poseNet.on('pose', function(results) {
    poses = results;
  });

  video.hide();
  jumpCooldown = 0;
}

function findNose() {
  jumpCooldown = max(0, jumpCooldown - 1);
  var yDiff = 0; // By default don't register movement
  if(poses.length > 0) {
    // Only detect nose keypoint of first pose
    let nose = poses[0].pose.keypoints[0];
    // Only recognize if probability is bigger than 0.2
    if (nose.score > 0.2) {
      noseX = nose.position.x;
      noseY = nose.position.y;

      yDiff = pNoseY - noseY;

      pNoseX = noseX;
      pNoseY = noseY;
    }
  }

  if (jumpCooldown == 0 && yDiff > 20) {
    jumpCooldown = 7; // Reset jump cooldown
    console.log("JUMP");
    gameInstance.SendMessage("Player", "JumpDiscrete");
  }
}

function registerNoseFinder() {
  // Locate nose at max frequency possible
  setInterval(function() { findNose() }, 0);
}
