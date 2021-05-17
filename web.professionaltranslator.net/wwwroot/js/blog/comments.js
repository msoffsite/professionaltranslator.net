const commentResultRow = $("#comment_result_row");

const commentResultTextContainer = $("#comment_result_text");

$(document).ready(function () {
    "use strict";

    $("#save_comment").on("click", function (e) {
        e.preventDefault();
        savePost();
    });
});

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
        $.ajax({
            type: "POST",
            url: "/Blog/Index?handler=Comment",
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
                processResultMessages(response.status, commentResultRow, commentResultTextContainer, response.messages);
                if (response.status === 2) {
                    clearSubscriberInfo();
                }
            },
            failure: function (xhr) {
                processResultMessages(0, commentResultRow, commentResultTextContainer, xhr.statusText);
            }
        });
    }
}