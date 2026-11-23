using Microsoft.EntityFrameworkCore;
using TestingPlatform.Domain.Models;

namespace TestingPlatform.Infrastructure
{
    /// <summary>
    /// Контекст базы данных
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Пользователи (менеджеры и студенты)
        /// </summary>
        public DbSet<User> Users => Set<User>();

        /// <summary>
        /// Студенты
        /// </summary>
        public DbSet<Student> Students => Set<Student>();

        /// <summary>
        /// Проекты
        /// </summary>
        public DbSet<Project> Projects => Set<Project>();

        /// <summary>
        /// Группы
        /// </summary>
        public DbSet<Group> Groups => Set<Group>();

        /// <summary>
        /// Направления
        /// </summary>
        public DbSet<Direction> Directions => Set<Direction>();

        /// <summary>
        /// Курсы
        /// </summary>
        public DbSet<Course> Courses => Set<Course>();

        /// <summary>
        /// Тесты
        /// </summary>
        public DbSet<Test> Tests => Set<Test>();

        /// <summary>
        /// Вопросы тестов
        /// </summary>
        public DbSet<Question> Questions => Set<Question>();

        /// <summary>
        /// Варианты ответов на вопросы
        /// </summary>
        public DbSet<Answer> Answers => Set<Answer>();

        /// <summary>
        /// Попытки прохождения тестов
        /// </summary>
        public DbSet<Attempt> Attempts => Set<Attempt>();

        /// <summary>
        /// Ответы студентов на вопросы в рамках попытки
        /// </summary>
        public DbSet<UserAttemptAnswer> UserAttemptAnswers => Set<UserAttemptAnswer>();

        /// <summary>
        /// Выбранные студентами варианты ответа
        /// </summary>
        public DbSet<UserSelectedOption> UserSelectedOptions => Set<UserSelectedOption>();

        /// <summary>
        /// Текстовые ответы студентов
        /// </summary>
        public DbSet<UserTextAnswer> UserTextAnswers => Set<UserTextAnswer>();

        /// <summary>
        /// Результаты прохождения тестов
        /// </summary>
        public DbSet<TestResult> TestResults => Set<TestResult>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ==================== Пользователь и студент ====================

