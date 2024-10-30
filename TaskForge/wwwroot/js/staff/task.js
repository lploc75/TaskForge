function checkIfSubmit(event, title, subtaskId, assignedDate, deadline, submissionDate, priority, difficulty, status, description, taskId, teamId) {
    // Kiểm tra nếu phần tử được click là nút submit
    if (event.target.tagName === 'BUTTON' && event.target.type === 'submit') {
        return; // Không làm gì nếu là nút submit
    }

    // Nếu không phải nút submit, mở modal
    openSubtaskModal(title, subtaskId, assignedDate, deadline, submissionDate, priority, difficulty, status, description, taskId, teamId);
}
function checkIfPTaskSubmit(event, title, assignedDate, deadline, priority, status, description) {
    // Kiểm tra nếu phần tử được click là nút submit
    if (event.target.tagName === 'BUTTON' && event.target.type === 'submit') {
        return; // Không làm gì nếu là nút submit
    }

    // Nếu không phải nút submit, mở modal hiển thị thông tin PersonalTask
    openPersonalTaskModal(title, assignedDate, deadline, priority, status, description);
}

function openSubtaskModal(title, subtaskId, assignedDate, deadline, submissionDate, priority, difficulty, status, description, taskId, teamId) {
    const reward = difficulty * 10;

    document.getElementById('modalTitle').innerText = title;
    document.getElementById('modalSubtaskId').innerText = `Subtask ID: ${subtaskId}`;
    document.getElementById('modalTaskId').innerText = `Task ID: ${taskId}`;
    document.getElementById('modalTeamId').innerText = `Team ID: ${teamId}`;
    document.getElementById('modalAssignedDate').innerText = `Assigned Date: ${assignedDate}`;
    document.getElementById('modalDueDate').innerText = `Deadline: ${deadline}`;
    document.getElementById('modalSubmissionDate').innerText = `Submission Date: ${submissionDate}`;
    document.getElementById('modalReward').innerText = `Reward: ${reward} Credits`;

    document.getElementById('modalPriority').innerText = priority;
    document.getElementById('modalDifficulty').innerText = difficulty;
    document.getElementById('modalStatus').innerText = status;
    document.getElementById('modalDescription').innerText = description;

    document.getElementById('taskModal').style.display = 'flex';
}


// Open modal for Personal Task
function openPersonalTaskModal(title, assignedDate, deadline, priority, status, description) {
    document.getElementById('personalTaskTitle').innerText = title;
    document.getElementById('personalTaskAssignedDate').innerText = `Assigned Date: ${assignedDate}`;
    document.getElementById('personalTaskDueDate').innerText = `Deadline: ${deadline}`;
    document.getElementById('personalTaskContent').innerHTML = `
                <p>Priority: ${priority}</p>
                <p>Status: ${status}</p>
                <p>Description: ${description}</p>
            `;
    document.getElementById('personalTaskModal').style.display = 'flex';
}
// Mở modal để tạo Personal Task mới
function openCreatePersonalTask() {
    document.getElementById('createPersonalTaskModal').style.display = 'flex';
}
function openUpdatePersonalTaskModal(ptaskId, title, assignedDate, deadline, priority, description) {
    document.getElementById('PtaskId').value = ptaskId;
    document.getElementById('updateTaskTitle').value = title;
    document.getElementById('updateAssignedDate').value = formatDateTime(assignedDate); // Format assignedDate
    document.getElementById('updateDeadline').value = formatDateTime(deadline);         // Format deadline
    document.getElementById('updatePriority').value = priority;
    document.getElementById('updateDescription').value = description;

    document.getElementById('updatePersonalTaskModal').style.display = 'flex';
}

function formatDateTime(dateString) {
    const date = new Date(dateString); // Convert to Date object
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0'); // Month is zero-based
    const day = String(date.getDate()).padStart(2, '0');
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');
    const seconds = String(date.getSeconds()).padStart(2, '0');

    return `${year}-${month}-${day} ${hours}:${minutes}:${seconds}`;
}

    function toggleSection(sectionId, headerElement) {
            const section = document.getElementById(sectionId);
    const arrowIcon = headerElement.querySelector('.arrow-icon');

    // Chuyển đổi trạng thái hiển thị
    if (section.style.display === 'block') {
        section.style.display = 'none';
        arrowIcon.innerHTML = '&#9662;'; // Mũi tên xuống
        } else {
            section.style.display = 'block';
            arrowIcon.innerHTML = '&#9652;'; // Mũi tên lên
        }
    }


    // Close modal
function closeModal(modalId) {
    document.getElementById(modalId).style.display = 'none';
}

function createPersonalTask() {
    lert("Opening personal task creation form.");
}

    // Hàm xác nhận khi submit task
function confirmSubmitTask() {
    return confirm("Are you sure you want to submit this task?");
}
    // Hàm xác nhận khi unsubmit task (nếu có)
function confirmUnsubmitTask() {
    return confirm("Are you sure you want to unsubmit this task?");
}
// Hàm xác nhận khi accept task
function confirmAcceptTask() {
    return confirm("Are you sure you want to accept this task?");
}
// Hàm xác nhận khi accept task
function confirmAcceptTask() {
    return confirm("Are you sure you want to change this personal task status?");
}
// Hàm xác nhận khi accept task
function confirmDeletePersonalTask() {
    return confirm("Are you sure you want to delete this task?'");
}
// Hàm xác nhận khi reject task
function confirmRejectTask() {
    return confirm("Are you sure you want to reject this task?");
}
// Chờ cho trang load xong
window.onload = function () {
    // Tìm các thông báo
    var successMessage = document.getElementById("successMessage");
    var message = document.getElementById("errorMessage");

    // Nếu thông báo thành công tồn tại, ẩn nó sau 5 giây
    if (successMessage) {
        setTimeout(function () {
            successMessage.style.display = 'none';
        }, 5000); // 5000ms = 5 giây
    }

    // Nếu thông báo lỗi tồn tại, ẩn nó sau 10 giây
    if (message) {
        setTimeout(function () {
            message.style.display = 'none';
        }, 10000); // 10000ms = 10 giây
    }
};

function confirmCreateTask() {
    return confirm("Are you sure you want to create this task?");
}
function confirmUpdateTask() {
    return confirm("Are you sure you want to update this task?");
}

function toggleDifficultyField() {
    const taskType = document.getElementById("taskType").value;
    const difficultyField = document.getElementById("difficultyField");

    if (taskType === "personal") {
        difficultyField.style.display = "none"; // Hide Difficulty for Personal Task
    } else {
        difficultyField.style.display = "block"; // Show Difficulty for Subtask
    }
}

