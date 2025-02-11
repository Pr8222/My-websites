$(document).ready(function () {
    // Declaring the username and password
    $("#login-btn").click( function(event) {
        event.preventDefault();

        
    // Declaring a variable to store the input values
    var userData = {
        username: $("#username").val(),
        password: $("#userPassword").val()
    }
        $.ajax({
            type: "POST",
            url: "http://localhost:5224/api/Auth/login",
            data: JSON.stringify(userData),
            contentType: "application/json",
            dataType: "json",
            success: function (response) {
                document.location.href = "/HTML/userPage.html";
            }, error: function (xhr, status, error) {
                alert(error)
            }
        });
    });    
});