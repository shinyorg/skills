using McpServer.Models;

namespace McpServer.Services;

public sealed class SkillService
{
    private readonly Dictionary<string, SkillInfo> _skills = new(StringComparer.OrdinalIgnoreCase);

    public SkillService(string skillsDirectory)
    {
        LoadSkills(skillsDirectory);
    }

    public IReadOnlyDictionary<string, SkillInfo> Skills => _skills;

    public SkillInfo? GetSkill(string name)
        => _skills.GetValueOrDefault(name);

    private void LoadSkills(string skillsDirectory)
    {
        if (!Directory.Exists(skillsDirectory))
            return;

        foreach (var skillDir in Directory.GetDirectories(skillsDirectory))
        {
            var skillFile = Path.Combine(skillDir, "SKILL.md");
            if (!File.Exists(skillFile))
                continue;

            var rawContent = File.ReadAllText(skillFile);
            var skill = ParseSkillFile(Path.GetFileName(skillDir), rawContent);

            // Load reference files
            var refDir = Path.Combine(skillDir, "reference");
            if (Directory.Exists(refDir))
            {
                foreach (var refFile in Directory.GetFiles(refDir))
                {
                    skill.References[Path.GetFileName(refFile)] = File.ReadAllText(refFile);
                }
            }

            _skills[skill.Name] = skill;
        }
    }

    private static SkillInfo ParseSkillFile(string directoryName, string rawContent)
    {
        var name = directoryName;
        var description = "";
        var triggers = new List<string>();
        var content = rawContent;

        // Parse YAML frontmatter between --- delimiters
        if (rawContent.StartsWith("---"))
        {
            var endIndex = rawContent.IndexOf("---", 3);
            if (endIndex > 0)
            {
                var frontmatter = rawContent[3..endIndex].Trim();
                content = rawContent[(endIndex + 3)..].TrimStart('\r', '\n');

                var inTriggers = false;
                foreach (var line in frontmatter.Split('\n'))
                {
                    var trimmed = line.Trim();

                    if (trimmed.StartsWith("name:"))
                    {
                        name = trimmed["name:".Length..].Trim();
                        inTriggers = false;
                    }
                    else if (trimmed.StartsWith("description:"))
                    {
                        description = trimmed["description:".Length..].Trim();
                        inTriggers = false;
                    }
                    else if (trimmed.StartsWith("triggers:"))
                    {
                        inTriggers = true;
                    }
                    else if (trimmed.StartsWith("- ") && inTriggers)
                    {
                        triggers.Add(trimmed[2..].Trim());
                    }
                    else if (!trimmed.StartsWith("- "))
                    {
                        inTriggers = false;
                    }
                }
            }
        }

        if (triggers.Count == 0)
            triggers = ExtractMarkdownListSection(content, "Triggers", "Keywords");

        return new SkillInfo
        {
            Name = name,
            Description = description,
            Triggers = triggers,
            Content = content,
            References = []
        };
    }

    private static List<string> ExtractMarkdownListSection(string content, params string[] sectionNames)
    {
        var results = new List<string>();
        var inSection = false;

        foreach (var line in content.Split('\n'))
        {
            var trimmed = line.Trim();

            if (trimmed.StartsWith('#'))
            {
                var heading = trimmed.TrimStart('#').Trim();
                if (inSection)
                    break;

                inSection = sectionNames.Any(x => heading.Equals(x, StringComparison.OrdinalIgnoreCase));
                continue;
            }

            if (!inSection)
                continue;

            if (trimmed.StartsWith("- "))
            {
                results.Add(trimmed[2..].Trim());
                continue;
            }

            if (results.Count > 0 && !String.IsNullOrWhiteSpace(trimmed))
                break;
        }

        return results;
    }
}
