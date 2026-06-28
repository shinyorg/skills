# ChatView

A modern chat UI control with message bubbles, typing indicators, cursor-based load-more paging, reactions, read receipts, a markdown composition toolbar, and image attachments.

ChatView is **provider-driven**: the control is *styles + layout only*. All data, lifecycle, permissions, and real-time behavior live behind an `IChatSessionProvider` you implement (the same integration pattern as the Scheduler control). You do **not** bind a `Messages` collection or wire `SendCommand`/`LoadMoreCommand` — you give the control a `Provider` and a `SessionId`, and it does the rest.

> ChatView is **v1 beta** — the API may still change.

## Basic Usage

```xml
<shiny:ChatView Provider="{Binding Provider}"
                SessionId="{Binding SessionId}"
                MyBubbleColor="#DCF8C6"
                OtherBubbleColor="White" />
```

```csharp
public partial class ChatViewModel : ObservableObject
{
    public ChatViewModel(IChatSessionProvider provider)
    {
        this.Provider = provider;
        this.SessionId = "demo";
    }

    public IChatSessionProvider Provider { get; }
    public string SessionId { get; }
}
```

Blazor:

```razor
<ChatView Provider="provider" SessionId="@sessionId" />

@code {
    [Inject] public IChatSessionProvider provider { get; set; } = default!;
    string sessionId = "demo";
}
```

## The integration interface

You implement `IChatSessionProvider`, which returns a session-scoped `IChatSession` handle. The control subscribes to the session's events on attach and disposes it on detach.

```csharp
public interface IChatSessionProvider
{
    Task<IChatSession> CreateSessionAsync(string[] userIds, CancellationToken cancellationToken = default);

    // throws ChatSessionException if missing or no access
    Task<IChatSession> GetSessionAsync(string sessionId, CancellationToken cancellationToken = default);
}

public interface IChatSession : IAsyncDisposable
{
    ChatSessionInfo Info { get; }     // always current — refreshed before SessionUpdated fires
    string CurrentUserId { get; }     // drives bubble alignment + ownership checks

    // cursor-based paging (stable under live inserts); null + Older = newest page
    Task<MessagePage> GetMessagesAsync(string? cursorMessageId, MessagePageDirection direction, int count, CancellationToken ct = default);

    Task<ChatMessage> SendMessageAsync(OutgoingMessage message, CancellationToken ct = default);
    Task<ChatMessage> ResendMessageAsync(string clientMessageId, CancellationToken ct = default);
    Task EditMessageAsync(string messageId, string body, CancellationToken ct = default);
    Task DeleteMessageAsync(string messageId, CancellationToken ct = default);

    // add == true toggles the emoji on; add == false removes it
    Task ReactToMessageAsync(string messageId, string emoji, bool add, CancellationToken ct = default);

    Task MarkReadAsync(string[] messageIds, CancellationToken ct = default);   // control passes only visible, not-mine, unread ids
    Task ToggleTypingAsync(bool isTyping, CancellationToken ct = default);
    Task InviteUserAsync(string userId, CancellationToken ct = default);
    Task LeaveAsync(CancellationToken ct = default);
    Task RenameAsync(string sessionName, CancellationToken ct = default);

    event EventHandler<ChatMessage> MessageReceived;       // includes echoes of own sends (multi-device)
    event EventHandler<MessageChanged> MessageUpdated;     // carries WHAT changed
    event EventHandler<string> MessageDeleted;
    event EventHandler<UserTypingEvent> UserTyping;
    event EventHandler<ChatSessionUserInfo> UserJoined;
    event EventHandler<ChatSessionUserInfo> UserLeft;
    event EventHandler<ChatSessionInfo> SessionUpdated;
    event EventHandler<ChatConnectionState> ConnectionStateChanged;
}
```

Events may fire off the UI thread — the control marshals them. The control merges/dedups messages by `MessageId` and reconciles optimistic sends by `ClientMessageId`. Ownership is `message.SenderId == session.CurrentUserId`.

## Data Models

These records are identical on MAUI and Blazor **except** `ChatSessionUserInfo` (MAUI uses `ImageSource`/`Color`; Blazor uses `string` URL / CSS color).