            modelBuilder.Entity<User>(e =>
            {
                e.HasKey(x => x.Id);

                e.Property(x => x.Login).HasMaxLength(50).IsRequired();
                e.Property(x => x.PasswordHash).IsRequired();
                e.Property(x => x.Email).HasMaxLength(100).IsRequired();
                e.Property(x => x.FirstName).IsRequired();
                e.Property(x => x.LastName).IsRequired();

                // Логин и email должны быть уникальными
                e.HasIndex(x => x.Login).IsUnique();
                e.HasIndex(x => x.Email).IsUnique();

                // Роль храним в БД строкой (Manager/Student)
                e.Property(x => x.Role).HasConversion<string>();

                e.Property(x => x.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                // 1:1 со студентом, FK на Student.UserId, каскадное удаление
                e.HasOne(x => x.Student)
                    .WithOne(s => s.User)
                    .HasForeignKey<Student>(s => s.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Student>(e =>
            {
                e.HasKey(x => x.Id);

                e.Property(x => x.Phone).HasMaxLength(30).IsRequired();
                e.Property(x => x.VkProfileLink).IsRequired();

                // Связь 1:1 реализуется через уникальный индекс на FK
                e.HasIndex(x => x.UserId).IsUnique();

                // Студент <-> Группа (many:many), join-таблица student_groups
                e.HasMany(x => x.Groups)
                    .WithMany(g => g.Students)
                    .UsingEntity(j => j.ToTable("student_groups"));

                // Адресные назначения тестов студенту (many:many), join-таблица test_students
                e.HasMany(x => x.Tests)
                    .WithMany(t => t.Students)
                    .UsingEntity(j => j.ToTable("test_students"));
            });

            // ==================== Справочники ====================

            modelBuilder.Entity<Group>(e =>
            {
                e.HasKey(x => x.Id);

                e.Property(x => x.Name).IsRequired();
                e.HasIndex(x => x.Name).IsUnique();

                // Справочники нельзя удалить, пока на них ссылаются группы
                e.HasOne(x => x.Direction)
                    .WithMany(d => d.Groups)
                    .HasForeignKey(x => x.DirectionId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Course)
                    .WithMany(c => c.Groups)
                    .HasForeignKey(x => x.CourseId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Project)
                    .WithMany(p => p.Groups)
                    .HasForeignKey(x => x.ProjectId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Course>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Name).IsRequired();
                e.HasIndex(x => x.Name).IsUnique();
            });

            modelBuilder.Entity<Project>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Name).IsRequired();
                e.HasIndex(x => x.Name).IsUnique();
            });

            modelBuilder.Entity<Direction>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Name).IsRequired();
                e.HasIndex(x => x.Name).IsUnique();
            });

            // ==================== Тесты, вопросы, ответы ====================

            modelBuilder.Entity<Test>(e =>
            {
                e.HasKey(x => x.Id);

                e.Property(x => x.Title).IsRequired();
                e.Property(x => x.Description).IsRequired();

                // Тип теста храним в БД строкой (Education/Activity/Other)
                e.Property(x => x.Type).HasConversion<string>();

                e.Property(x => x.IsRepeatable).HasDefaultValue(false);
                e.Property(x => x.IsPublic).HasDefaultValue(false);
                e.Property(x => x.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                e.Property(x => x.PublishedAt).IsRequired();
                e.Property(x => x.Deadline).IsRequired();

                // 1:many вопросов, при удалении теста вопросы удаляются каскадом
                e.HasMany(x => x.Questions)
                    .WithOne(q => q.Test)
                    .HasForeignKey(q => q.TestId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Адресные назначения тестов - явные join-таблицы
                e.HasMany(x => x.Projects)
                    .WithMany(p => p.Tests)
                    .UsingEntity(j => j.ToTable("test_projects"));

                e.HasMany(x => x.Courses)
                    .WithMany(c => c.Tests)
                    .UsingEntity(j => j.ToTable("test_courses"));

                e.HasMany(x => x.Groups)
                    .WithMany(g => g.Tests)
                    .UsingEntity(j => j.ToTable("test_groups"));

                e.HasMany(x => x.Directions)
                    .WithMany(d => d.Tests)
                    .UsingEntity(j => j.ToTable("test_directions"));
            });

            modelBuilder.Entity<Question>(e =>
            {
                e.HasKey(x => x.Id);

                e.Property(x => x.Text).IsRequired();
                e.Property(x => x.Description).HasMaxLength(2000);

                // Тип ответа храним в БД строкой (Single/Multiple/Text)
                e.Property(x => x.AnswerType).HasConversion<string>();

                e.Property(x => x.IsScoring).HasDefaultValue(true);

                // Номер вопроса уникален внутри теста
                e.HasIndex(x => new { x.TestId, x.Number }).IsUnique();
            });

            modelBuilder.Entity<Answer>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Text).IsRequired();

                // При удалении вопроса варианты ответа удаляются каскадом
                e.HasOne(x => x.Question)
                    .WithMany(q => q.Answers)
                    .HasForeignKey(x => x.QuestionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ==================== Прохождение тестов ====================

            modelBuilder.Entity<Attempt>(e =>
            {
                e.HasKey(x => x.Id);

                e.Property(x => x.StartedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Нельзя удалить тест, по которому уже есть попытки
                e.HasOne(x => x.Test)
                    .WithMany()
                    .HasForeignKey(x => x.TestId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Удаление студента удаляет его попытки
                e.HasOne(x => x.Student)
                    .WithMany(s => s.Attempts)
                    .HasForeignKey(x => x.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UserAttemptAnswer>(e =>
            {
                e.HasKey(x => x.Id);

                // На один вопрос в рамках попытки - только один ответ
                e.HasIndex(x => new { x.AttemptId, x.QuestionId }).IsUnique();

                e.HasOne(x => x.Attempt)
                    .WithMany(a => a.UserAttemptAnswers)
                    .HasForeignKey(x => x.AttemptId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Question)
                    .WithMany(q => q.UserAttemptAnswers)
                    .HasForeignKey(x => x.QuestionId)
                    .OnDelete(DeleteBehavior.Restrict);

                // 1:1 с текстовым ответом, FK на UserTextAnswer.UserAttemptAnswerId
                e.HasOne(x => x.UserTextAnswer)
                    .WithOne(t => t.UserAttemptAnswer)
                    .HasForeignKey<UserTextAnswer>(t => t.UserAttemptAnswerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UserTextAnswer>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.TextAnswer).IsRequired();
            });

            modelBuilder.Entity<UserSelectedOption>(e =>
            {
                e.HasKey(x => x.Id);

                e.HasOne(x => x.UserAttemptAnswer)
                    .WithMany(a => a.UserSelectedOptions)
                    .HasForeignKey(x => x.UserAttemptAnswerId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Вариант ответа нельзя удалить, если его кто-то выбрал
                e.HasOne(x => x.Answer)
                    .WithMany(a => a.UserSelectedOptions)
                    .HasForeignKey(x => x.AnswerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TestResult>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Passed).IsRequired();

                // Результат уникален в рамках теста, студента и попытки
                e.HasIndex(x => new { x.TestId, x.StudentId, x.AttemptId }).IsUnique();

                e.HasOne(x => x.Test)
                    .WithMany()
                    .HasForeignKey(x => x.TestId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Attempt)
                    .WithMany()
                    .HasForeignKey(x => x.AttemptId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Student)
                    .WithMany(s => s.TestResults)
                    .HasForeignKey(x => x.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
