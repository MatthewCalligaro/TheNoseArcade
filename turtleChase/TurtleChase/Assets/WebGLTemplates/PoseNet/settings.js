$(document).ready(function(){

    var focusGameCanvas = function() {
        console.log("Try to focus game.");
        var gameCanvas = $("#gameContainer>canvas");
        if(gameCanvas.length) {
            console.log("Focusing game.");
            gameCanvas[0].setAttribute("tabindex", "1");
            gameCanvas[0].focus(); 
        }
        else {
           setTimeout(focusGameCanvas, 100);
        }
    }
 
    focusGameCanvas();

    // In case no change is made, give the game focus. 
    $("input").on("blur", function() {
        focusGameCanvas();
    });

    // Listener for any input updating.
    $("input").on("change", function(){
        let name = $(this).attr("name");
        let type = $(this).attr("type");
        // Radio inputs
        if(type == "radio") {
            // Trigger mode
            if(name == "inputMode") {
                mode = $(this).val();
                console.log("Now in "+mode+" mode.");
            }
            // Constant magnitude
            if(name == "magScaling") {
                magScaling = $(this).val();
                console.log("Now using "+magScaling+" scaling.");
            }
        }
        // Text inputs
        else if(type == "text") {
            let newVal = parseInt($(this).val());
            // Delay
            if(name == "delay") {
                // Check if input is good. 
                if(!isNaN(newVal) // Verify input is numerical.
                    && newVal >= $("#delaySlider").slider("option", "min") // Verify within min/max.
                    && newVal <= $("#delaySlider").slider("option", "max")) {
                    delay = newVal;
                }
                $(this).val(delay); // Refresh input, whether we got a good value or not. 
                $("#delaySlider").slider("value", delay);
                console.log("Delay is "+delay);
            }
            // Active mode threshold
            else if(name == "threshold") {
                // Check if input is good. 
                if(!isNaN(newVal) // Verify input is numerical.
                    && newVal >= $("#thresholdSlider").slider("option", "min") // Verify within min/max.
                    && newVal <= $("#thresholdSlider").slider("option", "max")) {
                    threshold = newVal; // Invert the threshold. 
                }
                $(this).val(threshold); // Refresh input, whether we got a good value or not. 
                $("#thresholdSlider").slider("value", threshold);
                console.log("Threshold is "+threshold);
            }
            // Velocity minimum
            else if(name == "velocityMin") {
                // Check if input is good. 
                if(!isNaN(newVal) // Verify input is numerical.
                    && newVal >= $("#velocityMinSlider").slider("option", "min") // Verify within min/max.
                    && newVal <= $("#velocityMinSlider").slider("option", "max")) {
                    velocityMin = newVal;
                }
                $(this).val(velocityMin); // Refresh input, whether we got a good value or not. 
                $("#velocityMinSlider").slider("value", velocityMin);
                console.log("Velocity min is "+velocityMin);
            }
        }
    });


    // Delay slider
    $("#delaySlider").slider({
        min: 0,
        max: 1000,
        step: 1,
        value: delay,
        slide: function(event, ui) {
            delay = ui.value;
            $("input.delayValue").val(delay);
        },
    });

    // Delay input initial value
    $("input.delayValue").attr({
        value: delay
    });


    // Threshold slider
    $("#thresholdSlider").slider({
        min: 0,
        max: vidHeight,
        step: 1,
        value: threshold,
        slide: function(event, ui) {
            threshold = ui.value;
            $("input.thresholdValue").val(threshold);
        },
    });

    // Threshold input initial value
    $("input.thresholdValue").attr({
        value: threshold
    });


    // Velocity min slider
    $("#velocityMinSlider").slider({
        min: 0,
        max: vidHeight/3, // TODO no don't do that. 
        step: 1,
        value: velocityMin,
        slide: function(event, ui) {
            velocityMin = ui.value;
            $("input.velocityMinValue").val(velocityMin);
        },
    });

    // Velocity min initial value
    $("input.velocityMinValue").attr({
        value: velocityMin
    });

});

// Pop up helptext. 
$( document ).tooltip({
    items: "[data-helptext]",
    content: function() {
        return $(this).attr("data-helptext");
    }
});