function fetchCoaches(specialtyId) {
    const coachDropdown = document.getElementById('CoachID');
    const roleName = coachDropdown.dataset.roleName || 'Teacher';
    const rolePlural = coachDropdown.dataset.rolePlural || `${roleName}s`;

    if (!specialtyId) {
        coachDropdown.innerHTML = `<option value="">-- Select ${roleName} --</option>`;
        return;
    }

    fetch(`/Course/GetCoachesBySpecialty?specialtyId=${specialtyId}`)
        .then(response => {
            if (!response.ok) {
                throw new Error(`Unable to load active ${rolePlural.toLowerCase()}.`);
            }
            return response.json();
        })
        .then(data => {
            coachDropdown.innerHTML = data.length
                ? `<option value="">-- Select ${roleName} --</option>`
                : `<option value="">-- No active ${roleName.toLowerCase()} available --</option>`;

            data.forEach(coach => {
                const option = document.createElement('option');
                option.value = coach.coachID;
                option.text = coach.name;
                coachDropdown.add(option);
            });
        })
        .catch(() => {
            coachDropdown.innerHTML = `<option value="">-- Unable to load ${rolePlural.toLowerCase()} --</option>`;
        });
}
