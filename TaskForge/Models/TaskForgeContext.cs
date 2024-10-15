using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TaskForge.Models;

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

    public virtual DbSet<EmployeeTask> EmployeeTasks { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<Staff> Staff { get; set; }

    public virtual DbSet<Subtask> Subtasks { get; set; }

    public virtual DbSet<SubtaskEvaluation> SubtaskEvaluations { get; set; }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<TaskEvaluation> TaskEvaluations { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=LAPTOP-LL;Initial Catalog=TaskForge;User ID=sa;Password=admin@123; Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__Account__46A222CD2B642543");

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
            entity.HasKey(e => e.CommentId).HasName("PK__Comment__E7957687D114341C");

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
                .HasConstraintName("FK__Comment__subtask__6FE99F9F");
        });

        modelBuilder.Entity<Credit>(entity =>
        {
            entity.HasKey(e => e.Difficulty).HasName("PK__Credit__79CF999F9DCFC3DB");

            entity.ToTable("Credit");

            entity.Property(e => e.Difficulty)
                .ValueGeneratedNever()
                .HasColumnName("difficulty");
            entity.Property(e => e.Credits).HasColumnName("credits");
        });

        modelBuilder.Entity<CreditExchange>(entity =>
        {
            entity.HasKey(e => e.ExchangeId).HasName("PK__CreditEx__FAAC5D3E12AFBC5C");

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
                .HasConstraintName("FK__CreditExc__accou__571DF1D5");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DeptId).HasName("PK__Departme__DCA65974769E54B4");

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
            entity.HasKey(e => e.AccountId).HasName("PK__Employee__46A222CD074822CB");

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
                        j.HasKey("AccountId", "ProjectId").HasName("PK__Employee__AD65BB2C02134524");
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
                        j.HasKey("AccountId", "TeamId").HasName("PK__Employee__9920FC164B736ECB");
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

        modelBuilder.Entity<EmployeeTask>(entity =>
        {
            entity.HasKey(e => new { e.TaskId, e.CreatedBy, e.AssignedTo }).HasName("PK__Employee__1FF55B57D4681875");

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
                .HasConstraintName("FK__EmployeeT__assig__4F7CD00D");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.EmployeeTaskCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EmployeeT__creat__4E88ABD4");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__Feedback__7A6B2B8CD4D1ED75");

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
            entity.HasKey(e => e.ProjectId).HasName("PK__Project__BC799E1F6247AAF4");

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
            entity.HasKey(e => e.AccountId).HasName("PK__Staff__46A222CDA4EB7317");

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
                .HasConstraintName("FK__Staff__account_i__52593CB8");

            entity.HasOne(d => d.Dept).WithMany(p => p.Staff)
                .HasForeignKey(d => d.DeptId)
                .HasConstraintName("FK__Staff__dept_id__534D60F1");
        });

        modelBuilder.Entity<Subtask>(entity =>
        {
            entity.HasKey(e => e.SubtaskId).HasName("PK__Subtask__C2AC5F05B6EA6676");

            entity.ToTable("Subtask");

            entity.Property(e => e.SubtaskId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("subtask_id");
            entity.Property(e => e.AssignedTo)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("assigned_to");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("created_by");
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

            entity.HasOne(d => d.AssignedToNavigation).WithMany(p => p.SubtaskAssignedToNavigations)
                .HasForeignKey(d => d.AssignedTo)
                .HasConstraintName("FK__Subtask__assigne__628FA481");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.SubtaskCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Subtask__created__619B8048");

            entity.HasOne(d => d.Task).WithMany(p => p.Subtasks)
                .HasForeignKey(d => d.TaskId)
                .HasConstraintName("FK__Subtask__task_id__6383C8BA");

            entity.HasOne(d => d.Team).WithMany(p => p.Subtasks)
                .HasForeignKey(d => d.TeamId)
                .HasConstraintName("FK__Subtask__team_id__6477ECF3");

            entity.HasMany(d => d.Difficulties).WithMany(p => p.Subtasks)
                .UsingEntity<Dictionary<string, object>>(
                    "SubtaskCredit",
                    r => r.HasOne<Credit>().WithMany()
                        .HasForeignKey("Difficulty")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__SubtaskCr__diffi__6D0D32F4"),
                    l => l.HasOne<Subtask>().WithMany()
                        .HasForeignKey("SubtaskId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__SubtaskCr__subta__6C190EBB"),
                    j =>
                    {
                        j.HasKey("SubtaskId", "Difficulty").HasName("PK__SubtaskC__2530A69CFD1C0C07");
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
            entity.HasKey(e => e.EvaluationId).HasName("PK__SubtaskE__827C592DCA06B676");

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
                .HasConstraintName("FK__SubtaskEv__subta__6754599E");
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PK__Task__0492148D2B4637CA");

            entity.ToTable("Task");

            entity.Property(e => e.TaskId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("task_id");
            entity.Property(e => e.AssignedTo)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("assigned_to");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("created_by");
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

            entity.HasOne(d => d.AssignedToNavigation).WithMany(p => p.TaskAssignedToNavigations)
                .HasForeignKey(d => d.AssignedTo)
                .HasConstraintName("FK__Task__assigned_t__5AEE82B9");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TaskCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Task__created_by__59FA5E80");

            entity.HasOne(d => d.Project).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK__Task__project_id__5BE2A6F2");
        });

        modelBuilder.Entity<TaskEvaluation>(entity =>
        {
            entity.HasKey(e => e.EvaluationId).HasName("PK__TaskEval__827C592D4E7C098F");

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
                .HasConstraintName("FK__TaskEvalu__task___5EBF139D");
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.TeamId).HasName("PK__Team__F82DEDBCB7E50EAE");

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
