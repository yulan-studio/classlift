//$(document).ready(function () {
//    $('#CourseType').on('change', function () {
//        const selected = $(this).val();

//        if (selected === 'Private') {
//            $('#MaxCapacity').prop('disabled', true);
//            $('#CoachID').prop('disabled', false);
//        }

//        if (selected === 'Group') {
//            $('#MaxCapacity').prop('disabled', false);
//            $('#CoachID').prop('disabled', true);
//        }

//        //if (selected === 'Group') {
//        //    $('#MaxCapacity').prop('disabled', false);
//        //} else {
//        //    $('#MaxCapacity').prop('disabled', false);
//        //}
//    });
//});



window.addEventListener('DOMContentLoaded', function () {
    const courseType = document.getElementById("CourseType");
    const sessionCount = document.getElementById("SessionCount");
    const maxCapacity = document.getElementById("MaxCapacity");
    const hourlyCost = document.getElementById("HourlyCost");
    const sessionCost = document.getElementById("SessionCost");

    function updateCostFields() {
        const isPrivate = courseType.value === "Private";
        const hasSessionCount = sessionCount.value.trim() !== "";

        if (!hasSessionCount) {
            sessionCost.value = "";
        }

        if (isPrivate) {
            maxCapacity.value = "";
        }

        maxCapacity.disabled = isPrivate;
        hourlyCost.disabled = !isPrivate || hasSessionCount;
        hourlyCost.required = isPrivate && !hasSessionCount;
        sessionCost.disabled = isPrivate && !hasSessionCount;
        sessionCost.required = isPrivate && hasSessionCount;
    }

    courseType.addEventListener("change", updateCostFields);
    sessionCount.addEventListener("input", updateCostFields);
    updateCostFields();
});
