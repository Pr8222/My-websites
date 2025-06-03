$(document).ready(function () {
  let table = $("#keysTable").DataTable({
    processing: true,
    stateSave: true,
    ajax: {
      url: "http://localhost:5224/api/User/showKeys",
      type: "GET",
      dataSrc: "",
      headers: {
        Authorization: "Bearer " + localStorage.getItem("token"),
      },
    },
    columns: [
      {
        data: "id",
      },
      {
        data: "keyName",
      },
      {
        data: "friendlyKeyName",
      },
    ],
  });
 

  // Go back to the adminDashboard
  $("#go-back").on("click", (ev) => {
    ev.preventDefault();
    document.location.href = "/HTML/adminDashboard.html";
  });


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
