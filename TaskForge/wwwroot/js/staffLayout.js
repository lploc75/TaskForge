        // Mở popup khi nhấn vào nút Settings
    document.getElementById('settingsButton').addEventListener('click', function (e) {
        e.preventDefault();
    e.stopPropagation();
    const popup = document.getElementById('settingsPopup');
    popup.style.display = popup.style.display === 'block' ? 'none' : 'block';
        });

    // Đóng popup khi nhấp ra ngoài
    document.addEventListener('click', function (event) {
            const popup = document.getElementById('settingsPopup');
    const settingsButton = document.getElementById('settingsButton');
    if (popup.style.display === 'block' && !popup.contains(event.target) && event.target !== settingsButton) {
        popup.style.display = 'none';
            }
        });
// Lấy các phần tử modal
const modal = document.getElementById("changePasswordModal");
const btn = document.getElementById("changePasswordLink");
const span = document.getElementsByClassName("close")[0];

// Khi người dùng nhấn vào liên kết "Đổi mật khẩu", mở modal
if (btn) {
    btn.onclick = function (event) {
        event.preventDefault();
        modal.style.display = "block";
    }
}

// Khi người dùng nhấn vào nút đóng (x), đóng modal
if (span) {
    span.onclick = function () {
        modal.style.display = "none";
    }
}

// Khi người dùng nhấp bên ngoài modal, đóng modal
window.onclick = function (event) {
    if (event.target == modal) {
        modal.style.display = "none";
    }
}
