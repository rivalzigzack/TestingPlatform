using System.Text.Json.Serialization;

namespace TestingPlatform.Domain.Models
{
    /// <summary>
    /// Курс
    /// </summary>
    public class Course
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название курса
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Список групп внутри курса
        /// </summary>
        [JsonIgnore]
        public List<Group> Groups { get; set; } = new();

        /// <summary>
        /// Тесты, назначенные курсу (many:many)
        /// </summary>
        [JsonIgnore]
        public List<Test> Tests { get; set; } = new();
    }
}
