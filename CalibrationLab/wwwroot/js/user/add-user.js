document.addEventListener('DOMContentLoaded', function () {
    const submitBtn = document.getElementById('submitBtn');

    submitBtn.addEventListener('click', function (event) {
        event.preventDefault(); // Prevent any default button behavior
        hideErrorMessage()
        const name = document.getElementById('name').value;
        const employeeId = document.getElementById('employeeid').value;
        const mail = document.getElementById('mail').value;
        const password = document.getElementById('password').value;
        const signature = document.getElementById('signature').files[0];

        const formData = new FormData();
        formData.append('name', name);
        formData.append('employeeid', employeeId);
        formData.append('mail', mail);
        formData.append('password', password);
        if (signature) {
            formData.append('signature', signature);
        }

        fetch('/api/UserAPI/addUser', {
            method: 'POST',
            body: formData,
        })
            .then(response => {
                if (response.ok) {
                    alert('User added successfully.');
                } else {
                    return response.json().then(json => {
                        displayErrorMessage(json.message);
                    });
                }
            })
            .catch(error => {
                displayErrorMessage("Please enter valid details!");
            });
    });
});

function displayErrorMessage(message) {
    const errorMessageDiv = document.getElementById('errorMessage');
    errorMessageDiv.innerText = message;
    errorMessageDiv.style.display = 'block';
}

function hideErrorMessage() {
    const errorMessageDiv = document.getElementById('errorMessage');
    errorMessageDiv.innerText = "";
    errorMessageDiv.style.display = 'none';
}