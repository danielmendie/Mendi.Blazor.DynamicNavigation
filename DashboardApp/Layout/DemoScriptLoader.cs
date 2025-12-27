using CountryApp.Abstractions.Services.Providers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace CountryApp.Layout
{
    public class DemoScriptLoader : ComponentBase, IDisposable
    {
        [Parameter] public string? Src { get; set; }
        [Parameter] public string? Code { get; set; }
        public Task Loaded => _scriptTcs.Task;
        [Inject] JsLoaderService DemoService { get; set; } = null!;
        [Inject] IJSRuntime JSRuntime { get; set; } = null!;

        bool _canLoadScript, _jsAttached, _isInlinedMethod;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        TaskCompletionSource<bool> _scriptTcs;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public async ValueTask InvokeVoidAsync(string identifier, params object[] args)
        {
            if (!_scriptTcs.Task.IsCompleted)
                await _scriptTcs.Task;
            await JSRuntime.InvokeVoidAsync(identifier, args);
        }
        public async ValueTask<T> InvokeAsync<T>(string identifier, params object[] args)
        {
            if (!_scriptTcs.Task.IsCompleted)
                await _scriptTcs.Task;
            return await JSRuntime.InvokeAsync<T>(identifier, args);
        }

        protected override void OnInitialized()
        {
            _isInlinedMethod = string.IsNullOrEmpty(Src) && !string.IsNullOrEmpty(Code);
            _scriptTcs = DemoService.ResourcesReadyState.GetOrAdd(_isInlinedMethod ? Code! : Src!, CreateScriptReadyTcs);
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (_jsAttached && _canLoadScript)
            {
                _canLoadScript = false;
                builder.OpenElement(0, "script");
                builder.AddAttribute(1, "type", "text/javascript");
                if (!_isInlinedMethod)
                {
                    builder.AddAttribute(2, "src", Src);
                    builder.AddAttribute(3, "async", true);
                    builder.AddAttribute(4, "onload", EventCallback.Factory.Create(this, OnScriptLoaded));
                }
                else
                    builder.AddContent(5, Code);
                builder.CloseElement();
            }
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender && _canLoadScript)
            {
                _jsAttached = true;
                StateHasChanged();
            }
            else if (!firstRender && _isInlinedMethod)
                OnScriptLoaded();
        }

        TaskCompletionSource<bool> CreateScriptReadyTcs(string _)
        {
            _canLoadScript = true;
            return new TaskCompletionSource<bool>();
        }

        void OnScriptLoaded()
        {
            _scriptTcs?.TrySetResult(true);
        }

        void IDisposable.Dispose()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            _scriptTcs = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }
    }
}
