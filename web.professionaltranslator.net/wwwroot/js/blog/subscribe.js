const subscriberResultRow = $("#subscriber_result_row");

const subscriberResultTextContainer = $("#subscriber_result_text");

$(document).ready(function () {
    "use strict";

    $("#confirm_subscription").on("click", function (e) {
        e.preventDefault();
        saveSubscriber();
    });
});

function clearSubscriberInfo() {
    $("#subscriber_first_name").val("");
    $("#subscriber_last_name").val("");
    $("#subscriber_email_address").val("");
}

function saveSubscriber() {

    const input = $("#subscriber_form .validate-input .input-element");

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
            url: "/Blog/Index?handler=Subscribe",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            data: JSON.stringify({
                FirstName: $("#subscriber_first_name").val(),
                LastName: $("#subscriber_last_name").val(),
                EmailAddress: $("#subscriber_email_address").val()
            }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                processResultMessages(response.status, subscriberResultRow, subscriberResultTextContainer, response.messages);
                if (response.status === 2) {
                    clearSubscriberInfo();
                }
            },
            failure: function (xhr) {
                processResultMessages(0, subscriberResultRow, subscriberResultTextContainer, xhr.statusText);
            }
        });
    }
}