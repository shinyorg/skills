# API Reference

## Installation

```bash
dotnet add package Shiny.Maui.Shell
```

The NuGet package includes both the runtime library and the source generator. No additional analyzer package is needed.

## Namespace

All public types are in the `Shiny` namespace:
```csharp
using Shiny;
```

## INavigator Interface

The primary navigation service. Injected via DI as a singleton.

```csharp
public interface INavigator
{
    // Fires before navigation occurs - includes the source ViewModel instance
    event EventHandler<NavigationEventArgs>? Navigating;

    // Fires after navigation completes - includes the destination ViewModel instance
    event EventHandler<NavigatedEventArgs>? Navigated;

    // Create a fluent navigation builder for multi-segment navigation
    // fromRoot: if true, builds an absolute URI ("//route"); only works for shell-declared routes
    INavigationBuilder CreateBuilder(bool fromRoot = false);

    // Navigate to a registered route with key-value arguments
    // relativeNavigation: true (default) for relative push, false for absolute "//" navigation
    Task NavigateTo(string route, bool relativeNavigation = true, params IEnumerable<(string Key, object Value)> args);

    // Navigate to the page associated with a ViewModel type
    // relativeNavigation: true (default) for relative push, false for absolute "//" navigation
    Task NavigateTo<TViewModel>(
        Action<TViewModel>? configure = null,
        bool relativeNavigation = true,
        params IEnumerable<(string Key, object Value)> args
    );

    // Pop to the root page, optionally passing arguments
    Task PopToRoot(params IEnumerable<(string Key, object Value)> args);

    // Go back one page, optionally passing arguments
    Task GoBack(params IEnumerable<(string Key, object Value)> args);

    // Go back multiple pages
    Task GoBack(int backCount = 1, params IEnumerable<(string Key, object Value)> args);

    // Switch to a different Shell instance (replaces Application.MainPage)
    Task SwitchShell(Shell shell);

    // Switch to a Shell resolved from the DI container
    Task SwitchShell<TShell>() where TShell : Shell;
}
```

## INavigationBuilder Interface

A fluent builder for constructing multi-segment navigation URIs. Created via `INavigator.CreateBuilder()`.

```csharp
public interface INavigationBuilder
{
    // Pop back one or more pages before pushing new segments. Must be called before any Add calls.
    INavigationBuilder PopBack(int count = 1);

    // Add a navigation segment for the specified ViewModel type
    INavigationBuilder Add<TViewModel>() where TViewModel : class;

    // Add a segment with a configure callback invoked on the ViewModel when the page is created
    INavigationBuilder Add<TViewModel>(Action<TViewModel> configure) where TViewModel : class;

    // Add a segment using a raw route string
    INavigationBuilder Add(string routeName);

    // Execute the navigation
    Task Navigate();
}
```

### Navigation Builder Constraints

- **All pages in a chain must be globally registered** (`registerRoute: true`, the default). Pages declared as `ShellContent` in XAML cannot be used in multi-segment relative URIs — Shell throws "Relative routing to shell elements is currently not supported".
- **`PopBack()` must be called before `Add()`** — you cannot interleave pops and pushes.
- **`fromRoot: true`** generates a `//` prefix and only works when navigating to shell-declared routes. Global routes cannot be the only page on the stack.

### Usage Examples

```csharp
// Push a chain of pages
await navigator
    .CreateBuilder()
    .Add<OneViewModel>(x => x.Text = "First")
    .Add<AnotherViewModel>(x => x.Arg = "Middle")
    .Add<TwoViewModel>(x => x.Text = "Last")
    .Navigate();

// Pop back 2, then push
await navigator
    .CreateBuilder()
    .PopBack(2)
    .Add<OneViewModel>(x => x.Text = "Replaced")
    .Navigate();
```

## IDialogs Interface

A testable dialog service. Injected via DI as a singleton. Use this instead of `Shell.Current.DisplayAlert`, `Shell.Current.DisplayPromptAsync`, or `Shell.Current.DisplayActionSheet`.

```csharp
public interface IDialogs
{
    // Display an alert dialog
    Task Alert(string? title, string message, string acceptText = "OK");

    // Display a confirmation dialog, returns true if accepted
    Task<bool> Confirm(string? title, string message, string acceptText = "Yes", string cancelText = "No");

    // Display a text input prompt, returns entered text or null if cancelled
    Task<string?> Prompt(
        string? title,
        string message,
        string acceptText = "OK",
        string cancelText = "Cancel",
        string? placeholder = null,
        string initialValue = "",
        int maxLength = -1,
        Keyboard? keyboard = null
    );

    // Display an action sheet with multiple options, returns selected button text
    Task<string> ActionSheet(string? title, string? cancel, string? destruction, params string[] buttons);
}
```

