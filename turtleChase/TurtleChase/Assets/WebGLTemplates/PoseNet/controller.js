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
let inMenu;

// Arrow limits not inverted. 
let xLimMin = vidWidth / 4;
let xLimMax = 3 * vidWidth / 4;
let yLimMin = vidHeight / 4;
let yLimMax = 3 * vidHeight / 4;
let limDists = {
    x: {
        min: 0,
        max: 0,
    },
    y: {
        min: 0,
        max: 0,
    },
}

let arrowRegion = null;
let lastArrowRegion = null;
let arrowDelay = 1000; // ms

let adjMouseX;
let adjMouseY;

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
    });

    // Hide the video so we can render it flipped in the draw loop. 
    video.hide();

    // Show graphics
    pg.show();
    // Flip graphics so you get proper mirroring of video and nose dot
    pg.translate(vidWidth,0);
    pg.scale(-1.0, 1.0);

    lastJump = Date.now();
    thisDetect = Date.now();
    lastArrowChange = Date.now();
}

// Function that p5 calls repeatedly to render graphics
function draw() {
    // Adjust mouse coords.
    adjMouseX = 3*vidWidth-mouseX;
    adjMouseY = mouseY+20;

    pg.clear();

    // Render video
    pg.image(video, 0, 0);

    inMenu = $("#menuStatus").attr("menu-status") == "true";

    detectNose();
    updateVisuals();
}

// Built-in p5 function that gets called on mouse drag. 
function mouseDragged() {
    let chooseMaxForX = false;
    let chooseMaxForY = false;

    // Calculate distances between mouse and limits. 
    limDists.x.min = Math.abs(adjMouseX - xLimMin);
    limDists.x.max = Math.abs(adjMouseX - xLimMax);
    limDists.y.min = Math.abs(adjMouseY - yLimMin);
    limDists.y.max = Math.abs(adjMouseY - yLimMax);

    // Decide whether to use min or max. 
    if(limDists.x.max < limDists.x.min) {
        chooseMaxForX = true;
    }
    if(limDists.y.max < limDists.y.min) {
        chooseMaxForY = true;
    }

    // Decide whether to use x or y. 
    let xDist = (chooseMaxForX ? limDists.x.max : limDists.x.min);
    let yDist = (chooseMaxForY ? limDists.y.max : limDists.y.min);

    if(xDist < yDist) {
        // Drag x. 
        if(chooseMaxForX) {
            // Handle lim-switching. 
            if(adjMouseX < xLimMin) {
                xLimMax = xLimMin;
                xLimMin = adjMouseX;
            }
            else {
                xLimMax = adjMouseX;
            }
        }
        else {
            // Handle lim-switching. 
            if(adjMouseX > xLimMax) {
                xLimMin = xLimMax;
                xLimMax = adjMouseX;
            }
            else {
                xLimMin = adjMouseX;
            }
        }
    }
    else {
        // Drag y. 
        if(chooseMaxForY) {
            // Handle lim-switching. 
            if(adjMouseY < yLimMin) {
                yLimMax = yLimMin;
                yLimMin = adjMouseY;
            }
            else {
                yLimMax = adjMouseY;
            }
        }
        else {
            // Handle lim-switching. 
            if(adjMouseY > yLimMax) {
                yLimMin = yLimMax;
                yLimMax = adjMouseY;
            }
            else {
                yLimMin = adjMouseY;
            }
        }
    }
}

// Return the int corresponding to the arrowRegion we're in.
function getRegion(x, y) {
    if(x < xLimMax && x > xLimMin && y < yLimMin) { // Up
        return 0;
    }
    else if(x < xLimMax && x > xLimMin && y > yLimMax) { // Down
        return 1;
    }
    else if(y < yLimMax && y > yLimMin && x < xLimMin) { // Left
        return 2;
    }
    else if(y < yLimMax && y > yLimMin && x > xLimMax) { // Right
        return 3;
    }
    else {
        return null;
    }
}

function detectNose() {
    // No poses detected, nothing to do. 
    if(poses.length <= 0) {
        return;
    }

    let trigger = 0; // By default don't register movement
    lastDetect = thisDetect;
    thisDetect = Date.now();
    // Only detect nose keypoint of first pose
    let nose = poses[0].pose.keypoints[0];
    noseX = nose.position.x;
    noseY = nose.position.y;

    // Decide what to do with nose position. 
    if(inMenu) {
        arrowRegion = getRegion(noseX, noseY);
        if(arrowRegion != lastArrowRegion) {
            lastArrowChange = Date.now();
            lastArrowRegion = arrowRegion; // Only need to update last region if they are different. 
        }
        else if(lastArrowRegion != null && Date.now() - lastArrowChange > arrowDelay) {
            // Send arrow key to game. 
            try {
                gameInstance.SendMessage("Menu", "ArrowKey", arrowRegion);
                console.log("Sent arrow key in direction "+arrowRegion);
            }
            catch(err) {
                console.log("Arrow key "+arrowRegion+" failed with error: "+err);
            }
            lastArrowChange = Date.now();
        }
    }
    else {
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
    }

    // Render nose dot
    pg.stroke(0, 225, 0); // Approximately Lime
    pg.strokeWeight(5);
    pg.ellipse(noseX, noseY, 1, 1);

    // Handle jump. 
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

    // Set latest values to old values. 
    pNoseX = noseX;
    pNoseY = noseY;
}

// Visually update the threshold value
function updateVisuals() {
    if(inMenu) {
        // Render arrow key areas.
        pg.noStroke();

        // Up
        pg.fill(255, 0, 0, 125); // Super Friggin Red
        pg.rect(xLimMax, 0, xLimMin-xLimMax, yLimMin); 
        // Down
        pg.fill(0, 255, 0, 125); // Super Friggin Green
        pg.rect(xLimMax, yLimMax, xLimMin-xLimMax, vidHeight); 
        // Right
        pg.fill(0, 0, 255, 125); // Super Friggin Blue
        pg.rect(0, yLimMax, xLimMin, yLimMin-yLimMax);
        // Left
        pg.fill(255, 0, 255, 125); // Super Friggin Not-Green
        pg.rect(xLimMax, yLimMax, vidWidth, yLimMin-yLimMax);
    }
    else {
        // Only render line in active mode. 
        if(mode == "active") { 
            pg.stroke(230, 80, 0); // Kinda Red
            pg.strokeWeight(1);
            pg.line(0, vidHeight - threshold, vidWidth, vidHeight - threshold);
        }
    }

    // Crosshairs the mouse. 
    pg.stroke(255, 255, 0); // Jarate
    pg.strokeWeight(1);
    pg.line(0, adjMouseY, vidWidth, adjMouseY);
    pg.line(adjMouseX, 0, adjMouseX, vidHeight);
}