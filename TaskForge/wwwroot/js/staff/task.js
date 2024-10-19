
    function checkIfSubmit(event, title, assigner, assignedDate, deadline, submissionDate, priority, difficulty, status, description) {
            // Kiểm tra nếu phần tử được click là nút submit
            if (event.target.tagName === 'BUTTON' && event.target.type === 'submit') {
                return; // Không làm gì nếu là nút submit
            }

    // Nếu không phải nút submit, mở modal
    openModal(title, assigner, assignedDate, deadline, submissionDate, priority, difficulty, status, description);
        }

    function openModal(title, assigner, assignedDate, deadline ,submissionDate, priority, difficulty, status, description) {
            const reward = difficulty * 10; // Tính phần thưởng dựa trên độ khó
    // Gán các giá trị cho modal
    document.getElementById('modalTitle').innerText = title;
    document.getElementById('modalAssigner').innerText = `Assigner: ${assigner}`;
    document.getElementById('modalAssignedDate').innerText = `Assigned Date: ${assignedDate}`;
    document.getElementById('modalDueDate').innerText = `Deadline: ${deadline}`;
    document.getElementById('modalSubmissionDate').innerText = `Submission Date: ${submissionDate}`;
    document.getElementById('modalReward').innerText = `Reward: ${reward} Credits`;
    document.getElementById('modalContent').innerHTML = `
    <p>Priority: ${priority}</p>
    <p>Difficulty: ${difficulty}</p>
    <p>Status: ${status}</p>
    <p>Description: ${description}</p>
    `;
    // Hiển thị modal
    document.getElementById('taskModal').style.display = 'flex';
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


    function closeModal() {
        document.getElementById('taskModal').style.display = 'none';
        }

    function createPersonalTask() {
        alert("Opening personal task creation form.");
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
