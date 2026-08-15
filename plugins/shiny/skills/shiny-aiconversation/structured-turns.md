# Structured Turns, Questions & Multiple Choice

## Why this exists

`IChatClient` has no typed signal for "that was a question, keep the microphone open" —
`ChatFinishReason` is only `Stop` / `Length` / `ToolCalls` / `ContentFilter`, and the in-box
`InputRequestContent` hierarchy only covers tool approvals. So the conversation service asks the model
for a **structured turn** and reads the signal off the shape instead of guessing at the wording.

## The contract

**Namespace**: `Shiny.AiConversation`

```csharp
public record AiTurn(string Reply, AiQuestion[]? Questions = null)
{
    public bool ExpectsResponse { get; }   // Questions is { Length: > 0 }
}

public record AiQuestion(string Id, string Text, AiChoice[]? Choices = null, bool AllowMultiple = false)
{
    public bool HasChoices { get; }        // Choices is { Length: > 0 }
}

public record AiChoice(string Id, string Label);
```

**The rule: `Reply` is what the user sees and hears. `Questions` drives the interface.** The model
phrases the question naturally inside `Reply`; nothing reads the structured question text aloud. Never
render or speak `AiResponse.Response.Text` — that is the raw JSON envelope. Use `AiResponse.Text`.

## Consuming it

```csharp
aiService.AiResponded += response =>
{
    var text = response.Text;              // parsed reply, or raw text when unstructured
    var turn = response.Turn;              // null when the model returned plain text

    if (response.ExpectsResponse)
    {
        foreach (var question in response.Questions)
        {
            if (question.HasChoices)
            {
                // render buttons - AiChatView does this for you
                foreach (var choice in question.Choices!)
                    Console.WriteLine(choice.Label);
            }
        }
    }
};

// The live queue, replaced on every turn
IReadOnlyList<AiQuestion> pending = aiService.PendingQuestions;
```

## Queue semantics

Each turn **replaces** `PendingQuestions`. The model is the source of truth for what it still needs, so
anything it stops asking about is treated as answered. Do not maintain your own queue on the side and do
not try to pop questions locally — send the answer, let the model re-ask whatever is still outstanding.

Answers go back as plain text (`TalkTo`). **Do not fuzzy-match a spoken answer to a choice locally** —
the model already has the question and its choices in context and resolves "uh, the afternoon one"
itself. Local matching applies only to a tapped button, which sends `choice.Label` verbatim.

## The follow-up window

When a turn carries questions, the microphone stays open for the answer without requiring the wake word
again. `IAiConversationService.FollowUpTimeout` (default 20 seconds) bounds that wait:

- **Answer arrives** — sent to the AI, conversation continues
- **Window expires** — `PendingQuestions` is cleared and the conversation returns to requiring the wake
  word (or ends the `ListenAndTalk` loop)

Set it to `null` to wait indefinitely, but understand the trade-off: an abandoned question leaves the
microphone hot, so the next unrelated thing said in the room becomes the answer.

In `ListenAndTalk` the *first* listen is unbounded (the user tapped the mic); only follow-ups time out.

## Voice vs text question density

The service injects a system prompt that adapts to the delivery mode:

- `Acknowledgement` above `AudioBlip` (spoken aloud) — "ask at most one question per turn"
- otherwise — "you may ask more than one question when they are genuinely independent"

Three questions in one breath renders fine as chips and is unusable as audio.

## Provider support

`IChatClientProvider.StructuredOutputMode` declares how the endpoint can be asked. It is a default
interface member, so custom providers only override it when the default is wrong.

| Mode | Request shape | Use for |
|------|---------------|---------|
| `JsonSchema` *(default)* | Native schema-constrained response format | OpenAI and compatible endpoints |
| `Json` | JSON response format + shape described in the prompt | GitHub Copilot, models without schema support |
| `Prompt` | Shape in the prompt only, no format constraint | Endpoints that reject both of the above |
| `None` | Plain text — opts out entirely | Providers that can't do any of it |

```csharp
public class MyProvider : IChatClientProvider
{
    public Task<IChatClient> GetChatClient(CancellationToken cancelToken = default) => ...;

    // only when the default is wrong for your endpoint
    public AiStructuredOutputMode StructuredOutputMode => AiStructuredOutputMode.Json;
}
```

Both GitHub Copilot providers default to `Json` because the proxy passes `json_schema` support through
per-model. Override per app with `IAiConversationService.StructuredOutputMode`.

## Failure behavior — never assume it worked

Everything degrades to plain text rather than erroring:

- Parsing tolerates markdown fences and surrounding prose (`AiTurnSerializer.Parse`)
- A request that the endpoint rejects outright is retried unstructured
- On any failure, `AiResponse.Turn` is null, `AiResponse.Text` is the raw reply, and `ExpectsResponse`
  falls back to checking whether the reply ends in a question

So always null-check `Turn`, and treat `PendingQuestions` as possibly empty even when
`ExpectsResponse` is true.

## Choice buttons in `AiChatView`

`ShowChoiceButtons` (default `true`) renders a button per `AiChoice` under the AI bubble. Tapping one
sends its `Label` as the user's answer. Questions with `AllowMultiple` collect picks and commit with a
send chip (`ChoiceSendText`, default "Send").

```xml
<ai:AiChatView ShowChoiceButtons="True" ChoiceSendText="Confirm" />
```

The built-in selector is only installed when you have **not** set `MessageTemplate` or
`MessageTemplateSelector`. With your own template, read the choices off the message metadata:

```csharp
var questions = AiChoiceTemplateSelector.ReadQuestions(chatMessage);   // null when the bubble has none
```

The metadata key is `AiChoiceTemplateSelector.QuestionsMetadataKey`, and the payload round-trips through
`AiTurnSerializer.SerializeQuestions` / `DeserializeQuestions`.

## Opting out

```csharp
aiService.StructuredOutputMode = AiStructuredOutputMode.None;
```

Replies become plain text and `ExpectsResponse` falls back to the wording heuristic. `PendingQuestions`
stays empty and choice buttons never render.
