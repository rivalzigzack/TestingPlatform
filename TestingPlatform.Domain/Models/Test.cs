using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TestingPlatform.Domain.Enums;

namespace TestingPlatform.Domain.Models
{
    /// <summary>
    /// Тест
    /// </summary>
    public class Test
    {
        /// <summary>
        /// Идентификатор теста
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название теста
        /// </summary>
        [Required]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Описание теста
        /// </summary>
        [Required]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Можно ли тест пройти повторно или есть только одна попытка
        /// </summary>
        public bool IsRepeatable { get; set; }

        /// <summary>
        /// Тип теста - образовательный, дополнительные активности, другое
        /// </summary>
        public TestType Type { get; set; }

        /// <summary>
        /// Когда создан
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Когда опубликован (стал доступен студентам)
        /// </summary>
        public DateTime PublishedAt { get; set; }

        /// <summary>
        /// Срок выполнения теста
        /// </summary>
        public DateTime Deadline { get; set; }

        /// <summary>
        /// Время на выполнение теста
        /// </summary>
        public int? DurationMinutes { get; set; }

        /// <summary>
        /// Опубликован ли (доступен студентам для прохождения)
        /// </summary>
        public bool IsPublic { get; set; }

        /// <summary>
        /// Проходной балл (достигнув которого тест больше нельзя пройти)
        /// </summary>
        public int? PassingScore { get; set; }

        /// <summary>
        /// Максимальное количество попыток прохождения теста
        /// </summary>
        public int? MaxAttempts { get; set; }

        /// <summary>
        /// Вопросы теста (1:N)
        /// </summary>
        public List<Question> Questions { get; set; } = new();

        /// <summary>
        /// Студенты, для которых адресно доступен тест (M:N)
        /// </summary>
        [JsonIgnore]
        public List<Student> Students { get; set; } = new();

        /// <summary>
        /// Проекты, для которых доступен тест (M:N)
        /// </summary>
        [JsonIgnore]
        public List<Project> Projects { get; set; } = new();

        /// <summary>
        /// Курсы, для которых доступен тест (M:N)
        /// </summary>
        [JsonIgnore]
        public List<Course> Courses { get; set; } = new();

        /// <summary>
        /// Группы, для которых доступен тест (M:N)
        /// </summary>
        [JsonIgnore]
        public List<Group> Groups { get; set; } = new();

        /// <summary>
        /// Направления, для которых доступен тест (M:N)
        /// </summary>
        [JsonIgnore]
        public List<Direction> Directions { get; set; } = new();
    }
}
