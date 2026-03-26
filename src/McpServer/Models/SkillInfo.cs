namespace McpServer.Models;

public sealed class SkillInfo
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public List<string> Triggers { get; init; } = [];
    public required string Content { get; init; }
    public Dictionary<string, string> References { get; init; } = [];
}
