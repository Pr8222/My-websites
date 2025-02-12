$(document).ready(function () {
    $("#logoutBtn").click(function (ev) {
        ev.preventDefault();
        // Remove the token from local storage
        localStorage.removeItem('token');
        //Remove the toke from the session storage
        sessionStorage.removeItem('token');
        // Remove the token from the cookie
        document.cookie = "token=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
        // Redirect the user to the login page
        document.location.href = "/HTML/login.html";
    });

    $("#deleteBtn").click(function(ev) {
        ev.preventDefault();
        $.ajax({
            type: "DELETE",
            url:`http://localhost:5224/api/User/DeleteUser?username=${username}`,
            headers: {
                "Authorization": "Bearer " + localStorage.getItem("token")
            },
            success: function (response) {
                alert("Account Deleted Successfully");
                document.location.href = "/HTML/login.html";
            }
        });
    });

    $("#editInfoBtn").click(function(ev) {
        ev.preventDefault();
        if($("#userPassword").val() === $("#confirmPassword")) {
            // Declaring a variable to store the input tags values
        var updatedData = {
            username: $("#username").val(),
            email: $("#userEmail").val(),
            age: $("#userAge").val(),
            password: $("#userPassword").val()
        };
        $.ajax({
            type: "PUT",
            url: `http://localhost:5224/api/User/EditUser?username=${username}`,
            data: JSON.stringify(updatedData),
            contentType: "application/json",
            dataType: "json",
            headers: {
                "Authorization": "Bearer " + localStorage.getItem("token")
            },
            success: function (response) {
                alert("Account has been edited successfully!");
                location.reload();
            }, error: function (xhr, status, error) {
                alert("Unable to edit the user. " + error);
            }
        });
        }
    })
});