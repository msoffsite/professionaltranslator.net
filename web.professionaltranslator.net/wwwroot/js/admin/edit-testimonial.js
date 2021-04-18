$(document).ready(function () {
    "use strict";

    const resultRow = $("#result_row");

    const resultText = $("#result_text");
    resultText.html("");

    var input = $(".validate-input .input-element");

    $("#save_message").on("click", function (e) {
        e.preventDefault();
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
                url: "/Admin/EditTestimonial?handler=Save",
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                data: JSON.stringify({
                    Author: $("#author").val(),
                    EmailAddress: $("#email_address").val(),
                    Text: $("#testimonial").val()
                }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    processResultMessages(response.status, resultRow, resultText, response.messages);
                },
                failure: function (xhr) {
                    processResultMessages(0, resultRow, resultText, xhr.statusText);
                }
            });
        }
    });
});