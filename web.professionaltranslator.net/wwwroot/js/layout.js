$(document).ready(function () {

    $(window).on("load resize", function () {
        //console.log("sidebar toggle.");
        const documentWidth = $(document).width();
        if ((documentWidth <= 1600) && ($("#wrapper").hasClass("toggled"))) {
            $("#wrapper").removeClass("toggled");
        } else if ((documentWidth >= 1600) && (!$("#wrapper").hasClass("toggled"))) {
            $("#wrapper").addClass("toggled");
        }
        //else {
        //    if (($("#menu-toggle").is(":hidden")) && ($("#wrapper").hasClass("toggled"))) {
        //        console.log("show sidebar");
        //        $("#wrapper").removeClass("toggled");
        //    }
        //}
    });

    $("#menu-toggle").click(function (e) {
        e.preventDefault();
        $("#wrapper").toggleClass("toggled");
    });
});