let blogDirectoryOpen = false;
let subscribeFormOpen = false;

function openBlogDirectory() {
    $("#blog_directory_container").addClass("show");
    $("#blog_directory_icon").removeClass("fa-folder");
    $("#blog_directory_icon").addClass("fa-folder-open");
    blogDirectoryOpen = true;
}

function toggleBlogDirectoryIcon() {
    if (blogDirectoryOpen) {
        $("#blog_directory_icon").removeClass("fa-folder");
        $("#blog_directory_icon").addClass("fa-folder-open");
    } else {
        $("#blog_directory_icon").removeClass("fa-folder-open");
        $("#blog_directory_icon").addClass("fa-folder");
        localStorage.setItem("open-directory", "0");
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
    $("#subscribe_form_container").removeClass("show");
    subscribeFormOpen = false;
    toggleSubscribeIcon();
}

$(document).ready(function () {

    let reopenDirectory = localStorage.getItem("open-directory");
    if (reopenDirectory === undefined) {
        reopenDirectory = "0";
    }

    if (reopenDirectory === "1") {
        openBlogDirectory();
    }

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
                $("#blog_directory_container").removeClass("show");
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

    // Convert URL to links in comments
    var comments = document.querySelectorAll("#comments .content [itemprop=text]");

    requestAnimationFrame(function () {
        for (let i = 0; i < comments.length; i++) {
            const comment = comments[i];
            comment.innerHTML = urlify(comment.textContent) || "";
        }
    });

    function urlify(text) {
        return text && text.replace(/(((https?:\/\/)|(www\.))[^\s]+)/g, function (url, b, c) {
            const url2 = c === "www." ? `http://${url}` : url;
            return `<a href="${url2}" rel="nofollow noreferrer">${url}</a>`;
        });
    }

    // Lazy load images/iframes
    window.addEventListener("load", function () {

        var timer,
            images,
            viewHeight;

        function init() {
            images = document.body.querySelectorAll("[data-src]");
            viewHeight = Math.max(document.documentElement.clientHeight, window.innerHeight);

            lazyload(0);
        }

        function scroll() {
            lazyload(200);
        }

        function lazyload(delay) {
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