# Images & Attachments

## Sending an image

```csharp
var photo = await File.ReadAllBytesAsync(path);
session.Send(AiInput.Text("What is this?").WithImage(photo, "image/jpeg", description: "the plant", retention: ImageRetention.DropAfterTurn));
```

`AiInput.Images` (`IReadOnlyList<AiImage>`) are stored by the session; the stored user message gets an
`ImagePart(AttachmentRef Attachment, string? Description, ImageRetention Retention)`. **Binary is never inline in
message documents**, only the reference.

## ImageRetention

| Value | Model sees the image | Stored |
|---|---|---|
| `DropAfterTurn` (default) | In the turn it arrived; later prompts get `[image: description]` | Yes |
| `KeepInContext` | Every prompt until compacted (~1K tokens each) | Yes |
| `DontStore` | Once, from memory, for that turn only | No (ref id `ephemeral:…`) |

## Attachment stores

```csharp
opts.UseFileAttachments(Path.Combine(FileSystem.AppDataDirectory, "ai_attachments"));   // {dir}/{sessionId}/{id}.jpg
opts.SetAttachmentStore<MyStore>();                                                     // default InMemoryAiAttachmentStore
```

```csharp
public interface IAiAttachmentStore
{
    Task<AttachmentRef> Save(ReadOnlyMemory<byte> content, string mediaType, string sessionId, CancellationToken ct = default);
    Task<byte[]?> Load(string attachmentId, CancellationToken ct = default);
    Task Delete(string attachmentId, CancellationToken ct = default);
    Task DeleteForSession(string sessionId, CancellationToken ct = default);
    string? GetDisplayUri(string attachmentId) => null;    // AiChatView uses it for ChatMessage.ImageUrl
}
```

`IAiSessionManager.Delete(id)` also deletes that session's attachments. `FileAiAttachmentStore` rejects ids with `..`.

## Follow-ups within a turn

A tool added in `OnBeforeTurn` can queue more user input for the same turn (this is how the camera sends photos):

```csharp
public Task OnBeforeTurn(TurnContext ctx)
{
    ctx.AddTool(new SchemaFunction("attach_scan", "Attach the latest scan.", """{ "type": "object" }""", async (args, ct) =>
    {
        ctx.QueueFollowUp(AiInput.Text("Here is the scan.", MessageSource.Camera).WithImage(bytes, "image/png"));
        if (FunctionInvokingChatClient.CurrentContext is { } c)
            c.Terminate = true;                    // end this round; the follow-up becomes the next round
        return "The scan follows in the next message.";
    }));
    return Task.CompletedTask;
}
```

The round's reply is stored, the follow-up is appended as a user message, and a new assistant message streams in
the same `IAiTurn` (its `Events` continue; `Reply` is the last assistant message). Follow-ups are ignored when the
round called `ask_user`.

`SchemaFunction` also takes `Func<AIFunctionArguments, CancellationToken, Task<object?>>`, so a tool can return
AI content such as `new AIContent[] { new TextContent("…"), new DataContent(bytes, "image/jpeg") }` (stored as a
text placeholder).
