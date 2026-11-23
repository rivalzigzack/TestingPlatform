using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TestingPlatform.Domain.Models
{
    /// <summary>
    /// Студент
    /// </summary>
    public class Student
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Номер телефона
        /// </summary>
        [Required]
        [Phone]
        [MaxLength(30)]
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// Ссылка на профиль в ВК
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string VkProfileLink { get; set; } = string.Empty;

        /// <summary>
        /// 1:1 к пользователю (обязателен для студента)
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Навигация к пользователю
        /// </summary>
        public User? User { get; set; }

        /// <summary>
        /// Группы, в которых состоит студент (many:many)
        /// </summary>
        public List<Group> Groups { get; set; } = new();

        /// <summary>
        /// Тесты, назначенные студенту адресно (many:many)
        /// </summary>
        [JsonIgnore]
        public List<Test> Tests { get; set; } = new();

        /// <summary>
        /// Попытки прохождения тестов
        /// </summary>
        [JsonIgnore]
        public List<Attempt> Attempts { get; set; } = new();

        /// <summary>
        /// Результаты прохождения тестов
        /// </summary>
        [JsonIgnore]
        public List<TestResult> TestResults { get; set; } = new();
    }
}
