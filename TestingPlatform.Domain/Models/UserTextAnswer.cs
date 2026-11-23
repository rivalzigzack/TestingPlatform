using System.Text.Json.Serialization;

namespace TestingPlatform.Domain.Models
{
    /// <summary>
    /// Текстовый ответ студента на вопрос
    /// </summary>
    public class UserTextAnswer
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Текст ответа
        /// </summary>
        public string TextAnswer { get; set; } = string.Empty;

        /// <summary>
        /// Идентификатор ответа студента на вопрос (FK, 1:1)
        /// </summary>
        public int UserAttemptAnswerId { get; set; }

        /// <summary>
        /// Навигация к ответу студента (1:1)
        /// </summary>
        [JsonIgnore]
        public UserAttemptAnswer? UserAttemptAnswer { get; set; }
    }
}
