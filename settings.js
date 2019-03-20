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

    // Delay input initial value
    $("input.delayValue").attr({
        value: delay
    });

    // Delay input listener
    $("input.delayValue").change(function() {
        let newVal = parseInt($(this).val());
        if(isNaN(newVal) // Non-numerical input.
            || (newVal < 0) // Beyond min/max.
            || (newVal > 20)) {
            $(this).val(delay); // Bad input, put it back. Can handle more gracefully later. 
        }
        else {
            delay = newVal;
            $("#delaySlider").slider("value", delay);
        }
    });


    // Threshold slider
    // Note that the slider displays the inverse of threshold instead of its actual value. 
    $("#thresholdSlider").slider({
        min: 0,
        max: vidHeight,
        step: 1,
        value: vidHeight - threshold,
        slide: function(event, ui) {
            threshold = vidHeight - ui.value; // Invert the threshold. 
            $("input.thresholdValue").val(ui.value);
        },
    });

    // Threshold input initial value
    $("input.thresholdValue").attr({
        value: vidHeight - threshold
    });

    // Threshold input listener
    $("input.thresholdValue").change(function() {
        let newVal = parseInt($(this).val());
        if(isNaN(newVal) // Non-numerical input.
            || (newVal < 0) // Beyond min/max.
            || (newVal > vidHeight)) {
            $(this).val(vidHeight - threshold); // Bad input, put it back. Can handle more gracefully later. 
        }
        else {
            threshold = vidHeight - newVal; // Invert the threshold. 
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

    // Sensitivity input initial lower bound
    $("input.sensitivityValue[data-index=0]").attr({
        value: sensitivity[0]
    });

    // Sensitivity input initial upper bound
    $("input.sensitivityValue[data-index=1]").attr({
        value: sensitivity[1]
    });

    // Sensitivity input listener
    $("input.sensitivityValue").change(function() {
        var $this = $(this);
        let index = $this.data("index");
        let newVal = parseInt($this.val());
        if(isNaN(newVal) // Non-numerical input. 
            || (index == 0 && newVal > sensitivity[1]) // Past other slider. 
            || (index == 1 && newVal < sensitivity[0])
            || (newVal < 0) // Beyond min/max.
            || (newVal > 2*vidHeight/3)) {
            $(this).val(sensitivity[index]); // Bad input, put it back. Can handle more gracefully later. 
        }
        else {
            sensitivity[index] = newVal;
            $("#sensitivitySlider").slider("values", index, newVal);
        }
    });


    // Constant magnitude initial settings
    $("#constantMag").attr({
        checked: constantMag
    });

    // Const magnitude listener
    $("#constantMag").on("change", function() {
        constantMag = $(this).prop('checked');
    });

});


$( document ).tooltip({
    items: "[data-helptext]",
    content: function() {
        return $(this).attr("data-helptext");
    }
});