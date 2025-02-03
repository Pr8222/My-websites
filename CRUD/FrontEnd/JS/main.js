$(document).ready(function () {
  let table = $("#userTable").DataTable({
    ajax: {
      url: "http://localhost:5203/api/Users", // Ensure this is correct
      dataSrc: "", // Important: Since your API returns an array
    },
    stateSave: true,
    processing: true,
    language: {
        processing: "در حال پردازش...",
        search: "جستجو:",
        lengthMenu: "نمایش _MENU_ رکورد",
        info: "نمایش _START_ تا _END_ از _TOTAL_ رکورد",
        infoEmpty: "نمایش 0 تا 0 از 0 رکورد",
        infoFiltered: "(فیلتر شده از _MAX_ رکورد)",
        infoPostFix: "",
        loadingRecords: "در حال بارگزاری...",
        zeroRecords: "رکوردی یافت نشد",
        emptyTable: "اطلاعاتی در جدول وجود ندارد",
        paginate: {
          first: "اول",
          previous: "قبلی",
          next: "بعدی",
          last: "آخر",
        },
        aria: {
          sortAscending: ": فعال‌سازی برای مرتب‌سازی صعودی",
          sortDescending: ": فعال‌سازی برای مرتب‌سازی نزولی",
        },
      },
    columns: [
      { data: "id" },
      { data: "name" },
      { data: "email" },
      { data: "age" },
      {
        data: null,
        defaultContent: `
            <div class="action-buttons">
              <button class="btn btn-light btn-sm editBtn mx-1">Edit</button>
              <button class="btn btn-success btn-sm saveBtn mx-1" hidden>Save</button>
              <button class="btn btn-warning btn-sm modalBtn">Modal Edit</button>
              <button class="btn btn-danger btn-sm deleteBtn mx-1">Delete</button>
            </div>
                `,
      },
    ],
  });
  $(".addBtn").on("click", function() {
    $("#addModal").modal("show");
    $("#addModalBtn").off("click").on("click", function () {
        addUser();
    })
  });
  $("#userTable tbody").on("click", ".deleteBtn", function() {
    var row = $(this).closest("tr");
    deleteRow(row, table);
  });
  $("#userTable tbody").on("click", ".modalBtn", function() {
    $("#editModal").modal("show");
  });
  $("#userTable tbody").on("click", "saveBtn", function() {
  });

  // Defining The Add User function
  function addUser(){   
    var newUserData = {
        Id: $("#addId").val(),
        Name: $("#addName").val(),
        Email: $("#addEmail").val(),

    }

  }
  // Defining The Delete Row function
  function deleteRow(row, table) {
    if(
        confirm(
            `آیا از حذف سطر با آیدی "${row
                .find("td")
                .first()
                .html()}" اطمینان دارید؟`
        )
    ) {
        let rowData = table.row(row).data();
        let userId = rowData.id;
        console.log(userId);
        debugger;
        console.log("Row being deleted: ", rowData);

        // Remove the current row from the api that is connected to the database
        $.ajax({
            type: "Delete",
            url: `http://localhost:5203/api/Users/${userId}`,
            success: function (response) {
                alert(`user with id ${userId} has been deleted successfully.`);
                table.ajax.reload(null, false);
            },
            error: function (xhr, status, error) {
                error("Error deleting user: " + error);
            }
            
        });

    }
    }
});
