document.getElementById('login-form').addEventListener('submit', function (e) {
    e.preventDefault();

    var username = document.getElementById('username').value;
    var password = document.getElementById('password').value;

    if (username === '' || password === '') {
        alert('Please fill in all fields.');
    } else {
        // Perform login (this is just a placeholder, replace with actual login logic)
        alert('Logging in...');
    }
});