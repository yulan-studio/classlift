

function fetchCoursesBySpecialty(specialtyId) {
        if (!specialtyId) {
            document.getElementById('courseId').innerHTML = '<option value="">-- Select Course --</option>';
            return;
        }

        fetch(`/Child/GetCoursesBySpecialty?specialtyId=${specialtyId}`)
            .then(response => response.json())
            .then(data => {
                const courseDropdown = document.getElementById('courseId');
                courseDropdown.innerHTML = '<option value="">-- Select Course --</option>';
                data.forEach(course => {
                    const option = document.createElement('option');
                    option.value = course.courseID;
                    option.text = course.title;
                    option.setAttribute("data-course-type", course.courseType);
                    courseDropdown.add(option);
                });
            });
}


let isGroupClass = false;

function checkIfGroupClass(courseId) {
    const courseDropdown = document.getElementById("courseId");
    const selectedOption = courseDropdown.options[courseDropdown.selectedIndex];
    const courseType = selectedOption.getAttribute("data-course-type");

    if (courseType === "Group") {
        isGroupClass = true;
    } else {
        isGroupClass = false;
    }

    toggleFeeInput();
}

function toggleFeeInput() {
    const paymentModel = document.getElementById("paymentModel").value;
    const feeSection = document.getElementById("feeSection");

    if (paymentModel === "Direct") {
        feeSection.style.display = "block";
        document.getElementById("totalCost").setAttribute("required", "required");
    }
    else if (paymentModel === "Token") {
        if (isGroupClass) {
            feeSection.style.display = "block";
            document.getElementById("totalCost").setAttribute("required", "required");
            

            
        }
        else {
            feeSection.style.display = "none";
            document.getElementById("totalCost").removeAttribute("required");
        }
    }
    else {
        feeSection.style.display = "none";
        document.getElementById("totalCost").removeAttribute("required");
    }

    
}


function loadEditCourseFeeForm(courseEnrollmentId) {
    $.get("/Fee/EditCourseFee/" + courseEnrollmentId, function (data) {
        $("#courseModalContent").html(data);
        $("#courseFeeModal").modal("show");
    });
}

function loadEditActivityFeeForm(activityEnrollmentId) {
    $.get("/Fee/EditActivityFee/" + activityEnrollmentId, function (data) {
        $("#activityModalContent").html(data);
        $("#activityFeeModal").modal("show");
    });
}



function toggleActivityFeeInput() {
    const paymentModel = document.getElementById("PaymentModel").value;
    const feeSection = document.getElementById("activityFeeSection");

    if (paymentModel === "Direct") {
        feeSection.style.display = "block";
        document.getElementById("totalCost").setAttribute("required", "required");
    }
    else if (paymentModel === "Token") {
       
            feeSection.style.display = "block";
            document.getElementById("totalCost").setAttribute("required", "required");
    }
    else {
            feeSection.style.display = "none";
            document.getElementById("totalCost").removeAttribute("required");
    }
}
   


