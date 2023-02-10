$(document).ready(function () {
    $(".js-delete").on("click", function () {
        var btn = $(this);
        bootbox.confirm({
            message: "Are You Sure to Delete This User?",
            buttons: {
                confirm: {
                    label: 'Yes',
                    className: 'btn-danger'
                },
                cancel: {
                    label: 'No',
                    className: 'btn-secondary'
                }
            },
            callback: function (result) {
                if (result) {
                    $.ajax({

                        url: '/api/users/?userId=' + btn.data('id'),
                        type: "DELETE",
                        success: function () {
                            var UserContainer = btn.parents('tr');
                            UserContainer.addClass("animate__animated animate__zoomOutLeft");
                            setTimeout(function () {
                                UserContainer.remove();
                            }, 1000)
                            $('#alert').removeClass('d-none').addClass('d-block');
                        },
                        Error: function () {
                            alert("Error");

                        }

                    })
                }
            }
        });
    });

})