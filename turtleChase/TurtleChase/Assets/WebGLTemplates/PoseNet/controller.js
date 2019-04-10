// Canvas size
let vidWidth = 320;
let vidHeight = 240;

// Display elements
let video;

let poseNet;
let poses = [];

// Recognized coordinates
let noseX;
let noseY;


/**
 * Function that p5 calls initially to set up graphics
 */
function setup() {
  // Insert default canvas in specific container so it doesn't become junk at the end of the DOM
  defaultCanvas = createCanvas(vidWidth, vidHeight);
  defaultCanvas.parent('videoContainer');
  
  // Webcam capture
  video = createCapture(VIDEO);
  video.size(vidWidth, vidHeight);
  video.parent('videoContainer')

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
  let noseX = nose.position.x;
  let noseY = nose.position.y;

  sendCoords(noseX, noseY);
}

/**
 * Send new nose coordinates to the game
 * @param x the x position of the nose
 * @param y the y position of the nose
 */
function sendCoords(x, y) {
  // Truncate to int
  let fixedNoseX = parseInt(x);
  let fixedNoseY = parseInt(y);

  // Bitpack x into bits 0-9, y into 10-19
  let packedCoords = 0;
  packedCoords |= fixedNoseX;
  packedCoords |= fixedNoseY << 10;

  // Bottom 9 bits get corrupted; move coords out of the way
  packedCoords = packedCoords << 9;

  // TODO: add this to the C# code in place of existing
  // int x = (packed >> 9) & 0x3FF;
  // int y = (packed >> 19) & 0x3FF;

  // Attempt to send packed coordinates to the game
  try {
    gameInstance.SendMessage('Player', 'UpdateFacePosition', packedCoords);
    console.log('Success - Coordinates ' + fixedNoseX + ', ' + fixedNoseY + ' sent successfully.');
  } catch (err) {
    console.log('Failure - Coordinates ' + fixedNoseX + ', ' + fixedNoseY + ' failed to send: ' + err);
  }
}
