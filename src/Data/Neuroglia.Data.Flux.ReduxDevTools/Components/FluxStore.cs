using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Neuroglia.Data.Flux.Configuration;

namespace Neuroglia.Data.Flux.Components
{

    /// <summary>
    /// Represents the <see cref="ComponentBase"/> used to initialize an <see cref="IStore"/> for the current user
    /// </summary>
    public class FluxStore
        : ComponentBase
    {

		/// <summary>
		/// Gets/sets the current <see cref="IJSRuntime"/>
		/// </summary>
		[Inject]
		protected IJSRuntime JSRuntime { get; set; } = null!;

		/// <summary>
		/// Gets/sets the current <see cref="IStore"/>
		/// </summary>
		[Inject]
		protected IStore Store { get; set; } = null!;

		/// <summary>
		/// Gets/sets the options used to configure the <see cref="FluxStore"/>
		/// </summary>
		[Parameter]
		public FluxStoreOptions Options { get; set; } = new();

		/// <summary>
		/// Gets the <see cref="FluxStore"/>'s script
		/// </summary>
		protected virtual string Script
        {
            get
            {
				return $@"
window.{ReduxDevToolsPlugin.JSPrefix} = new (function() {{
const reduxDevTools = window.__REDUX_DEVTOOLS_EXTENSION__;
this.{ReduxDevToolsPlugin.JSInitializeMethodName} = function() {{}};
if (reduxDevTools !== undefined && reduxDevTools !== null) {{
	const fluxDevTools = reduxDevTools.connect({{ {this.Options} }});
	if (fluxDevTools !== undefined && reduxDevTools !== null) {{
		fluxDevTools.subscribe((message) => {{ 
			if (window.reduxDevToolsCallback) {{
				const json = JSON.stringify(message);
				window.reduxDevToolsCallback.invokeMethodAsync('{ReduxDevToolsPlugin.JSCallbackMethodName}', json); 
			}}
		}});
	}}
	this.{ReduxDevToolsPlugin.JSInitializeMethodName} = function(dotNetCallbacks, state) {{
		window.reduxDevToolsCallback = dotNetCallbacks;
		fluxDevTools.init(state);
		if (window.reduxDevToolsCallback) {{
			const message = {{
				payload: {{
					type: '{ReduxDevToolsPlugin.JSOnDetectedMethodName}'
				}}
			}};
			window.reduxDevToolsCallback.invokeMethodAsync('{ReduxDevToolsPlugin.JSCallbackMethodName}', JSON.stringify(message));
		}}
	}};
	this.{ReduxDevToolsPlugin.JSDispatchMethodName} = function(action, state) {{
		action = JSON.parse(action);
		state = JSON.parse(state);
		fluxDevTools.send(action, state);
	}};
}}
}})();";
			}
        }

		/// <inheritdoc/>
		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			await base.OnAfterRenderAsync(firstRender);
			if (firstRender)
			{
				try
				{
					await this.JSRuntime.InvokeVoidAsync("eval", this.Script);
				}
				catch (TaskCanceledException)
				{
					return;
				}
			}
		}

	}

}
