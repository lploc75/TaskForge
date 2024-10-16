CREATE TABLE Account (
    account_id VARCHAR(10) PRIMARY KEY,
    username VARCHAR(50) NOT NULL,
    password VARCHAR(255) NOT NULL,
    email VARCHAR(100),
    phone_number VARCHAR(15)
);
CREATE TABLE Feedback (
    feedback_id INT PRIMARY KEY,
    context TEXT,
    date_submitted DATETIME,
    account_id VARCHAR(10),
    FOREIGN KEY (account_id) REFERENCES Account(account_id)
);
CREATE TABLE Department (
    dept_id VARCHAR(10) PRIMARY KEY,
    dept_name VARCHAR(100),
    description TEXT,
    number_of_team INT	
);
CREATE TABLE Team (
    team_id VARCHAR(10) PRIMARY KEY,
    team_name VARCHAR(100),
    created_date DATE,
    number_of_member INT,
    dept_id VARCHAR(10),
    FOREIGN KEY (dept_id) REFERENCES Department(dept_id)
);
CREATE TABLE Employee (
    account_id VARCHAR(10) PRIMARY KEY,
    fullname VARCHAR(100) NOT NULL,
    gender CHAR(1),
    dob DATE,
    start_date DATE,
    end_date DATE,
    role VARCHAR(50),
    status VARCHAR(50),
    dept_id VARCHAR(10),
    FOREIGN KEY (account_id) REFERENCES Account(account_id),
    FOREIGN KEY (dept_id) REFERENCES Department(dept_id)
);
CREATE TABLE Project (
    project_id INT PRIMARY KEY,
    project_name VARCHAR(100),
    description TEXT,
    status VARCHAR(50),
    deadline DATETIME,
);
CREATE TABLE EmployeeProject(
    account_id VARCHAR(10),
    project_id INT,
    PRIMARY KEY (account_id, project_id),
    FOREIGN KEY (account_id) REFERENCES Employee(account_id),
    FOREIGN KEY (project_id) REFERENCES Project(project_id)
);
CREATE TABLE EmployeeTeam(
    account_id VARCHAR(10),
    team_id VARCHAR(10),
    PRIMARY KEY (account_id, team_id),
    FOREIGN KEY (account_id) REFERENCES Employee(account_id),
    FOREIGN KEY (team_id) REFERENCES Team(team_id)
);
CREATE TABLE StaffAndLeader (
    account_id VARCHAR(10) PRIMARY KEY,
    total_kpi DECIMAL(5,2),
    total_timeliness DECIMAL(5,2),
    total_teamwork DECIMAL(5,2),
    credit_points INT,
    number_of_team INT,
    FOREIGN KEY (account_id) REFERENCES Account(account_id),
);
CREATE TABLE CreditExchange(
    exchange_id INT PRIMARY KEY IDENTITY(1,1),
    account_id VARCHAR(10), -- Foreign key referencing Staff
    exchange_date DATETIME NOT NULL,
    credit_points_used INT NOT NULL,
    cash_amount DECIMAL(10,2) NOT NULL,
    status VARCHAR(50) DEFAULT 'Pending',
    FOREIGN KEY (account_id) REFERENCES StaffAndLeader(account_id)
);
CREATE TABLE Task (
    task_id VARCHAR(10) PRIMARY KEY,
    task_name VARCHAR(100),
    description TEXT,
    status VARCHAR(50),
    priority INT,
	assignment_date DATETIME,
    deadline DATETIME,
    submission_date DATETIME,
    project_id INT,
    FOREIGN KEY (project_id) REFERENCES Project(project_id)
);
CREATE TABLE PersonalTask (
    ptask_id VARCHAR(10) PRIMARY KEY,
	account_id VARCHAR(10),
	status VARCHAR(50),
    priority INT,
	assignment_date DATETIME,
    deadline DATE,
	description TEXT,
    FOREIGN KEY (account_id) REFERENCES StaffAndLeader(account_id)
);
CREATE TABLE TaskAssignment(
    task_id VARCHAR(10),
	created_by VARCHAR(10),
    assigned_to VARCHAR(10),
    PRIMARY KEY(task_id, created_by, assigned_to), 
	FOREIGN KEY (task_id) REFERENCES Task(task_id),
    FOREIGN KEY (created_by) REFERENCES Employee(account_id),
    FOREIGN KEY (assigned_to) REFERENCES Employee(account_id),
);
CREATE TABLE TaskEvaluation (
    evaluation_id VARCHAR(10) PRIMARY KEY,
    evaluation_date DATETIME,
    comment TEXT,
    task_id VARCHAR(10),
    FOREIGN KEY (task_id) REFERENCES Task(task_id)
);
CREATE TABLE Subtask (
    subtask_id VARCHAR(10) PRIMARY KEY,
    subtask_name VARCHAR(100),
    description TEXT,
    status VARCHAR(50),
    priority INT,
    difficulty INT,
	assignment_date DATETIME,
    deadline DATETIME,
    submission_date DATETIME,
    task_id VARCHAR(10),
    team_id VARCHAR(10),
    FOREIGN KEY (task_id) REFERENCES Task(task_id),
    FOREIGN KEY (team_id) REFERENCES Team(team_id)
);
CREATE TABLE SubtaskAssignment(
    subtask_id VARCHAR(10),
    created_by VARCHAR(10),
    assigned_to VARCHAR(10),
    PRIMARY KEY(subtask_id, created_by, assigned_to), 
	FOREIGN KEY (subtask_id) REFERENCES Subtask(subtask_id),
    FOREIGN KEY (created_by) REFERENCES StaffAndLeader(account_id),
    FOREIGN KEY (assigned_to) REFERENCES StaffAndLeader(account_id),
);
CREATE TABLE SubtaskEvaluation (
    evaluation_id VARCHAR(10) PRIMARY KEY,
    evaluation_date DATETIME,
    comment TEXT,
    subtask_id VARCHAR(10),
    teamwork_rating INT,
    timeliness_rating INT,
    kpi_rating INT,
    FOREIGN KEY (subtask_id) REFERENCES Subtask(subtask_id)
);
CREATE TABLE Credit (
    difficulty INT PRIMARY KEY,
    credits INT,
);
CREATE TABLE SubtaskCredit(
    subtask_id VARCHAR(10),
    difficulty INT,
    PRIMARY KEY(subtask_id, difficulty),
    FOREIGN KEY (subtask_id) REFERENCES Subtask(subtask_id),
    FOREIGN KEY (difficulty) REFERENCES Credit(difficulty)
);
CREATE TABLE Comment (
    comment_id VARCHAR(10) PRIMARY KEY,
    content TEXT,
    date_submitted DATETIME,
    subtask_id VARCHAR(10),
    FOREIGN KEY (subtask_id) REFERENCES Subtask(subtask_id)
);

