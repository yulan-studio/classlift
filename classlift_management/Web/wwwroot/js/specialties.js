/*$(document).ready(function () {*/
// Load Add/Edit Form in Modal
function loadAddForm() {
    $.get("/Specialty/Add/", function (data) {
        $("#modalContent").html(data);
        $("#specialtyModal").modal("show");
    });
}

function loadEditForm(specialtyId) {
    $.get("/Specialty/Edit/" + specialtyId, function (data) {
        $("#modalContent").html(data);
        $("#specialtyModal").modal("show");
    });
}


// Submit Add/Edit Form
//function saveSpecialty() {
//    var formData = $("#specialtyForm").serialize();
//    $.post("/Specialty/Save?" + new Date().getTime(), formData, function (response) {
//        console.log(response);
//        if (response.success) 
//        {
//            location.reload();  // Refresh list after saving
//        } 
//        else 
//        {
//            $("#errorMessage").text(response.message).show();
//        }
//    });

    //$.post("/City/Save", formData)
    //    .done(function (response) {
    //        console.log("Response received:", response);
    //        if (response.success) {
    //            alert("City saved successfully!"); 
    //            location.reload(); // ✅ Reload the page if successful
    //        } else {
    //            $("#errorMessage").text(response.message).show(); // ✅ Show error in modal
    //        }
    //    })
    //    .fail(function () {
    //        $("#errorMessage").text("An unexpected error occurred.").show();
    //        alert("An error occurred while saving.");
    //    });
//}

// Delete City
function loadDeleteSpecialtyConfirm(specialtyId) {

    $.get("/Specialty/DeleteConfirm/" + specialtyId, function (data) {
        $("#modalContent").html(data);
        $("#specialtyModal").modal("show");
    });

}


function deleteSpecialty(specialtyId) {
    //if (!confirm("Are you sure you want to delete this city?")) return;
    $.post("/Specialty/Delete/" + specialtyId, function (response) {
        if (response.success) {
            $("#row-" + specialtyId).remove(); // Remove deleted city row
            location.reload();  // Refresh list after delete
        } else {
            alert("Failed to delete specialty.");
        }
    });
}
/*});*/



