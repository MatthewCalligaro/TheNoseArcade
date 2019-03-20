let video;
let poseNet;
let poses = [];

let noseX;
let noseY;

let coords = new Array();

// Canvas size
let vidWidth = 320;
let vidHeight = 240;

// Function that p5 calls initially to set up graphics
function setup() {
    video = createCapture(VIDEO);
    video.size(vidWidth, vidHeight);

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
    });

    // Hide the video so we can render it flipped in the draw loop. 
    video.hide();
}

// Function that p5 calls repeatedly to render graphics
function draw() {
    findNose();
}

function findNose() {
    if(poses.length > 0) {
        // Only detect nose keypoint of first pose
        let nose = poses[0].pose.keypoints[0];
        noseX = nose.position.x;
        noseY = nose.position.y;

        console.log("sending X, Y: "+noseX+", "+noseY);

        coords.push(noseX);
        coords.push(noseY);

        gameInstance.SendMessage("Player", "setCoords", coords);

        // gameInstance.SendMessage("Player", "setX", noseX); // FIXME: Possibly can send as an array, as above. If not, send both messages.
        // gameInstance.SendMessage("Player", "setY", noseY);

        coords = [];
    }
}
