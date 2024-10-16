
    function toggleDayNight() {
            const now = new Date();
    const currentHour = now.getHours();

    const moon = document.getElementById("moon");
    const sun = document.getElementById("sun");
    const greeting = document.getElementById("greeting");

            if (currentHour >= 6 && currentHour < 18) {
        sun.style.display = "block";
    moon.style.display = "none";
    greeting.innerText = "Good Morning";
            } else {
        sun.style.display = "none";
    moon.style.display = "block";
    greeting.innerText = "Good Evening";
            }
        }

    window.onload = toggleDayNight;
    const passwordInput = document.getElementById('password');
    const owl = document.getElementById('owl');
    const hidingOwl = document.getElementById('hidingOwl');

        passwordInput.addEventListener('focus', () => {
        owl.style.display = 'none';
    hidingOwl.style.display = 'block';
        });

        passwordInput.addEventListener('blur', () => {
        owl.style.display = 'block';
    hidingOwl.style.display = 'none';
        });

    function showOwl() {
        owl.style.display = 'block';
    hidingOwl.style.display = 'none';
        }
