let originalImageHeight = 0;

$(window).on("load resize",
    function() {
        const windowHeight = $(window).height();
        let imageHeight = $(".intro-hola-hello").height();
        if (originalImageHeight === 0) {
            originalImageHeight = imageHeight;
        }
        if (windowHeight < 1080) {
            imageHeight = windowHeight * .70;
            $(".intro-hola-hello").height(imageHeight);
            console.log('$(".intro-hola-hello").height(): ' + $(".intro-hola-hello").height());
        } else {
            $(".intro-hola-hello").height(originalImageHeight);
        }
    });

    //$(document).ready(function() {
    //    $(window).on("load resize", function () {
    //        const screenWidth = $(window).width();
    //        if (screenWidth > 1200) {
    //            const percentageWidth = screenWidth * .50;
    //            $(".intro-hola-hello").width(percentageWidth);
    //        } else {
    //            const screenHeight = $(window).height();
    //            const percentageHeight = screenHeight * .75;
    //            $(".intro-hola-hello").height(percentageHeight);
    //        }
            
    //    });
    //});