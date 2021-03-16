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
                if (validate(this) == false) {
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
        if ($(element).val().trim() == "") {
            return false;
        }
    }
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