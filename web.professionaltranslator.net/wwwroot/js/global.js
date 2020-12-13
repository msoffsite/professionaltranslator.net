$(window).on("load", function () {
    $(".loading-container").fadeOut("slow");
});

$(window).on("beforeunload", function () {
    $(".loading-container").fadeIn("slow");
    $(".loading").center();
});

$.fn.center = function () {
    this.css("position", "absolute");
    this.css("top", Math.max(0, (($(window).height() - $(this).outerHeight()) / 2) +
        $(window).scrollTop()) + "px");
    this.css("left", Math.max(0, (($(window).width() - $(this).outerWidth()) / 2) +
        $(window).scrollLeft()) + "px");
    return this;
}

$.fn.scrollTo = function(anchor, navHeight) {
    const goTo = $(anchor).offset().top - navHeight;
    if ($(window).scrollTop() !== goTo) {
        $("html, body").stop().animate({ scrollTop: goTo }, 750);
    }
}


$(document).ready(function () {

    $("a.scroll-link").on("click", function (e) {
        e.preventDefault();
        const anchor = $(this).attr("href");
        $(this).scrollTo(anchor, 0);
    });

    new WOW().init();

});
