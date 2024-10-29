using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TaskForge.Models;

namespace TaskForge.DBContext;

public partial class TaskForgeContext : DbContext
{
    public TaskForgeContext()
    {
    }

    public TaskForgeContext(DbContextOptions<TaskForgeContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Credit> Credits { get; set; }

    public virtual DbSet<CreditExchange> CreditExchanges { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<DepartmentTask> DepartmentTasks { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<EmployeeProject> EmployeeProjects { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Models.File> Files { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<PersonalTask> PersonalTasks { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<StaffAndLeader> StaffAndLeaders { get; set; }

    public virtual DbSet<Subtask> Subtasks { get; set; }

    public virtual DbSet<SubtaskAssignment> SubtaskAssignments { get; set; }

    public virtual DbSet<SubtaskEvaluation> SubtaskEvaluations { get; set; }

    public virtual DbSet<Models.Task> Tasks { get; set; }

    public virtual DbSet<TaskEvaluation> TaskEvaluations { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=LAPTOP-IP1RMDOK\\MAYCHU;Initial Catalog=TaskForge;User ID=sa;Password=123456; Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__Account__46A222CD49AAFFE1");

            entity.ToTable("Account");

            entity.Property(e => e.AccountId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("account_id");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("phone_number");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("username");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__Comment__E7957687624DCC99");

            entity.ToTable("Comment");

            entity.Property(e => e.CommentId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("comment_id");
            entity.Property(e => e.Content)
                .HasColumnType("text")
                .HasColumnName("content");
            entity.Property(e => e.DateSubmitted)
                .HasColumnType("datetime")
                .HasColumnName("date_submitted");
            entity.Property(e => e.SubtaskId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("subtask_id");

            entity.HasOne(d => d.Subtask).WithMany(p => p.Comments)
                .HasForeignKey(d => d.SubtaskId)
                .HasConstraintName("FK__Comment__subtask__76969D2E");
        });

        modelBuilder.Entity<Credit>(entity =>
        {
            entity.HasKey(e => e.Difficulty).HasName("PK__Credit__79CF999F5F09318B");

            entity.ToTable("Credit");

            entity.Property(e => e.Difficulty)
                .ValueGeneratedNever()
                .HasColumnName("difficulty");
            entity.Property(e => e.Credits).HasColumnName("credits");
        });

        modelBuilder.Entity<CreditExchange>(entity =>
        {
            entity.HasKey(e => e.ExchangeId).HasName("PK__CreditEx__FAAC5D3EDA864C8B");

            entity.ToTable("CreditExchange");

            entity.Property(e => e.ExchangeId).HasColumnName("exchange_id");
            entity.Property(e => e.AccountId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("account_id");
            entity.Property(e => e.CashAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("cash_amount");
            entity.Property(e => e.CreditPointsUsed).HasColumnName("credit_points_used");
            entity.Property(e => e.ExchangeDate)
                .HasColumnType("datetime")
                .HasColumnName("exchange_date");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("Pending")
                .HasColumnName("status");

            entity.HasOne(d => d.Account).WithMany(p => p.CreditExchanges)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK__CreditExc__accou__5629CD9C");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DeptId).HasName("PK__Departme__DCA65974EE222045");

            entity.ToTable("Department");

            entity.Property(e => e.DeptId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("dept_id");
            entity.Property(e => e.DeptName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("dept_name");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.NumberOfTeam).HasColumnName("number_of_team");
        });

        modelBuilder.Entity<DepartmentTask>(entity =>
        {
            entity.HasKey(e => new { e.TaskId, e.DeptId }).HasName("PK__Departme__5958711AA143D517");

            entity.ToTable("DepartmentTask");

            entity.Property(e => e.TaskId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("task_id");
            entity.Property(e => e.DeptId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("dept_id");
            entity.Property(e => e.AdditonalDept)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("additonal_dept");
            entity.Property(e => e.DeptParticipantCount).HasColumnName("dept_participant_count");

            entity.HasOne(d => d.Dept).WithMany(p => p.DepartmentTasks)
                .HasForeignKey(d => d.DeptId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Departmen__dept___5CD6CB2B");

            entity.HasOne(d => d.Task).WithMany(p => p.DepartmentTasks)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Departmen__task___5BE2A6F2");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__Employee__46A222CDBDA33CD3");

            entity.ToTable("Employee");

            entity.Property(e => e.AccountId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("account_id");
            entity.Property(e => e.DeptId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("dept_id");
            entity.Property(e => e.Dob).HasColumnName("dob");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.Fullname)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("fullname");
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("gender");
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("role");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");

            entity.HasOne(d => d.Account).WithOne(p => p.Employee)
                .HasForeignKey<Employee>(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Employee__accoun__412EB0B6");

            entity.HasOne(d => d.Dept).WithMany(p => p.Employees)
                .HasForeignKey(d => d.DeptId)
                .HasConstraintName("FK__Employee__dept_i__4222D4EF");

            entity.HasMany(d => d.Teams).WithMany(p => p.Accounts)
                .UsingEntity<Dictionary<string, object>>(
                    "EmployeeTeam",
                    r => r.HasOne<Team>().WithMany()
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__EmployeeT__team___4F7CD00D"),
                    l => l.HasOne<Employee>().WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__EmployeeT__accou__4E88ABD4"),
                    j =>
                    {
                        j.HasKey("AccountId", "TeamId").HasName("PK__Employee__9920FC1613BAA4DA");
                        j.ToTable("EmployeeTeam");
                        j.IndexerProperty<string>("AccountId")
                            .HasMaxLength(10)
                            .IsUnicode(false)
                            .HasColumnName("account_id");
                        j.IndexerProperty<string>("TeamId")
                            .HasMaxLength(10)
                            .IsUnicode(false)
                            .HasColumnName("team_id");
                    });
        });

        modelBuilder.Entity<EmployeeProject>(entity =>
        {
            entity.HasKey(e => new { e.AccountId, e.ProjectId }).HasName("PK__Employee__AD65BB2C8A97A26D");

            entity.ToTable("EmployeeProject");

            entity.Property(e => e.AccountId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("account_id");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("role");

            entity.HasOne(d => d.Account).WithMany(p => p.EmployeeProjects)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EmployeeP__accou__4AB81AF0");

            entity.HasOne(d => d.Project).WithMany(p => p.EmployeeProjects)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EmployeeP__proje__4BAC3F29");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__Feedback__7A6B2B8C3A2DE89E");

            entity.ToTable("Feedback");

            entity.Property(e => e.FeedbackId)
                .ValueGeneratedNever()
                .HasColumnName("feedback_id");
            entity.Property(e => e.AccountId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("account_id");
            entity.Property(e => e.Context)
                .HasColumnType("text")
                .HasColumnName("context");
            entity.Property(e => e.DateSubmitted)
                .HasColumnType("datetime")
                .HasColumnName("date_submitted");

            entity.HasOne(d => d.Account).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK__Feedback__accoun__398D8EEE");
        });

        modelBuilder.Entity<Models.File>(entity =>
        {
            entity.HasKey(e => e.FileId).HasName("PK__File__07D884C6B4C1D5E0");

            entity.ToTable("File");

            entity.Property(e => e.FileId)
                .HasMaxLength(50)
                .HasColumnName("file_id");
            entity.Property(e => e.AccountId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("account_id");
            entity.Property(e => e.FileName)
                .HasMaxLength(255)
                .HasColumnName("file_name");
            entity.Property(e => e.FilePath)
                .HasMaxLength(255)
                .HasColumnName("file_path");
            entity.Property(e => e.SubtaskId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("subtask_id");
            entity.Property(e => e.UploadDate).HasColumnName("upload_date");

            entity.HasOne(d => d.Account).WithMany(p => p.Files)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK__File__account_id__797309D9");

            entity.HasOne(d => d.Subtask).WithMany(p => p.Files)
                .HasForeignKey(d => d.SubtaskId)
                .HasConstraintName("FK__File__subtask_id__7A672E12");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__E059842F2568CA1E");

            entity.Property(e => e.NotificationId).HasColumnName("notification_id");
            entity.Property(e => e.AccountId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("account_id");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.IsRead)
                .HasDefaultValue(false)
                .HasColumnName("is_read");
            entity.Property(e => e.Message)
                .HasMaxLength(500)
                .HasColumnName("message");

            entity.HasOne(d => d.Account).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK__Notificat__is_re__7E37BEF6");
        });

        modelBuilder.Entity<PersonalTask>(entity =>
        {
            entity.HasKey(e => e.PtaskId).HasName("PK__Personal__C9316169234E3ED5");

            entity.ToTable("PersonalTask");

            entity.Property(e => e.PtaskId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ptask_id");
            entity.Property(e => e.AccountId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("account_id");
            entity.Property(e => e.AssignmentDate)
                .HasColumnType("datetime")
                .HasColumnName("assignment_date");
            entity.Property(e => e.Deadline)
                .HasColumnType("datetime")
                .HasColumnName("deadline");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.Priority).HasColumnName("priority");
            entity.Property(e => e.PtaskName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ptask_name");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");

            entity.HasOne(d => d.Account).WithMany(p => p.PersonalTasks)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK__PersonalT__accou__5FB337D6");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("PK__Project__BC799E1FE754957D");

            entity.ToTable("Project");

            entity.Property(e => e.ProjectId)
                .ValueGeneratedNever()
                .HasColumnName("project_id");
            entity.Property(e => e.Deadline)
                .HasColumnType("datetime")
                .HasColumnName("deadline");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.ProjectName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("project_name");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");

            entity.HasMany(d => d.Departments).WithMany(p => p.Projects)
                .UsingEntity<Dictionary<string, object>>(
                    "DepartmentProject",
                    r => r.HasOne<Department>().WithMany()
                        .HasForeignKey("DeptId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Departmen__dept___47DBAE45"),
                    l => l.HasOne<Project>().WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Departmen__proje__46E78A0C"),
                    j =>
                    {
                        j.HasKey("ProjectId", "DeptId").HasName("PK__Departme__E1B3FB88DB6C9722");
                        j.ToTable("DepartmentProject");
                        j.IndexerProperty<int>("ProjectId").HasColumnName("project_id");
                        j.IndexerProperty<string>("DeptId")
                            .HasMaxLength(10)
                            .IsUnicode(false)
                            .HasColumnName("dept_id");
                    });
        });

        modelBuilder.Entity<StaffAndLeader>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__StaffAnd__46A222CDDB30E327");

            entity.ToTable("StaffAndLeader");

            entity.Property(e => e.AccountId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("account_id");
            entity.Property(e => e.CreditPoints).HasColumnName("credit_points");
            entity.Property(e => e.NumberOfTeam).HasColumnName("number_of_team");
            entity.Property(e => e.TotalKpi)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("total_kpi");
            entity.Property(e => e.TotalTeamwork)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("total_teamwork");
            entity.Property(e => e.TotalTimeliness)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("total_timeliness");

            entity.HasOne(d => d.Account).WithOne(p => p.StaffAndLeader)
                .HasForeignKey<StaffAndLeader>(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__StaffAndL__accou__52593CB8");
        });

        modelBuilder.Entity<Subtask>(entity =>
        {
            entity.HasKey(e => e.SubtaskId).HasName("PK__Subtask__C2AC5F05C420AF84");

            entity.ToTable("Subtask");

            entity.Property(e => e.SubtaskId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("subtask_id");
            entity.Property(e => e.AssignmentDate)
                .HasColumnType("datetime")
                .HasColumnName("assignment_date");
            entity.Property(e => e.Deadline)
                .HasColumnType("datetime")
                .HasColumnName("deadline");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.Difficulty).HasColumnName("difficulty");
            entity.Property(e => e.Priority).HasColumnName("priority");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.SubmissionDate)
                .HasColumnType("datetime")
                .HasColumnName("submission_date");
            entity.Property(e => e.SubtaskName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("subtask_name");
            entity.Property(e => e.TaskId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("task_id");
            entity.Property(e => e.TeamId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("team_id");

            entity.HasOne(d => d.Task).WithMany(p => p.Subtasks)
                .HasForeignKey(d => d.TaskId)
                .HasConstraintName("FK__Subtask__task_id__656C112C");

            entity.HasOne(d => d.Team).WithMany(p => p.Subtasks)
                .HasForeignKey(d => d.TeamId)
                .HasConstraintName("FK__Subtask__team_id__66603565");

            entity.HasMany(d => d.Difficulties).WithMany(p => p.Subtasks)
                .UsingEntity<Dictionary<string, object>>(
                    "SubtaskCredit",
                    r => r.HasOne<Credit>().WithMany()
                        .HasForeignKey("Difficulty")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__SubtaskCr__diffi__73BA3083"),
                    l => l.HasOne<Subtask>().WithMany()
                        .HasForeignKey("SubtaskId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__SubtaskCr__subta__72C60C4A"),
                    j =>
                    {
                        j.HasKey("SubtaskId", "Difficulty").HasName("PK__SubtaskC__2530A69C404E73F8");
                        j.ToTable("SubtaskCredit");
                        j.IndexerProperty<string>("SubtaskId")
                            .HasMaxLength(10)
                            .IsUnicode(false)
                            .HasColumnName("subtask_id");
                        j.IndexerProperty<int>("Difficulty").HasColumnName("difficulty");
                    });
        });

        modelBuilder.Entity<SubtaskAssignment>(entity =>
        {
            entity.HasKey(e => new { e.SubtaskId, e.CreatedBy, e.AssignedTo }).HasName("PK__SubtaskA__D9CB10DF1792BAD5");

            entity.ToTable("SubtaskAssignment");

            entity.Property(e => e.SubtaskId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("subtask_id");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("created_by");
            entity.Property(e => e.AssignedTo)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("assigned_to");

            entity.HasOne(d => d.AssignedToNavigation).WithMany(p => p.SubtaskAssignmentAssignedToNavigations)
                .HasForeignKey(d => d.AssignedTo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SubtaskAs__assig__6B24EA82");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.SubtaskAssignmentCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SubtaskAs__creat__6A30C649");

            entity.HasOne(d => d.Subtask).WithMany(p => p.SubtaskAssignments)
                .HasForeignKey(d => d.SubtaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SubtaskAs__subta__693CA210");
        });

        modelBuilder.Entity<SubtaskEvaluation>(entity =>
        {
            entity.HasKey(e => e.EvaluationId).HasName("PK__SubtaskE__827C592D0D40C3D1");

            entity.ToTable("SubtaskEvaluation");

            entity.Property(e => e.EvaluationId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("evaluation_id");
            entity.Property(e => e.Comment)
                .HasColumnType("text")
                .HasColumnName("comment");
            entity.Property(e => e.EvaluationDate)
                .HasColumnType("datetime")
                .HasColumnName("evaluation_date");
            entity.Property(e => e.KpiRating).HasColumnName("kpi_rating");
            entity.Property(e => e.SubtaskId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("subtask_id");
            entity.Property(e => e.TeamworkRating).HasColumnName("teamwork_rating");
            entity.Property(e => e.TimelinessRating).HasColumnName("timeliness_rating");

            entity.HasOne(d => d.Subtask).WithMany(p => p.SubtaskEvaluations)
                .HasForeignKey(d => d.SubtaskId)
                .HasConstraintName("FK__SubtaskEv__subta__6E01572D");
        });

        modelBuilder.Entity<Models.Task>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PK__Task__0492148D14C95E05");

            entity.ToTable("Task");

            entity.Property(e => e.TaskId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("task_id");
            entity.Property(e => e.AssignmentDate)
                .HasColumnType("datetime")
                .HasColumnName("assignment_date");
            entity.Property(e => e.Deadline)
                .HasColumnType("datetime")
                .HasColumnName("deadline");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.Priority).HasColumnName("priority");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.SubmissionDate)
                .HasColumnType("datetime")
                .HasColumnName("submission_date");
            entity.Property(e => e.TaskName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("task_name");

            entity.HasOne(d => d.Project).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK__Task__project_id__59063A47");
        });

        modelBuilder.Entity<TaskEvaluation>(entity =>
        {
            entity.HasKey(e => e.EvaluationId).HasName("PK__TaskEval__827C592D1FB8F7E4");

            entity.ToTable("TaskEvaluation");

            entity.Property(e => e.EvaluationId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("evaluation_id");
            entity.Property(e => e.Comment)
                .HasColumnType("text")
                .HasColumnName("comment");
            entity.Property(e => e.EvaluationDate)
                .HasColumnType("datetime")
                .HasColumnName("evaluation_date");
            entity.Property(e => e.TaskId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("task_id");

            entity.HasOne(d => d.Task).WithMany(p => p.TaskEvaluations)
                .HasForeignKey(d => d.TaskId)
                .HasConstraintName("FK__TaskEvalu__task___628FA481");
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.TeamId).HasName("PK__Team__F82DEDBC953CCD14");

            entity.ToTable("Team");

            entity.Property(e => e.TeamId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("team_id");
            entity.Property(e => e.CreatedDate).HasColumnName("created_date");
            entity.Property(e => e.DeptId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("dept_id");
            entity.Property(e => e.NumberOfMember).HasColumnName("number_of_member");
            entity.Property(e => e.TeamName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("team_name");

            entity.HasOne(d => d.Dept).WithMany(p => p.Teams)
                .HasForeignKey(d => d.DeptId)
                .HasConstraintName("FK__Team__dept_id__3E52440B");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
