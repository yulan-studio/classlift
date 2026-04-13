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
    document.getElementById("CourseType").addEventListener("change", function () {
        const selected = this.value;
        const disable = selected === "Private";

        document.getElementById("MaxCapacity").disabled = disable;
       /* document.getElementById("CoachID").disabled = !disable;*/

       
       /* document.getElementById("CoachIDHidden").disabled = disable; */

        document.getElementById("HourlyCost").disabled = !disable; 
        document.getElementById("HourlyCost2").disabled = !disable; 



    });
});