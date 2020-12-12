$(window).on("load", function () {
    $(".loading-container").fadeOut("slow");
});

$(window).on("beforeunload", function () {
    $(".loading-container").fadeIn("slow");
});


function scroll_to(clicked_link, nav_height) {
    const element_class = clicked_link.attr("href");
    console.log(element_class);
    let scroll_to = 0;
    if (element_class != "#wrapper") {
        //element_class += "-container";
        scroll_to = $(element_class).offset().top - nav_height;
    }
    if ($(window).scrollTop() != scroll_to) {
        $("html, body").stop().animate({ scrollTop: scroll_to }, 750);
    }
}


$(document).ready(function () {

    /*
        Scroll link
    */
    $("a.scroll-link").on("click", function (e) {
        e.preventDefault();
        scroll_to($(this), 0);
    });

    /*
        Wow
    */
    new WOW().init();

});
