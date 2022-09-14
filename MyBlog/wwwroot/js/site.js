// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function SaveComment() {
    var formData = $("#commentForm").serialize();
    $.ajax({
        url: "/Comment/Save",
        type: "POST",
        data: formData,
        success: function (response) {
            $("#CommentText").val('');
            GetComments('Read');
        },
        error: function (request, status, error) {
            alert(request.responseText);
        }
    });
}

function EditComment(id, content) {
    $("#CommentId").val(id);
    $("#CommentText").val(content);
    $("#CommentText").focus();
};

function DeleteComment(id) {
    $.ajax({
        url: "/Comment/Delete",
        type: "POST",
        data: { commentId: id },
        success: function (response) {
            GetComments();
        }
    });
}

function UpdateComment(status) {
    var formData = $("#commentForm").serialize();
    $.ajax({
        url: "/Comment/Update",
        type: "POST",
        data: formData,
        success: function (response) {
            $("#CommentText").val('');
            GetComments(status);
        },
        error: function (request, status, error) {
            alert(request.responseText);
        }
    });
}
function GetComments(status) {
    $.ajax({
        url: "/Comment/GetAll",
        type: "POST",
        data: {
            articleId: $("#articleId").val(),
            status: status,
        },
        dataType: "html",
        success: function (result) {
            $("#partial").html(result);
        }
    });
}

function SaveArticle() {
    CKupdate();
    var formData = $("#editArticleForm").serialize();
    confirm("Вы уверены?");
    $.ajax({
        url: "/Article/Save",
        type: "POST",
        data: formData,
        success: function (response) {
            alert(response);
        },
        error: function (request, status, error) {
            //alert(request.responseText);
            alert("Что-то пошло не так. Возможно отсутствует интернет соединение.");
        }
    });
}

function CKupdate() {
    for (instance in CKEDITOR.instances)
        CKEDITOR.instances[instance].updateElement();
}

document.addEventListener("DOMContentLoaded", function (event) {
    var editor = CKEDITOR.replace('content');
});
