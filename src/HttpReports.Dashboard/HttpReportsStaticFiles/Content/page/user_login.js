 $(function () {
	$("input[type='password'][data-eye]").each(function(i) {
		let $this = $(this);

		$this.wrap($("<div/>", {
			style: 'position:relative'
		}));
		$this.css({
			paddingRight: 60
		});
		$this.after($("<div/>", {
			html: 'Show',
			class: 'btn btn-primary btn-sm',
			id: 'passeye-toggle-'+i,
			style: 'position:absolute;right:10px;top:50%;transform:translate(0,-50%);padding: 2px 7px;font-size:12px;cursor:pointer;'
		}));
		$this.after($("<input/>", {
			type: 'hidden',
			id: 'passeye-' + i
		}));
		$this.on("keyup paste", function() {
			$("#passeye-"+i).val($(this).val());
		});
		$("#passeye-toggle-"+i).on("click", function() {
			if($this.hasClass("show")) {
				$this.attr('type', 'password');
				$this.removeClass("show");
				$(this).removeClass("btn-outline-primary");
			}else{
				$this.attr('type', 'text');
				$this.val($("#passeye-"+i).val());				
				$this.addClass("show");
				$(this).addClass("btn-outline-primary");
			}
		});
	});
}); 

$(document).ready(function () {

	var current = localStorage.getItem("httpreports.theme");

	if (current == null || current == "dark") { 

		$('#particles').particleground({
			dotColor: '#c7c8ca',
			lineColor: '#c7c8ca',
			particleRadius:1,
			proximity: 50,
			density: 30000
		});  

	} 

}); 

$(document).keydown(function (e) {


	if (e.keyCode == 13) {

		login();
	}
});

function login() { 

	var username = $(".username").val().trim();
	var password = $(".password").val().trim();

	if (username.length == 0 || password.length == 0) { 
		alertError(lang.Login_CheckRule);
		return;
	} 

	$.ajax({
		url: "/HttpReportsData/CheckUserLogin",
		type: "POST",
		contentType: "application/json; charset=utf-8",
		data: JSON.stringify({
			username, password
		}),  
		success: function (result) {

			if (result.code == 1) { 

				location.href = "/HttpReports/Index"; 
			}
			else { 

				alertError(result.msg); 

			} 
		}

	}); 

}
