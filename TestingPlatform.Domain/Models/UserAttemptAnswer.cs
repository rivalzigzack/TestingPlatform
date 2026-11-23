using System.Text.Json.Serialization;

namespace TestingPlatform.Domain.Models
{
    /// <summary>
    /// Ответ пользователя на вопрос в рамках попытки
    /// </summary>
    public class UserAttemptAnswer
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Правильный ли ответ дал студент
        /// </summary>
        public bool IsCorrect { get; set; }

        /// <summary>
        /// Начисленный балл (0/1 или до MaxScore)
        /// </summary>
        public int ScoreAwarded { get; set; }

        /// <summary>
        /// Идентификатор попытки (FK)
        /// </summary>
        public int AttemptId { get; set; }

        /// <summary>
        /// Навигация к попытке (many:1)
        /// </summary>
        [JsonIgnore]
        public Attempt? Attempt { get; set; }

        /// <summary>
        /// Идентификатор вопроса (FK)
        /// </summary>
        public int QuestionId { get; set; }

        /// <summary>
        /// Навигация к вопросу (many:1)
        /// </summary>
        public Question? Question { get; set; }

        /// <summary>
        /// Выбранные студентом варианты ответа
        /// </summary>
        public List<UserSelectedOption>? UserSelectedOptions { get; set; }

        /// <summary>
        /// Текстовый ответ студента (если вопрос с текстовым ответом)
        /// </summary>
        public UserTextAnswer? UserTextAnswer { get; set; }
    }
}
