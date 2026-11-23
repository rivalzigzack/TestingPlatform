using System.Text.Json.Serialization;

namespace TestingPlatform.Domain.Models
{
    /// <summary>
    /// Попытка прохождения теста студентом
    /// </summary>
    public class Attempt
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Когда начата попытка
        /// </summary>
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Когда попытка была завершена
        /// </summary>
        public DateTime? SubmittedAt { get; set; }

        /// <summary>
        /// Набранный балл
        /// </summary>
        public int? Score { get; set; }

        /// <summary>
        /// Идентификатор теста (FK)
        /// </summary>
        public int TestId { get; set; }

        /// <summary>
        /// Навигация к тесту (many:1)
        /// </summary>
        public Test? Test { get; set; }

        /// <summary>
        /// Идентификатор студента (FK)
        /// </summary>
        public int StudentId { get; set; }

        /// <summary>
        /// Навигация к студенту (many:1)
        /// </summary>
        [JsonIgnore]
        public Student? Student { get; set; }

        /// <summary>
        /// Ответы студента в рамках этой попытки
        /// </summary>
        public List<UserAttemptAnswer> UserAttemptAnswers { get; set; } = new();
    }
}
