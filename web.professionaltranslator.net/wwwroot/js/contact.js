﻿$(document).ready(function() {
    "use strict";

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

    var input = $(".validate-input .input-element");

    $(".validate-form").on("submit", function () {
        var check = true;

        for (let i = 0; i < input.length; i++) {
            if (validate(input[i]) === false) {
                showValidationMessage(input[i]);
                check = false;
            }
        }

        return check;
    });

    $(".validate-form .input-element").each(function () {
        $(this).focus(function () {
            closeValidationMessage(this);
            $(this).parent().removeClass("validated");
        });
    });
}); 

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