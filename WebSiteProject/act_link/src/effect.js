$(document).ready(function(){
	// menu
	$("#btn1").click(function() {
		$('html, body').animate({
			scrollTop: $("#regi").offset().top
		}, 800);
	});
	$("#btn2").click(function() {
		$('html, body').animate({
			scrollTop: $("#prize").offset().top
		}, 1000);
	});
	$("#btn3").click(function() {
		$('html, body').animate({
			scrollTop: $("#desc").offset().top
		}, 2000);
	});
	// top
	$("#gotop").hide();
	$(function () {
		$(window).scroll(function () {
			if ($(this).scrollTop() > 100) {
				$('#gotop').fadeIn();
			} else {
				$('#gotop').fadeOut();
			}
		});
		$('#gotop').click(function () {
			$('body,html').animate({
				scrollTop:0
			}, 800);
			return false;
		});
	});
	// RWD menu
    $(".btn-menu").click(function() {
        $(this).toggleClass("active");
        $("#menu").fadeToggle();
    });
	// popup
	$('#winner').popup({
		transition: 'all 0.3s',
		scrolllock: true
	});
	$('#regist').popup({
		transition: 'all 0.3s',
		scrolllock: true
	});

});
