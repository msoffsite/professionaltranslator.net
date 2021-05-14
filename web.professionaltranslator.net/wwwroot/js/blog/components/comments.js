(function (window, document) {

    // Show comment form. It's invisible by default in case visitor
    // has disabled javascript
    const commentForm = document.querySelector("#comments form");
    if (commentForm) {
        commentForm.classList.add("js-enabled");

        commentForm.addEventListener("submit", function (e) {
            this.querySelector("input[type=submit]").value = "Posting comment...";
            const elements = this.elements;
            for (let i = 0; i < elements.length; ++i) {
                elements[i].readOnly = true;
            }
        });
    }

    // Expand comment form
    const content = document.querySelector("#comments textarea");
    if (content) {
        content.addEventListener("focus", function () {
            document.querySelector(".details").className += " show";

            // Removes the hidden website form field to fight spam
            setTimeout(function () {
                const honeypot = document.querySelector("input[name=website]");
                honeypot.parentNode.removeChild(honeypot);
            }, 2000);
        }, false);
    }

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

})(window, document);