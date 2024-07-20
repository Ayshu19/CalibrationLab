document.querySelectorAll('.dropdown-toggle').forEach(function (element) {
    element.addEventListener('click', function (e) {
        e.preventDefault();
        var dropdownMenu = this.nextElementSibling;
        var icon = this.querySelector('.icon');
        if (dropdownMenu.style.display === 'block') {
            dropdownMenu.style.display = 'none';
            icon.textContent = '+';
        } else {
            dropdownMenu.style.display = 'block';
            icon.textContent = '-';
        }
    });
});