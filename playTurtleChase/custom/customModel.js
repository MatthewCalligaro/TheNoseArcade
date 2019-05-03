let imported = document.createElement('script');
document.head.appendChild(imported);

// Model
let model;

// Evaluating
let interval;
let start = 0;
let ticks = 0;

let overlay;
let video;

let vidWidth = 320;
let vidHeight = 240;

// Nose coords (monitor frame)
let noseX;
let noseY;

// Bounding box overlay coords (monitor frame)
let boundX;
let boundY;
let boundWidth;
let boundHeight;

let src;
let cap;
let gray;
let face;
let classifier;

let xWeights = [0.013330983, 0.36554886, 0.005527861, -0.382274539, 1.01703989, 0.000242652];
let xBias = -1.34480293;
let yWeights = [0.44016716, 0.48229461, -0.22590793, 0.53768469, 0.01016914, -0.00998019];
let yBias = 2.07304674;

cv['onRuntimeInitialized']=()=>{
  // Create desired matricies
  src = new cv.Mat(webcamElement.height, webcamElement.width, cv.CV_8UC4);
  cap = new cv.VideoCapture(webcam); 
  gray = new cv.Mat();
  face = new cv.Mat();

  classifier = new cv.CascadeClassifier();  // initialize classifier
  let utils = new Utils('errorMessage'); //use utils class
  let faceCascadeFile = 'haarcascade_frontalface_default.xml'; // path to xml
  // use createFileFromUrl to "pre-build" the xml
  utils.createFileFromUrl(faceCascadeFile, faceCascadeFile, () => {
    classifier.load(faceCascadeFile); // in the callback, load the cascade from file 
  });
}

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
    processVideo();
  }, 1);
};

/**
 * Using the video stream get an image and then record the nose X and Y positions and 
 * passes them to the game.
 * @callback sendCoords(noseX,noseY)
 */
function processVideo() {
  // Capture the image as an OpenCV.js image
  cap.read(src);

  // Identify the face
  cv.cvtColor(src, gray, cv.COLOR_RGBA2GRAY, 0);
  
  // Initialize bounding box
  let faces = new cv.RectVector();

  // Detect faces
  let msize = new cv.Size(0, 0);
  classifier.detectMultiScale(gray, faces, 1.1, 3, 0, msize, msize);

  // If no faces detected, stop
  if (faces.size() == 0) {
    return;
  }

  let faceTransforms = faces.get(0);

  // Get region of interest
  let roiSrc = src.roi(faceTransforms);

  let dsize = new cv.Size(96, 96);
  cv.resize(roiSrc, face, dsize, 0, 0, cv.INTER_AREA);

  // Convert to ImageData
  let imgData = new ImageData(new Uint8ClampedArray(face.data),face.cols,face.rows);

  const image = tf.browser.fromPixels(imgData);
  const img = image.reshape([1, 96, 96, 3]);

  // Predict
  const prediction = model.predict(img);

  // Record the result
  prediction.array().then(function(result) {
    // Face keypoint coordinates in face frame
    let noseXInFace = math.dot(result[0], xWeights) + xBias;
    let noseYInFace = math.dot(result[0], yWeights) + yBias;

    // Nose coordinates
    noseX = ((noseXInFace * this.width / 96.0) + this.x) * vidWidth  / 240;
    noseY = ((noseYInFace * this.height / 96.0) + this.y) * vidHeight / 240;

    // Bounding box overlay coords
    boundX = this.x * vidWidth / 240;
    boundY = this.y * vidHeight / 240;
    boundWidth = this.width * vidWidth  / 240;
    boundHeight = this.height * vidHeight / 240;
  }.bind(faceTransforms));
}

/**
 * Function that p5 calls initially to set up graphics
 */
function setup() {
  // Webcam capture
  video = createCapture(VIDEO);
  video.size(vidWidth, vidHeight);
  video.parent('videoContainer');

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

  // Render bounding box
  overlay.stroke(255, 0, 0); // Red
  overlay.noFill();
  overlay.rect(boundX, boundY, boundWidth, boundHeight);

  // Render bounding box origin dot
  overlay.stroke(0, 0, 255); // Blue
  overlay.ellipse(boundX, boundY, 1, 1);

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