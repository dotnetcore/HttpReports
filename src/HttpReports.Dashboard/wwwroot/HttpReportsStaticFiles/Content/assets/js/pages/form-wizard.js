function isEmail(email) {
  var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
  return regex.test(email);
}

$(document).ready(function(){
	
	$('#ccnumber-w1').mask("9999 9999 9999 9999");
	
	/* ---------- Wizard ---------- */
	$('#email-w1').keyup(function(){
		
		if(isEmail($(this).val())) {
			$(this).parent().parent().removeClass('has-error');
		}
		
	});
	
	$('#password-w1, #name-w1').keyup(function(){
		
		if($(this).val()) {
			$(this).parent().parent().removeClass('has-error');
		}
		
	});
	
	$('#ccnumber-w1').keyup(function(){
		
		var getCCNumber = $(this).val();
		getCCNumber = getCCNumber.replace(/ /g,'').replace(/_/g,'');
		
		if(getCCNumber.length == 16) {
			$(this).parent().parent().removeClass('has-error');
		}
		
	});
	
	$('#cvv-w1').keyup(function(){
		
		if($(this).val().length == 3) {
			$(this).parent().parent().removeClass('has-error');
		} else {
			$(this).parent().parent().addClass('has-error');
		}
		
	});

	$('#wizard1').bootstrapWizard({
		'nextSelector': '.button-next',
		'previousSelector': '.button-previous', 
		onNext: function(tab, navigation, index) {
		
		if(index==1) {
			
			var bugs = 0;
			
			if(!isEmail($('#email-w1').val())) {				
				$('#email-w1').parent().parent().addClass('has-error')
				bugs = 1;
			}
			
			if(!$('#password-w1').val()) {
				$('#password-w1').parent().parent().addClass('has-error');
				bugs = 1;
			}
			
			
			if( bugs == 1) {
				return false;
			}
			
		}

		if(index==2) {
			
			var bugs = 0;
			
			if(!$('#name-w1').val()) {				
				$('#name-w1').parent().parent().addClass('has-error')
				bugs = 1;
			}
			
			if(!$('#ccnumber-w1').val()) {
				$('#ccnumber-w1').parent().parent().addClass('has-error');
				bugs = 1;
			}
			
			if(!$('#cvv-w1').val()) {
				$('#cvv-w1').parent().parent().addClass('has-error');
				bugs = 1;
			}
			
			if( bugs == 1) {
				return false;
			}
			
		}

	}, onTabShow: function(tab, navigation, index) {
		var $total = navigation.find('li').length;
		var $current = index+1;
		var $percent = ($current/$total) * 100;
		$('#wizard1').find('.progress-bar').css({width:$percent+'%'});
		
		$('#wizard1 > .steps li').each( function (index) {
			$(this).removeClass('complete');
		  	index += 1;
		  	if(index < $current) {
		    	$(this).addClass('complete');
		  	}
		 });
		
		if($current >= $total) {
			$('#wizard1').find('.button-next').hide();
			$('#wizard1').find('.button-finish').show();
		} else {
			$('#wizard1').find('.button-next').show();
			$('#wizard1').find('.button-finish').hide();
		}	
	}});
	
	$('#wizard2').bootstrapWizard({
		'nextSelector': '.button-next',
		'previousSelector': '.button-previous', 
		onNext: function(tab, navigation, index) {
		

	}, onTabShow: function(tab, navigation, index) {
		var $total = navigation.find('li').length;
		var $current = index+1;
		var $percent = ($current/$total) * 100;
		$('#wizard2').find('.progress-bar').css({width:$percent+'%'});
		
		$('#wizard2 > .steps li').each( function (index) {
			$(this).removeClass('complete');
		  	index += 1;
		  	if(index < $current) {
		    	$(this).addClass('complete');
		  	}
		 });
		
		if($current >= $total) {
			$('#wizard2').find('.button-next').hide();
			$('#wizard2').find('.button-finish').show();
		} else {
			$('#wizard2').find('.button-next').show();
			$('#wizard2').find('.button-finish').hide();
		}	
	}});
	
	/* ---------- Datapicker ---------- */
	$('.datepicker').datepicker();

	/* ---------- Choosen ---------- */
	$('[data-rel="chosen"],[rel="chosen"]').chosen();

	/* ---------- Placeholder Fix for IE ---------- */
	$('input, textarea').placeholder();

	/* ---------- Auto Height texarea ---------- */
	$('textarea').autosize();   
});