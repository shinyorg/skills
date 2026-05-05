# ChatView

A modern chat UI control with message bubbles, typing indicators, load-more pagination, and a bottom input bar. Supports single-person and multi-person conversations with per-participant colors and avatars.

## Basic Usage

```xml
<shiny:ChatView Messages="{Binding Messages}"
                Participants="{Binding Participants}"
                IsMultiPerson="True"
                TypingParticipants="{Binding TypingParticipants}"
                SendCommand="{Binding SendCommand}"
                AttachImageCommand="{Binding AttachImageCommand}"
                LoadMoreCommand="{Binding LoadMoreCommand}"
                MyBubbleColor="#DCF8C6"
                OtherBubbleColor="White" />
```

## Data Models

### ChatMessage

```csharp
public class ChatMessage
{
    public string Id { get; set; }                          // Auto-generated GUID
    public string? Text { get; set; }                       // null for image messages
    public string? ImageUrl { get; set; }                   // null for text messages
    public string SenderId { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public bool IsFromMe { get; set; }
    public string? Identifier { get; set; }                 // Optional user-defined identifier for post-send context
    public DateTimeOffset? DateSent { get; set; }           // When null, bubble renders dimmed (pending/offline). Only applies to user messages.
    public List<Acknowledgement>? Acknowledgements { get; set; } // Reactions displayed as badges below bubble
    public IList<FabMenuItem>? ToolItems { get; set; }     // Per-message bubble tool overrides (MAUI only)
}
```

### Acknowledgement

```csharp
public class Acknowledgement
{
    public string? Glyph { get; set; }     // Emoji/character (e.g., 👍, ❤️, 💯)
    public string UserId { get; set; }     // ID of the user who reacted
    public DateTime Timestamp { get; set; } // When the reaction was added
}
```

Acknowledgements are grouped by `Glyph` and rendered as small badge pills below the chat bubble. The count is displayed beside the glyph only when > 1.

### ChatParticipant

```csharp
// MAUI
public class ChatParticipant
{
    public string Id { get; set; }
    public string DisplayName { get; set; }
    public ImageSource? Avatar { get; set; }     // MAUI ImageSource
    public Color? BubbleColor { get; set; }      // MAUI Color
}

// Blazor
public class ChatParticipant
{
    public string Id { get; set; }
    public string DisplayName { get; set; }
    public string? AvatarUrl { get; set; }       // URL string
    public string? BubbleColor { get; set; }     // CSS color string
}
```

## ChatView Properties

