$(document).ready(function(){
    // LISTENERS

    // Change trigger mode
    $("#mode>input").on("change", function(){
        mode = $(this).val();
        console.log("Now in "+mode+" mode.");
    });


    // Change delay between jumps
    $("#delay").on("change", function() {
        delay = $(this).val();
        $("#delayValue").html(delay);
    });

    // Change active zone limit
    $("#threshold").on("change", function() {
        threshold = $(this).val();
        $("#thresholdValue").html(threshold);
    });

    // Change sensitivity for velocity/magnitude
    $("#sensitivity").on("change", function() {
        sensitivity = $(this).val();
        $("#sensitivityValue").html(sensitivity);
    });

    
    // INITIAL SETTINGS

    // Set initial limits for sliders
    $("#delay").attr({
        "min": 0,
        "max": 20,
        "value": delay
    });

    $("#threshold").attr({
        "min": 0,
        "max": vidHeight,
        "value": threshold
    });

    $("#sensitivity").attr({
        "min": 1,
        "max": vidHeight / 2,
        "value": sensitivity
    });

});