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

    // Navigate to a registered route with key-value arguments
    Task NavigateTo(string route, params IEnumerable<(string Key, object Value)> args);

    // Navigate to the page associated with a ViewModel type
    // configure: optional action to set ViewModel properties before navigation
    Task NavigateTo<TViewModel>(
        Action<TViewModel>? configure = null,
        params IEnumerable<(string Key, object Value)> args
    );

    // Replace the root page with one associated with the ViewModel type
    Task SetRoot<TViewModel>(
        Action<TViewModel>? configure = null,
        params IEnumerable<(string Key, object Value)> args
    );

    // Pop to the root page, optionally passing arguments
    Task PopToRoot(params IEnumerable<(string Key, object Value)> args);

    // Go back one page, optionally passing arguments
    Task GoBack(params IEnumerable<(string Key, object Value)> args);

    // Go back multiple pages
    Task GoBack(int backCount = 1, params IEnumerable<(string Key, object Value)> args);

    // Display an alert dialog
    Task Alert(string? title, string message, string acceptText = "OK");

    // Display a confirmation dialog, returns true if accepted
    Task<bool> Confirm(string? title, string message, string acceptText = "Yes", string cancelText = "No");
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
    PopToRoot
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
    await navigator.NavigateTo("detail", ("ItemId", "abc"), ("Mode", "edit"));

    // ViewModel-based navigation
    await navigator.NavigateTo<DetailViewModel>(vm => vm.ItemId = "abc");

    // Go back with result
    await navigator.GoBack(("Result", selectedValue));

    // Go back 2 pages
    await navigator.GoBack(2);

    // Pop entire stack to root
    await navigator.PopToRoot();

    // Replace root
    await navigator.SetRoot<DashboardViewModel>();

    // Alert
    await navigator.Alert("Error", "Something went wrong");

    // Confirm
    if (await navigator.Confirm("Delete", "Are you sure?"))
    {
        // delete item
    }
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

    return await navigator.Confirm("Unsaved Changes", "Discard changes?");
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

Fluent builder for registering Page-to-ViewModel mappings. Used inside `UseShinyShell()`.

```csharp
public sealed class ShinyAppBuilder
{
    // Register a Page-ViewModel pair
    // route: optional route name (defaults to page class name)
    // registerRoute: set false for pages already in AppShell.xaml
    ShinyAppBuilder Add<TPage, TViewModel>(string? route = null, bool registerRoute = true)
        where TPage : Page
        where TViewModel : class, INotifyPropertyChanged;
}
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
    string? route = null,         // Route name (defaults to page class name)
    bool registerRoute = true     // Set false for AppShell.xaml pages
) : Attribute;
```

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
[ShellMap<DetailPage>("detail")]
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
    public const string Detail = "detail";
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

**NavigationBuilderExtensions.g.cs:**
```csharp
public static class NavigationBuilderExtensions
{
    public static ShinyAppBuilder AddGeneratedMaps(this ShinyAppBuilder builder)
    {
        builder.Add<DetailPage, DetailViewModel>(Routes.Detail);
        return builder;
    }
}
```

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
- Clean and rebuild the project

### OnAppearing/OnDisappearing not firing
- ViewModel must implement `IPageLifecycleAware`
- Verify the ViewModel is bound to the Page (check BindingContext)

### CanNavigate not called
- ViewModel must implement `INavigationConfirmation`
- Only fires when navigating away from the page (not when navigating to it)