### Usage Examples

```csharp
public class MyViewModel(IDialogs dialogs)
{
    // Alert
    await dialogs.Alert("Error", "Something went wrong");

    // Confirm
    if (await dialogs.Confirm("Delete", "Are you sure?"))
    {
        // delete item
    }

    // Prompt for text input
    var name = await dialogs.Prompt("Name", "Enter your name", placeholder: "John Doe");
    if (name != null)
    {
        // user accepted with a value
    }

    // Prompt with numeric keyboard and max length
    var code = await dialogs.Prompt("Code", "Enter PIN", maxLength: 4, keyboard: Keyboard.Numeric);

    // Action sheet
    var action = await dialogs.ActionSheet("Photo", "Cancel", "Delete", "Take Photo", "Choose from Library");
}
```

## Navigation Events

### NavigationEventArgs (pre-navigation)

Fired via `INavigator.Navigating` before navigation occurs. Provides the source ViewModel instance.

```csharp
public record NavigationEventArgs(
    string? FromUri,                                  // Current location URI
    object? FromViewModel,                            // Source ViewModel instance (cast as needed)
    string ToUri,                                     // Destination route URI
    NavigationType NavigationType,                    // Push, SetRoot, GoBack, or PopToRoot
    IReadOnlyDictionary<string, object> Parameters    // Navigation parameters
);
```

### NavigatedEventArgs (post-navigation)

Fired via `INavigator.Navigated` after navigation completes and the destination page's ViewModel is resolved. Provides the destination ViewModel instance.

```csharp
public record NavigatedEventArgs(
    string ToUri,                                     // Destination route URI
    object? ToViewModel,                              // Destination ViewModel instance (cast as needed)
    NavigationType NavigationType,                    // Push, SetRoot, GoBack, or PopToRoot
    IReadOnlyDictionary<string, object> Parameters    // Navigation parameters
);
```

### NavigationType Enum

```csharp
public enum NavigationType
{
    Push,
    SetRoot,
    GoBack,
    PopToRoot,
    SwitchShell
}
```

### Usage

```csharp
navigator.Navigating += (sender, args) =>
{
    // Access the source ViewModel
    if (args.FromViewModel is MyViewModel vm)
        Console.WriteLine($"Leaving {vm.Title}");
};

navigator.Navigated += (sender, args) =>
{
    // Access the destination ViewModel
    if (args.ToViewModel is DetailViewModel detail)
        Console.WriteLine($"Arrived at {detail.ItemId}");
};
```

### Usage Examples

```csharp
public class MyViewModel(INavigator navigator)
{
    // Route-based navigation
    await navigator.NavigateTo("Detail", args: [("ItemId", "abc"), ("Mode", "edit")]);

    // ViewModel-based navigation
    await navigator.NavigateTo<DetailViewModel>(vm => vm.ItemId = "abc");

    // Absolute navigation (navigates to root route "//Detail")
    await navigator.NavigateTo("Detail", relativeNavigation: false);
    await navigator.NavigateTo<DetailViewModel>(relativeNavigation: false);

    // Go back with result
    await navigator.GoBack(("Result", selectedValue));

    // Go back 2 pages
    await navigator.GoBack(2);

    // Pop entire stack to root
    await navigator.PopToRoot();

    // Switch to a different Shell instance
    await navigator.SwitchShell(new MainAppShell());

    // Switch to a Shell resolved from DI
    await navigator.SwitchShell<MainAppShell>();

    // Multi-segment navigation via builder
    await navigator
        .CreateBuilder()
        .Add<OneViewModel>(x => x.Text = "First")
        .Add<TwoViewModel>(x => x.Text = "Last")
        .Navigate();
}
```

## IPageLifecycleAware Interface

Provides page appearing/disappearing lifecycle hooks on ViewModels.

```csharp
public interface IPageLifecycleAware
{
    // Called when the page becomes visible (or re-appears after navigation back)
    void OnAppearing();

    // Called when the page is hidden or removed from the navigation stack
    void OnDisappearing();
}
```

## INavigationConfirmation Interface

Allows a ViewModel to block navigation away from its page.

```csharp
public interface INavigationConfirmation
{
    // Return true to allow navigation, false to block it
    Task<bool> CanNavigate();
}
```

### Usage
```csharp
public async Task<bool> CanNavigate()
{
    if (!hasUnsavedChanges)
        return true;

    return await dialogs.Confirm("Unsaved Changes", "Discard changes?");
}
```

## INavigationAware Interface

Allows a ViewModel to add or modify navigation parameters before the page navigates away.

