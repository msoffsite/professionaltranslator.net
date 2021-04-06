$.fn.toggleGetStarted = function () {
    $(".info-safe-with-me").hide();

    const path = $(location).attr("pathname").toLowerCase();
    const idx = path.toString().indexOf("contact");
    if (idx > -1) {
        $(".get-started").hide();
        $(".info-safe-with-me").show();
    }
};

$(document).ready(function () {

    $(".get-started").toggleGetStarted();

    $(window).on("load resize", function () {
        //console.log("sidebar toggle.");
        const documentWidth = $(document).width();
        if ((documentWidth <= 1600) && ($("#wrapper").hasClass("toggled"))) {
            $("#wrapper").removeClass("toggled");
        } else if ((documentWidth >= 1600) && (!$("#wrapper").hasClass("toggled"))) {
            //$("#wrapper").addClass("toggled");
        }
        //else {
        //    if (($("#menu-toggle").is(":hidden")) && ($("#wrapper").hasClass("toggled"))) {
        //        console.log("show sidebar");
        //        $("#wrapper").removeClass("toggled");
        //    }
        //}
    });

    $(".toggle-menu").click(function (e) {
        e.preventDefault();
        $("#wrapper").toggleClass("toggled");
    });
});