```csharp
public record ChatMessage(
    string MessageId,
    string? ClientMessageId,                 // matches OutgoingMessage.ClientMessageId for echo reconciliation
    string SenderId,
    string? Body,                            // markdown
    string? ImageUrl,
    MessageStatus Status,                    // Sending/Sent/Delivered/Read/Failed/Rejected
    string? StatusReason,                    // shown on Failed/Rejected bubbles
    DateTimeOffset Timestamp,
    DateTimeOffset? EditedTimestamp,
    IReadOnlyList<Reaction> Reactions,
    IReadOnlyList<ReadReceipt> ReadReceipts, // per-user; control collapses to a "Read" hint for 1:1
    string? Identifier = null,               // template-selector discriminator
    IReadOnlyDictionary<string, string>? Metadata = null   // custom payload for templates
);

public enum MessageStatus { Sending, Sent, Delivered, Read, Failed, Rejected }

public record Reaction(string UserId, string Emoji, DateTimeOffset Timestamp);
public record ReadReceipt(string UserId, DateTimeOffset Timestamp);

public record MessageChanged(ChatMessage Message, MessageChangeKind Change);
public enum MessageChangeKind { Edited, ReactionChanged, ReadReceiptChanged, StatusChanged }

public record MessagePage(IReadOnlyList<ChatMessage> Messages, bool HasMore);  // Messages always chronological asc
public enum MessagePageDirection { Older, Newer }

public record OutgoingMessage(string? Body, OutgoingAttachment? Attachment = null, string ClientMessageId = "");
public record OutgoingAttachment(ChatAttachmentKind Kind, Stream Content, string FileName, string ContentType); // provider OWNS + DISPOSES Content
public enum ChatAttachmentKind { Image }

public record ChatSessionInfo(
    string SessionId,
    string SessionName,
    ChatSessionUserInfo[] Users,
    string[]? PermittedEmojis,               // null => control default set; empty => no reactions
    MessageBodyPermissions BodyPermissions,  // drives the markdown toolbar
    ChatSessionPermissions Permissions,      // drives every action affordance
    DateTimeOffset CreatedAt,
    DateTimeOffset? LastReadDate,
    int UnreadMessageCount
);

// MAUI
public record ChatSessionUserInfo(string UserId, string DisplayName, ImageSource? Avatar, Color? BubbleColor, DateTimeOffset JoinedDate);
// Blazor
public record ChatSessionUserInfo(string UserId, string DisplayName, string? AvatarUrl, string? BubbleColor, DateTimeOffset JoinedDate);

public record UserTypingEvent(string UserId, bool IsTyping, DateTimeOffset Timestamp);
public enum ChatConnectionState { Connected, Reconnecting, Offline }
```

### Permissions

The control derives every action affordance from `Info.Permissions` + ownership — there is no per-tool wiring anymore.

```csharp
[Flags]
public enum ChatSessionPermissions
{
    None = 0,
    CanSendMessages = 1,
    CanEditMessages = 2,    // own messages only
    CanDeleteMessages = 4,  // own messages only
    CanReactToMessages = 8,
    CanInviteUsers = 16,
    CanLeaveSession = 32,
    CanChangeSessionName = 64,
    CanSendImages = 128,    // gates the gallery/camera attach affordance

    Default = CanSendMessages | CanSendImages | CanReactToMessages | CanInviteUsers | CanLeaveSession,
    All = CanSendMessages | CanEditMessages | CanDeleteMessages | CanReactToMessages
        | CanInviteUsers | CanLeaveSession | CanChangeSessionName | CanSendImages
}

[Flags]
public enum MessageBodyPermissions
{
    None = 0,
    Links = 1, Bold = 2, Italics = 4, Underline = 8, Strikethrough = 16, Codeblocks = 32,
    All = Links | Bold | Italics | Underline | Strikethrough | Codeblocks
}
```

| Permission | Surfaced as |
|---|---|
| `CanSendMessages` | input bar enabled; `SendMessageAsync` |
| `CanEditMessages` | "Edit" on own bubbles → `EditMessageAsync` |
| `CanDeleteMessages` | "Delete" on own bubbles → `DeleteMessageAsync` |
| `CanReactToMessages` | reaction picker (filtered to `PermittedEmojis`) → `ReactToMessageAsync` |
| `CanInviteUsers` | invite affordance → `InviteUserAsync` |
| `CanLeaveSession` | leave affordance → `LeaveAsync` |
| `CanChangeSessionName` | rename affordance → `RenameAsync` |
| `CanSendImages` | gallery/camera attach affordance |

### Exceptions — provider rejects, control reacts

Validation that depends on server/business rules lives in the **provider**; the control attempts optimistically and renders the verdict — it never pre-checks size/count.

```csharp
// GetSessionAsync: session missing or no access
public class ChatSessionException : Exception { ... }

// SendMessageAsync/ResendMessageAsync/EditMessageAsync: content refused (too big, too many images, etc.)
// -> control flips the optimistic bubble to MessageStatus.Rejected, sets StatusReason, no retry.
public class ChatSendRejectedException : Exception { public SendRejectionKind Kind { get; } }
public enum SendRejectionKind { MessageTooLarge, TooManyAttachments, AttachmentTooLarge, UnsupportedContent, NotPermitted, Other }
```

