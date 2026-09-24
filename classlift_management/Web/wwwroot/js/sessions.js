
function loadAddForm() {
    $.get("/City/Add/", function (data) {
        $("#modalContent").html(data);
        $("#cityModal").modal("show");
    });
}

function showSessionModal() {
    const modalElement = document.getElementById("sessionModal");
    if (!modalElement) {
        alert("Session dialog is not available on this page.");
        return;
    }

    if (window.bootstrap && bootstrap.Modal) {
        bootstrap.Modal.getOrCreateInstance(modalElement).show();
        return;
    }

    // Fallback for pages where the Bootstrap bundle has not loaded yet.
    modalElement.style.display = "block";
    modalElement.classList.add("show");
    modalElement.setAttribute("aria-hidden", "false");
}

function loadEditSessionForm(enrollmentId) {
    $.get("/Course/EditSession/" + enrollmentId, function (data) {
        $("#modalContent").html(data);
        showSessionModal();
    }).fail(function (xhr) {
        alert("Unable to load the session editor: " + (xhr.responseText || xhr.statusText));
    });
}


function loadDeleteSessionConfirm(enrollmentId) {

    $.get("/Course/DeleteSessionConfirm/" + enrollmentId, function (data) {
        $("#modalContent").html(data);
        showSessionModal();
    });

}


// Submit Add/Edit Form
function saveSession() {
    var formData = $("#sessionForm").serialize();
    $.post("/Course/SaveSession?" + new Date().getTime(), formData, function (response) {
        if (response.success) {
            location.reload();  // Refresh list after saving
        } else {
            const message = response.message || "The session could not be saved.";
            if ($("#errorMessage").length) {
                $("#errorMessage").text(message).show();
            } else {
                alert(message);
            }
        }
    }).fail(function (xhr) {
        const message = xhr.responseJSON?.message || xhr.responseText || xhr.statusText || "The session could not be saved.";
        alert("Save failed: " + message);
    });
  
}

// Delete City


function loadDeleteSessionConfirm(enrollmentId) {

    $.get("/Course/DeleteSessionConfirm/" + enrollmentId, function (data) {
        $("#modalContent").html(data);
        showSessionModal();
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




