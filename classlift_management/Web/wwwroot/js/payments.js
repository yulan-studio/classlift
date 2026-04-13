function fetchPackageAmount(packageId) {
    if (!packageId) {
        document.querySelector("input[name='amount']").value = "";
        return;
    }

    fetch(`/PaymentPackage/GetPackageAmount?packageId=${packageId}`)
        .then(response => response.json())
        .then(data => {
            const amountInput = document.querySelector("input[name='amount']");
            if (data.success) {
                amountInput.value = data.amount;
            } else {
                amountInput.value = "";
                alert("Failed to retrieve package amount.");
            }
        })
        .catch(error => {
            console.error("Error fetching package amount:", error);
            document.querySelector("input[name='amount']").value = "";
        });
}
