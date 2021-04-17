$(document).ready(function() {
    "use strict";

    $("input").each(function () {
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

    $("#client_email_address").on("change",
        function() {
            postEmailAddress("#client_email_address");
        });

    $("#send_message").on("click", function (e) {
        e.preventDefault();
        sendMessage();
    });

    $(".validate-form .input-element").each(function () {
        $(this).focus(function () {
            closeValidationMessage(this);
            $(this).parent().removeClass("validated");
        });
    });
}); 

function closeValidationMessage(element) {
    const thisAlert = $(element).parent();
    $(thisAlert).removeClass("validation-message");
    $(thisAlert).find(".close-validation-message").remove();
}

function randomString(length) {
    let result = "";
    const characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    const charactersLength = characters.length;
    for (let i = 0; i < length; i++) {
        result += characters.charAt(Math.floor(Math.random() * charactersLength));
    }
    return result;
}

function postEmailAddress(id) {
    const input = $(id);

    let passed = true;

    if (validate(input) === false) {
        showValidationMessage(input[i]);
        passed = false;
    }

    if (passed) {
        $.ajax({
            type: "POST",
            url: "/Contact?handler=EmailAddressChange",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            data: JSON.stringify({
                Name: $("#client_name").val(),
                EmailAddress: input.val()
            }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            failure: function (response) {
                console.log(response);
            }
        });
    }
}

function sendMessage() {

    const input = $(".validate-input .input-element");

    let passed = true;

    for (let i = 0; i < input.length; i++) {
        if (validate(input[i]) === false) {
            showValidationMessage(input[i]);
            passed = false;
        }
    }

    if (passed) {
        $.ajax({
            type: "POST",
            url: "/Contact?handler=Send",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            data: JSON.stringify({
                Name: $("#client_name").val(),
                EmailAddress: $("#client_email_address").val(),
                Title: $("#work_title").val(),
                TranslationType: $("input[name='translation-type']:checked").val(),
                Genre: $("#genre").val(),
                WordCount: $("#word_count").val(),
                Message: $("#message").val()
            }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                window.location.href = "/InquiryResult";
            },
            failure: function (response) {
                console.log(response);
            }
        });
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