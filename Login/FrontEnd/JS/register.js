$(document).ready(function () {
  // Action after clicking on the register button
  $("#registerBtn").click(function (ev){
    ev.preventDefault();
    //declaring a varialbe that stores the values of the input tags
    var newUserData = {
      username: $("#username").val(),
      email: $("#userEmail").val(),
      password: $("#userPassword").val(),
      age: $("#userAge").val(),
    };
    if (newUserData.password !== $("#confirmPassword").val()) {
        return
    }
      // Calling the register function from the api
      $.ajax({
        type: "POST",
        url: "http://localhost:5224/api/Auth/Register",
        data: JSON.stringify(newUserData),
        contentType: "application/json",
        dataType: "json",
        success: function (response) {
          console.log("Response: " + response)
          document.location.href = "/HTML/login.html";
        },
        error: function (xhr, status, error) {
          console.log(error, status);
          console.log("Bye Bye")
        },
      });
    
  });
});
