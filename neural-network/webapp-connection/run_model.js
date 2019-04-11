var imported = document.createElement('script');
document.head.appendChild(imported);

var model;

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
imported.src = "https://cdn.jsdelivr.net/npm/@tensorflow/tfjs@1.0.0";

imported.onload = async function(){
    // Once the script has loaded tf should be available
    // the rest of your program would need to be in this function
    // await setupWebcam();
    model = await tf.loadLayersModel('http://s000.tinyupload.com/download.php?file_id=67010513132490573297&t=6701051313249057329711571');
    process_video();
  };

function process_video() {
    // const example = tf.browser.fromPixels(webcamElement);  // for example
    // const prediction = model.predict(example);
    // console.log(prediction);
    console.log("Hello, world!")
};