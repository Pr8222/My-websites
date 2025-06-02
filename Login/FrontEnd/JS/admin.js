$(document).ready(function () {
  // Opening the modal to edit the admin or super admin information
  $("#editAdmin").on("click", function (ev) {
    ev.preventDefault();
    // Editing the admin information
    $.ajax({
      type: "GET",
      url: `http://localhost:5224/api/User/userPage?username=${localStorage.getItem(
        "username"
      )}`,
      headers: {
        Authorization: "Bearer " + localStorage.getItem("token"),
      },
      success: function (response) {
        $("#editUsername").val(response.userName);
        $("#editEmail").val(response.email);
        $("#editAge").val(response.age);
        $("#modalEditing").modal("show");

        $("#saveBtn").click(function (e) {
          e.preventDefault();
          if (
            $("#editPassword").val().trim() !==
            $("#confirmPassword").val().trim()
          ) {
            ShowToast("Passwords do not match!", "danger");
            return;
          }
          var updatedData = {
            username: $("#editUsername").val().trim(),
            email: $("#editEmail").val().trim(),
            age: $("#editAge").val().trim(),
            password: $("#editPassword").val().trim(),
          };

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
            }, error: function (error, xhr, status) {
              ShowToast("Unable to edit the user info!", "danger")
            }
          });
        });
      },
    });
  });

  // Logs out the admin
  $("#logout").on("click", (ev) => {
    ev.preventDefault();

    localStorage.removeItem("token");
    sessionStorage.removeItem("token");
    document.cookie = "token=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";

    document.location.href = "/HTML/login.html";
  });

  // Opening the key pannels
  $("#editKeys").click(function (e) {
    e.preventDefault();
    document.location.href = "/HTML/showKeys.html";
  });

  // Opening user management pannel
  $("#userManagement").click(function (e) {
    e.preventDefault();
    document.location.href = "/HTML/userManagement.html";
  });

  function ShowToast(message, type) {
    $("#actionToast .toast-body").html(message);
    $("#actionToast")
      .removeClass("bg-success bg-danger bg-warning")
      .addClass(`bg-${type}`);
    let toast = new bootstrap.Toast($("#actionToast"));
    toast.show();
  }
});
