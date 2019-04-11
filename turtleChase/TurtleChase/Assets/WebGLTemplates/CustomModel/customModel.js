var imported = document.createElement('script');
document.head.appendChild(imported);

// Model and output
var model;
let noseX;
let noseY;

// Evaluating
var interval;
var start = 0;
var ticks = 0;

let overlay;
let video;

let vidWidth = 160;
let vidHeight = 160;

// Set up the webcam
// let webcamElement = document.querySelector('#videoContainer > video');
const webcamElement = document.getElementById('webcam');
async function setupWebcam() {
  return new Promise((resolve, reject) => {
    const navigatorAny = navigator;
    navigator.getUserMedia = navigator.getUserMedia ||
      navigatorAny.webkitGetUserMedia || navigatorAny.mozGetUserMedia ||
      navigatorAny.msGetUserMedia;
    if (navigator.getUserMedia) {
      navigator.getUserMedia({video: true},
      stream => {
        webcamElement.srcObject = stream;
        webcamElement.addEventListener('loadeddata',  () => resolve(), false);
      },
      error => reject());
    } else {
      reject();
    }
  });
}

// Start the loading process
imported.src = 'https://cdn.jsdelivr.net/npm/@tensorflow/tfjs@1.0.1';

imported.onload = async function(){
  // Set up
  await setupWebcam();
  model = await tf.loadLayersModel('https://matthewcalligaro.github.io/TheNoseArcade/playTurtleChase/custom/model.json');
  // model = await tf.loadLayersModel('https://giselleserate.github.io/nosearcade-sandbox/playTurtleChase/custom/model.json');

  // Process the video
  interval = window.setInterval(function () {
    processVideo();
  }, 1);
};

function processVideo() {
  // Create the array
  const image = tf.browser.fromPixels(webcamElement);  // for example
  const img = image.reshape([1, vidWidth, vidHeight, 3]);

  // Predict
  const prediction = model.predict(img);

  // Record the result
  prediction.array().then(function(result) {
    noseX = result[0][1];
    noseY = result[0][0];

    sendCoords(noseX, noseY);
  });
}

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