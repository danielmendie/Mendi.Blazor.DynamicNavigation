using Microsoft.JSInterop;

namespace Mendi.Blazor.DynamicNavigation.Tests.Mocks
{
    public class MockJsRuntime : IJSRuntime
    {
        private readonly Dictionary<string, object?> _store = new();

        public ValueTask<TValue> InvokeAsync<TValue>(
            string identifier,
            object?[]? args)
        {
            return new ValueTask<TValue>(HandleInvoke<TValue>(identifier, args));
        }

        public ValueTask<TValue> InvokeAsync<TValue>(
            string identifier,
            CancellationToken cancellationToken,
            object?[]? args)
        {
            return new ValueTask<TValue>(HandleInvoke<TValue>(identifier, args));
        }

        private TValue HandleInvoke<TValue>(string identifier, object?[]? args)
        {
            // Adjust identifiers to match what Blazored.LocalStorage uses.
            // This is illustrative – you need to line up with the actual JS names.

            switch (identifier)
            {
                case "localStorage.setItem":
                    var key = (string)args![0]!;
                    var value = args[1];
                    _store[key] = value;
                    return default!;

                case "localStorage.getItem":
                    var getKey = (string)args![0]!;
                    if (_store.TryGetValue(getKey, out var stored))
                        return (TValue?)stored!;

                    return default!;

                default:
                    // For everything else, just return default
                    return default!;
            }
        }
    }
}
