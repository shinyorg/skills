using System.ComponentModel;
using System.Text.Json;
using McpServer.Services;
using ModelContextProtocol.Server;

namespace McpServer.Tools;

[McpServerToolType]
public sealed class SkillTools
{
    [McpServerTool, Description("Lists all available skills with their names, descriptions, and trigger keywords")]
    public static string ListSkills(SkillService skillService)
    {
        var skills = skillService.Skills.Values.Select(s => new
        {
            s.Name,
            s.Description,
            s.Triggers
        });

        return JsonSerializer.Serialize(skills, new JsonSerializerOptions { WriteIndented = true });
    }

    [McpServerTool, Description("Gets the full content of a skill by name, including all reference files")]
    public static string GetSkill(SkillService skillService, [Description("The name of the skill to retrieve")] string name)
    {
        var skill = skillService.GetSkill(name);
        if (skill is null)
        {
            var available = string.Join(", ", skillService.Skills.Keys.Order());
            return $"Skill '{name}' not found. Available skills: {available}";
        }

        var result = skill.Content;

        foreach (var (fileName, fileContent) in skill.References)
        {
            result += $"\n\n---\n# Reference: {fileName}\n\n{fileContent}";
        }

        return result;
    }
}
