
//document.addEventListener("DOMContentLoaded", function () {
//    //let selectedChildId = sessionStorage.getItem("SelectedChildId");
//    //if (selectedChildId) {
//    //    document.getElementById("childId").value = selectedChildId;
//    //}
//});

//window.addEventListener("beforeunload", function () {
//    sessionStorage.removeItem("SelectedChildId");
//});

//function saveSelectedChild(childId, courseId) {
//    if (childId) {
//        sessionStorage.setItem("SelectedChildId", childId);
//    } else {
//        sessionStorage.removeItem("SelectedChildId");
//    }
//    /*document.getElementById("courseId").value = courseId;*/
//    loadSchedules(childId, courseId);
//}


//function loadSchedules(childId, courseId) {



//    if (!childId) {
//        document.getElementById("scheduleTable").querySelector("tbody").innerHTML = "";
//        return;
//    }

//    document.getElementById("scheduleTable").querySelector("tbody").innerHTML = ""; 

//    fetch(`/Coach/GetSchedules?courseId=${courseId}&childId=${childId}`)
//        .then(response => response.json())
//        .then(data => {
//            const tbody = document.getElementById("scheduleTable").querySelector("tbody");
//            tbody.innerHTML = "";

//            if (data.length === 0) {
//                // If no schedules, show a "No scheduled courses." message
//                tbody.innerHTML = `<tr>
//                <td colspan="3" style="text-align: center; padding: 10px;">No scheduled courses.</td>
//            </tr>`;
//            } else {
//                data.forEach(schedule => {
//                    const row = `<tr>
//                        <td style="padding: 8px; border: 1px solid #ddd;">${schedule.scheduledAt}</td>
//                        <td style="padding: 8px; border: 1px solid #ddd;">${schedule.scheduledHours}</td>
//                        <td style="padding: 8px; border: 1px solid #ddd;">
//                            <form method="post" action="/Coach/DeleteSchedule">
//                                <input type="hidden" name="enrollmentId" value="${schedule.enrollmentID}" />
//                                <button type="submit" style="background-color: #ff4d4d; color: white; padding: 5px 10px; border: none; cursor: pointer;">
//                                    Remove
//                                </button>
//                            </form>
//                        </td>
//                    </tr>`;
//                    tbody.innerHTML += row;
//                });
//            }
//        })
//        .catch(error => console.error("Error loading schedules:", error));
//}






//function updateMinutesDisplay() {
//    const input = document.getElementById("scheduledHours");
//    const display = document.getElementById("minutesDisplay");

//    if (input && display) {
//        const hours = parseFloat(input.value);
//        if (!isNaN(hours)) {
//            const minutes = Math.round(hours * 60);
//            display.textContent = `(${minutes} minutes)`;
//        } else {
//            display.textContent = "";
//        }
//    }
//}

//document.addEventListener("DOMContentLoaded", function () {
//    const input = document.getElementById("scheduledHours");
//    if (input) {
//        input.addEventListener("input", updateMinutesDisplay);
//        updateMinutesDisplay(); // Initial display
//    }
//});