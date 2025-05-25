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
    if (
      ContainsHtml(newUserData.username) ||
      ContainsHtml(newUserData.email)
    ) {
      ev.preventDefault(); // Prevent form submission
      ShowToast("HTML tags are not allowed in any input fields.", "danger");
    }
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
  // Checks if the input has an html tag or not
  function ContainsHtml(input) {
    const htmlPatterns = /<[^>]*>/g;
    return htmlPatterns.test(input);
  }
  function ShowToast(message, type) {
    // Create a new toast element dynamically using jQuery
    $("#actionToast .toast-body").html(message);
    $("#actionToast")
      .removeClass("bg-success bg-danger bg-warning")
      .addClass(`bg-${type}`);
    let toast = new bootstrap.Toast($("#actionToast"));
    toast.show();
  }
});
