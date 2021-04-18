const resultRow = $("#result_row");

const resultTextContainer = $("#result_text");

const fileElementId = "file_upload_container";

const uploadFilesButton = $("#upload_files");
const filesContainer = $("#file_upload_container");

let validEmailAddress = false;
let validName = false;

$(document).ready(function () {
    "use strict";

    uploadFilesButton.attr("disabled", true);
    filesContainer.attr("disabled", true);

    $("#client_name").on("change",
        function () {
            validName = $(this).val().length > 0;
            refreshUploadAbility();
        });

    $("#client_email_address").on("change",
        function() {
            postEmailAddress("#client_email_address");
        });

    $("#send_message").on("click", function (e) {
        e.preventDefault();
        sendMessage();
    });

    $("#file_upload_container").on("change",
        function () {
            const validExtensions = $(`#${fileElementId}`).attr("accept").split(",");
            const validated = validateUploads(fileElementId, validExtensions, resultRow, resultTextContainer, 25);
            uploadFilesButton.attr("disabled", !validated);
        });

    $("#upload_files").on("click",
        function(e) {
            e.preventDefault();
            uploadFiles();
        });
}); 

function postEmailAddress(id) {
    const input = $(id);

    let passed = true;

    validEmailAddress = true;

    if (validate(input) === false) {
        showValidationMessage(input);
        passed = false;
        validEmailAddress = false;
    }

    if (passed) {
        $.ajax({
            type: "POST",
            url: "/Contact/EmailAddressChange",
            beforeSend: function(xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            data: JSON.stringify({
                Name: $("#client_name").val(),
                EmailAddress: input.val()
            }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function(response) {
                if (response.status === 0) {
                    processResultMessages(0, resultRow, resultTextContainer, response.messages);
                }
            },
            failure: function(xhr) {
                processResultMessages(0, resultRow, resultTextContainer, xhr.statusText);
            }
        });
    }

    refreshUploadAbility();
}

function refreshUploadAbility() {
    if ((validName) && (validEmailAddress)) {
        filesContainer.attr("disabled", false);
    } else {
        filesContainer.attr("disabled", true);
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
            url: "/Contact/Send",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            data: JSON.stringify({
                Name: $("#client_name").val(),
                EmailAddress: $("#client_email_address").val(),
                TranslationDirection: $("input[name='translation-direction']:checked").val(),
                TranslationType: $("input[name='translation-type']:checked").val(),
                SubjectMatter: $("#subject_matter").val(),
                WordCount: $("#word_count").val(),
                Message: $("#message").val()
            }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function () {
                window.location.href = "/InquiryResult";
            },
            failure: function (xhr) {
                processResultMessages(0, resultRow, resultTextContainer, xhr.statusText);
            }
        });
    }
}

function uploadFiles() {
    const files = filesContainer.prop("files");
    
    if (files.length > 0) {

        toggleLoad(true);

        const fileList = new FormData();
        for (let i = 0; i < files.length; i++) {
            fileList.append("files", files[i]);
        }

        $.ajax({
            type: "POST",
            url: "/Contact/Upload",
            beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            data: fileList,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.status === 0) {
                    processResultMessages(0, resultRow, resultTextContainer, response.messages);
                } else {
                    processResultMessages(1, resultRow, resultTextContainer, "Files uploaded.");
                }
                toggleLoad(false);
                filesContainer.val("");
            },
            failure: function (xhr) {
                processResultMessages(0, resultRow, resultTextContainer, xhr.statusText);
                toggleLoad(false);
                filesContainer.val("");
            }
        });
    }
}