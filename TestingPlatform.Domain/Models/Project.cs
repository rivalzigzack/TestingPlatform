using System.Text.Json.Serialization;

namespace TestingPlatform.Domain.Models
{
    /// <summary>
    /// Проект
    /// </summary>
    public class Project
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название проекта
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Список групп внутри проекта
        /// </summary>
        [JsonIgnore]
        public List<Group> Groups { get; set; } = new();

        /// <summary>
        /// Тесты, назначенные проекту (many:many)
        /// </summary>
        [JsonIgnore]
        public List<Test> Tests { get; set; } = new();
    }
}
