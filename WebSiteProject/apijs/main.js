//include共用頁面
$(function ($) {
	$.include = function (url) {
		$.ajax({
			url: url,
			async: false,
			success: function (result) {
				document.write(result);
			}
		});
	};
}(jQuery));


//滾動效果
$(function($) {
	'use strict';
	window.sr= new scrollReveal({
	  reset: false,
	  move: '50px',
	  mobile: false
	});
});


//清除header style 高度, 需於頁面最後執行
$(function () {
	$("#header").removeAttr("style"); 
	$(".header-logo").removeAttr("style"); 
});



//font-size
function size_set(mysize){
    if(mysize=="font_l"){
        document.body.className="font_l";
    }
    if(mysize=="font_m"){
        document.body.className="font_m";
    }
    if(mysize=="font_s"){
        document.body.className="font_s";
    }
}



