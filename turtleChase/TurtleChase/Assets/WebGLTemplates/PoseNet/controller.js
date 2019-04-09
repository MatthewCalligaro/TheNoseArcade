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
  // Insert default canvas in specific container so it doesn't become space junk at the end of the DOM
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

// TODO um leave me alone??
function getNewCoords() {
  // Don't bother if there are no poses detected
  if (poses.length < 1) {
    return;
  }

  // Only detect nose keypoint of first pose.
  let nose = poses[0].pose.keypoints[0];
  let noseX = nose.position.x;
  let noseY = nose.position.y;
  // Convert to fixed 4 not-dec and 12 dec bits
  let fixedNoseX = parseInt(noseX * 10e12);
  let fixedNoseY = parseInt(noseY * 10e12);

  // Bitpack x into bits 0-15, y into 16-32
  let packedCoords = 0;
  packedCoords |= fixedNoseX;
  packedCoords |= fixedNoseY << 16;

  console.log('Recognized coords ' + fixedNoseX + ', ' + fixedNoseY + '.');
  // Attempt to send packed coordinates to the game
  try {
    gameInstance.SendMessage('Player', 'NewCoords', packedCoords);
    console.log('Success - Coordinates ' + packedCoords.toString(16) + ' sent successfully.');
  } catch (err) {
    console.log('Failure - Coordinates ' + packedCoords.toString(16) + ' failed to send: ' + err);
  } // TODO: Ok so I'm worried about what it's going to come out as on the other side. Write that ish. 
}
