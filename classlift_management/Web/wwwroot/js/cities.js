/*$(document).ready(function () {*/
// Load Add/Edit Form in Modal
function loadAddForm() {
    $.get("/City/Add/", function (data) {
        $("#modalContent").html(data);
        $("#cityModal").modal("show");
    });
}

function loadEditForm(cityId) {
    $.get("/City/Edit/" + cityId, function (data) {
        $("#modalContent").html(data);
        $("#cityModal").modal("show");
    });
}


// Submit Add/Edit Form
//function saveCity() {
//    var formData = $("#cityForm").serialize();
//    $.post("/City/Save?" + new Date().getTime(), formData, function (response) {
//        if (response.success) {
//            location.reload();  // Refresh list after saving
//        } else {
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
function loadDeleteCityConfirm(cityId) {

    $.get("/City/DeleteConfirm/" + cityId, function (data) {
        $("#modalContent").html(data);
        $("#cityModal").modal("show");
    });

}


function deleteCity(cityId) {
    //if (!confirm("Are you sure you want to delete this city?")) return;
    $.post("/City/Delete/" + cityId, function (response) {
        if (response.success) {
            $("#row-" + cityId).remove(); // Remove deleted city row
            location.reload();  // Refresh list after delete
        } else {
            alert("Failed to delete city.");
        }
    });
}
/*});*/



