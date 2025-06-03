$(document).ready(function () {
  let table = $("#usersTable").DataTable({
    processing: true,
    stateSave: true,
    ajax: {
      url: "http://localhost:5224/api/User",
      type: "GET",
      dataSrc: "data",
      responsive: true,
      headers: {
        Authorization: "Bearer " + localStorage.getItem("token"),
      },
    },
    initComplete: function () {
      var column = this.api().column(4); // Adjust column index as needed

      var select = $('<select><option value=""></option></select>')
        .appendTo($(column.footer()).empty())
        .on("change", function () {
          column.search($(this).val(), { exact: true }).draw();
        });

      // Use a Set to collect unique roles
      let roleSet = new Set();

      column.data().each(function (d) {
        // Split by comma and trim whitespace
        d.split(",").forEach(function (role) {
          roleSet.add(role.trim());
        });
      });

      // Add sorted unique roles to the select
      [...roleSet].sort().forEach(function (role) {
        select.append('<option value="' + role + '">' + role + "</option>");
      });
    },
    columns: [
      {
        data: "id",
        // Showing the ids as a link
        render: function (data, type, row) {
          return `<a class="openModal nowrap" data-id="${data}" style="cursor: pointer; color: #fff;
              text-decoration: none;
              font-weight: 600;">${data}</a>`;
        },
      },
      {
        data: "userName",
        render: function (data) {
          return EscapeHtml(data);
        },
      },
      {
        data: "email",
        render: function (data) {
          return EscapeHtml(data);
        },
      },
      { data: "age" },
      { data: "role" },
      {
        data: null,
        defaultContent: `
                <div class="action-buttons">
                    <button class="superAdminOnly promoteUser" hidden>Promote</button>
                    <button class="superAdminOnly demoteUser" hidden>Demote</button>
                </div>
                `,
      },
    ],
  });
  $(".verification").on("click", () => {
    if (localStorage.getItem("role") === "SuperAdmin") {
      table.rows().every(function () {
        var rowData = this.data();
        if (rowData.role === "Admin") {
          $(this.node()).find(".demoteUser").removeAttr("hidden");
        } else if (rowData.role === "User") {
          $(this.node()).find(".promoteUser").removeAttr("hidden");
        } else if (rowData.role === "SuperAdmin") {
          $(this.node()).find(".promoteUser").removeAttr("hidden");
          $(this.node()).find(".promoteUser").attr("disabled", true);
        }
      });
    } else {
      console.error("Sorry you are not a super User!");
    }
  });

  $("#usersTable tbody").on("click", ".promoteUser", function () {
    var row = $(this).closest("tr");
    Promote(table, row);
  });

  $("#usersTable tbody").on("click", ".demoteUser", function () {
    var row = $(this).closest("tr");
    Demote(table, row);
  });

  // Opening the modal to show all of the current user data
  $("#usersTable tbody").on("click", ".openModal", function () {
    var row = $(this).closest("tr");
    ShowCurrentUser(table, row);
  });

  // Opening the modal to edit the admin or super admin information
  $("#editAdmin").on("click", function (ev) {
    ev.preventDefault();
    $("#usersTable tbody tr").each(function () {
      var row = $(this);
      const rowUsername = row.find("td:eq(1)").text().trim();
      if (rowUsername == localStorage.getItem("username")) {
        var rowData = row
          .find("td")
          .map(function () {
            return $(this).text().trim();
          })
          .get();
        // Setting the values of each inputs
        $("#editId").val(rowData[0]);
        $("#editUsername").val(rowData[1]);
        $("#editEmail").val(rowData[2]);
        $("#editAge").val(rowData[3]);
        // Open modal
        $("#modalEditing").modal("show");
        $("#saveBtn")
          .off("click")
          .on("click", function () {
            ChangeInfo();
          });
      }
    });
  });

  // Logs out the admin
  $("#go-back").on("click", (ev) => {
    ev.preventDefault();
    document.location.href = "/HTML/adminDashboard.html";
  });

  // This function is called when the super admin wants to promote a normal user
  function Promote(table, row) {
    let rowData = table.row(row).data();
    let username = rowData.userName;

    // Call the promote api
    $.ajax({
      type: "PUT",
      url: `http://localhost:5224/api/User/Promote?username=${username}`,
      headers: {
        Authorization: "Bearer " + localStorage.getItem("token"),
      },
      success: function (response) {
        ShowToast(
          `The user with username <u>${username}</u> has been promoted to admin.`,
          "success"
        );
        table.ajax.reload(null, false);
      },
      error: function (xhr, status, error) {
        ShowToast(`Unable to promote <u>${username}</u>`, "danger");
      },
    });
  }
  // This function is called when the super admin wants to demote an admin
  function Demote(table, row) {
    // Getting the data of the selected row
    var rowData = table.row(row).data();
    var username = rowData.userName;

    $.ajax({
      type: "PUT",
      url: `http://localhost:5224/api/User/Demote?username=${username}`,
      headers: {
        Authorization: "Bearer " + localStorage.getItem("token"),
      },
      success: function (response) {
        ShowToast(
          `The admin with usernamer <u>${username}</u> has been demoted to user.`,
          "success"
        );
        table.ajax.reload(null, false);
      },
      error: function (xhr, status, error) {
        ShowToast(
          `Unable to demote admin with username <u>${username}</u>!`,
          "danger"
        );
      },
    });
  }
  // This function shows all of a user data
  function ShowCurrentUser(table, row) {
    var rowData = table.row(row).data();

    $(".userId").html(`<p>User id: ${rowData.id}</p>`);
    $(".userName").html(`<p>Username: ${EscapeHtml(rowData.userName)}</p>`);
    $(".userEmail").html(`<p>User email: ${EscapeHtml(rowData.email)}</p>`);
    $(".userAge").html(`<p>User age: ${rowData.age}</p>`);
    $(".userRole").html(`<p>User role: ${rowData.role}</p>`);
    $(".userKeys").html(
      `<p title="${rowData.keys}">User keys: ${rowData.keys}</p>`
    );
    $("#showModal").modal("show");
  }
  // This function edits superAdmin or admin information such as username, password, and email
  function ChangeInfo() {
    var updatedData = {
      username: $("#editUsername").val().trim(),
      email: $("#editEmail").val().trim(),
      password: $("#editPassword").val().trim(),
      age: $("#editAge").val().trim(),
    };
    $.ajax({
      type: "PUT",
      url: `http://localhost:5224/api/User/EditUser?username=${localStorage.getItem(
        "username"
      )}`,
      data: JSON.stringify(updatedData),
      dataType: "json",
      contentType: "application/json",
      headers: {
        Authorization: "Bearer " + localStorage.getItem("token"),
      },
      success: function (response) {
        localStorage.setItem("username", updatedData.username);
        location.reload();
      },
      error: function (error, xhr, status) {
        ShowToast("Unable to change the info!", "danger");
      },
    });
  }
  function ShowToast(message, type) {
    $("#actionToast .toast-body").html(message);
    $("#actionToast")
      .removeClass("bg-success bg-danger bg-warning")
      .addClass(`bg-${type}`);
    let toast = new bootstrap.Toast($("#actionToast"));
    toast.show();
  }
  function EscapeHtml(text) {
    return text
      ?.replace(/&/g, "&amp;")
      ?.replace(/</g, "&alt;")
      ?.replace(/>/g, "&gt;")
      ?.replace(/"/g, "&quot;")
      ?.replace(/'/g, "$#039");
  }
});