-- Dữ liệu cho bảng Account
INSERT INTO Account (account_id, username, password, email, phone_number) VALUES
('ACC001', 'admin', 'password123', 'admin@example.com', '0123456789'),
('ACC002', 'manager', 'password123', 'manager@example.com','0123456788'),
('ACC003', 'staff01', 'password123', 'staff01@example.com', '0123456787'),
('ACC004', 'leader01', 'password123', 'leader01@example.com','0123456786'),
('ACC005', 'staff02', 'password123', 'staff02@example.com','0123456785'),
('ACC006', 'depthead01', 'password123', 'depthead01@example.com','0123456784');

-- Dữ liệu cho bảng Feedback
INSERT INTO Feedback (feedback_id, context, date_submitted, account_id) VALUES
(1, 'Testing the system', '2024-10-01', 'ACC001'),
(2, 'Hello im new.', '2024-10-02', 'ACC003');

-- Dữ liệu cho bảng Department
INSERT INTO Department (dept_id, dept_name, description, number_of_team) VALUES
('DEP001', 'IT Department', 'Handles IT services and support.', 0),
('DEP002', 'HR Department', 'Manages human resources.', 1),
('DEP003', 'Sales Department', 'Handles sales and customer relations.', 0);

-- Dữ liệu cho bảng Team
INSERT INTO Team (team_id, team_name, created_date, number_of_member, dept_id) VALUES
('TEAM001', 'Development Team', '2024-01-01', 5, 'DEP002'),
('TEAM002', 'HR Team', '2024-01-02', 4, 'DEP002'),
('TEAM003', 'Sales Team', '2024-01-03', 3, 'DEP002');

