# TextEntry

A text entry control with animated floating placeholder, customizable border, Bootstrap input-group style left/right tool slots (flush addon segments with background, shared borders, outer-only corner radius), hint text for validation errors, and character count display. Available on both MAUI and Blazor.

## MAUI

**Namespace**: `Shiny.Maui.Controls`
**XAML Namespace**: `http://shiny.net/maui/controls` (prefix: `shiny`)

### Basic Usage

```xml
<shiny:TextEntry Placeholder="Email"
                 Text="{Binding Email, Mode=TwoWay}"
                 Keyboard="Email"
                 HasError="{Binding HasEmailError}"
                 HintText="{Binding EmailError}">
    <shiny:ClearButtonTool />
</shiny:TextEntry>
```

### Properties

| Property | Type | Default | Description |
|---|---|---|---|
| Text | string | "" | Current text value (TwoWay) |
| Placeholder | string | "" | Floating placeholder text |
| PlaceholderColor | Color | Grey | Placeholder color when unfocused |
| FocusedPlaceholderColor | Color | #007AFF | Placeholder color when focused/floating |
| BorderColor | Color | #CCCCCC | Border color when unfocused |
| FocusedBorderColor | Color | #007AFF | Border color when focused |
| BorderThickness | double | 1.0 | Border thickness when unfocused |
| FocusedBorderThickness | double | 2.0 | Border thickness when focused |
| CornerRadius | CornerRadius | 8 | Border corner radius |
| EntryBackgroundColor | Color | Transparent | Background color inside the border |
| FontSize | double | 15 | Text and placeholder font size |
| FontFamily | string? | null | Font family |
| FontAttributes | FontAttributes | None | Bold/Italic |
| TextColor | Color | Black | Input text color |
| IsReadOnly | bool | false | Read-only mode |
| IsPassword | bool | false | Password masking |
| ReturnType | ReturnType | Default | Keyboard return button type |
| Keyboard | Keyboard | Default | Keyboard type (Email, Numeric, etc.) |
| MaxLength | int | int.MaxValue | Maximum character count |
| HintText | string? | null | Hint/helper text below the field |
| HintColor | Color | Grey | Hint text color (when no error) |
| HasError | bool | false | Error state — turns border and hint red |
| ErrorColor | Color | #DC3545 | Color used when HasError is true |
| ShowCharacterCount | bool | false | Show "N/MaxLength" counter |
| LeftTools | IList<TextEntryTool> | [] | Tools on the left side |
| RightTools | IList<TextEntryTool> | [] | Tools on the right side (ContentProperty) |
| Mask | string? | null | Input mask pattern (`#` = digit slot, other chars are literals auto-inserted) |
| FormattedText | string | "" | Read-only formatted display value when Mask is set |
| TextChangedCommand | ICommand? | null | Fires on text change |
| CompletedCommand | ICommand? | null | Fires on return key |

### Events

| Event | Args | Description |
|---|---|---|
| TextChanged | TextChangedEventArgs | Fired when text changes |
| Completed | EventArgs | Fired on return key press |

### Tools

TextEntry supports left and right tool slots. Tools are `TextEntryTool` instances (or subclasses) placed in XAML.

**Built-in tools:**
- `ClearButtonTool` — Shows ✕ when text is non-empty, clears on tap. Auto-hides when empty.
- `TextEntryStepperTool` — Increments/decrements the numeric entry value by `Step` on each tap. Auto-displays `+N` or `-N` if `Text` is not explicitly set.
- `TextEntrySpeechToTextTool` — (Shiny.Maui.Controls.SpeechAddins) Voice input that backfills entry text.

**Stepper tool:**
```xml
<shiny:TextEntry Placeholder="Quantity"
                 Text="{Binding Quantity, Mode=TwoWay}"
                 Keyboard="Numeric">
    <shiny:TextEntry.LeftTools>
        <shiny:TextEntryStepperTool Step="-1" />
    </shiny:TextEntry.LeftTools>
    <shiny:TextEntryStepperTool Step="1" />
</shiny:TextEntry>
```

