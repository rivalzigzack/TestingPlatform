using TestingPlatform.Domain.Models;

namespace TestingPlatform.Infrastructure
{
    /// <summary>
    /// Заполнение справочников при первом запуске приложения
    /// </summary>
    public static class DbInitializer
    {
        /// <summary>
        /// Добавить направления, курсы и проекты, если таблицы пустые
        /// </summary>
        public static void Seed(AppDbContext db)
        {
            if (!db.Directions.Any())
            {
                db.Directions.AddRange(
                    new Direction { Name = "Фронтенд" },
                    new Direction { Name = "Бэкенд" },
                    new Direction { Name = "Дизайн" });
            }

            if (!db.Courses.Any())
            {
                db.Courses.AddRange(
                    new Course { Name = "1 курс" },
                    new Course { Name = "2 курс" },
                    new Course { Name = "3 курс" });
            }

            if (!db.Projects.Any())
            {
                db.Projects.AddRange(
                    new Project { Name = "КОД" },
                    new Project { Name = "ПАЗЛ" });
            }

            db.SaveChanges();

            // Демонстрационная группа, чтобы связь many:many со студентами можно было проверить
            if (!db.Groups.Any())
            {
                var direction = db.Directions.FirstOrDefault(d => d.Name == "Бэкенд");
                var course = db.Courses.FirstOrDefault(c => c.Name == "2 курс");
                var project = db.Projects.FirstOrDefault(p => p.Name == "КОД");

                if (direction is not null && course is not null && project is not null)
                {
                    db.Groups.Add(new Group
                    {
                        Name = "Б-2-24",
                        DirectionId = direction.Id,
                        CourseId = course.Id,
                        ProjectId = project.Id,
                    });

                    db.SaveChanges();
                }
            }
        }
    }
}
