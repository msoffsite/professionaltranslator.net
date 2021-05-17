function loadDirectory() {

    const aspPage = $("#blog_asp_page").val();

    $.ajax({
        type: "POST",
        url: `/Blog/${aspPage}?handler=ShowDirectory`,
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        }
    }).done(function (result) {
        $("#blog_directory_container").html(result);
    });
}

$(document).ready(function () {

    loadDirectory();
});