$(document).ready(function(){
    // Listener for any input updating.
    $("input").on("change", function(){
        let name = $(this).attr("name");
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
                if(isNaN(newVal) // Non-numerical input.
                    || (newVal < $("#delaySlider").slider("option", "min")) // Beyond min/max.
                    || (newVal > $("#delaySlider").slider("option", "max"))) {
                    $(this).val(delay); // Bad input, put it back. Can handle more gracefully later. 
                }
                else {
                    delay = newVal;
                    $("#delaySlider").slider("value", delay);
                }
                console.log("Delay is "+delay);
            }
            // Active mode threshold
            else if(name == "threshold") {
                if(isNaN(newVal) // Non-numerical input.
                    || (newVal < $("#thresholdSlider").slider("option", "min")) // Beyond min/max.
                    || (newVal > $("#thresholdSlider").slider("option", "max"))) {
                    $(this).val(threshold); // Bad input, put it back. Can handle more gracefully later. 
                }
                else {
                    threshold = newVal; // Invert the threshold. 
                    $("#thresholdSlider").slider("value", threshold);
                }
                console.log("Threshold is "+threshold);
            }
            // Velocity minimum
            else if(name == "velocityMin") {
                if(isNaN(newVal) // Non-numerical input. 
                    || (newVal < $("#velocityMinSlider").slider("option", "min")) // Beyond min/max.
                    || (newVal > $("#velocityMinSlider").slider("option", "max"))) {
                    $(this).val(velocityMin); // Bad input, put it back. Can handle more gracefully later. 
                }
                else {
                    velocityMin = newVal;
                    $("#velocityMinSlider").slider("value", velocityMin);
                }
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