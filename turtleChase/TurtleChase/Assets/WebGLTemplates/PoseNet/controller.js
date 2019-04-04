const UP = 0;
const DOWN = 1;
const RIGHT = 2;
const LEFT = 3;

let defaultCanvas;
let video;
let overlay;

let poseNet;
let poses = [];

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

// Size of open menu button. 
let openMenuWidth = 100;
let openMenuHeight = 70;

// Set defaults
let mode = "active";
let delay = 200; // ms
let threshold = vidHeight / 2; // Measured in canvas coordinates. 
let velocityMin = vidHeight / 12;
let velocityScalar = vidHeight / 3; // Currently not controllable by user.
let magScaling = "constant";
let inMenu;

// Arrow limits measured in canvas coordinates. 
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
    // Insert default canvas in our container so it doesn't get added at the end of the DOM. 
    // Note that this is where the mouse gets registered, so it is important to position properly. 
    defaultCanvas = createCanvas(vidWidth, vidHeight);
    defaultCanvas.parent('videoContainer');

    video = createCapture(VIDEO);
    video.size(vidWidth, vidHeight);
    video.parent('videoContainer')

    pixelDensity(1);
    overlay = createGraphics(vidWidth, vidHeight);
    overlay.parent('videoContainer');

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
    overlay.show();
    // Flip graphics so you get proper mirroring of video and nose dot
    overlay.translate(vidWidth,0);
    overlay.scale(-1.0, 1.0);

    lastJump = Date.now();
    thisDetect = Date.now();
    lastArrowChange = Date.now();
}

// Function that p5 calls repeatedly to render graphics
function draw() {
    // Adjust mouse coords.
    adjMouseX = vidWidth - mouseX
    adjMouseY = mouseY

    overlay.clear();

    // Render video
    overlay.image(video, 0, 0);

    inMenu = $("#menuStatus").attr("menu-status") == "true";

    detectNose();
    updateVisuals();
}

// Built-in p5 function that gets called on mouse drag. 
function mouseDragged() {
    if(inMenu) {
        setArrowLims();
    }
    else {
        // If mouse is reasonably close, set threshold.
        if(Math.abs(adjMouseY-threshold) < 60) {
            threshold = adjMouseY;
        }
    }
}

// Set arrow key areas according to mouse position. 
function setArrowLims() {
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

// Return the int corresponding to the region we're in.
function getRegion(x, y) {
    // Calculate and return arrow key. 
    if(x < xLimMax && x > xLimMin && y < yLimMin) {
        return UP;
    }
    else if(x < xLimMax && x > xLimMin && y > yLimMax) {
        return DOWN;
    }
    else if(y < yLimMax && y > yLimMin && x < xLimMin) {
        return RIGHT;
    }
    else if(y < yLimMax && y > yLimMin && x > xLimMax) {
        return LEFT;
    }
    // Not in a special area. 
    return null;
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

    // Render nose dot
    overlay.stroke(0, 225, 0); // Approximately Lime
    overlay.strokeWeight(5);
    overlay.ellipse(noseX, noseY, 1, 1);

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

        // Check if we want to open the menu. 
        if(noseX > vidWidth - openMenuWidth && noseY > vidHeight - openMenuHeight) {
            try {
                gameInstance.SendMessage("Player", "OpenMenu");
                console.log("Opened menu.");
                return; // Leave to handle menu interaction asap. 
            }
            catch(err) {
                console.log("Menu open failed with error: "+err);
                // Continue playing, possibly we'll succeed at opening menu later. 
            }
        }

        switch(mode) {
            case "active":
                trigger = pNoseY < threshold != noseY < threshold;
                on = noseY < threshold;
                break;
            case "velocity":
                rawMagnitude = (pNoseY - noseY) / (thisDetect - lastDetect);
                trigger = (pNoseY - noseY) / (thisDetect - lastDetect) > velocityMin; 
                break;
            default:
        }

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
    }

    // Set latest values to old values. 
    pNoseX = noseX;
    pNoseY = noseY;
}

// Visually update the threshold value
function updateVisuals() {
    if(inMenu) {
        // Render arrow key areas.
        overlay.noStroke();

        let percentLoaded = (Date.now()-lastArrowChange)/arrowDelay;

        // Up
        overlay.fill(255, 0, 0, 125); // Super Friggin Red
        overlay.rect(xLimMax, 0, xLimMin-xLimMax, yLimMin); 
        if(arrowRegion == UP) {
            overlay.rect(xLimMax, (1-percentLoaded)*yLimMin, xLimMin-xLimMax, percentLoaded*yLimMin); 
        }
        // Down
        overlay.fill(0, 255, 0, 125); // Super Friggin Green
        overlay.rect(xLimMax, yLimMax, xLimMin-xLimMax, vidHeight); 
        if(arrowRegion == DOWN) {
            overlay.rect(xLimMax, yLimMax, xLimMin-xLimMax, percentLoaded*yLimMax); 
        }
        // Right
        overlay.fill(0, 0, 255, 125); // Super Friggin Blue
        overlay.rect(0, yLimMax, xLimMin, yLimMin-yLimMax);
        if(arrowRegion == RIGHT) {
            overlay.rect((1-percentLoaded)*xLimMin, yLimMax, percentLoaded*xLimMin, yLimMin-yLimMax);
        }
        // Left
        overlay.fill(255, 0, 255, 125); // Super Friggin Not-Green
        overlay.rect(xLimMax, yLimMax, vidWidth, yLimMin-yLimMax);
        if(arrowRegion == LEFT) {
            overlay.rect(xLimMax, yLimMax, percentLoaded*xLimMax, yLimMin-yLimMax);
        }
    }
    else {
        // Render area to open menu. 
        overlay.noStroke();
        overlay.fill(0, 255, 255, 125); // Super Friggin Not-Red
        overlay.rect(vidWidth-openMenuWidth, vidHeight-openMenuHeight, openMenuWidth, openMenuHeight);

        // Only render line in active mode. 
        if(mode == "active") { 
            overlay.stroke(230, 80, 0); // Kinda Red
            overlay.strokeWeight(2);
            overlay.line(0, threshold, vidWidth, threshold);
        }
    }

    // Crosshairs the mouse. 
    overlay.stroke(255, 255, 0); // Jarate
    overlay.strokeWeight(1);
    overlay.line(0, adjMouseY, vidWidth, adjMouseY);
    overlay.line(adjMouseX, 0, adjMouseX, vidHeight);
}