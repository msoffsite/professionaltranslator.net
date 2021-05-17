// ReSharper disable PossiblyUnassignedProperty

const postResultRow = $("#post_result_row");

const postResultTextContainer = $("#post_result_text");

const postContentValidator = $("#post_content_validator");

$(document).ready(function () {
    "use strict";

    $("#save_post").on("click", function (e) {
        e.preventDefault();
        savePost();
    });
});

function validateTinyMce(postContent) {
    if (postContent.length > 0) {
        postContentValidator.val(postContent);
    } else {
        postContentValidator.val("");
    }
    if (validate(postContentValidator) === false) {
        showValidationMessage(postContentValidator);
        postContentValidator.removeClass("hide");
        postContentValidator.parent().removeClass("validated");
    } else {
        closeValidationMessage(postContentValidator);
        postContentValidator.addClass("hide");
        postContentValidator.parent().addClass("validated");
    }
}

function savePost() {

    const input = $(".validate-input .input-element");

    let passed = true;

    for (let i = 0; i < input.length; i++) {
        if (validate(input[i]) === false) {
            showValidationMessage(input[i]);
            passed = false;
        }
    }

    if (passed) {

        window.onbeforeunload = null;

        const postTitle = $("#post_title").val();
        const slug = postTitle.replace(/\s+/g, '-').toLowerCase();

        // ReSharper disable once CssBrowserCompatibility
        const publish = $("input[name='publish-post']:checked").val() === "show";

        $.ajax({
            type: "POST",
            url: "/Blog/Edit?handler=Save",
            beforeSend: function(xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            data: JSON.stringify({
                Id: $("#post_id").val(),
                Title: postTitle,
                Excerpt: $("#post_excerpt").val(),
                Content: postContentValidator.val(),
                Categories: $("#post_categories").val(),
                Slug: slug,
                PubDate: $("#post_pub_date").datepicker("getDate"),
                IsPublished: publish
            }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.status === 2) {
                    window.location.href = response.messages;
                }
                processResultMessages(response.status, postResultRow, postResultTextContainer, response.messages);
            },
            failure: function (xhr) {
                processResultMessages(0, postResultRow, postResultTextContainer, xhr.statusText);
            }
        });
    }
}

(function () {

    // File upload
    function handleFileSelect(event) {
        if (window.File && window.FileList && window.FileReader) {

            const files = event.target.files;

            for (let x = 0; x < files.length; x++) {
                const file = files[x];

                // Only image uploads supported
                if (!file.type.match("image"))
                    continue;

                const reader = new FileReader();
                reader.addEventListener("load", function () {
                    var image = new Image();
                    image.alt = file.name;
                    image.onload = function () {
                        image.setAttribute("data-filename", file.name);
                        image.setAttribute("width", image.width.toString());
                        image.setAttribute("height", image.height.toString());
                        window.tinymce.activeEditor.execCommand("mceInsertContent", false, image.outerHTML);
                    };
                    image.src = this.result;

                });

                reader.readAsDataURL(file);
            }

            document.body.removeChild(event.target);
        }
        else {
            console.log("Your browser does not support File API");
        }
    }

    // remove empty strings
    function removeEmpty(item) {
        const trimmedItem = item.trim();
        if (trimmedItem.length > 0) {
            return trimmedItem;
        }
    }

    const editForm = document.getElementById("edit_post");
    const postContent = document.getElementById("post_content");

    if (editForm && postContent) {

        if (typeof window.orientation !== "undefined" || navigator.userAgent.indexOf("IEMobile") !== -1) {
            window.tinymce.init({
                selector: "#post_content",
                theme: "modern",
                mobile: {
                    theme: "mobile",
                    plugins: ["autosave", "lists", "autolink"],
                    toolbar: ["undo", "bold", "italic", "styleselect"]
                }
            });
        } else {
            window.tinymce.init({
                selector: "#post_content",
                autoresize_min_height: 200,
                plugins: "autosave preview searchreplace visualchars image link media fullscreen code codesample table hr pagebreak autoresize nonbreaking anchor insertdatetime advlist lists textcolor wordcount imagetools colorpicker",
                menubar: "edit view format insert table",
                toolbar1: "formatselect | bold italic blockquote forecolor backcolor | imageupload link | alignleft aligncenter alignright  | numlist bullist outdent indent | fullscreen",
                selection_toolbar: "bold italic | quicklink h2 h3 blockquote",
                autoresize_bottom_margin: 0,
                paste_data_images: true,
                image_advtab: true,
                file_picker_types: "image",
                relative_urls: false,
                convert_urls: false,
                branding: false,

                setup: function (editor) {
                    editor.addButton("imageupload", {
                        icon: "image",
                        onclick: function () {
                            const fileInput = document.createElement("input");
                            fileInput.type = "file";
                            fileInput.multiple = true;
                            fileInput.accept = "image/*";
                            fileInput.addEventListener("change", handleFileSelect, false);
                            fileInput.click();
                        }
                    });
                    editor.on("change", function () {
                        validateTinyMce(editor.getContent());
                    });
                    editor.on("init", function () {
                        validateTinyMce(editor.getContent());
                    });
                }
            });
        }

        // Delete post
        const deleteButton = editForm.querySelector(".delete");
        if (deleteButton) {
            deleteButton.addEventListener("click", function (e) {
                if (!confirm("Are you sure you want to delete the post?")) {
                    e.preventDefault();
                }
            });
        }
    }

    // Delete comments
    const deleteLinks = document.querySelectorAll("#comments a.delete");
    if (deleteLinks) {
        for (let i = 0; i < deleteLinks.length; i++) {
            const link = deleteLinks[i];
            link.addEventListener("click", function (e) {
                if (!confirm("Are you sure you want to delete the comment?")) {
                    e.preventDefault();
                }
            });
        }
    }

    // Tag input enhancement - using autocomplete input
    var selectTag = document.getElementById("select_category");
    var categories = document.getElementById("post_categories");
    if (selectTag && categories) {

        selectTag.onchange = function () {

            const phv = selectTag.placeholder;
            const val = selectTag.value.toLowerCase();

            const phvArray = phv.split(",").map(function (item) {
                return removeEmpty(item);
            });

            const valArray = val.split(",").map(function (item) {
                return removeEmpty(item);
            });

            for (let j = valArray.length - 1; j >= 0; j--) {
                const v = valArray[j];
                let flag = false;
                for (let k = phvArray.length - 1; k >= 0; k--) {
                    if (phvArray[k] === v) {
                        phvArray.splice(k, 1);
                        flag = true;
                    }
                }
                if (!flag) {
                    phvArray.push(v);
                }
            }

            selectTag.placeholder = phvArray.join(", ");
            categories.value = selectTag.placeholder;
            selectTag.value = "";
        };
    }

})();