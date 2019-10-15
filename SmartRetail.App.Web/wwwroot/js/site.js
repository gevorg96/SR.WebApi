// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(function () {
    var placeholderElement = $('#modal-placeholder');
    $('button[data-toggle="modal"]').click(function (event) {
        var url = $(this).data('url');
        $.get(url).done(function (data) {
            placeholderElement .html(data);
            placeholderElement.find('.modal').modal('show');
        });
    });
});