// Đoạn mã JavaScript đã sao chép

document.addEventListener("DOMContentLoaded", function () {
    // Sử dụng các biến đã được truyền từ ViewData
    const ctx = document.getElementById('radarChart').getContext('2d');
    const radarChart = new Chart(ctx, {
        type: 'radar',
        data: {
            labels: ['Quality', 'Timeliness', 'Communication'],
            datasets: [{
                label: 'KPI Metrics',
                data: [totalKPI, totalTimeliness, totalTeamwork], // Cập nhật dữ liệu động ở đây
                fill: true,
                backgroundColor: 'rgba(29, 198, 247, 0.2)',
                borderColor: 'rgba(29, 198, 247, 1)',
                pointBackgroundColor: 'rgba(29, 198, 247, 1)'
            }]
        },
        options: {
            responsive: true,
            scales: {
                r: {
                    angleLines: { color: '#ddd' },
                    suggestedMin: 0,
                    suggestedMax: 100, // Chỉnh theo mức tối đa của KPI nếu cần
                    ticks: { display: true, stepSize: 10 },
                    grid: { color: '#eee' },
                    pointLabels: {
                        color: '#333',
                        font: { size: 14 }
                    }
                }
            },
            plugins: {
                legend: { display: true }
            }
        }
    });
});


const notificationDropdown = document.getElementById("notificationDropdown");
const bellIcon = document.querySelector(".icon-container .fa-bell");
const allTab = document.getElementById("all-tab");
const unreadTab = document.getElementById("unread-tab");
const notificationItems = document.querySelectorAll(".notification-item");

bellIcon.addEventListener("click", function (event) {
    event.stopPropagation();
    notificationDropdown.style.display = notificationDropdown.style.display === "none" ? "block" : "none";
});

window.addEventListener("click", function (event) {
    if (!notificationDropdown.contains(event.target) && event.target !== bellIcon) {
        notificationDropdown.style.display = "none";
    }
});

function markAsRead(item) {
    item.classList.remove("unread");
}

allTab.addEventListener("click", function () {
    allTab.classList.add("active");
    unreadTab.classList.remove("active");
    notificationItems.forEach(item => item.style.display = "flex");
});

unreadTab.addEventListener("click", function () {
    unreadTab.classList.add("active");
    allTab.classList.remove("active");
    notificationItems.forEach(item => {
        if (item.classList.contains("unread")) {
            item.style.display = "flex";
        } else {
            item.style.display = "none";
        }
    });
});
