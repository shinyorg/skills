# Questions (`ask_user`)

## How it works

Every turn includes a built-in tool, `ask_user`. When the model needs something from the user it
phrases the question in its normal reply **and** calls the tool:

```text
ask_user(questions: [{ id, text, choices?: [{ id, label }], allowMultiple? }])
```

- The call is **terminating**: the turn ends right after it (no second model round trip).
- The turn outcome is `AiTurnOutcome.AwaitingUser`, the session state is `AwaitingUser`, and
  `session.PendingQuestions` holds the questions (persisted, restored on `Get`).
- `QuestionsAsked(Questions)` is raised as a turn event.
- Voice input (`AiInput.Voice`) limits it to one question per turn; text input allows several.
- Fallback: a reply that ends with `?` but never called the tool still sets `AwaitingUser`, with one
  free-form `AiQuestion` (id `"reply"`, no choices) containing the last question sentence.
- **Each turn replaces `PendingQuestions`.** Anything the model stops asking about counts as answered.

Never add your own JSON format or prompt instructions for questions - this is built in.

## Types

```csharp
public record AiQuestion(string Id, string Text, IReadOnlyList<AiChoice>? Choices = null, bool AllowMultiple = false)
{
    public bool HasChoices { get; }
}
public record AiChoice(string Id, string Label);
```

## Showing and answering

```csharp
// after a turn, or on Snapshot / StateChanged(AwaitingUser)
foreach (var q in session.PendingQuestions)
{
    if (q.HasChoices)
        ShowChips(q, q.Choices!);   // one button per AiChoice.Label
}

// user taps a chip
session.Send(AiInput.Answer(question, choice.Id));

// AllowMultiple
session.Send(AiInput.Answer(question, selected.Select(c => c.Id).ToArray()));

// free-form answer: just send text
session.Send(AiInput.Text(entry.Text));
```

`AiInput.Answer` stores an `AnswerPart` whose text is the chosen labels joined with ", " - that text is
what the chat shows and what the model receives. The stored assistant reply's `Text` is the plain
reply; the `ask_user` call is kept as model-only parts.
