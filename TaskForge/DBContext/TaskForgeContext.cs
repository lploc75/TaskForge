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

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<EmployeeSubTask> EmployeeSubTasks { get; set; }

    public virtual DbSet<EmployeeTask> EmployeeTasks { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<Staff> Staff { get; set; }

    public virtual DbSet<Subtask> Subtasks { get; set; }

    public virtual DbSet<SubtaskEvaluation> SubtaskEvaluations { get; set; }

    public virtual DbSet<Models.Task> Tasks { get; set; }

    public virtual DbSet<TaskEvaluation> TaskEvaluations { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=TaskForge;User ID=sa;Password=12345; Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__Account__46A222CD0285EA15");

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
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("role");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("username");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__Comment__E795768735DFAD1F");

            entity.ToTable("Comment");

            entity.Property(e => e.CommentId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("comment_id");
            entity.Property(e => e.Content)
                .HasColumnType("text")
                .HasColumnName("content");
            entity.Property(e => e.DateSubmitted).HasColumnName("date_submitted");
            entity.Property(e => e.SubtaskId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("subtask_id");

            entity.HasOne(d => d.Subtask).WithMany(p => p.Comments)
                .HasForeignKey(d => d.SubtaskId)
                .HasConstraintName("FK__Comment__subtask__71D1E811");
        });

        modelBuilder.Entity<Credit>(entity =>
        {
            entity.HasKey(e => e.Difficulty).HasName("PK__Credit__79CF999FE1A88A91");

            entity.ToTable("Credit");

            entity.Property(e => e.Difficulty)
                .ValueGeneratedNever()
                .HasColumnName("difficulty");
            entity.Property(e => e.Credits).HasColumnName("credits");
        });

        modelBuilder.Entity<CreditExchange>(entity =>
        {
            entity.HasKey(e => e.ExchangeId).HasName("PK__CreditEx__FAAC5D3ED4E6EFCE");

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
            entity.Property(e => e.ExchangeDate).HasColumnName("exchange_date");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("Pending")
                .HasColumnName("status");

            entity.HasOne(d => d.Account).WithMany(p => p.CreditExchanges)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK__CreditExc__accou__534D60F1");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DeptId).HasName("PK__Departme__DCA65974417E6C20");

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

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__Employee__46A222CD0B3829D2");

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

            entity.HasMany(d => d.Projects).WithMany(p => p.Accounts)
                .UsingEntity<Dictionary<string, object>>(
                    "EmployeeProject",
                    r => r.HasOne<Project>().WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__EmployeeP__proje__47DBAE45"),
                    l => l.HasOne<Employee>().WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__EmployeeP__accou__46E78A0C"),
                    j =>
                    {
                        j.HasKey("AccountId", "ProjectId").HasName("PK__Employee__AD65BB2CD1A268EC");
                        j.ToTable("EmployeeProject");
                        j.IndexerProperty<string>("AccountId")
                            .HasMaxLength(10)
                            .IsUnicode(false)
                            .HasColumnName("account_id");
                        j.IndexerProperty<int>("ProjectId").HasColumnName("project_id");
                    });

            entity.HasMany(d => d.Teams).WithMany(p => p.Accounts)
                .UsingEntity<Dictionary<string, object>>(
                    "EmployeeTeam",
                    r => r.HasOne<Team>().WithMany()
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__EmployeeT__team___4BAC3F29"),
                    l => l.HasOne<Employee>().WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__EmployeeT__accou__4AB81AF0"),
                    j =>
                    {
                        j.HasKey("AccountId", "TeamId").HasName("PK__Employee__9920FC16136261BC");
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

        modelBuilder.Entity<EmployeeSubTask>(entity =>
        {
            entity.HasKey(e => new { e.SubtaskId, e.CreatedBy, e.AssignedTo }).HasName("PK__Employee__D9CB10DFC77B0BBD");

            entity.ToTable("EmployeeSubTask");

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

            entity.HasOne(d => d.AssignedToNavigation).WithMany(p => p.EmployeeSubTaskAssignedToNavigations)
                .HasForeignKey(d => d.AssignedTo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EmployeeS__assig__66603565");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.EmployeeSubTaskCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EmployeeS__creat__656C112C");

            entity.HasOne(d => d.Subtask).WithMany(p => p.EmployeeSubTasks)
                .HasForeignKey(d => d.SubtaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EmployeeS__subta__6477ECF3");
        });

        modelBuilder.Entity<EmployeeTask>(entity =>
        {
            entity.HasKey(e => new { e.TaskId, e.CreatedBy, e.AssignedTo }).HasName("PK__Employee__1FF55B57C721714F");

            entity.ToTable("EmployeeTask");

            entity.Property(e => e.TaskId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("task_id");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("created_by");
            entity.Property(e => e.AssignedTo)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("assigned_to");

            entity.HasOne(d => d.AssignedToNavigation).WithMany(p => p.EmployeeTaskAssignedToNavigations)
                .HasForeignKey(d => d.AssignedTo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EmployeeT__assig__5AEE82B9");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.EmployeeTaskCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EmployeeT__creat__59FA5E80");

            entity.HasOne(d => d.Task).WithMany(p => p.EmployeeTasks)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EmployeeT__task___59063A47");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__Feedback__7A6B2B8C4A74DC5F");

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
            entity.Property(e => e.DateSubmitted).HasColumnName("date_submitted");

            entity.HasOne(d => d.Account).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK__Feedback__accoun__398D8EEE");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("PK__Project__BC799E1F30096492");

            entity.ToTable("Project");

            entity.Property(e => e.ProjectId)
                .ValueGeneratedNever()
                .HasColumnName("project_id");
            entity.Property(e => e.Deadline).HasColumnName("deadline");
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
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__Staff__46A222CDA7916D4B");

            entity.Property(e => e.AccountId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("account_id");
            entity.Property(e => e.CreditPoints).HasColumnName("credit_points");
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
            entity.Property(e => e.NumberOfTeam).HasColumnName("number_of_team");
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("role");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.TotalKpi)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("total_kpi");
            entity.Property(e => e.TotalTeamwork)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("total_teamwork");
            entity.Property(e => e.TotalTimeliness)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("total_timeliness");

            entity.HasOne(d => d.Account).WithOne(p => p.Staff)
                .HasForeignKey<Staff>(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Staff__account_i__4E88ABD4");

            entity.HasOne(d => d.Dept).WithMany(p => p.Staff)
                .HasForeignKey(d => d.DeptId)
                .HasConstraintName("FK__Staff__dept_id__4F7CD00D");
        });

        modelBuilder.Entity<Subtask>(entity =>
        {
            entity.HasKey(e => e.SubtaskId).HasName("PK__Subtask__C2AC5F056ABE8ADD");

            entity.ToTable("Subtask");

            entity.Property(e => e.SubtaskId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("subtask_id");
            entity.Property(e => e.Deadline).HasColumnName("deadline");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.Difficulty).HasColumnName("difficulty");
            entity.Property(e => e.Priority).HasColumnName("priority");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.SubmissionDate).HasColumnName("submission_date");
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
                .HasConstraintName("FK__Subtask__task_id__60A75C0F");

            entity.HasOne(d => d.Team).WithMany(p => p.Subtasks)
                .HasForeignKey(d => d.TeamId)
                .HasConstraintName("FK__Subtask__team_id__619B8048");

            entity.HasMany(d => d.Difficulties).WithMany(p => p.Subtasks)
                .UsingEntity<Dictionary<string, object>>(
                    "SubtaskCredit",
                    r => r.HasOne<Credit>().WithMany()
                        .HasForeignKey("Difficulty")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__SubtaskCr__diffi__6EF57B66"),
                    l => l.HasOne<Subtask>().WithMany()
                        .HasForeignKey("SubtaskId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__SubtaskCr__subta__6E01572D"),
                    j =>
                    {
                        j.HasKey("SubtaskId", "Difficulty").HasName("PK__SubtaskC__2530A69CD1C111CB");
                        j.ToTable("SubtaskCredit");
                        j.IndexerProperty<string>("SubtaskId")
                            .HasMaxLength(10)
                            .IsUnicode(false)
                            .HasColumnName("subtask_id");
                        j.IndexerProperty<int>("Difficulty").HasColumnName("difficulty");
                    });
        });

        modelBuilder.Entity<SubtaskEvaluation>(entity =>
        {
            entity.HasKey(e => e.EvaluationId).HasName("PK__SubtaskE__827C592DDE5AF078");

            entity.ToTable("SubtaskEvaluation");

            entity.Property(e => e.EvaluationId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("evaluation_id");
            entity.Property(e => e.Comment)
                .HasColumnType("text")
                .HasColumnName("comment");
            entity.Property(e => e.EvaluationDate).HasColumnName("evaluation_date");
            entity.Property(e => e.KpiRating).HasColumnName("kpi_rating");
            entity.Property(e => e.SubtaskId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("subtask_id");
            entity.Property(e => e.TeamworkRating).HasColumnName("teamwork_rating");
            entity.Property(e => e.TimelinessRating).HasColumnName("timeliness_rating");

            entity.HasOne(d => d.Subtask).WithMany(p => p.SubtaskEvaluations)
                .HasForeignKey(d => d.SubtaskId)
                .HasConstraintName("FK__SubtaskEv__subta__693CA210");
        });

        modelBuilder.Entity<Models.Task>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PK__Task__0492148D674500F6");

            entity.ToTable("Task");

            entity.Property(e => e.TaskId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("task_id");
            entity.Property(e => e.Deadline).HasColumnName("deadline");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.Priority).HasColumnName("priority");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.SubmissionDate).HasColumnName("submission_date");
            entity.Property(e => e.TaskName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("task_name");

            entity.HasOne(d => d.Project).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK__Task__project_id__5629CD9C");
        });

        modelBuilder.Entity<TaskEvaluation>(entity =>
        {
            entity.HasKey(e => e.EvaluationId).HasName("PK__TaskEval__827C592DAFA526CE");

            entity.ToTable("TaskEvaluation");

            entity.Property(e => e.EvaluationId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("evaluation_id");
            entity.Property(e => e.Comment)
                .HasColumnType("text")
                .HasColumnName("comment");
            entity.Property(e => e.EvaluationDate).HasColumnName("evaluation_date");
            entity.Property(e => e.TaskId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("task_id");

            entity.HasOne(d => d.Task).WithMany(p => p.TaskEvaluations)
                .HasForeignKey(d => d.TaskId)
                .HasConstraintName("FK__TaskEvalu__task___5DCAEF64");
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.TeamId).HasName("PK__Team__F82DEDBC1109083C");

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
