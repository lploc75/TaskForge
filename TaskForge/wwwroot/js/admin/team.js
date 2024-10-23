// Đặt giá trị mặc định cho ngày hiện tại khi tạo team
function openCreateTeamModal() {
    var today = new Date().toISOString().split('T')[0];
    document.getElementById('newCreatedDate').value = today;
    // Mở modal
    document.getElementById('createTeamModal').style.display = 'flex';
}

// Function to close the Create Team modal
function closeCreateTeamModal() {
    var createModal = document.getElementById("createTeamModal");
    createModal.style.display = "none";
}


// Đặt giá trị mặc định cho ngày hiện tại khi cập nhật team (nếu cần)
function openUpdateTeamModal(teamId, teamName, createdDate, numberOfMembers, deptId) {
    document.getElementById('modalTeamId').value = teamId;
    document.getElementById('modalTeamName').value = teamName;
    document.getElementById('modalCreatedDate').value = createdDate || new Date().toISOString().split('T')[0];
    document.getElementById('modalNumberOfMember').value = numberOfMembers;
    document.getElementById('modalDeptId').value = deptId;

    // Mở modal
    document.getElementById('updateTeamModal').style.display = 'flex';
}


// Function to close the Update Team modal
function closeUpdateTeamModal() {
    var updateModal = document.getElementById("updateTeamModal");
    updateModal.style.display = "none";
}

// Function to confirm before creating a team
function confirmCreateTeam() {
    return confirm("Are you sure you want to create this team?");
}

// Function to confirm before updating a team
function confirmUpdateTeam() {
    return confirm("Are you sure you want to save the changes?");
}

// Function to confirm before deleting a team
function confirmDeleteTeam() {
    return confirm("Are you sure you want to delete this team?");
}

// Close modal if clicked outside of the modal
window.onclick = function (event) {
    var createModal = document.getElementById("createTeamModal");
    var updateModal = document.getElementById("updateTeamModal");

    if (event.target === createModal) {
        createModal.style.display = "none";
    }
    if (event.target === updateModal) {
        updateModal.style.display = "none";
    }
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