**TextEntryStepperTool properties:**

| Property | Type | Default | Description |
|---|---|---|---|
| Step | double | 1 | Amount to add (negative = subtract) |
| Text | string? | null | Override button text (defaults to "+N" or "-N") |

**Custom tool:**
```xml
<shiny:TextEntry Placeholder="Amount">
    <shiny:TextEntry.LeftTools>
        <shiny:TextEntryTool Text="$" ToolColor="#059669" />
    </shiny:TextEntry.LeftTools>
    <shiny:ClearButtonTool />
</shiny:TextEntry>
```

**TextEntryTool properties:**

| Property | Type | Description |
|---|---|---|
| Icon | ImageSource? | Tool icon |
| Text | string? | Tool text label |
| ToolColor | Color | Text/icon color |
| Command | ICommand? | Tap command |
| CommandParameter | object? | Command parameter |

**ITextEntryAwareTool** — Implement this interface on a TextEntryTool subclass to get Attach/Detach lifecycle calls with access to the parent TextEntry.

### Input Masking

Set `Mask` to automatically format input as the user types. `#` represents a digit slot; all other characters are literal separators inserted automatically.

When `Mask` is set:
- `Text` always contains **raw digits only** (e.g., `"5551234567"`)
- `FormattedText` contains the display value (e.g., `"(555) 123-4567"`)
- `Keyboard` auto-sets to `Numeric`
- `MaxLength` is auto-calculated from the mask length

```xml
<!-- Phone number -->
<shiny:TextEntry Placeholder="Phone Number"
                 Mask="(###) ###-####"
                 Text="{Binding Phone, Mode=TwoWay}" />

<!-- Credit card -->
<shiny:TextEntry Placeholder="Credit Card"
                 Mask="#### #### #### ####"
                 Text="{Binding CardNumber, Mode=TwoWay}" />

<!-- Date -->
<shiny:TextEntry Placeholder="MM/DD/YYYY"
                 Mask="##/##/####"
                 Text="{Binding DateString, Mode=TwoWay}" />

<!-- SSN -->
<shiny:TextEntry Placeholder="SSN"
                 Mask="###-##-####"
                 Text="{Binding SSN, Mode=TwoWay}" />

<!-- ZIP+4 -->
<shiny:TextEntry Placeholder="ZIP Code"
                 Mask="#####-####"
                 Text="{Binding ZipCode, Mode=TwoWay}" />
```

### Full Example

```xml
<shiny:TextEntry Placeholder="Username"
                 Text="{Binding Username, Mode=TwoWay}"
                 MaxLength="30"
                 ShowCharacterCount="True"
                 HasError="{Binding HasUsernameError}"
                 HintText="{Binding UsernameHint}"
                 FocusedBorderColor="#7C3AED"
                 FocusedPlaceholderColor="#7C3AED"
                 CornerRadius="12">
    <shiny:ClearButtonTool />
</shiny:TextEntry>
```

## Blazor

**Namespace**: `Shiny.Blazor.Controls`

### Basic Usage

```razor
<TextEntry Placeholder="Email"
           @bind-Text="email"
           HasError="@hasError"
           HintText="@errorMessage"
           RightTools="emailTools" />

@code {
    string email = "";
    bool hasError;
    string? errorMessage;
    List<TextEntryTool> emailTools = [new ClearButtonTool()];
}
```

### Parameters

