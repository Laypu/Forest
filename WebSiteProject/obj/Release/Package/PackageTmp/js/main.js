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


//清除header style 高度, 需於頁面最後執行
$(function () {
	$("#header").removeAttr("style"); 
	$(".header-logo").removeAttr("style"); 
});


//輪播圖左告按鈕加title
window.onload=function(){
	$(".owl-prev").attr("title","prev").attr("tabindex","0");
	$(".owl-next").attr("title","next").attr("tabindex","0");
    $(".tp-leftarrow").attr("title","prev");
	$(".tp-rightarrow").attr("title","next");
}


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


//滾動效果
$(function($) {
	'use strict';
	window.sr= new scrollReveal({
	  reset: false,
	  move: '50px',
	  mobile: false
	});
});


//icon font 加 aria-hidden="true"
$(function () {
	$(".fa").attr("aria-hidden","true") ;
	 document.getElementById("web_top").innerHTML = 'Top';
})



//輪播圖左告按鈕加title
window.onload=function(){
	$(".owl-prev").attr("title","prev").attr("tabindex","0");
	$(".owl-next").attr("title","next").attr("tabindex","0");
    $(".tp-leftarrow").attr("title","prev");
	$(".tp-rightarrow").attr("title","next");
}

//owl-carousel 加tabindex=-1
$(function () {
	$('.owl-carousel .cloned').find('a').attr('tabindex','-1');
});

//左右鍵盤操作
$(function () {
	var owl = $(".owl-carousel");

        $(document).on('keydown', function (e) {

            var $focusedElement = $(document.activeElement),
                singleOwl = $(".owl-carousel"),
                type = e.which == 39 ? 'next' : null,
                type = e.which == 37 ? 'prev' : type,
                type = e.which == 13 ? 'enter' : type;

            singleOwl.owlCarousel();

            // if the carousel is focused, use left and right arrow keys to navigate
            console.log($focusedElement.attr('class'));
            if ($focusedElement.attr('class') == "owl-carousel") {
                debugger;
                if (type == 'next') {
                    singleOwl.trigger('next.owl.carousel');
                } else if (type == 'prev') {
                    singleOwl.trigger('prev.owl.carousel');
                }

            } else if (type == 'enter') {
                if ($focusedElement.hasClass('owl-next')) {

                    singleOwl.trigger('next.owl.carousel');
                } else if ($focusedElement.hasClass('owl-prev')) {

                    singleOwl.trigger('prev.owl.carousel');
                }
            }
        });
});



//mobile search icon
$(document).ready(function(){
    $(".sidetitle").click(function(){
        $(".sidemenu").slideToggle();
    });
});



