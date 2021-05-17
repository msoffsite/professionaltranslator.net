function deletePost(postId) {
    const aspPage = $("#blog_asp_page").val();

    $.ajax({
        type: "POST",
        url: `/Blog/${aspPage}?handler=DeletePost`,
        data: {
            postId: postId
        },
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        }
    }).done(function (result) {
        $("#blog_directory_container").html(result);
        directoryTop();
    });
}
    function directoryTop() {
        const top = document.getElementById("directoryTop").offsetTop;
        window.scrollTo(0, top);
    }

    function filterDirectory(category) {

        const aspPage = $("#blog_asp_page").val();

        $.ajax({
            type: "POST",
            url: `/Blog/${aspPage}?handler=FilterDirectory`,
            data: {
                category: category
            },
            beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            }
        }).done(function (result) {
            $("#blog_directory_container").html(result);
            directoryTop();
        });
    }

    function showNextDirectoryPage() {

        const aspPage = $("#blog_asp_page").val();

        $.ajax({
            type: "POST",
            url: `/Blog/${aspPage}?handler=ShowDirectoryNextPage`,
            beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            }
        }).done(function (result) {
            $("#blog_directory_container").html(result);
            directoryTop();
        });
    }

    function showPreviousDirectoryPage() {

        const aspPage = $("#blog_asp_page").val();

        $.ajax({
            type: "POST",
            url: `/Blog/${aspPage}?handler=ShowDirectoryPreviousPage`,
            beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            }
        }).done(function (result) {
            $("#blog_directory_container").html(result);
            directoryTop();
        });
    }

    $(document).ready(function () {

        $("#blog_directory_next_href").on("click",
            function () {
                showNextDirectoryPage();
            });

        $("#blog_directory_previous_href").on("click",
            function() {
                showPreviousDirectoryPage();
            });

        $(".category-link").on("click",
            function () {
                const category = $(this).data("category");
                filterDirectory(category);
            });

        $(".delete-post").on("click",
            function () {
                const postId = $(this).data("post-id");
                const postTitle = $(this).data("post-title");
                const deleteMessage = `Are you sure you wish to delete "${postTitle}"?`;
                if (confirm(deleteMessage)) {
                    deletePost(postId);
                }
            });

    });