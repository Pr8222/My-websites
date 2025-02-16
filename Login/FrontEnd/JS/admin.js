$(document).ready(function () {

  let userRole = localStorage.getItem("role");
  let table = $("#usersTable").DataTable({
    processing: true,
    stateSave: true,
    serverSide: true,
    ajax: {
      url: "http://localhost:5224/api/User",
      type: "GET",
      beforeSend: function (xhr) {
        const token = localStorage.getItem("token"); // Retrieve token
        if (token) {
          xhr.setRequestHeader("Authorization", "Bearer " + token); // Set JWT token
        }
      },
      dataSrc: ""
    },
    columns: [
        {data: "id"},
        {data: "userName"},
        {data: "email"},
        {data: "age"},
        {data: "role"},
        {
            data: null,
            defaultContent: `
            <div class="action-buttons">
                <button class="superAdminOnly promoteUser" disabled>Promote</button>
                <button class="superAdminOnly demoteUser" disabled>Demote</button>
            </div>
            `
        }
    ]
  });

  $(".verification").on("click", () => {
    if(userRole === "SuperAdmin") {
      $(".promoteUser").removeAttr("disabled");
      $(".demoteUser").removeAttr("disabled");
    } else {
      console.error("Sorry you are not a super User!");
    }
  })

  $("#usersTable tbody").on("click", ".promoteUser", function () {
    var row = $(this).closest("tr");
    Promote(table, row);
  });

  $("#usersTable tbody").on("click", ".demoteUser", function () {
    var row = $(this).closest("tr");
    Demote(table, row);
  });

  function Promote(table, row) {

  }
});