| Property | Type | Default | Description |
|---|---|---|---|
| `Messages` | `IList<ChatMessage>` | `null` | Message collection; observes `INotifyCollectionChanged` on MAUI |
| `Participants` | `IList<ChatParticipant>` | `null` | Participant info for avatar/name/color lookup by `SenderId` |
| `IsMultiPerson` | `bool` | `false` | Show avatars and names for other participants |
| `ShowAvatarsInSingleChat` | `bool` | `false` | Force avatars/names even in single-person mode |
| `MyBubbleColor` | `Color` / `string` | `#DCF8C6` | Local user bubble color |
| `MyTextColor` | `Color` / `string` | `Black` | Local user text color |
| `OtherBubbleColor` | `Color` / `string` | `White` | Default other-user bubble color (overridden by participant's `BubbleColor`) |
| `OtherTextColor` | `Color` / `string` | `Black` | Other-user text color |
| `PlaceholderText` | `string` | `"Type a message..."` | Input field placeholder |
| `SendButtonText` | `string` | `"Send"` | Send button label |
| `IsInputBarVisible` | `bool` | `true` | Show/hide the entire input bar |
| `ShowTypingIndicator` | `bool` | `true` | Enable/disable typing indicator |
| `TypingParticipants` | `IList<ChatParticipant>` | `null` | Currently typing participants (do not include "me") |
| `ScrollToFirstUnread` | `bool` | `false` | Scroll to first unread message instead of end |
| `FirstUnreadMessageId` | `string?` | `null` | ID of the first unread message |
| `ToolItems` | `IList<ChatEntryTool>` | `null` | Input bar tools FAB menu items (MAUI only) |
| `ToolsIcon` | `ImageSource` | `null` | Icon for the tools FabMenu button (MAUI only) |
| `ToolsText` | `string?` | `null` | Text label for the tools FabMenu button (MAUI only) |
| `ToolsFabBackgroundColor` | `Color` | `#007AFF` | Background color of the tools FabMenu button (MAUI only) |
| `BubbleToolItems` | `IList<ChatBubbleTool>` | `null` | Bubble tools for received (other user) messages (MAUI only) |
| `MyBubbleToolItems` | `IList<ChatBubbleTool>` | `null` | Bubble tools for the local user's own messages (MAUI only) |
| `MessageTappedCommand` | `ICommand` | `null` | Fired when a message bubble is tapped (MAUI only) |
| `MessageTemplate` | `DataTemplate?` | `null` | Single template for all message content (MAUI only) |
| `MessageTemplateSelector` | `DataTemplateSelector?` | `null` | Per-type template selector (MAUI only) |
| `UseFeedback` | `bool` | `true` | Haptic feedback on interactions (MAUI only) |

## Commands (MAUI ICommand) / Events (Blazor EventCallback)

| MAUI | Blazor | Parameter | Description |
|---|---|---|---|
| `SendCommand` | `EventCallback<string>` | text string | Fires when user sends a text message via Enter or Send button |
| `AttachImageCommand` | `EventCallback` | -- | Fires when user taps attach button; user implements own image picker |
| `LoadMoreCommand` | `EventCallback` | -- | Fires when user scrolls near top; prepend older messages to the list |
| `MessageTappedCommand` | `EventCallback<ChatMessage>` | message | Fires when a message bubble is tapped |

## Methods (MAUI only)

| Method | Description |
|---|---|
| `ScrollToEnd(bool animate)` | Scroll to the latest message |
| `ScrollToMessage(string messageId, bool animate)` | Scroll to a specific message by ID |
| `SubmitEntry()` | Programmatically submit current input text |
| `EntryText` (property) | Get/set the input field text |

## Tool Base Classes (MAUI only)

### ChatEntryTool

Non-abstract base class for input bar tools that need ChatView access. Can be used directly in XAML with a `Command` binding, or subclassed for self-contained tools.

```csharp
public class ChatEntryTool : FabMenuItem
{
    protected ChatView? ChatView { get; private set; }
    // Attach/Detach called automatically by ChatView
}
```

Use directly in XAML:
```xml
<shiny:ChatEntryTool Text="Camera" Icon="camera.png"
                     FabBackgroundColor="#4CAF50"
                     Command="{Binding TakePhotoCommand}" />
```

Or subclass for tools that need to read/write the input text or submit:

```csharp
public class MyCustomTool : ChatEntryTool
{
    public MyCustomTool()
    {
        Text = "My Tool";
        FabBackgroundColor = Colors.Purple;
        Clicked += OnClicked;
    }

    void OnClicked(object? sender, EventArgs e)
    {
        if (ChatView is null) return;
        ChatView.EntryText = "Hello!";
        ChatView.SubmitEntry();
    }
}
```

### ChatBubbleTool

Non-abstract base class for bubble tools that act on a message. Can be used directly in XAML with a `Command` binding (receives `ChatMessage` via `CommandParameter`), or subclassed for self-contained tools.

```csharp
public class ChatBubbleTool : FabMenuItem
{
    protected ChatMessage? Message { get; } // Auto-populated via CommandParameter
    protected void RequestRefresh();         // Triggers UI refresh after modifying message data
}
```

Use directly in XAML:
```xml
<shiny:ChatBubbleTool Text="Translate" FabBackgroundColor="#9C27B0"
                      Command="{Binding TranslateCommand}" />
```

Or subclass for tools that operate on the tapped message:

```csharp
public class MyBubbleTool : ChatBubbleTool
{
    public MyBubbleTool()
    {
        Text = "Translate";
        FabBackgroundColor = Colors.Teal;
        Clicked += OnClicked;
    }

    async void OnClicked(object? sender, EventArgs e)
    {
        if (Message is null) return;
        // Do something with Message.Text
    }
}
```

### Built-in Tools

| Tool | Base Class | Package | Description |
|---|---|---|---|
| `CopyBubbleTool` | `ChatBubbleTool` | `Shiny.Maui.Controls` | Copies message text/ImageUrl to clipboard |
| `TextToSpeechBubbleTool` | `ChatBubbleTool` | `Shiny.Maui.Controls.SpeechAddins` | Reads message text aloud |
| `SpeechToTextTool` | `ChatEntryTool` | `Shiny.Maui.Controls.SpeechAddins` | Voice input for chat entry |
| `PhotoGalleryEntryTool` | `ChatEntryTool` | `Shiny.Maui.Controls` | Opens device photo gallery via MAUI MediaPicker, fires `AttachImageCommand` with file path |
| `TakePhotoEntryTool` | `ChatEntryTool` | `Shiny.Maui.Controls` | Opens device camera via MAUI MediaPicker, fires `AttachImageCommand` with file path |
| `AcknowledgementBubbleTool` | `ChatBubbleTool` | `Shiny.Maui.Controls` | Single-tap toggle for a specific reaction emoji. Set `Glyph` and optionally `UserId`. Bind `Command` to notify server (receives `AcknowledgementChangedContext`). |
| `AcknowledgementSelectorBubbleTool` | `ChatBubbleTool` | `Shiny.Maui.Controls` | Opens action sheet with 12 default emoji reactions. Customizable via `Glyphs` property. Bind `Command` to notify server (receives `AcknowledgementChangedContext`). |

## Bubble Tools (MAUI only)

Bubble tools are split by message ownership:
- `BubbleToolItems` — shown on received (other user) messages
- `MyBubbleToolItems` — shown on the local user's own messages

The ⋮ button appears on each bubble that has applicable tools:

```xml
<shiny:ChatView Messages="{Binding Messages}"
                SendCommand="{Binding SendCommand}">
    <!-- Tools for received messages -->
    <shiny:ChatView.BubbleToolItems>
        <shiny:CopyBubbleTool />
        <shiny:AcknowledgementBubbleTool Glyph="👍" Command="{Binding AckCommand}" />
        <shiny:AcknowledgementBubbleTool Glyph="👎" Command="{Binding AckCommand}" />
        <shiny:AcknowledgementSelectorBubbleTool Command="{Binding AckCommand}" />
        <shiny:ChatBubbleTool Text="Reply" FabBackgroundColor="#2196F3"
                              Command="{Binding ReplyCommand}" />
    </shiny:ChatView.BubbleToolItems>

    <!-- Tools for my own messages -->
    <shiny:ChatView.MyBubbleToolItems>
        <shiny:CopyBubbleTool />
    </shiny:ChatView.MyBubbleToolItems>
</shiny:ChatView>
```

- `ChatBubbleTool` with `Command`: `CommandParameter` is automatically set to the `ChatMessage`
- `AcknowledgementBubbleTool` / `AcknowledgementSelectorBubbleTool` with `Command`: receives `AcknowledgementChangedContext` with `.Message` and `.Glyph`
- Per-message override: set `ChatMessage.ToolItems` to replace the default tools for that message

## Input Bar Tools (MAUI only)

```xml
<shiny:ChatView Messages="{Binding Messages}"
                SendCommand="{Binding SendCommand}"
                ToolsIcon="tools.png"
                ToolsFabBackgroundColor="#007AFF">
    <shiny:ChatView.ToolItems>
        <shiny:ChatEntryTool Text="Camera" Icon="camera.png"
                             FabBackgroundColor="#4CAF50"
                             Command="{Binding TakePhotoCommand}" />
        <shiny:SpeechToTextTool AutoSend="False" SilenceTimeout="00:00:03" />
    </shiny:ChatView.ToolItems>
</shiny:ChatView>
```

## Custom Message Templates (MAUI only)

Subclass `ChatMessage` for different message types, then use a `DataTemplateSelector`:

```csharp
public class ActionChatMessage : ChatMessage
{
    public string ActionText { get; set; } = "Accept";
}

public class ChatMessageTemplateSelector : DataTemplateSelector
{
    public DataTemplate? TextTemplate { get; set; }
    public DataTemplate? ActionTemplate { get; set; }

    protected override DataTemplate? OnSelectTemplate(object item, BindableObject container)
    {
        return item switch
        {
            ActionChatMessage => ActionTemplate,
            _ => TextTemplate
        };
    }
}
```

```xml
<shiny:ChatView.MessageTemplateSelector>
    <local:ChatMessageTemplateSelector>
        <local:ChatMessageTemplateSelector.TextTemplate>
            <DataTemplate x:DataType="shiny:ChatMessage">
                <Label Text="{Binding Text}" />
            </DataTemplate>
        </local:ChatMessageTemplateSelector.TextTemplate>
        <local:ChatMessageTemplateSelector.ActionTemplate>
            <DataTemplate x:DataType="local:ActionChatMessage">
                <VerticalStackLayout Spacing="8">
                    <Label Text="{Binding Text}" />
                    <Button Text="{Binding ActionText}"
                            Command="{Binding Source={RelativeSource AncestorType={x:Type vm:MyViewModel}}, Path=AcceptCommand}"
                            CommandParameter="{Binding .}" />
                </VerticalStackLayout>
            </DataTemplate>
        </local:ChatMessageTemplateSelector.ActionTemplate>
    </local:ChatMessageTemplateSelector>
</shiny:ChatView.MessageTemplateSelector>
```

## Code Generation Guidance

- Use `ChatView` for any chat/messaging/conversation UI — do not hand-build bubble layouts with `CollectionView`
- Set `DateSent = null` on outgoing messages to dim the bubble until server confirmation arrives, then set `DateSent = DateTimeOffset.Now` (supports offline/background send scenarios)
- Use `Identifier` to associate server-side context (e.g., a server message ID) with a ChatMessage after sending
- Add `Acknowledgement` items to `Acknowledgements` to show reaction badges (grouped by glyph, count shown when > 1)
- Always provide a `Participants` list for multi-person chats; each participant's `BubbleColor` is optional
- `SendCommand` receives the text string — the control clears the input after sending
- `AttachImageCommand` fires a signal; the user implements their own image picker and adds a `ChatMessage` with `ImageUrl`
- For input bar tools, use `ChatEntryTool` directly with a `Command` binding, or subclass for self-contained tools. Never use `FabMenuItem` in `ToolItems`.
- For bubble tools, use `ChatBubbleTool` directly with a `Command` binding, or subclass for self-contained tools. For acknowledgement reactions, use `AcknowledgementBubbleTool` or `AcknowledgementSelectorBubbleTool`.
- `LoadMoreCommand` fires when the user scrolls near the top; prepend older messages with `Insert(0, msg)`
- `TypingParticipants` should never include the local user (the "you are typing" is excluded by design)
- Set `IsInputBarVisible = false` for read-only chat views (e.g., chat history, support logs)
- Use `MessageTemplate` for simple customization; use `MessageTemplateSelector` for multiple message types
