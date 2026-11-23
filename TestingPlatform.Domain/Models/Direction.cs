using System.Text.Json.Serialization;

namespace TestingPlatform.Domain.Models
{
    /// <summary>
    /// Направление
    /// </summary>
    public class Direction
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название направления
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Список групп внутри направления
        /// </summary>
        [JsonIgnore]
        public List<Group> Groups { get; set; } = new();

        /// <summary>
        /// Тесты, назначенные направлению (many:many)
        /// </summary>
        [JsonIgnore]
        public List<Test> Tests { get; set; } = new();
    }
}
