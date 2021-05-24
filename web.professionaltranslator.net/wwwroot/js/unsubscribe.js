const unsubscribeResultRow = $("#unsubscribe_result_row");

const unsubscribeResultText = $("#unsubscribe_result_text");

let validEmailAddress = false;

$(document).ready(function () {
    "use strict";

    $("#unsubscribe_me").on("click", function (e) {
        e.preventDefault();
        sendMessage();
    });
});

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
            url: `/Unsubscribe/?handler=Send`,
            beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            data: {
                emailAddress: $("#unsubscribe_email_address").val()
            },
            success: function (response) {
                processResultMessages(response.status, unsubscribeResultRow, unsubscribeResultText, response.messages);
                if (response.status > 0 ) {
                    $("#unsubscribe_email_address").val("");
                }
            },
            failure: function (xhr) {
                processResultMessages(0, unsubscribeResultRow, unsubscribeResultText, xhr.statusText);
            }
        });
    }
}