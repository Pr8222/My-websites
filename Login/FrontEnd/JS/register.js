$(document).ready(function () {
  // Action after clicking on the register button
  $("#registerBtn").click(function (ev) {
    ev.preventDefault();
    //declaring a varialbe that stores the values of the input tags
    var newUserData = {
      username: $("#username").val(),
      email: $("#userEmail").val(),
      password: $("#userPassword").val(),
      age: $("#userAge").val(),
    };
    if (newUserData.password !== $("#confirmPassword").val()) {
      ShowToast("The password doesn't match.", "warning");
    }
    // Calling the register function from the api
    $.ajax({
      type: "POST",
      url: "http://localhost:5224/api/Auth/Register",
      data: JSON.stringify(newUserData),
      contentType: "application/json",
      success: function (response) {
        document.location.href = "/HTML/login.html";
      },
      error: function (xhr, status, error) {
        let responseError = xhr.responseJSON.errors
          ? xhr.responseJSON.errors
          : null;

        if (responseError) {
          // Checking if the username error
          if (responseError.UserName) {
            ShowToast(responseError.UserName[0], "danger");
          }

          // Checking for the email error
          if (responseError.Email) {
            ShowToast(responseError.Email[0], "danger");
          }

          // Checinf for the password error
          if (responseError.Password) {
            ShowToast(responseError.Password[0], "danger");
          }
        } else {
          ShowToast(error, "danger");
        }
      },
    });
  });
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
