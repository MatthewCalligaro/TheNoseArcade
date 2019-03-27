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
let delay = 200; // ms
let threshold = vidHeight / 2; // Inverted! NOT measured in canvas coordinates. 
let velocityMin = vidHeight / 12;
let velocityScalar = vidHeight / 3; // Currently not controllable by user.
let magScaling = "constant";
let inMenu = true;

// I didn't invert these because I'm not giving you sliders soz
let xArrowLims = [vidWidth / 4, 3 * vidWidth / 4];
let yArrowLims = [vidHeight / 4, 3 * vidHeight / 4];

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

    inMenu = $("#menuStatus").attr("menu-status");

    detectNose();
    updateVisuals();
}

function detectNose() {
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
                trigger = (pNoseY - noseY) / (thisDetect - lastDetect) > velocityMin; 
                break;
            default:
        }

        pNoseX = noseX;
        pNoseY = noseY;

        // Render nose dot
        pg.stroke(0, 225, 0); // Approximately Lime
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
function updateVisuals() {
    if(inMenu) {
        // pg.stroke(245, 60, 180); // Vague Pink
        pg.noStroke();
        pg.fill(245, 60, 180, 125); // Vague Pink
        // pg.strokeWeight(1);
        // Ensure that 0 index is the larger index
        xArrowLims.sort();
        yArrowLims.sort();
        console.log(xArrowLims)
        console.log(yArrowLims);
        // Up
        pg.rect(xArrowLims[0], 0, xArrowLims[1]-xArrowLims[0], yArrowLims[1]); 
        // Down
        pg.rect(xArrowLims[0], yArrowLims[0], xArrowLims[1]-xArrowLims[0], vidHeight); 
        // Right
        pg.rect(0, yArrowLims[0], xArrowLims[1], yArrowLims[1]-yArrowLims[0]);
        // Left
        pg.rect(xArrowLims[0], yArrowLims[0], vidWidth, yArrowLims[1]-yArrowLims[0]);
        // for(var i = 0; i < 2; i++) {
        //     // Render x limit
        //     pg.line(xArrowLims[i], 0, xArrowLims[i], vidHeight);
        //     // Render y limit
        //     pg.line(0, yArrowLims[i], vidWidth, yArrowLims[i]);
        // }
    }
    else {
        // Only render line in active mode. 
        if(mode == "active") { 
            pg.stroke(230, 80, 0); // Kinda Red
            pg.strokeWeight(1);
            pg.line(0, vidHeight - threshold, vidWidth, vidHeight - threshold);
        }
    }
}