```csharp
public interface INavigationAware
{
    // Called before navigation. Mutate the parameters dictionary to pass data back.
    void OnNavigatingFrom(IDictionary<string, object> parameters);
}
```

### Usage
```csharp
public void OnNavigatingFrom(IDictionary<string, object> parameters)
{
    parameters["LastViewed"] = CurrentItemId;
    parameters["Timestamp"] = DateTime.UtcNow;
}
```

## ShinyAppBuilder Class

Fluent builder for registering Page-to-ViewModel mappings and configuring shell services. Used inside `UseShinyShell()`.

```csharp
public sealed class ShinyAppBuilder(MauiAppBuilder builder)
{
    // Register a Page-ViewModel pair
    // route: optional route name (defaults to page class name)
    // registerRoute: set false for pages already in AppShell.xaml
    ShinyAppBuilder Add<TPage, TViewModel>(string? route = null, bool registerRoute = true)
        where TPage : Page
        where TViewModel : class, INotifyPropertyChanged;

    // Replace the default IDialogs provider with a custom implementation.
    // Registered as a singleton. Call order does not matter — UseDialogs<>
    // always wins over the default ShellDialogs (registered via TryAddSingleton).
    ShinyAppBuilder UseDialogs<TDialog>() where TDialog : class, IDialogs;
}
```

### UseDialogs Example

```csharp
builder.UseShinyShell(x => x
    .AddGeneratedMaps()
    .UseDialogs<MyCustomDialogs>()
);
```

### Constraints
- `TPage` must inherit from `Microsoft.Maui.Controls.Page`
- `TViewModel` must implement `INotifyPropertyChanged`
- Both are registered as Transient in DI automatically

## Attributes

### ShellMapAttribute\<TPage\>

Marks a ViewModel class for source generation. Applied to the ViewModel class.

```csharp
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class ShellMapAttribute<TPage>(
    string? route = null,         // Route name — must be a valid C# identifier; used as generated constant and method name
    bool registerRoute = true     // Set false for AppShell.xaml pages
) : Attribute;
```

The `route` parameter drives naming:
- `[ShellMap<DetailPage>("Detail")]` → `Routes.Detail`, `NavigateToDetail(...)`
- `[ShellMap<HomePage>]` (no route) → `Routes.Home`, `NavigateToHome(...)`

Invalid route names (hyphens, spaces, leading digits) produce a **SHINY001** compiler error.

### ShellPropertyAttribute

Marks a ViewModel property as a navigation parameter for source generation.

```csharp
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class ShellPropertyAttribute(
    bool required = true          // Whether this parameter is required in generated methods
) : Attribute;
```

### Source Generation Output

Given this input:
```csharp
[ShellMap<DetailPage>("Detail")]
public partial class DetailViewModel : ObservableObject
{
    [ShellProperty] public string ItemId { get; set; }
    [ShellProperty(required: false)] public int Page { get; set; }
}
```

The source generator produces:

**Routes.g.cs:**
```csharp
public static class Routes
{
    public const string Detail = "Detail";
}
```

**NavigationExtensions.g.cs:**
```csharp
public static class NavigationExtensions
{
    public static Task NavigateToDetail(this INavigator navigator, string itemId, int page = default)
    {
        return navigator.NavigateTo<DetailViewModel>(x =>
        {
            x.ItemId = itemId;
            x.Page = page;
        });
    }
}
```

**NavigationBuilderExtensions.g.cs** (uses string literals, not `Routes.*`):
```csharp
public static class NavigationBuilderExtensions
{
    public static ShinyAppBuilder AddGeneratedMaps(this ShinyAppBuilder builder)
    {
        builder.Add<DetailPage, DetailViewModel>("Detail");
        return builder;
    }
}
```

### Configuring Source Generation

Disable individual generated files via MSBuild properties:

| Property | Default | Controls |
|---|---|---|
| `ShinyMauiShell_GenerateRouteConstants` | `true` | `Routes.g.cs` |
| `ShinyMauiShell_GenerateNavExtensions` | `true` | `NavigationExtensions.g.cs` |

`NavigationBuilderExtensions.g.cs` is always generated.

## ShinyShell Base Class

Your `AppShell` must inherit from `ShinyShell` instead of `Shell`. This ensures the initial page's BindingContext is set deterministically via Shell's own `OnNavigated` lifecycle — avoiding a race condition where the `Application.PageAppearing` event can fire before the handler is registered.

```csharp
// AppShell.xaml.cs
using Shiny;

public partial class AppShell : ShinyShell
{
    public AppShell()
    {
        InitializeComponent();
    }
}
```

