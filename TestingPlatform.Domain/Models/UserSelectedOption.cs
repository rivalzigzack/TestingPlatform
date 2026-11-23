using System.Text.Json.Serialization;

namespace TestingPlatform.Domain.Models
{
    /// <summary>
    /// Выбранный студентом вариант ответа
    /// </summary>
    public class UserSelectedOption
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Идентификатор ответа студента на вопрос (FK)
        /// </summary>
        public int UserAttemptAnswerId { get; set; }

        /// <summary>
        /// Навигация к ответу студента (many:1)
        /// </summary>
        [JsonIgnore]
        public UserAttemptAnswer? UserAttemptAnswer { get; set; }

        /// <summary>
        /// Идентификатор выбранного варианта ответа (FK)
        /// </summary>
        public int AnswerId { get; set; }

        /// <summary>
        /// Навигация к выбранному варианту ответа (many:1)
        /// </summary>
        public Answer? Answer { get; set; }
    }
}
