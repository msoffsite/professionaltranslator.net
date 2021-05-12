const postResultRow = $("#post_result_row");

const postResultTextContainer = $("#post_result_text");

$("iframe").contents().find("body").attr("contenteditable", "true");

$(document).ready(function () {
    "use strict";

    $("#save_post").on("click", function (e) {
        e.preventDefault();
        savePost();
    });

    $(document).on("keyup",
        function() {
            alert($("iframe").contents().find("body").val());
        });

    $("iframe").contents().find("body").on("keyup",
        function () {
            alert("change detected");
            const postContent = $("iframe").contents().find("body").val();
            if (postContent.length > 0) {
                $("#post_content_validator").val(postContent);
                $("#post_content_validator").addClass("hide");
            } else {
                $("#post_content_validator").val("");
                $("#post_content_validator").removeClass("hide");
            }
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
                console.log(JSON.stringify(response));
                processResultMessages(response.status, postResultRow, postResultTextContainer, response.messages);
                if (response.status === 2) {
                    clearSubscriberInfo();
                }
            },
            failure: function (xhr) {
                processResultMessages(0, postResultRow, postResultTextContainer, xhr.statusText);
            }
        });
    }
}