A *transient* failure (no exception of this type) → `MessageStatus.Failed` + retry via `ResendMessageAsync`. A *rejection* → `MessageStatus.Rejected` + reason, no retry.

## ChatView Properties

| Property | Type | Default | Description |
|---|---|---|---|
| `Provider` | `IChatSessionProvider?` | `null` | The integration provider |
| `SessionId` | `string?` | `null` | Session to resolve via `GetSessionAsync` |
| `PageSize` | `int` | `30` | Messages fetched per page |
| `OpenImagesInViewer` | `bool` | `true` | Tapping an image bubble opens the built-in `ImageViewer` |
| `MyBubbleColor` | `Color`/`string` | `#DCF8C6` | Local user bubble color |
| `MyTextColor` | `Color`/`string` | `Black` | Local user text color |
| `OtherBubbleColor` | `Color`/`string` | `White` | Default other-user bubble color (overridden by user's `BubbleColor`) |
| `OtherTextColor` | `Color`/`string` | `Black` | Other-user text color |
| `ChatBackgroundColor` | `Color?`/`string?` | `null` | Messages area background |
| `BubbleFontSize` | `double` | `15` | Bubble text size (MAUI) |
| `BubbleFontFamily` | `string?` | `null` | Bubble font family (MAUI) |
| `TimestampFontSize` | `double` | `11` | Timestamp size (MAUI) |
| `BubbleCornerRadius` | `double` | `18` | Bubble corner radius (tail corner remains 4) (MAUI) |
| `PlaceholderText` | `string` | `"Type a message..."` | Input placeholder |
| `SendButtonText` | `string` | `"Send"` | Send button label |
| `SendButtonBackgroundColor` | `Color`/`string` | `#007AFF` | Send button background (MAUI) |
| `SendButtonTextColor` | `Color`/`string` | `White` | Send button text (MAUI) |
| `InputBarBackgroundColor` | `Color`/`string` | `#F5F5F5` | Input bar background (MAUI) |
| `InputBarBorderColor` | `Color`/`string` | `#E0E0E0` | Input bar separator (MAUI) |
| `IsInputBarVisible` | `bool` | `true` | Show/hide the input bar (set `false` for read-only chats) |
| `ShowTypingIndicator` | `bool` | `true` | Enable typing indicators |
| `ScrollToFirstUnread` | `bool` | `false` | Anchor initial scroll at the first unread (via `Info.LastReadDate`) instead of the end |
| `InputActions` | `IList<ChatInputAction>` | `[]` | Custom input-bar actions (MAUI only) |
| `CustomBubbleActions` | `IList<ChatBubbleAction>` | `[]` | Custom bubble actions appended to the permission-driven set (MAUI only) |
| `MessageTemplate` | `DataTemplate?` | `null` | Single template for all message content (MAUI only) |
| `MessageTemplateSelector` | `DataTemplateSelector?` | `null` | Per-type template selector (MAUI only) |
| `UseFeedback` | `bool` | `true` | Haptic feedback on interactions (MAUI only) |
| `AdjustForKeyboard` | `bool` | `true` | iOS-only keyboard padding. **Set `False` when ChatView is hosted inside a FloatingPanel** (otherwise keyboard handling fights itself and the Entry drops keystrokes). (MAUI only) |

## Methods (MAUI only)

| Member | Description |
|---|---|
| `ScrollToEnd(bool animate)` | Scroll to the latest message |
| `ScrollToMessage(string messageId, bool animate)` | Scroll to a message by id |
| `SubmitEntry()` | Programmatically submit the input text |
| `EntryText` (property) | Get/set the input field text |
| `MessageTapped` (event) | Fires for non-image bubble taps |

## What the control handles for you

- **Optimistic send:** generates `ClientMessageId`, shows a `Sending` bubble, reconciles with the echo. `ChatSendRejectedException` → `Rejected` + reason (no retry); other failure → `Failed` + retry (`ResendMessageAsync`). No offline queue — send is only attempted while `Connected`.
- **Cursor paging:** initial `GetMessagesAsync(null, Older, PageSize)`; loads older on scroll-to-top using the oldest `MessageId`; stops when `HasMore` is false.
- **Reactions:** picker filtered to `Info.PermittedEmojis`; `null` falls back to the built-in default set (👍 👎 ❤️ 😂 😮 😢 😡 🔥 👏 🙏 💯 🎉); empty array = no reactions.
- **Read receipts:** marks only visible, not-mine, unread ids; ignores inbound `ReadReceiptChanged` for the current user (no loops).
- **Typing:** debounces `ToggleTypingAsync`; expires stale inbound typing indicators.
- **Connection:** shows a banner and disables the input bar while not `Connected`.
- **Markdown toolbar:** shows formatting buttons per `Info.BodyPermissions`; renders the markdown subset (`**bold**`, `*italic*`, `~~strike~~`, `` `code` ``, underline, `[text](url)`) in bubbles. Self-contained — no Markdown-package dependency.
- **Images:** an attach affordance (shown when `CanSendImages`) offers Gallery, plus Camera when the platform supports capture (MAUI `MediaPicker.IsCaptureSupported`; Blazor `<InputFile capture>`). Tapping an image bubble opens the `ImageViewer` when `OpenImagesInViewer`.

## Custom actions (MAUI only)

The old `ChatEntryTool`/`ChatBubbleTool` FAB tool tree is gone. Built-in actions (react/edit/delete/copy) are derived from permissions. For app-specific verbs, add lightweight actions:

```csharp
public class ChatInputAction : BindableObject
{
    public string? Text { get; set; }
    public ImageSource? Icon { get; set; }
    public Func<ChatView, Task>? Handler { get; set; }
    public event EventHandler<ChatView>? Clicked;
    public virtual Task InvokeAsync(ChatView chatView);   // overridable
}

public class ChatBubbleAction : BindableObject
{
    public string? Text { get; set; }
    public ImageSource? Icon { get; set; }
    public Func<ChatMessage, Task>? Handler { get; set; }
    public event EventHandler<ChatMessage>? Clicked;
    public virtual Task InvokeAsync(ChatMessage message);  // overridable
}
```

```xml
<shiny:ChatView Provider="{Binding Provider}" SessionId="{Binding SessionId}">
    <shiny:ChatView.InputActions>
        <speech:SpeechToTextTool AutoSend="False" SilenceTimeout="00:00:03" />
    </shiny:ChatView.InputActions>
    <shiny:ChatView.CustomBubbleActions>
        <speech:TextToSpeechBubbleTool />
    </shiny:ChatView.CustomBubbleActions>
</shiny:ChatView>
```

`SpeechToTextTool : ChatInputAction` and `TextToSpeechBubbleTool : ChatBubbleAction` ship in `Shiny.Maui.Controls.SpeechAddins`.

## Custom message templates (MAUI only)

Use `ChatMessage.Identifier` or `Metadata` as the discriminator in a `DataTemplateSelector`:

```csharp
public class ChatMessageTemplateSelector : DataTemplateSelector
{
    public DataTemplate? TextTemplate { get; set; }
    public DataTemplate? ActionTemplate { get; set; }

    protected override DataTemplate? OnSelectTemplate(object item, BindableObject container)
        => item is ChatMessage { Identifier: "action" } ? ActionTemplate : TextTemplate;
}
```

```xml
<shiny:ChatView.MessageTemplateSelector>
    <local:ChatMessageTemplateSelector>
        <local:ChatMessageTemplateSelector.TextTemplate>
            <DataTemplate x:DataType="shiny:ChatMessage">
                <Label Text="{Binding Body}" />
            </DataTemplate>
        </local:ChatMessageTemplateSelector.TextTemplate>
    </local:ChatMessageTemplateSelector>
</shiny:ChatView.MessageTemplateSelector>
```

## Code Generation Guidance

- Implement `IChatSessionProvider` to back the chat — **do not** bind a `Messages` collection or wire `SendCommand`/`LoadMoreCommand`/`AttachImageCommand` (those no longer exist). Give the control a `Provider` + `SessionId`.
- Put all validation (size caps, image counts, content policy) in the provider; throw `ChatSendRejectedException` to reject a send and `ChatSessionException` for an unknown/forbidden session. Never reimplement those checks in the UI.
- Gate features with `ChatSessionPermissions` on `ChatSessionInfo` — the control shows/hides affordances automatically. Set `MessageBodyPermissions` to control the markdown toolbar; set `CanSendImages` to allow attachments.
- Raise `MessageReceived` for inbound messages and echoes of the sender's own messages; raise `MessageUpdated` with the right `MessageChangeKind` for edits/reactions/receipts; the control updates by `MessageId`.
- `GetMessagesAsync` is cursor-based — return `HasMore=false` when there's no more history. Messages within a page must be chronological ascending.
- For reactions, expose `PermittedEmojis` (or `null` for the default set); the control toggles via `ReactToMessageAsync(id, emoji, add)`.
- Set `IsInputBarVisible = false` for read-only chats. Set `AdjustForKeyboard = false` when hosting inside a FloatingPanel.
- For app-specific verbs (MAUI), add `ChatInputAction`/`ChatBubbleAction` — don't try to recreate the removed tool classes.
