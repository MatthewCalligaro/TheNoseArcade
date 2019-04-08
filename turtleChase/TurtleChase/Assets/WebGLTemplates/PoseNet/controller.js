let video;
let poseNet;
let poses = [];

let pg;

let noseX;
let noseY;

let pNoseX;
let pNoseY;

let scaledMagnitude;
let rawMagnitude;

let lastJump;
let thisDetect;
let lastDetect;

// Benchmark variables
let ticks;
let start;

// Canvas size
let vidWidth = 320;
let vidHeight = 240;

// Set defaults
let mode = "active";
let delay = 500; // ms
let threshold = vidHeight / 2; // Inverted! NOT measured in canvas coordinates. 
let velocityMin = 150; // px / s
let velocityScalar = vidHeight / 3; // Currently not controllable by user.
let magScaling = "constant";

// Function that p5 calls initially to set up graphics
function setup() {
    video = createCapture(VIDEO);
    video.size(vidWidth, vidHeight);
    video.parent('videoContainer')

    pixelDensity(1);
    pg = createGraphics(vidWidth, vidHeight);
    pg.parent('videoContainer');

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
        // console.log(poses);
        // ticks++;
        // console.log(ticks/(Date.now()-start));
    });

    // Hide the video so we can render it flipped in the draw loop. 
    video.hide();

    // Show graphics
    pg.show();
    // Flip graphics so you get proper mirroring of video and nose dot
    pg.translate(vidWidth,0);
    pg.scale(-1.0, 1.0);

    // Benchmarking code
    ticks = 0;
    start = Date.now();

    lastJump = Date.now();
    thisDetect = Date.now();
}

// Function that p5 calls repeatedly to render graphics
function draw() {
    pg.clear();

    // Render video
    pg.image(video, 0, 0);

    findNose();
    updateThreshold();
}

function findNose() {
    let trigger = 0; // By default don't register movement
    if(poses.length > 0) {
        lastDetect = thisDetect;
        thisDetect = Date.now();
        // Only detect nose keypoint of first pose
        let nose = poses[0].pose.keypoints[0];
        noseX = nose.position.x;
        noseY = nose.position.y;

        switch(mode) {
            case "active":
                trigger = pNoseY > (vidHeight - threshold) != noseY > (vidHeight - threshold);
                on = noseY < (vidHeight - threshold);
                break;
            case "velocity":
                rawMagnitude = (pNoseY - noseY) / (thisDetect - lastDetect);
                trigger = rawMagnitude > velocityMin / 1000.0; 
                // console.log(rawMagnitude + " " + velocityMin / 1000.0 + " " + trigger);
                break;
            default:
        }

        pNoseX = noseX;
        pNoseY = noseY;

        // Render nose dot
        pg.stroke(0, 225, 0);
        pg.strokeWeight(5);
        pg.ellipse(noseX, noseY, 1, 1);

        // Benchmarking code
        // ticks++;
        // console.log(ticks/(Date.now()-start));
    }

    if (Date.now() - lastJump > delay && trigger) {
        lastJump = Date.now();
        switch(mode) {
            case "active":
                if(on) {
                    console.log("ACTIVE: JumpEnter")
                    try {
                        gameInstance.SendMessage("Player", "JumpEnter", 1);
                        console.log("Jump succeeded.");
                    }
                    catch(err) {
                        console.log("Jump failed with error: "+err);
                    }
                }
                else {
                    console.log("ACTIVE: JumpExit");
                    try {
                        gameInstance.SendMessage("Player", "JumpExit");
                        console.log("Jump succeeded.");
                    }
                    catch(err) {
                        console.log("Jump failed with error: "+err);
                    }
                }
                break;
            case "velocity":
                // Scale magnitude to velocity range
                scaledMagnitude = (magScaling == "constant" ? 1 : rawMagnitude / (velocityScalar - velocityMin)); 
                console.log("VELOCITY: JumpEnter ("+scaledMagnitude+")");
                try {
                    gameInstance.SendMessage("Player", "JumpEnter", scaledMagnitude);
                    console.log("Jump succeeded.");
                }
                catch(err) {
                    console.log("Jump failed with error: "+err);
                }
                break;
            default:
        }
    }
}

// Visually update the threshold value
function updateThreshold() {
    // Only render line in active mode. 
    if(mode == "active") { 
        pg.stroke(230, 80, 0);
        pg.strokeWeight(1);
        pg.line(0, vidHeight - threshold, vidWidth, vidHeight - threshold);
    }
}