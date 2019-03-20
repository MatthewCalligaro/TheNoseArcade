$(document).ready(function(){
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

    $("#constantMag").attr({
        "checked": constantMag
    });

    $("#sensitivitySlider").slider({
        min: 0,
        max: 2*vidHeight/3,
        step: 1,
        values: [sensitivity[0], sensitivity[1]],
        range: true,
        slide: function(event, ui) {
            for (var i = 0; i < ui.values.length; ++i) {
                sensitivity[i] = ui.values[i];
                $("input.sensitivityValue[data-index=" + i + "]").val(ui.values[i]);
            }
        },
    });

    $("input.sensitivityValue").change(function() {
        var $this = $(this);
        $("#sensitivitySlider").slider("values", $this.data("index"), $this.val());
    });



    // // Populate slider labels // TODO: so this needs to maybe happen
    // $("#delay").html(delay);
    // $("#threshold").html(threshold);
    // $("#sensitivity").html(sensitivity);


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

    // Change sensitivity for velocity
    $("#sensitivity").on("change", function() {
        sensitivity = $(this).val();
        $("#sensitivityValue").html(sensitivity);
    });

    // Change const magnitude for velocity
    $("#constantMag").on("change", function() {
        constantMag = $(this).prop('checked');
    });

});