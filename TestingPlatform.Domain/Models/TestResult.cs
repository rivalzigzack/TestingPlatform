using System.Text.Json.Serialization;

namespace TestingPlatform.Domain.Models
{
    /// <summary>
    /// Результат прохождения теста студентом
    /// </summary>
    public class TestResult
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Пройден ли тест
        /// </summary>
        public bool Passed { get; set; }

        /// <summary>
        /// Идентификатор теста (FK)
        /// </summary>
        public int TestId { get; set; }

        /// <summary>
        /// Навигация к тесту (many:1)
        /// </summary>
        public Test? Test { get; set; }

        /// <summary>
        /// Идентификатор лучшей попытки (FK)
        /// </summary>
        public int AttemptId { get; set; }

        /// <summary>
        /// Навигация к лучшей попытке (many:1)
        /// </summary>
        public Attempt? Attempt { get; set; }

        /// <summary>
        /// Идентификатор студента (FK)
        /// </summary>
        public int StudentId { get; set; }

        /// <summary>
        /// Навигация к студенту (many:1)
        /// </summary>
        [JsonIgnore]
        public Student? Student { get; set; }
    }
}
