const addCommentResultRow = $("#addComment_result_row");

const addCommentResultTextContainer = $("#addComment_result_text");

function loadComments() {

    $.ajax({
        type: "POST",
        url: "/Blog/Index?handler=ShowComments",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        }
    }).done(function (result) {
        $("#comments_container").html(result);
    });
}

function saveComment() {

    const input = $("#comments_form .validate-input .input-element");

    let passed = true;

    for (let i = 0; i < input.length; i++) {
        if (validate(input[i]) === false) {
            showValidationMessage(input[i]);
            passed = false;
        }
    }

    if (passed) {

        //window.onbeforeunload = null;

        $.ajax({
            type: "POST",
            url: "/Blog/Index?handler=SaveComment",
            beforeSend: function(xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            data: {
                author: $("#comment_author").val(),
                email: $("#comment_email").val(),
                text: $("#comment_text").val(),
                exists: $("#website").length > 0 ? "true" : "false"
            }
        }).done(function (result) {
            $("#comments_container").html(result);
        }).fail(function (xhr) {
            processResultMessages(0, addCommentResultRow, addCommentResultTextContainer, xhr.statusText);
        });
    }
}

$(document).on("click",
    "#save_add_comment",
    function (e) {
        e.preventDefault();
        saveComment();
    });
$(document).ready(function () {

    //$("#save_add_comment").on("click",
    //    function () {
    //        alert("save comment");
    //        //saveComment();
    //    });

    $("#test_comments").on("click",
        function (e) {
            e.preventDefault();
            loadComments();
        });

    loadComments();

    $("p").each(function () {
        if ($(this).has("img").length > 0) {
            $(this).addClass("d-flex justify-content-center");
        }
    });
});