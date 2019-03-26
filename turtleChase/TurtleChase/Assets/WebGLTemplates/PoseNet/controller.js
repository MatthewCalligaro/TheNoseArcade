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

let jumpCooldown;

// Benchmark variables
let ticks;
let start;

// Canvas size
let vidWidth = 320;
let vidHeight = 240;

// Set defaults
let mode = "active";
let delay = 7;
let threshold = vidHeight / 2;
let range = [vidHeight / 12, vidHeight / 3];
let constantMag = true;

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

    jumpCooldown = 0;

    // Benchmarking code
    ticks = 0;
    start = Date.now();
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
    jumpCooldown = max(0, jumpCooldown - 1);
    let trigger = 0; // By default don't register movement
    if(poses.length > 0) {
        // Only detect nose keypoint of first pose
        let nose = poses[0].pose.keypoints[0];
        noseX = nose.position.x;
        noseY = nose.position.y;

        switch(mode) {
            case "active":
                trigger = pNoseY > threshold != noseY > threshold;
                on = noseY < threshold;
                break;
            case "velocity":
                rawMagnitude = (constantMag ? 1 : pNoseY - noseY);
                trigger = (pNoseY - noseY) > range[0]; 
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

    if (jumpCooldown == 0 && trigger) {
        jumpCooldown = delay; // Reset jump cooldown
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
                console.log(range);
                scaledMagnitude = rawMagnitude / (range[1] - range[0]);
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
        pg.line(0, threshold, vidWidth, threshold);
    }
}