$(window).on("load", function () {
    $(".loading-container").fadeOut("slow");
});

$(window).on("beforeunload", function () {
    $(".loading-container").fadeIn("slow");
});