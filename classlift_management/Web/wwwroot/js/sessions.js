
function loadAddForm() {
    $.get("/City/Add/", function (data) {
        $("#modalContent").html(data);
        $("#cityModal").modal("show");
    });
}

function loadEditSessionForm(enrollmentId) {
    $.get("/Course/EditSession/" + enrollmentId, function (data) {
        $("#modalContent").html(data);
        $("#sessionModal").modal("show");
    });
}


function loadDeleteSessionConfirm(enrollmentId) {

    $.get("/Course/DeleteSessionConfirm/" + enrollmentId, function (data) {
        $("#modalContent").html(data);
        $("#sessionModal").modal("show");
    });

}


// Submit Add/Edit Form
function saveSession() {
    var formData = $("#sessionForm").serialize();
    $.post("/Course/SaveSession?" + new Date().getTime(), formData, function (response) {
        if (response.success) {
            location.reload();  // Refresh list after saving
        } else {
            $("#errorMessage").text(response.message).show();
        }
    });
  
}

// Delete City


function loadDeleteSessionConfirm(enrollmentId) {

    $.get("/Course/DeleteSessionConfirm/" + enrollmentId, function (data) {
        $("#modalContent").html(data);
        $("#sessionModal").modal("show");
    });

}


function deleteSession(enrollmentId) {
   
    $.post("/Course/DeleteSessionConfirmed/" + enrollmentId, function (response) {
        if (response.success) {
            $("#row-" + enrollmentId).remove(); // Remove deleted city row
            location.reload();  // Refresh list after delete
        } else {
            alert("Failed to delete session.");
        }
    });
}




