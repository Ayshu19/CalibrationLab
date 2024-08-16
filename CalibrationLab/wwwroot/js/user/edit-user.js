document.addEventListener('DOMContentLoaded', function () {
    const searchButton = document.getElementById('searchButton');
    const searchEmployeeId = document.getElementById('searchEmployeeId');
    const editFormContainer = document.getElementById('editFormContainer');
    const editButton = document.getElementById('btnEdit');
    const checkPassword = document.getElementById('checkPassword');
    const checkSignature = document.getElementById('checkSignature');

    searchButton.addEventListener('click', function () {
        const employeeId = searchEmployeeId.value.trim();

        if (employeeId === '') {
            alert('Please enter an Employee ID.');
            return;
        }

        fetch(`/api/UserAPI/getUser?employeeId=${employeeId}`)
            .then(response => response.json())
            .then(data => {
                if (data) {
                    // Populate the form with user data
                    document.getElementById('name').value = data.name;
                    document.getElementById('employeeid').value = data.employeeId;
                    document.getElementById('mail').value = data.mail;
                    if (data.base64Image) {
                        const imageElement = document.getElementById('imgSign');
                        imageElement.src = `data:image/png;base64,${data.base64Image}`;
                    }
                    editFormContainer.style.display = 'block';
                } else {
                    alert('User not found.');
                }
            })
            .catch(error => console.error('Error fetching user:', error));
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