// Mở modal cập nhật tài khoản
function openUpdateAccountModal(accountId, username, password, email, phoneNumber) {
    // Điền thông tin vào modal cập nhật
    document.getElementById("modalAccountId").value = accountId;
    document.getElementById("modalUsername").value = username;
    document.getElementById("modalPassword").value = password;
    document.getElementById("modalEmail").value = email;
    document.getElementById("modalPhoneNumber").value = phoneNumber;

    // Hiển thị modal
    document.getElementById("updateAccountModal").style.display = "flex";
}

// Đóng modal cập nhật tài khoản
function closeUpdateAccountModal() {
    document.getElementById("updateAccountModal").style.display = "none";
}

// Mở modal tạo tài khoản mới
function openCreateAccountModal() {
    document.getElementById("createAccountModal").style.display = "flex";
}

// Đóng modal tạo tài khoản
function closeCreateAccountModal() {
    document.getElementById("createAccountModal").style.display = "none";
}

// Đóng modal khi nhấn vào bên ngoài
window.onclick = function (event) {
    var createAccountModal = document.getElementById("createAccountModal");
    var updateAccountModal = document.getElementById("updateAccountModal");

    // Nếu nhấn ra ngoài modal tạo tài khoản thì đóng nó
    if (event.target == createAccountModal) {
        createAccountModal.style.display = "none";
    }

    // Nếu nhấn ra ngoài modal cập nhật tài khoản thì đóng nó
    if (event.target == updateAccountModal) {
        updateAccountModal.style.display = "none";
    }
}
// Hàm xác nhận khi accept task
function confirmDeleteAccount() {
    return confirm("Are you sure you want to delete this account?'");
}
// Chờ cho trang load xong
window.onload = function () {
    // Tìm thông báo thành công
    var successMessage = document.getElementById("successMessage");

    // Nếu thông báo tồn tại, ẩn nó sau 5 giây
    if (successMessage) {
        setTimeout(function () {
            successMessage.style.display = 'none'; // Ẩn thông báo
        }, 5000); // 5000ms = 5 giây
    }
};
function confirmCreateAccount() {
    return confirm("Are you sure you want to create this account?");
}
function confirmUpdateAccount() {
    return confirm("Are you sure you want to update this account?");
}