| Parameter | Type | Default | Description |
|---|---|---|---|
| Text | string | "" | Current text (two-way via TextChanged) |
| Placeholder | string | "" | Floating placeholder |
| PlaceholderColor | string | #9CA3AF | CSS color |
| FocusedPlaceholderColor | string | #007AFF | CSS color |
| BorderColor | string | #CCCCCC | CSS color |
| FocusedBorderColor | string | #007AFF | CSS color |
| BorderThickness | double | 1 | Border width (px) |
| FocusedBorderThickness | double | 2 | Focused border width (px) |
| CornerRadius | string | 8px | CSS border-radius |
| EntryBackgroundColor | string | transparent | CSS color |
| FontSize | double | 15 | Font size (px) |
| FontFamily | string | inherit | CSS font-family |
| TextColor | string | inherit | CSS color |
| IsReadOnly | bool | false | Read-only mode |
| IsPassword | bool | false | Password masking |
| MaxLength | int | 0 | Max characters (0 = unlimited) |
| HintText | string? | null | Hint text below field |
| HintColor | string | #9CA3AF | CSS color |
| HasError | bool | false | Error state |
| ErrorColor | string | #DC3545 | CSS color |
| ShowCharacterCount | bool | false | Show counter |
| LeftTools | List\<TextEntryTool\>? | null | Left tool list |
| RightTools | List\<TextEntryTool\>? | null | Right tool list |
| CssClass | string? | null | Additional CSS class |
| Completed | EventCallback | - | Return key event |

### Blazor Tools

Tools use the same `TextEntryTool` base class as MAUI. Pass them as `List<TextEntryTool>` to `LeftTools` or `RightTools`.

**Built-in tools:**
- `ClearButtonTool` — Shows ✕ when text is non-empty, clears on click. Auto-hides when empty.
- `TextEntryStepperTool` — Increments/decrements numeric entry value by `Step`. Auto-displays `+N`/`-N` if `Text` not set.
- `SpeechToTextTool` — (Shiny.Blazor.Controls.SpeechAddins) Voice input via Web Speech API.

**Stepper tool example:**
```razor
<TextEntry Placeholder="Quantity"
           @bind-Text="qty"
           LeftTools="decTools"
           RightTools="incTools" />

@code {
    string qty = "0";
    List<TextEntryTool> decTools = [new TextEntryStepperTool { Step = -1 }];
    List<TextEntryTool> incTools = [new TextEntryStepperTool { Step = 1 }];
}
```

**TextEntryTool properties:**

| Property | Type | Description |
|---|---|---|
| Icon | string? | Icon text/emoji |
| Text | string? | Tool text label |
| ToolColor | string | CSS color for the tool |
| IsVisible | bool | Whether the tool is shown (default: true) |
| Clicked | Action? | Click callback |
| CssClass | string? | Additional CSS class |

**Lifecycle methods** — Override in subclasses:
- `OnTextChanged(string? text)` — Called when entry text changes
- `OnAttached(TextEntryContext context)` / `OnDetached()` — Lifecycle hooks
- `SetEntryText(string text)` / `GetEntryText()` — Read/write parent text

### Blazor Example

```razor
<TextEntry Placeholder="Search"
           @bind-Text="query"
           RightTools="tools" />

@code {
    string query = "";
    List<TextEntryTool> tools = [new ClearButtonTool()];
}
```

```razor
<TextEntry Placeholder="Amount"
           @bind-Text="amount"
           LeftTools="leftTools"
           RightTools="rightTools" />

@code {
    string amount = "";
    List<TextEntryTool> leftTools = [new TextEntryTool { Text = "$", ToolColor = "#059669" }];
    List<TextEntryTool> rightTools = [new ClearButtonTool()];
}
```

### Blazor Mask Example

```razor
<TextEntry Placeholder="Phone Number"
           Mask="(###) ###-####"
           @bind-Text="phone" />

<TextEntry Placeholder="Credit Card"
           Mask="#### #### #### ####"
           @bind-Text="card" />

@code {
    string phone = "";
    string card = "";
}
```

Parameters added for masking:

| Parameter | Type | Default | Description |
|---|---|---|---|
| Mask | string? | null | Input mask pattern (`#` = digit, others are literals) |
| FormattedText | string | "" | Read-only formatted display value |