-- Dữ liệu cho bảng Employee
INSERT INTO Employee (account_id, fullname, gender, dob, start_date, end_date, role, status, dept_id) VALUES
('ACC001', 'Admin User', 'M', '1985-01-01', '2022-01-01', NULL, 'Admin', 'Active', 'DEP001'),
('ACC002', 'Manager User', 'F', '1980-05-15', '2022-01-01', NULL, 'Manager', 'Active', 'DEP002'),
('ACC003', 'Alice Smith', 'F', '1990-05-01', '2022-01-10', NULL, 'Staff', 'Active', 'DEP002'),
('ACC004', 'Bob Johnson', 'M', '1985-03-15', '2022-01-15', NULL, 'Leader', 'Active', 'DEP002'),
('ACC005', 'Charlie Brown', 'M', '1992-08-25', '2023-01-01', NULL, 'Staff', 'Active', 'DEP002'),
('ACC006', 'David Wilson', 'M', '1980-09-10', '2020-01-01', NULL, 'Department Head', 'Active', 'DEP002');

-- Dữ liệu cho bảng Project
INSERT INTO Project (project_id, project_name, description, status, deadline) VALUES
(1, 'Project Alpha', 'This is the first project.', 'In Progress', '2024-12-31'),
(2, 'Project Beta', 'This is the second project.', 'Pending', '2025-01-15'),
(3, 'Project Gamma', 'This is the third project.', 'Completed', '2024-09-30');


-- Dữ liệu cho bảng EmployeeProject
INSERT INTO EmployeeProject (account_id, project_id) VALUES
('ACC003', 1),
('ACC004', 1),
('ACC005', 1);

-- Dữ liệu cho bảng EmployeeTeam
INSERT INTO EmployeeTeam (account_id, team_id) VALUES
('ACC003', 'TEAM001'),
('ACC004', 'TEAM001'),
('ACC005', 'TEAM001');

-- Dữ liệu cho bảng Staff
INSERT INTO StaffAndLeader (account_id, total_kpi, total_timeliness, total_teamwork, credit_points, number_of_team) VALUES
('ACC003',  3, 2, 3, 100, 1),
('ACC004', 4, 3, 4, 120, 1),
('ACC005',  4, 2, 3, 200, 1);

-- Dữ liệu cho bảng CreditExchange
INSERT INTO CreditExchange (account_id, exchange_date, credit_points_used, cash_amount, status) VALUES
('ACC003', '2024-10-05', 10, 50.00, 'Completed'),
('ACC004', '2024-10-06', 15, 75.00, 'Pending');

-- Dữ liệu cho bảng Task
INSERT INTO Task (task_id, task_name, description, status, priority, deadline, submission_date, project_id) VALUES
('TASK001', 'Prepare for Meeting', 'Prepare anything needed for the meeting.', 'In Progress', 1, '2024-11-01', NULL, 1),
('TASK002', 'Publish article', 'Publish an article to recruit new employees.', 'Pending', 2, '2024-11-15', NULL, 1);

-- Dữ liệu cho bảng TaskEvaluation
INSERT INTO TaskEvaluation (evaluation_id, evaluation_date, comment, task_id) VALUES
('EVAL001', '2024-10-01', 'Good progress on the task.', 'TASK002'),
('EVAL002', '2024-10-02', 'Needs more effort on development.', 'TASK001');

