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
              <button class="btn btn-primary btn-sm modalBtn">تغییر اطلاعات</button>
              <button class="btn btn-danger btn-sm deleteBtn mx-1">پاک کردن</button>
            </div>
                `,
      },
    ],
  });
  $(".addBtn").on("click", function () {
    $("#addModal").modal("show");
    $("#addModalBtn")
      .off("click")
      .on("click", function () {
        addUser();
      });
  });
  $("#userTable tbody").on("click", ".deleteBtn", function () {
    var row = $(this).closest("tr");
    deleteRow(row, table);
  });
  $("#userTable tbody").on("click", ".modalBtn", function () {
    $("#editModal").modal("show");
  });

  // Defining The Add User function
  function addUser() {
    // Check if passwords match
    if ($("#addPassword").val() !== $("#addConfirmPassword").val()) {
      $("#addConfirmPassword").addClass("is-invalid"); // Make field red
      $("#addPassword").addClass("is-invalid");
      alert("The password doesn't match!");
      return; // Stop function execution
    } else {
      $("#addConfirmPassword").removeClass("is-invalid"); // Remove red border if fixed
      $("#addPassword").removeClass("is-invalid");
    }
    var newUserData = {
      Id: $("#addId").val(),
      Name: $("#addName").val(),
      Email: $("#addEmail").val(),
      Password: $("#addPassword").val(),
      Age: $("#addAge").val(),
    };
    $.ajax({
      type: "Post",
      url: "http://localhost:5203/api/Users",
      data: JSON.stringify(newUserData),
      contentType: "application/json",
      dataType: "json",
      success: function (response) {
        //alert("New user has been created successfully!");
        showToast("کاربر جدید با موفقیت ساخته شد", "success");
        table.ajax.reload(null, false);
      },
      error: function (xhr, status, error) {
        showToast("در هنگام ساخت کاربر مشکلی ایجاد شد " + error, "danger");
      },
    });
    $("#addModal").modal("hide");
  }
  // Defining The Delete Row function
  function deleteRow(row, table) {
    if (
      confirm(
        `آیا از حذف سطر با آیدی "${row
          .find("td")
          .first()
          .html()}" اطمینان دارید؟`
      )
    ) {
      let rowData = table.row(row).data();
      let userId = rowData.id;
      console.log("Row being deleted: ", rowData);

      // Remove the current row from the api that is connected to the database
      $.ajax({
        type: "Delete",
        url: `http://localhost:5203/api/Users/${userId}`,
        success: function (response) {
          showToast(`کاربر با شناسه ${userId} با موفقیت حذف شد!`, "success");
          table.ajax.reload(null, false);
        },
        error: function (xhr, status, error) {
          showToast("خطا در هنگام حذف کردن کاربر " + error, "danger");
        },
      });
    } else {
      showToast("کاربر حذف نشد.", "warning");
    }
  }
  function showToast(message, type) {
    $("#actionToast .toast-body").text(message);
    $("#actionToast").removeClass("bg-success bg-danger bg-warning").addClass(`bg-${type}`);
    let toast = new bootstrap.Toast($("#actionToast"));
    toast.show();
}
});
