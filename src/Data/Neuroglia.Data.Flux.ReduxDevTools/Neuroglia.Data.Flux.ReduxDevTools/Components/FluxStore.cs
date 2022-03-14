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
		const reduxDevTools = reduxDevTools.connect({{ {this.Options} }});
		if (reduxDevTools !== undefined && reduxDevTools !== null) {{
			reduxDevTools.subscribe((message) => {{ 
				if (window.reduxDevToolsCallback) {{
					const messageAsJson = JSON.stringify(message);
					window.reduxDevToolsCallback.invokeMethodAsync('{ReduxDevToolsPlugin.JSCallbackMethodName}', messageAsJson); 
				}}
			}});
		}}
		this.{ReduxDevToolsPlugin.JSInitializeMethodName} = function(dotNetCallbacks, state) {{
			window.reduxDevToolsCallback = dotNetCallbacks;
			state = JSON.parse(state);
			reduxDevTools.init(state);
			if (window.reduxDevToolsCallback) {{
				// Notify Fluxor of the presence of the browser plugin
				const detectedMessage = {{
					payload: {{
						type: '{ReduxDevToolsPlugin.JSOnDetectedMethodName}'
					}}
				}};
				const detectedMessageAsJson = JSON.stringify(detectedMessage);
				window.reduxDevToolsCallback.invokeMethodAsync('{ReduxDevToolsPlugin.JSCallbackMethodName}', detectedMessageAsJson);
			}}
		}};
		this.{ReduxDevToolsPlugin.JSDispatchMethodName} = function(action, state) {{
			action = JSON.parse(action);
			state = JSON.parse(state);
			reduxDevTools.send(action, state);
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
