$(document).ready(function () {

    $(window).on("load resize", function () {
        console.log("sidebar toggle.");
        if (($("#menu-toggle").is(":hidden")) && ($("#wrapper").hasClass("toggled"))) {
            console.log("show sidebar");
            $("#wrapper").removeClass("toggled");
        }
    });

    $("#menu-toggle").click(function (e) {
        e.preventDefault();
        $("#wrapper").toggleClass("toggled");
    });
});