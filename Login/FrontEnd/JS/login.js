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
        const jsonToken  = ParseJWT(token);
        if (token) {
          localStorage.setItem("username", userData.username);
          localStorage.setItem("token", token);
          localStorage.setItem("role", jsonToken.role)
          CheckUserRole(userData.username);
        }
      },
      error: function (xhr, status, error) {
        ShowToast("No such user.", "danger");
      },
    });
  });

  function ParseJWT(JWTToken) {
    const base64Url = JWTToken.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(
        atob(base64)
            .split('')
            .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
            .join('')
    );

    return JSON.parse(jsonPayload);
  }

  function CheckUserRole(username) {
    $.ajax({
      type: "Get",
      url: `http://localhost:5224/api/User/userPage?username=${username}`,
      success: function (response) {
        if (response.role === "Admin" || response.role === "SuperAdmin") {
          document.location.href = "/HTML/adminDashboard.html";
          alert(`Welcome admin ${username}`);
        } else {
          document.location.href = "/HTML/dashboard.html";
          alert(`Welcome dear ${username}`);
        }
      },
      error: function () {
        document.location.href = "/HTML/login.html";
        ShowToast("User not found", "danger");
      },
    });
  }


  // Creating a pop-up notification function
  function ShowToast(message, type) {
    // Create a new toast element dynamically using jQuery
    var toast = $(
      '<div class="toast fade" role="alert" aria-live="assertive" aria-atomic="true"></div>'
    );
    toast.addClass(`bg-${type}`);

    // Add toast content
    toast.append(`
      <div class="toast-body" style="display: flex; justify-content: space-between;" >
          <p style="color: #FFF; font-size: 14px;">${message}</p>
          <button type="button" class="btn-close" data-bs-dismiss="toast" aria-label="Close" style="color: #FFF"></button>
      </div>
  `);
    // Append the toast to the container
    $("#toastContainer").append(toast);

    // Create a new Bootstrap toast instance and show it
    var bootstrapToast = new bootstrap.Toast(toast[0]);
    bootstrapToast.show();
    // Optional: Remove toast after 3 seconds
    setTimeout(function () {
      toast.remove();
    }, 5000);
  }
});
