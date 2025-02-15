$(document).ready(function () {
  // Declaring the username and password
  $("#login-btn").click(function (event) {
    event.preventDefault();

    // Declaring a variable to store the input values
    var userData = {
      username: $("#username").val(),
      password: $("#userPassword").val(),
    };
    $.ajax({
      type: "POST",
      url: "http://localhost:5224/api/Auth/login",
      data: JSON.stringify(userData),
      contentType: "application/json",
      dataType: "json",
      success: function (response) {
        let token = response.token;

        if (token) {
          localStorage.setItem("username", userData.username);
          localStorage.setItem("token", token);
          CheckUserRole(userData.username);
        }
      },
      error: function (xhr, status, error) {
        alert(error);
      },
    });
  });

  function CheckUserRole(username) {
    $.ajax({
      type: "Get",
      url: `http://localhost:5224/api/User/userPage?username=${username}`,
      success: function (response) {
        if (response.role === "Admin" || response.role === "SuperAdmin") {
          document.location.href = "/HTML/adminDashboard.html";
          alert(`Wellcome admin ${username}`);
        } else {
          document.location.href = "/HTML/dashboard.html";
          alert("Wellcome dear user")
        }
      },
      error: function () {
        document.location.href = "/HTML/login.html";
        alert("User not found")
      },
    });
  }
});
