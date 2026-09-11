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
    const privateSessionCountInstruction = document.getElementById("privateSessionCountInstruction");
    const groupSessionCountInstruction = document.getElementById("groupSessionCountInstruction");

    if (!courseType || !sessionCount || !maxCapacity || !sessionCost) {
        return;
    }

    function updateCostFields() {
        const isPrivate = courseType.value === "Private";
        const isGroup = courseType.value === "Group";
        const hasSessionCount = sessionCount.value.trim() !== "";

        if (!hasSessionCount) {
            sessionCost.value = "";
        }

        if (isPrivate) {
            maxCapacity.value = "";
        }

        maxCapacity.disabled = isPrivate;
        sessionCount.required = isGroup;
        if (privateSessionCountInstruction && groupSessionCountInstruction) {
            privateSessionCountInstruction.style.display = isGroup ? "none" : "inline";
            groupSessionCountInstruction.style.display = isGroup ? "inline" : "none";
        }

        if (hourlyCost) {
            if (hasSessionCount) {
                hourlyCost.value = "";
            }

            hourlyCost.disabled = !isPrivate || hasSessionCount;
            hourlyCost.required = isPrivate && !hasSessionCount;
        }
        
        sessionCost.disabled = isPrivate && !hasSessionCount;
        sessionCost.required = isGroup || (isPrivate && hasSessionCount);
    }

    courseType.addEventListener("change", updateCostFields);
    sessionCount.addEventListener("input", updateCostFields);
    updateCostFields();
});
