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

let balloon;
let jumpCooldown;

function setup() {
  createCanvas(640, 480);
  video = createCapture(VIDEO);
  video.size(width, height);

  // pixelDensity(1);
  // pg = createGraphics(width, height);

  // Create a new poseNet method with a single detection
  poseNet = ml5.poseNet(video, function() {});

  poseNet.on('pose', function(results) {
    poses = results;
  });

  // Hide the video element, and just show the canvas
  video.hide();
  // balloon = 200;
  jumpCooldown = 0;
}

// function draw() {
//   // image(video, 0, 0, width, height);

//   // image(pg, 0, 0, width, height);

//   // drawBalloon();
//   findNose();
  
// 	// Update progressively
//   // balloon = min(500, balloon + 3);
// }

// function drawBalloon() {
//   ellipse(200, balloon, 20, 20);
// }

// A function to find the nose
function findNose() {
  jumpCooldown = max(0, jumpCooldown - 1);
  var yDiff = 0; // By default don't register movement
  // Loop through all the poses detected
  for (let i = 0; i < poses.length; i++) {
    // For each pose detected, look at nose keypoint
    let keypoint = poses[i].pose.keypoints[0];
    // Only recognize if nose probability is bigger than 0.2
    if (keypoint.score > 0.2) {
      noseX = keypoint.position.x;
      noseY = keypoint.position.y;

      yDiff = pNoseY - noseY;

      // pg.stroke(230, 80, 0);
      // pg.strokeWeight(1);
      // pg.line(noseX, noseY, pNoseX, pNoseY);

      pNoseX = noseX;
      pNoseY = noseY;
    }
  }

  if (jumpCooldown == 0 && yDiff > 10) {
  	jumpCooldown = 15; // Reset jump cooldown
  	console.log("JUMP");
    gameInstance.SendMessage("Player", "jump");
  }
}

// function jump() {
//   // triangle(30, 75, 58, 20, 86, 75); // Visual feedback hackiness. 
//   // balloon -= 100;
// }

function registerNoseFinder() {
	setInterval(function() { findNose() }, 0);
}
