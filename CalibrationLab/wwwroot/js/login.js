document.getElementById('btnLogin').addEventListener('click', async e => {
    e.preventDefault();
    const username = document.getElementById('username').value;
    const password = document.getElementById('password').value;
    if (username === '' || password === '') {
        alert('Please fill in all fields.');
    } else {
        const form = document.getElementById('signInForm');
        const formData = new FormData(form);
        try {
            const response = await fetch('/api/accountApi/signIn', {
                method: 'POST',
                body: formData
            });

            if (response.ok) {
                const result = await response.json();
                if (result.isSuccess) {
                    window.location.href = '/'; // Redirect to home or another page
                } else {
                    alert(result.message);
                }
            } else {
                const errorText = await response.text();
                throw new Error(errorText);
            }
        } catch (error) {
            console.error('Error:', error);
            alert('Failed to sign in.');
        }
    }
});