```xml
<!-- AppShell.xaml -->
<shiny:ShinyShell
    x:Class="MyApp.AppShell"
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    xmlns:shiny="clr-namespace:Shiny;assembly=Shiny.Maui.Shell"
    xmlns:local="clr-namespace:MyApp"
    Title="MyApp">

    <ShellContent
        Title="Home"
        ContentTemplate="{DataTemplate local:MainPage}"
        Route="MainPage" />

</shiny:ShinyShell>
```

## ShellServices Record

A convenience aggregate that bundles the three shell services together, so a single constructor parameter is enough when a class needs most of them.

```csharp
public record ShellServices(
    INavigator Navigator,
    IDialogs Dialogs,
    IMainThread MainThread
);
```

Registered as a singleton by `UseShinyShell()`. Inject directly:

```csharp
public class MyViewModel(ShellServices shell)
{
    async Task DoWork()
    {
        shell.MainThread.BeginInvokeOnMainThread(() => /* UI update */);
        await shell.Dialogs.Alert("Done", "Work complete");
        await shell.Navigator.GoBack();
    }
}
```

## IMainThread Interface

Thread-marshalling abstraction used internally by `ShellNavigator` and `ShellDialogs`. Prefer this over `Microsoft.Maui.ApplicationModel.MainThread` inside Shiny Shell code because the default implementation (`MauiMainThread`) transparently works around platforms where MAUI's `MainThread.InvokeOnMainThreadAsync` is broken — currently macOS and Linux, where the implementation executes the delegate inline instead of dispatching it.

```csharp
public interface IMainThread
{
    Task InvokeOnMainThreadAsync(Action action);
    Task InvokeOnMainThreadAsync(Func<Task> func);
    Task<T> InvokeOnMainThreadAsync<T>(Func<Task<T>> func);
    void BeginInvokeOnMainThread(Action action);
}
```

Registered as a singleton by `UseShinyShell()` with the default `MauiMainThread` implementation.

## Extension Method

### UseShinyShell

Configures Shiny MAUI Shell on the `MauiAppBuilder`.

```csharp
public static MauiAppBuilder UseShinyShell(
    this MauiAppBuilder builder,
    Action<ShinyAppBuilder> navBuilderAction
);
```

Registers:
- `INavigator` as singleton
- `IDialogs` as singleton (default `ShellDialogs`, replaceable via `UseDialogs<>()`)
- `IMainThread` as singleton (default `MauiMainThread`)
- `ShellServices` as singleton (aggregate of `INavigator`, `IDialogs`, `IMainThread`)
- `IMauiInitializeService` for lifecycle hooks
- `ShinyAppBuilder` as singleton
- All mapped Pages and ViewModels as transient

## IQueryAttributable (MAUI Built-in)

Standard MAUI interface for receiving navigation parameters. Must be implemented on ViewModels that receive arguments.

```csharp
// From Microsoft.Maui.Controls
public interface IQueryAttributable
{
    void ApplyQueryAttributes(IDictionary<string, object> query);
}
```

## IDisposable (System)

When implemented on a ViewModel, `Dispose()` is called when the page is permanently removed from the navigation stack.

## Troubleshooting

### ViewModel not bound to Page
- Ensure your AppShell inherits from `ShinyShell`, not `Shell`
- Ensure the Page-ViewModel pair is registered via `Add<TPage, TViewModel>()` or `[ShellMap]` + `AddGeneratedMaps()`
- Check that `UseShinyShell()` is called in MauiProgram.cs

### Navigation parameters not received
- ViewModel must implement `IQueryAttributable`
- Parameter keys are case-sensitive and must match property names
- When using `NavigateTo<TViewModel>(configure)`, properties set via `configure` are available immediately (no need for `IQueryAttributable`)

### Page not found during navigation
- Pages in AppShell.xaml should use `registerRoute: false`
- Pages not in AppShell.xaml need route registration (default behavior)
- Verify the route string matches exactly

### Source generator not producing output
- ViewModel class must be `partial`
- Ensure `Shiny.Maui.Shell` NuGet is installed (includes the generator)
- Check that `[ShellMap<TPage>]` attribute is applied to the class
- Route names must be valid C# identifiers — check for **SHINY001** errors
- Route constants and nav extensions can be disabled via `ShinyMauiShell_GenerateRouteConstants` and `ShinyMauiShell_GenerateNavExtensions` MSBuild properties
- Clean and rebuild the project

### OnAppearing/OnDisappearing not firing
- ViewModel must implement `IPageLifecycleAware`
- Verify the ViewModel is bound to the Page (check BindingContext)

### CanNavigate not called
- ViewModel must implement `INavigationConfirmation`
- Only fires when navigating away from the page (not when navigating to it)
