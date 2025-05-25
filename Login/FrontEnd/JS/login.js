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
        const jsonToken = ParseJWT(token);
        if (token) {
          localStorage.setItem("username", userData.username);
          localStorage.setItem("token", token);
          localStorage.setItem("role", jsonToken.role);
          console.log(userData.username);
          CheckUserRole(userData.username);
        }
      },
      error: function (xhr, status, error) {
        console.log(error);
        debugger;
        ShowToast("No such user.", "danger");
      },
    });
  });

  function ParseJWT(JWTToken) {
    const base64Url = JWTToken.split(".")[1];
    const base64 = base64Url.replace(/-/g, "+").replace(/_/g, "/");
    const jsonPayload = decodeURIComponent(
      atob(base64)
        .split("")
        .map((c) => "%" + ("00" + c.charCodeAt(0).toString(16)).slice(-2))
        .join("")
    );

    return JSON.parse(jsonPayload);
  }

  function CheckUserRole(username) {
    $.ajax({
      type: "Get",
      url: `http://localhost:5224/api/User/userPage?username=${username}`,
      headers: {
        Authorization: "Bearer " + localStorage.getItem("token"),
      },
      success: function (response) {
        if (response.role == "Admin" || response.role == "SuperAdmin") {
          document.location.href = "/HTML/adminDashboard.html";
        } else {
          document.location.href = "/HTML/dashboard.html";
        }
      },
      error: function () {
        ShowToast("User not found!", "danger");
      },
    });
  }

  // Creating a pop-up notification function
  function ShowToast(message, type) {
    $("#actionToast .toast-body").html(message);
    $("#actionToast")
      .removeClass("bg-success bg-danger bg-warning")
      .addClass(`bg-${type}`);
    let toast = new bootstrap.Toast($("#actionToast"));
    toast.show();
  }
});
