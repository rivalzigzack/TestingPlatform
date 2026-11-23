using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TestingPlatform.Domain.Enums;

namespace TestingPlatform.Domain.Models
{
    /// <summary>
    /// Вопрос теста
    /// </summary>
    public class Question
    {
        /// <summary>
        /// Идентификатор вопроса
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Текст вопроса
        /// </summary>
        [Required]
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// Номер вопроса в тесте
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Уточняющее описание/пояснение к вопросу
        /// </summary>
        [MaxLength(2000)]
        public string? Description { get; set; }

        /// <summary>
        /// Тип ответа на вопрос (один, несколько, текст)
        /// </summary>
        public AnswerType AnswerType { get; set; }

        /// <summary>
        /// Учитывать ли вопрос при подсчёте баллов
        /// </summary>
        public bool IsScoring { get; set; } = true;

        /// <summary>
        /// Максимальный балл за вопрос
        /// </summary>
        public int? MaxScore { get; set; }

        /// <summary>
        /// Идентификатор теста (FK)
        /// </summary>
        public int TestId { get; set; }

        /// <summary>
        /// Навигация к тесту
        /// </summary>
        public Test? Test { get; set; }

        /// <summary>
        /// Варианты ответа на вопрос (1:N)
        /// </summary>
        public List<Answer> Answers { get; set; } = new();

        /// <summary>
        /// Ответы студентов на этот вопрос
        /// </summary>
        [JsonIgnore]
        public List<UserAttemptAnswer> UserAttemptAnswers { get; set; } = new();
    }
}
