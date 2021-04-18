$(document).ready(function () {
    "use strict";

    $("input").each(function () {
        console.log("input found.");
        $(this).attr("autocomplete", randomString(5));
    });
    //$(".form-container").disableAutoFill();

    $(".validate-input .input-element").each(function () {
        $(this).on("blur",
            function () {
                if (validate(this) === false) {
                    showValidationMessage(this);
                } else {
                    $(this).parent().addClass("validated");
                }
            });
    });

    $(".validate-form .input-element").each(function () {
        $(this).focus(function () {
            closeValidationMessage(this);
            $(this).parent().removeClass("validated");
        });
    });
});

function randomString(length) {
    let result = "";
    const characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    const charactersLength = characters.length;
    for (let i = 0; i < length; i++) {
        result += characters.charAt(Math.floor(Math.random() * charactersLength));
    }
    console.log("random string: " + result);
    return result;
}

function validate(element) {
    if ($(element).attr("type") === "email") {
        const regEx = /^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{1,5}|[0-9]{1,3})(\]?)$/;
        if ($(element).val().trim().match(regEx) == null) {
            return false;
        }
    } else {
        if ($(element).val().trim() === "") {
            return false;
        }
    }
}


// Example: arrayValidExtensions = [".jpg", ".jpeg", ".bmp", ".gif", ".png"];
function validateUploads(fileElementId, arrayValidExtensions, resultRow, resultTextContainer, maxFileSizeMb) {
    const fileContainer = document.getElementById(fileElementId);

    const messages = [];

    const files = fileContainer.files;
    if (files.length === 0) {
        messages.push("You must selected at least one file if you're going to upload.");
    }

    for (let i = 0; i < files.length; i++) {
        const inputElement = files[i];
        const filename = inputElement.name;
        if (filename.length > 0) {

            const extensionIndex = filename.lastIndexOf(".");
            if (extensionIndex === -1) {
                messages.push(filename + " must have an extension.<br />");
                continue;
            } else {
                const fileExtension = filename.substring(extensionIndex).toLowerCase();
                const arrayIndex = arrayValidExtensions.indexOf(fileExtension);
                if (arrayIndex === -1) {
                    messages.push(filename + " is not an accepted document.<br />");
                    continue;
                }
            }

            const fileSize = (inputElement.size / (1024 * 1024));
            if (fileSize > maxFileSizeMb) {
                messages.push(filename + " is too big.<br />");
                continue;
            }
        }
    }

    if (messages.length > 0) {
        processResultMessages(0, resultRow, resultTextContainer, messages);
        return false;
    }

    return true;
}

function processResultMessages(result, row, textContainer, messages) {
    row.fadeIn(1000);

    let html = "";
    if (Array.isArray(messages)) {
        const messageArray = messages;
        for (let i = 0; i < messageArray.length; ++i) {
            html += messageArray[i];
            html += "<br />";
        }
    } else {
        html = messages;
    }

    if (html.length > 0) {
        if (!textContainer.hasClass("alert")) {
            textContainer.addClass("alert");
        }

        if (result === 0) {
            textContainer.removeClass("alert-info");
            textContainer.addClass("alert-danger");
            row.fadeOut(10000);
        } else {
            textContainer.removeClass("alert-danger");
            textContainer.addClass("alert-info");
            row.fadeOut(5000);
        }
    }

    textContainer.html(`<p>${html}</p>`);
}

function showValidationMessage(element) {
    const thisAlert = $(element).parent();

    $(thisAlert).addClass("validation-message");

    $(thisAlert).append('<span class="close-validation-message">&#xf136;</span>');
    $(".close-validation-message").each(function () {
        $(this).on("click", function () {
            closeValidationMessage(this);
        });
    });
}

function closeValidationMessage(element) {
    const thisAlert = $(element).parent();
    $(thisAlert).removeClass("validation-message");
    $(thisAlert).find(".close-validation-message").remove();
}