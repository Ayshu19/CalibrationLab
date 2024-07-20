document.addEventListener('DOMContentLoaded', function () {
    const searchButton = document.getElementById('searchButton');
    const searchEmployeeId = document.getElementById('searchEmployeeId');
    const editFormContainer = document.getElementById('editFormContainer');
    const editUserForm = document.getElementById('editUserForm');

    searchButton.addEventListener('click', function () {
        const employeeId = searchEmployeeId.value.trim();

        if (employeeId === '') {
            alert('Please enter an Employee ID.');
            return;
        }

        fetch(`/Users/GetUser?employeeId=${employeeId}`)
            .then(response => response.json())
            .then(data => {
                if (data) {
                    // Populate the form with user data
                    editUserForm.querySelector('input[name="Name"]').value = data.name;
                    editUserForm.querySelector('input[name="EmployeeId"]').value = data.employeeId;
                    editUserForm.querySelector('input[name="Mail"]').value = data.mail;
                    // Clear the password field for security reasons
                    editUserForm.querySelector('input[name="Password"]').value = '';
                    editFormContainer.style.display = 'block';
                } else {
                    alert('User not found.');
                }
            })
            .catch(error => console.error('Error fetching user:', error));
    });
});