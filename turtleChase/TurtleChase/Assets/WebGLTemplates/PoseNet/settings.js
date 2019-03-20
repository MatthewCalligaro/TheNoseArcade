$(document).ready(function(){
    // Mode input
    $("#mode>input").on("change", function(){
        mode = $(this).val();
        console.log("Now in "+mode+" mode.");
    });


    // Delay slider
    $("#delaySlider").slider({
        min: 0,
        max: 20,
        step: 1,
        value: delay,
        slide: function(event, ui) {
            delay = ui.value;
            $("input.delayValue").val(delay);
        },
    });

    $("input.delayValue").change(function() {
        let newVal = parseInt($(this).val());
        if(isNaN(newVal)) {
            $(this).val(delay); // Bad input, put it back. Can handle more gracefully later. 
        }
        else {
            delay = newVal;
            $("#delaySlider").slider("value", delay);
        }
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

    $("input.thresholdValue").change(function() {
        let newVal = parseInt($(this).val());
        if(isNaN(newVal)) {
            $(this).val(threshold); // Bad input, put it back. Can handle more gracefully later. 
        }
        else {
            threshold = newVal;
            $("#thresholdSlider").slider("value", threshold);
        }
    });


    // Sensitivity slider
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
        let index = $this.data("index");
        let newVal = parseInt($this.val());
        if(newVal != NaN) {
            $(this).val(sensitivity[index]); // Bad input, put it back. Can handle more gracefully later. 
        }
        else {
            sensitivity[index] = newVal;
            $("#sensitivitySlider").slider("values", index, newVal);
        }
    });

    // Constant magnitude initial settings
    $("#constantMag").attr({
        "checked": constantMag
    });

    // Const magnitude listener
    $("#constantMag").on("change", function() {
        constantMag = $(this).prop('checked');
    });






    // // Populate slider labels // TODO: so this needs to maybe happen
    // $("#delay").html(delay);
    // $("#threshold").html(threshold);
    // $("#sensitivity").html(sensitivity);




});