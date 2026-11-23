using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TestingPlatform.Domain.Models
{
    /// <summary>
    /// Вариант ответа на вопрос
    /// </summary>
    public class Answer
    {
        /// <summary>
        /// Идентификатор ответа
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Текст ответа
        /// </summary>
        [Required]
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// Идентификатор вопроса (FK)
        /// </summary>
        public int QuestionId { get; set; }

        /// <summary>
        /// Навигация к вопросу
        /// </summary>
        public Question? Question { get; set; }

        /// <summary>
        /// Является ли ответ правильным
        /// </summary>
        public bool IsCorrect { get; set; }

        /// <summary>
        /// Выборы студентами этого варианта ответа
        /// </summary>
        [JsonIgnore]
        public List<UserSelectedOption> UserSelectedOptions { get; set; } = new();
    }
}
