$(document).ready(function () {
  $("#logoutBtn").click(function (ev) {
    ev.preventDefault();
    // Remove the token from local storage
    localStorage.removeItem("token");
    //Remove the toke from the session storage
    sessionStorage.removeItem("token");
    // Remove the token from the cookie
    document.cookie = "token=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
    // Redirect the user to the login page
    document.location.href = "/HTML/login.html";
  });

  $("#deleteBtn").click(function (ev) {
    ev.preventDefault();
    if (confirm("Are you sure to delete the account?")) {
    $.ajax({
      type: "DELETE",
      url: `http://localhost:5224/api/User/DeleteUser?username=${localStorage.getItem(
        "username"
      )}`,
      headers: {
        Authorization: "Bearer " + localStorage.getItem("token"),
      },
      success: function (response) {
        
          alert("Account Deleted Successfully");
          // Remove the token from local storage
          localStorage.removeItem("token");
          //Remove the toke from the session storage
          sessionStorage.removeItem("token");
          // Remove the token from the cookie
          document.cookie =
            "token=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
          // Redirect the user to the login page
          document.location.href = "/HTML/login.html";
      },
    });
  }
  else{
    ShowToast(`User with username <u>${localStorage.getItem("username")}</u> has not been deleted.`, "warning");
  }
  });

  $("#WelcomeUser").html(
    `Dear ${escapeHTML(localStorage.getItem("username"))}!`
  );

  $("#editBtn").click(function (ev) {
    ev.preventDefault();

    var updatedData = {
      username: $("#username").val().trim(),
      email: $("#userEmail").val().trim(),
      age: $("#userAge").val().trim(),
      password: $("#userPassword").val().trim(),
    };

    // Check if passwords match before proceeding
    if ($("#userPassword").val() !== $("#confirmPassword").val()) {
      alert("Passwords do not match!");
      return;
    }

    // Send GET request to get current user data
    $.ajax({
      type: "GET",
      url: `http://localhost:5224/api/User/userPage?username=${localStorage.getItem(
        "username"
      )}`,
      headers: {
        Authorization: "Bearer " + localStorage.getItem("token"),
      },
      success: function (response) {
        // Fallback to the existing values from the GET response if the user entered nothing
        updatedData.username = updatedData.username || response.userName;
        updatedData.email = updatedData.email || response.email;
        updatedData.age = updatedData.age || response.age;

        // If the password is provided, include it in updatedData, else set it to an empty string
        updatedData.password =
          $("#userPassword").val().trim() == null
            ? ""
            : $("#userPassword").val().trim();

        // Send the PUT request to update user data
        $.ajax({
          type: "PUT",
          url: `http://localhost:5224/api/User/EditUser?username=${localStorage.getItem(
            "username"
          )}`,
          data: JSON.stringify(updatedData),
          contentType: "application/json",
          dataType: "json",
          headers: {
            Authorization: "Bearer " + localStorage.getItem("token"),
          },
          success: function (response) {
            localStorage.setItem("username", updatedData.username);
            location.reload();
            ShowToast("The account has been changed successfully", "success");
          },
          error: function (xhr, status, error) {
            ShowToast("Unable to edit the user info.", "danger");
          },
        });
      },
      error: function (xhr, status, error) {
        ShowToast("Unable to get the user data from the server!", "danger");
      },
    });
  });
  function escapeHTML(text) {
    return $("<div>").text(text).html(); // Converts special characters to HTML entities
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
