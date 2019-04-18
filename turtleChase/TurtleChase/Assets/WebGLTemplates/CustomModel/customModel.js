var imported = document.createElement('script');
document.head.appendChild(imported);

// Model and output
var model;

// Evaluating
var interval;
var start = 0;
var ticks = 0;

// Set up the webcam
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
    process_video();
  }, 1);
};


/**
 * Using the video stream get an image and then record the nose X and Y positions and 
 * passes them to the game.
 * @callback sendCoords(noseX,noseY)
 */
function process_video() {
  // Create the array
  const image = tf.browser.fromPixels(webcamElement);  // for example
  const img = image.reshape([1, 160, 160, 3]);

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

  // Attempt to send packed coordinates to the game
  try {
    gameInstance.SendMessage('Controller', 'UpdateFacePosition', packedCoords);
    console.log('Success - Coordinates ' + fixedNoseX + ', ' + fixedNoseY + ' sent successfully.');
  } catch (err) {
    console.log('Failure - Coordinates ' + fixedNoseX + ', ' + fixedNoseY + ' failed to send: ' + err);
  }
}