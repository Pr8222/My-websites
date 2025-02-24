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
      password: $("#userPassword").val().trim()
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
            console.log(xhr, error);
          },
        });
      },
      error: function (xhr, status, error) {
        console.log(error);
      },
    });
  });
  function escapeHTML(text) {
    return $("<div>").text(text).html(); // Converts special characters to HTML entities
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
