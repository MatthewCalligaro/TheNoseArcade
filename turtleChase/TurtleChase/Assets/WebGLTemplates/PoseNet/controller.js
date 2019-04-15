// Canvas size
let vidWidth = 320;
let vidHeight = 240;

// Display elements
let video;
let overlay;

let poseNet;
let poses = [];

// Recognized coordinates
let noseX;
let noseY;


/**
 * Function that p5 calls initially to set up graphics
 */
function setup() {
  // Webcam capture
  video = createCapture(VIDEO);
  video.size(vidWidth, vidHeight);
  video.parent('videoContainer')

  // Graphics overlay for monitor annotations
  pixelDensity(1);
  overlay = createGraphics(vidWidth, vidHeight);
  overlay.parent('videoContainer');
  
  // Options for PoseNet
  let options = { 
    flipHorizontal: false,
    minConfidence: 0.2,
    maxPoseDetections: 1,
    scoreThreshold: 2,
    nmsRadius: 20,
    detectionType: 'single',
  }
  // Create a new poseNet method with a single detection
  poseNet = ml5.poseNet(video, options, function() {});

  poseNet.on('pose', function(results) {
    poses = results;
    getNewCoords();
  });

  // Hide the video so it doesn't render
  video.hide();

  // Show graphics
  overlay.show();
  // Flip graphics so you get proper mirroring of video and nose dot
  overlay.translate(vidWidth,0);
  overlay.scale(-1.0, 1.0);
}

/**
 * Function that p5 calls repeatedly to render graphics
 */
function draw() {
  overlay.clear();

  // Render video
  overlay.image(video, 0, 0);

  // Render nose dot
  overlay.stroke(0, 225, 0); // Green
  overlay.strokeWeight(5);
  overlay.ellipse(noseX, noseY, 1, 1);
}

/**
 * Get the coordinates of the nose from PoseNet's return
 */
function getNewCoords() {
  // Don't bother if there are no poses detected
  if (poses.length < 1) {
    return;
  }

  // Only detect nose keypoint of first pose.
  let nose = poses[0].pose.keypoints[0];
  noseX = nose.position.x;
  noseY = nose.position.y;

  sendCoords(noseX, noseY);
}

/**
 * Send new nose coordinates to the game
 * @param x the x position of the nose
 * @param y the y position of the nose
 */
function sendCoords(x, y) {
  // Truncate to int and invert both axes
  let fixedNoseX = parseInt(vidWidth - x);
  let fixedNoseY = parseInt(vidHeight - y);

  // Bitpack x into bits 0-9, y into 10-19
  let packedCoords = 0;
  packedCoords |= fixedNoseX;
  packedCoords |= fixedNoseY << 10;

  // Bottom 9 bits get corrupted; move coords out of the way
  packedCoords = packedCoords << 9;

  // Attempt to send packed coordinates to the game
  try {
    gameInstance.SendMessage('Controller', 'UpdateFacePosition', packedCoords);
    console.log('Success - Coordinates ' + fixedNoseX + ', ' + fixedNoseY + ' sent successfully.');
  } catch (err) {
    console.log('Failure - Coordinates ' + fixedNoseX + ', ' + fixedNoseY + ' failed to send: ' + err);
  }
}