-- Dữ liệu cho bảng Subtask
INSERT INTO Subtask (subtask_id, subtask_name, description, status, priority, difficulty, deadline, submission_date, task_id, team_id) VALUES
('SUBTASK001', 'Invintes involes people.', 'there are one from outside.', 'Completed', 1, 2, '2024-10-5', NULL, 'TASK001', 'TEAM001'),
('SUBTASK002', 'Publish Article Subtask', 'Prepare content for article.', 'Completed', 2, 3, '2024-10-20', NULL, 'TASK002', 'TEAM001'),
('SUBTASK003', 'Prepare Camera, documents', 'Camera must be abc.', 'In Progress', 2, 1, '2024-10-6', NULL, 'TASK001', 'TEAM001'),
('SUBTASK004', 'Buy drinks.', 'only water.', 'Not Start', 3, 1, '2024-10-7', NULL, 'TASK001', 'TEAM001'),
('SUBTASK005', 'Mail for every one who will attend.', 'Find other mails in my chat.', 'Pending', 1, 2, '2024-10-7', NULL, 'TASK001', 'TEAM001');

-- Chèn dữ liệu vào TaskAssignment
INSERT INTO TaskAssignment (task_id, created_by, assigned_to) VALUES
('TASK001', 'ACC006', 'ACC004'),
('TASK002', 'ACC006', 'ACC004');

-- Dữ liệu cho bảng PersonalTask
INSERT INTO PersonalTask (ptask_id, account_id, status, priority, assignment_date, deadline, description) VALUES
('PT001', 'ACC003', 'Not Started', 1, '2024-10-10 08:30:00', '2024-10-15', 'Review team performance reports.'),
('PT002', 'ACC004', 'In Progress', 2, '2024-10-11 09:00:00', '2024-10-20', 'Prepare for the quarterly team meeting.'),
('PT003', 'ACC003', 'Completed', 3, '2024-09-15 14:00:00', '2024-09-20', 'Complete the yearly project review documentation.'),
('PT004', 'ACC005', 'In Progress', 1, '2024-10-05 13:00:00', '2024-10-25', 'Research new tools for project management.'),
('PT005', 'ACC004', 'Not Started', 2, '2024-10-12 10:00:00', '2024-11-01', 'Draft proposals for upcoming team projects.'),
('PT006', 'ACC005', 'In Progress', 3, '2024-10-14 15:00:00', '2024-10-30', 'Organize files and update documentation for all ongoing projects.');

-- Chèn dữ liệu vào SubtaskAssignment
INSERT INTO SubtaskAssignment (subtask_id, created_by, assigned_to) VALUES
('SUBTASK001', 'ACC004', 'ACC003'),
('SUBTASK002', 'ACC004', 'ACC005'),
('SUBTASK003', 'ACC004', 'ACC003'),
('SUBTASK004', 'ACC004', 'ACC003'),
('SUBTASK005', 'ACC004', 'ACC003');

-- Dữ liệu cho bảng SubtaskEvaluation
INSERT INTO SubtaskEvaluation (evaluation_id, evaluation_date, comment, subtask_id, teamwork_rating, timeliness_rating, kpi_rating) VALUES
('SEVAL001', '2024-10-01', 'Great job!', 'SUBTASK002', 4, 5, 3),
('SEVAL002', '2024-10-02', 'setup is on track.', 'SUBTASK001', 4, 4, 4);

-- Dữ liệu cho bảng Credit
INSERT INTO Credit (difficulty, credits) VALUES
(1, 1),
(2, 2),
(3, 3),
(4, 4),
(5, 5);

-- Dữ liệu cho bảng SubtaskCredit
INSERT INTO SubtaskCredit (subtask_id, difficulty) VALUES
('SUBTASK001', 2),
('SUBTASK002', 3);

-- Dữ liệu cho bảng Comment
INSERT INTO Comment (comment_id, content, date_submitted, subtask_id) VALUES
('CMT001', 'Looks great, keep it up!', '2024-10-03', 'SUBTASK001'),
('CMT002', 'Need to add more details.', '2024-10-04', 'SUBTASK002');