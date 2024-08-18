document.addEventListener('DOMContentLoaded', function () {
    const searchButton = document.getElementById('searchButton');
    const searchEmployeeId = document.getElementById('searchEmployeeId');
    const editFormContainer = document.getElementById('editFormContainer');
    const editButton = document.getElementById('btnEdit');
    const checkPassword = document.getElementById('checkPassword');
    const checkSignature = document.getElementById('checkSignature');
    const saveBtn = document.getElementById('btnSave');


    searchButton.addEventListener('click', function () {
        const employeeId = searchEmployeeId.value.trim();

        if (employeeId === '') {
            alert('Please enter an Employee ID.');
            return;
        }

        fetch(`/api/UserAPI/getUser?employeeId=${employeeId}`)
            .then(response => {
                if (!response.ok) {
                    return alert('User not found.');
                }
                return response.json();
            })
            .then(data => {
                if (data) {
                    document.getElementById('userId').value = data.id;
                    document.getElementById('name').value = data.name;
                    document.getElementById('employeeid').value = data.employeeId;
                    document.getElementById('mail').value = data.mail;
                    if (data.base64Image) {
                        const imageElement = document.getElementById('imgSign');
                        imageElement.src = `data:image/png;base64,${data.base64Image}`;
                    }
                    editFormContainer.style.display = 'block';
                }
            })
            .catch(error => console.error('Error fetching user:', error));
    });

    saveBtn.addEventListener('click', function (event) {
        event.preventDefault();
        hideErrorMessage()
        const userId = document.getElementById('userId').value;
        const name = document.getElementById('name').value;
        const employeeId = document.getElementById('employeeid').value;
        const mail = document.getElementById('mail').value;
        const password = document.getElementById('password').value;
        const signature = document.getElementById('signature').files[0];
        const isPasswordRequired = checkPassword.checked
        const isSignatureRequired = checkSignature.checked

        const formData = new FormData();
        formData.append('id', userId);
        formData.append('name', name);
        formData.append('employeeid', employeeId);
        formData.append('mail', mail);
        if (isPasswordRequired) {
            formData.append('password', password);
        }
        if (signature && isSignatureRequired) {
            formData.append('signature', signature);
        }

        fetch('/api/UserAPI/updateUser', {
            method: 'POST',
            body: formData,
        })
            .then(response => {
                if (response.ok) {
                    alert('User updated successfully.');
                    location.reload();
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

    editButton.addEventListener('click', function () {
        document.getElementById('btnEdit').style.display = 'none';
        document.getElementById('sectionImage').style.display = 'none';

        document.getElementById('name').disabled = false;
        document.getElementById('employeeid').disabled = false;
        document.getElementById('mail').disabled = false;

        document.getElementById('btnSave').style.display = 'block';
        document.getElementById('btnCancel').style.display = 'block';
        document.getElementById('sectionSignature').style.display = 'block';
        document.getElementById('sectionPassword').style.display = 'block';
    });

    checkPassword.addEventListener('click', function () {
        const passwordField = document.getElementById('password');
        if (passwordField.style.display === 'none' || passwordField.style.display === '') {
            passwordField.style.display = 'block';
        } else {
            passwordField.style.display = 'none';
        }
    });

    checkSignature.addEventListener('click', function () {
        const signatureField = document.getElementById('signature');
        if (signatureField.style.display === 'none' || signatureField.style.display === '') {
            signatureField.style.display = 'block';
        } else {
            signatureField.style.display = 'none';
        }
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