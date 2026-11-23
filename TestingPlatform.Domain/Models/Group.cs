using System.Text.Json.Serialization;

namespace TestingPlatform.Domain.Models
{
    /// <summary>
    /// Группа
    /// </summary>
    public class Group
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название группы
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Направление группы
        /// </summary>
        public int DirectionId { get; set; }

        /// <summary>
        /// Навигация к направлению
        /// </summary>
        public Direction? Direction { get; set; }

        /// <summary>
        /// Курс группы
        /// </summary>
        public int CourseId { get; set; }

        /// <summary>
        /// Навигация к курсу
        /// </summary>
        public Course? Course { get; set; }

        /// <summary>
        /// Проект группы
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// Навигация к проекту
        /// </summary>
        public Project? Project { get; set; }

        /// <summary>
        /// Студенты, которые состоят в группе
        /// </summary>
        [JsonIgnore]
        public List<Student> Students { get; set; } = new();

        /// <summary>
        /// Тесты, назначенные группе (many:many)
        /// </summary>
        public List<Test> Tests { get; set; } = new();
    }
}
