using McpServer.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// Resolve skills directory relative to the assembly location
var assemblyDir = Path.GetDirectoryName(typeof(Program).Assembly.Location)!;
var skillsDir = Path.GetFullPath(Path.Combine(assemblyDir, "..", "..", "..", "..", "..", "skills"));

if (!Directory.Exists(skillsDir))
{
    // Fallback: try relative to current working directory
    skillsDir = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "skills"));
}

var builder = Host.CreateEmptyApplicationBuilder(settings: null);

builder.Services.AddSingleton(new SkillService(skillsDir));
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

var app = builder.Build();
await app.RunAsync();
