$(document).ready(function(){

  var focusGameCanvas = function() {
    var gameCanvas = $('#gameContainer>canvas');
    if (gameCanvas.length) {
      gameCanvas[0].setAttribute('tabindex', '1');
      gameCanvas[0].focus(); 
    }
    else {
       setTimeout(focusGameCanvas, 100);
    }
  }
 
  focusGameCanvas();

  // In case no change is made, give the game focus. 
  $('input').on('blur', function() {
    focusGameCanvas();
  });

  // Listener for any input updating.
  $('input').on('change', function(){
    let name = $(this).attr('name');
    let type = $(this).attr('type');

    if (type == 'radio') { // Radio inputs
      if (name == 'inputMode') { // Trigger mode
        mode = $(this).val();
        console.log('Now in ' + mode + ' mode.');
      }
      if (name == 'magScaling') { // Constant magnitude
        magScaling = $(this).val();
        console.log('Now using ' + magScaling + ' scaling.');
      }
    } else if (type == 'text') { // Text inputs
      let newVal = parseInt($(this).val());
      if (name == 'delay') { // Jump delay
        // Check if input is good. 
        if (!isNaN(newVal) // Verify input is numerical.
            && newVal >= $('#delaySlider').slider('option', 'min') // Verify within min/max.
            && newVal <= $('#delaySlider').slider('option', 'max')) {
          delay = newVal;
        }
        $(this).val(delay); // Refresh input, whether we got a good value or not. 
        $('#delaySlider').slider('value', delay);
        console.log('Delay is ' + delay);
      } else if (name == 'velocityMin') { // Velocity minimum
        // Check if input is good. 
        if (!isNaN(newVal) // Verify input is numerical.
            && newVal >= $('#velocityMinSlider').slider('option', 'min') // Verify within min/max.
            && newVal <= $('#velocityMinSlider').slider('option', 'max')) {
          velocityMin = newVal;
        }
        $(this).val(velocityMin); // Refresh input, whether we got a good value or not. 
        $('#velocityMinSlider').slider('value', velocityMin);
        console.log('Velocity min is '+velocityMin);
      }
    }
  });


  // Delay slider
  $('#delaySlider').slider({
    min: 0,
    max: 1000,
    step: 1,
    value: delay,
    slide: function(event, ui) {
      delay = ui.value;
      $('input.delayValue').val(delay);
    },
  });

  // Delay input initial value
  $('input.delayValue').attr({
    value: delay
  });


  // Velocity min slider
  $('#velocityMinSlider').slider({
    min: 0,
    max: vidHeight / 3, // TODO no don't do that. 
    step: 1,
    value: velocityMin,
    slide: function(event, ui) {
      velocityMin = ui.value;
      $('input.velocityMinValue').val(velocityMin);
    },
  });

  // Velocity min initial value
  $('input.velocityMinValue').attr({
    value: velocityMin
  });

});

// Pop up helptext. 
$( document ).tooltip({
  items: '[data-helptext]',
  content: function() {
    return $(this).attr('data-helptext');
  }
});