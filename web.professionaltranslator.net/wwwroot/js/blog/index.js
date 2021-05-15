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

$(document).ready(function () {

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