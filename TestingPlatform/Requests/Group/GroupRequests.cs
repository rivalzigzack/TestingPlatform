using System.ComponentModel.DataAnnotations;
namespace TestingPlatform.Requests.Group;
public class GroupFields
{
    [Required] public string Name { get; set; } = "";
    [Range(1, int.MaxValue)] public int DirectionId { get; set; }
    [Range(1, int.MaxValue)] public int CourseId { get; set; }
    [Range(1, int.MaxValue)] public int ProjectId { get; set; }
}
public class CreateGroupRequest : GroupFields;
public class UpdateGroupRequest : GroupFields
{
    public int Id { get; set; }
}
