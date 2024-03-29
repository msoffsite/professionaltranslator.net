﻿let blogDirectoryOpen = false;
let subscribeFormOpen = false;

function toggleBlogDirectoryIcon() {
    if (blogDirectoryOpen) {
        $("#blog_directory_icon").removeClass("fa-folder");
        $("#blog_directory_icon").addClass("fa-folder-open");
    } else {
        $("#blog_directory_icon").removeClass("fa-folder-open");
        $("#blog_directory_icon").addClass("fa-folder");
    }
};

function toggleSubscribeIcon() {
    if (subscribeFormOpen) {
        $("#subscribe_link_icon").removeClass("fa-bell");
        $("#subscribe_link_icon").addClass("fa-bell-slash");
    } else {
        $("#subscribe_link_icon").removeClass("fa-bell-slash");
        $("#subscribe_link_icon").addClass("fa-bell");
    }
};

function closeSubscribe() {
    $("#subscribe_form_toggle").removeClass("show");
    subscribeFormOpen = false;
    toggleSubscribeIcon();
}

$(document).ready(function () {

    $("#blog_directory").on("click",
        function () {
            if (subscribeFormOpen) {
                closeSubscribe();
            }
            blogDirectoryOpen = blogDirectoryOpen ? false : true;
            toggleBlogDirectoryIcon();
        });

    $("#subscribe_link").on("click", 
        function () {
            if (blogDirectoryOpen) {
                $("#blog_directory_toggle").removeClass("show");
                blogDirectoryOpen = false;
                toggleBlogDirectoryIcon();
            }
            subscribeFormOpen = subscribeFormOpen ? false : true;
            toggleSubscribeIcon();
        });
});

(function (window, document) {

    // Lazy load stylesheets
    requestAnimationFrame(function () {
        const stylesheets = document.querySelectorAll("link[as=style]");

        for (let i = 0; i < stylesheets.length; i++) {
            const link = stylesheets[i];
            link.setAttribute("rel", "stylesheet");
            link.removeAttribute("as");
        }
    });

    // Lazy load images/iframes
    window.addEventListener("load", function () {

        var timer,
            images,
            viewHeight;

        function init() {
            images = document.body.querySelectorAll("[data-src]");
            viewHeight = Math.max(document.documentElement.clientHeight, window.innerHeight);

            lazyLoad(0);
        }

        function scroll() {
            lazyLoad(200);
        }

        function lazyLoad(delay) {
            if (timer) {
                return;
            }

            timer = setTimeout(function () {
                var changed = false;

                requestAnimationFrame(function () {
                    for (let i = 0; i < images.length; i++) {
                        const img = images[i];
                        const rect = img.getBoundingClientRect();

                        if (!(rect.bottom < 0 || rect.top - 100 - viewHeight >= 0)) {
                            img.onload = function (e) {
                                e.target.className = "loaded";
                            };

                            img.className = "notloaded";
                            img.src = img.getAttribute("data-src");
                            img.removeAttribute("data-src");
                            changed = true;
                        }
                    }

                    if (changed) {
                        filterImages();
                    }

                    timer = null;
                });
            }, delay);
        }

        function filterImages() {
            images = Array.prototype.filter.call(
                images,
                function (img) {
                    return img.hasAttribute("data-src");
                }
            );

            if (images.length === 0) {
                window.removeEventListener("scroll", scroll);
                window.removeEventListener("resize", init);
                return;
            }
        }

        // polyfill for older browsers
        window.requestAnimationFrame = (function () {
            return window.requestAnimationFrame ||
                window.webkitRequestAnimationFrame ||
                window.mozRequestAnimationFrame ||
                function (callback) {
                    window.setTimeout(callback, 1000 / 60);
                };
        })();

        window.addEventListener("scroll", scroll);
        window.addEventListener("resize", init);

        init();
    });

})(window, document);