let video = document.getElementById('videoInput');
let cap = new cv.VideoCapture(video);

// take first frame of the video
let frame = new cv.Mat(video.height, video.width, cv.CV_8UC4);
cap.read(frame);

// hardcode the initial location of window
let trackWindow = new cv.Rect(150, 60, 63, 125);

// Setup the termination criteria, either 10 iteration or move by atleast 1 pt
let termCrit = new cv.TermCriteria(cv.TERM_CRITERIA_EPS | cv.TERM_CRITERIA_COUNT, 10, 1);

function process_video() {
    try {
        if (!streaming) {
            // clean and stop.
            frame.delete();
            return;
        }
        let begin = Date.now();

        // start processing.
        cap.read(frame);

        xhttp.open("POST", "http://0.0.0.0:8000/predict", true);
        xhttp.send(frame);

        // Draw it on image
        let [x, y, w, h] = [trackWindow.x, trackWindow.y, trackWindow.width, trackWindow.height];
        cv.rectangle(frame, new cv.Point(x, y), new cv.Point(x+w, y+h), [255, 0, 0, 255], 2);
        cv.imshow('canvasOutput', frame);

    } catch (err) {
        utils.printError(err);
    }
};

// schedule the first one.
setTimeout(processVideo, 0);