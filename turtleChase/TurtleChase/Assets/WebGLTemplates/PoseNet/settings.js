$(document).ready(function(){
    // Change trigger mode
    $("#mode>input").on("change", function(){
        mode = $(this).val();
        console.log("Now in "+mode+" mode.");
    });

    $("#threshold").on("change", function() {
        threshold = $(this).val();
    });

    $("#sensitivity").on("change", function() {
        sensitivity = $(this).val();
    });
});