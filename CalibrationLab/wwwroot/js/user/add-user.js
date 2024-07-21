document.addEventListener('DOMContentLoaded', function () {
    const submitBtn = document.getElementById('submitBtn');

    submitBtn.addEventListener('click', function (event) {
        event.preventDefault(); // Prevent any default button behavior

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
        formData.append('signature', signature);

        fetch('/api/UserAPI/addUser', {
            method: 'POST',
            body: formData,
        })
            .then(response => {
                if (response.ok) {
                    alert('User added successfully.');
                } else {
                    return response.text().then(text => { throw new Error(text) });
                }
            })
            .catch(error => {
                console.error('Error:', error);
                alert('Failed to add user.');
            });
    });
});