$(document).ready(function () {
    "use strict";

    const uploadResultRow = $("#upload_result_row");
    const uploadResultText = $("#upload_error_text");
    const workResultRow = $("#work_result_row");
    const workResultText = $("#work_result_text");

    var input = $(".validate-input .input-element");

    $("#save_work").on("click", function (e) {
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
                url: "/Admin/EditPortfolio?handler=Save",
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
                    if (response.status === 0) {
                        processResultMessages(workResultRow, workResultText, response);
                    } else {
                        processResultMessages(workResultRow, workResultText, "Work saved to portfolio.");
                    }
                },
                failure: function (xhr) {
                    processResultMessages(workResultRow, workResultText, xhr.statusText);
                }
            });
        }
    });

    $("#upload_cover").on("click", function (evt) {

        evt.preventDefault();

        uploadResultText.html("");

        $.ajax({
            url: "/Admin/EditPortfolio?handler=UploadImage",
            data: new FormData(document.getElementById("upload_form")),
            contentType: false,
            processData: false,
            type: "post",
            success: function (response) {
                if (response.status === 0) {
                    processResultMessages(uploadResultRow, uploadResultText, response);
                } else {
                    const imgSrc = response.messages[0];
                    $("#displayed_cover").attr("src", imgSrc);
                }
                
            },
            failure: function (xhr) {
                processResultMessages(uploadResultRow, uploadResultText, xhr.statusText);
            }
        });
    });

    function processResultMessages(row, textContainer, response) {
        row.fadeIn(1000);

        let html = "";
        if (response.messages.constructor === Array) {
            const messageArray = response.messages;
            for (let i = 0; i < messageArray.length; ++i) {
                html += messageArray[i];
                html += "<br />";
            }
        } else {
            html = response;
        }
        
        textContainer.html(`<p>${html}</p>`);
        row.fadeOut(15000);
    }
});