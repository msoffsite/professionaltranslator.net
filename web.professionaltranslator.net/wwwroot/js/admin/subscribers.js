function copyToClipboard(inputId) {
    const input = document.getElementById(inputId);
    input.select();
    input.setSelectionRange(0, input.value.length);
    document.execCommand("copy");
    alert("Copied to clipboard!");
}

function deleteSubscriber(subscriberEmailAddress) {
    if (confirm(`Are you sure you want to delete ${subscriberEmailAddress}?`)) {
        $.ajax({
            type: "POST",
            url: `/Admin/Subscribers/?handler=DeleteSubscriber`,
            beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            data: {
                emailAddress: subscriberEmailAddress
            },
            success: function (response) {
                alert(response.messages);
                if (response.status > 0) {
                    location.href = "/Admin/Subscribers";
                }
            },
            failure: function (xhr) {
                alert(xhr.statusText);
            }
        });
    }
}

$(document).ready(function () {

    $("#copy_email_addresses").on("click",
        function () {
            copyToClipboard("email_list");
        });

    $(".email-subscriber-link").on("click",
        function () {
            const emailInputId = $(this).data("input-id");
            copyToClipboard(emailInputId);
        });

    $(".remove-subscriber-link").on("click",
        function () {
            const subscriberEmailAddress = $(this).data("subscriber-email-address");
            deleteSubscriber(subscriberEmailAddress);
        });
});