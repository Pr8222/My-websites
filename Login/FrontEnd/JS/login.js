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

                let token = response.token;

                if(token) {
                    localStorage.setItem('token', token);
                    alert("login successful");
                    document.location.href = "/HTML/dashboard.html"
                }
            }, error: function (xhr, status, error) {
                alert(error)
            }
        });
    });    
});