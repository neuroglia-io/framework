﻿// Copyright © 2021-Present Neuroglia SRL. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License"),
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Microsoft.JSInterop;
using Neuroglia.Blazor.Dagre.Models;

namespace Neuroglia.Blazor.Dagre;

/// <summary>
/// Represents a <see cref="IGraphLib"/> graph instance, with a dagre layout
/// </summary>
public class GraphLib(IJSObjectReference jsInstance)
    : IGraphLib, IAsyncDisposable, IDisposable
{

    /// <inheritdoc />
    [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]
    [System.Text.Json.Serialization.JsonExtensionData]
    [Newtonsoft.Json.JsonProperty(NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    [Newtonsoft.Json.JsonExtensionData]
    public virtual IDictionary<string, object>? Metadata { get; set; }

    /// <summary>
    /// The Graph js instance
    /// </summary>
    readonly IJSObjectReference _jsInstance = jsInstance;

    /// <inheritdoc/>
    public virtual async Task<string[]> ChildrenAsync(string v) => await this._jsInstance.InvokeAsync<string[]>("children", v);
    /// <inheritdoc/>
    public virtual async Task<IGraphLib> FilterNodesAsync(Func<string, bool> filter) => (IGraphLib)await this._jsInstance.InvokeAsync<IJSObjectReference>("filterNodes", filter);
    /// <inheritdoc/>
    public virtual async Task<IDagreGraphConfig?> GraphAsync() => await this._jsInstance.InvokeAsync<DagreGraphOptions?>("graph");
    /// <inheritdoc/>
    public virtual async Task<bool> HasEdgeAsync(string v, string w, string name) => await this._jsInstance.InvokeAsync<bool>("hasEdge", v, w, name);
    /// <inheritdoc/>
    public virtual async Task<bool> HasEdgeAsync(GraphLibEdge edge) => await this._jsInstance.InvokeAsync<bool>("hasEdge", edge);
    /// <inheritdoc/>
    public virtual async Task<bool> HasNodeAsync(string name) => await this._jsInstance.InvokeAsync<bool>("hasNode", name);
    /// <inheritdoc/>
    public virtual async Task<GraphLibEdge> EdgeAsync(string v, string w) => await this._jsInstance.InvokeAsync<GraphLibEdge>("edge", v, w);
    /// <inheritdoc/>
    public virtual async Task<GraphLibEdge> EdgeAsync(string v, string w, string name) => await this._jsInstance.InvokeAsync<GraphLibEdge>("edge", v, w, name);
    /// <inheritdoc/>
    public virtual async Task<GraphLibEdge> EdgeAsync(GraphLibEdge e) => await this._jsInstance.InvokeAsync<GraphLibEdge>("edge", e);
    /// <inheritdoc/>
    public virtual async Task<double> EdgeCountAsync() => await this._jsInstance.InvokeAsync<double>("edgeCount");
    /// <inheritdoc/>
    public virtual async Task<GraphLibEdge[]> EdgesAsync() => await this._jsInstance.InvokeAsync<GraphLibEdge[]>("edges");
    /// <inheritdoc/>
    public virtual async Task<GraphLibEdge[]?> InEdgesAsync(string v, string w) => await this._jsInstance.InvokeAsync<GraphLibEdge[]?>("inEdges", v, w);
    /// <inheritdoc/>
    public virtual async Task<bool> IsCompoundAsync() => await this._jsInstance.InvokeAsync<bool>("isCompound");
    /// <inheritdoc/>
    public virtual async Task<bool> IsDirectedAsync() => await this._jsInstance.InvokeAsync<bool>("isDirected");
    /// <inheritdoc/>
    public virtual async Task<bool> IsMultigraphAsync() => await this._jsInstance.InvokeAsync<bool>("isMultigraph");
    /// <inheritdoc/>
    public virtual async Task<IJSObjectReference> InstanceAsync() => await Task.FromResult(this._jsInstance);
    /// <inheritdoc/>
    public virtual async Task<string[]?> NeighborsAsync(string v) => await this._jsInstance.InvokeAsync<string[]?>("neighbors", v);
    /// <inheritdoc/>
    public virtual async Task<GraphLibNode> NodeAsync(string name) => await this._jsInstance.InvokeAsync<GraphLibNode>("node", name);
    /// <inheritdoc/>
    public virtual async Task<double> NodeCountAsync() => await this._jsInstance.InvokeAsync<double>("nodeCount");
    /// <inheritdoc/>
    public virtual async Task<GraphLibEdge[]?> NodeEdgesAsync(string v, string w) => await this._jsInstance.InvokeAsync<GraphLibEdge[]?>("nodeEdges", v, w);
    /// <inheritdoc/>
    public virtual async Task<string[]> NodesAsync() => await this._jsInstance.InvokeAsync<string[]>("nodes");
    /// <inheritdoc/>
    public virtual async Task<GraphLibEdge[]?> OutEdgesAsync(string v, string w) => await this._jsInstance.InvokeAsync<GraphLibEdge[]?>("outEdges", v, w);
    /// <inheritdoc/>
    public virtual async Task<string[]?> ParentAsync(string v) => await this._jsInstance.InvokeAsync<string[]?>("parent", v);
    /// <inheritdoc/>
    public virtual async Task<string[]?> PredecessorsAsync(string v) => await this._jsInstance.InvokeAsync<string[]?>("predecessors", v);
    /// <inheritdoc/>
    public virtual async Task<IGraphLib> RemoveEdgeAsync(GraphLibEdge edge) => await this._jsInstance.InvokeAsync<IJSObjectReference>("removeEdge", edge) as IGraphLib;
    /// <inheritdoc/>
    public virtual async Task<IGraphLib> RemoveEdgeAsync(string v, string w, string name) => await this._jsInstance.InvokeAsync<IJSObjectReference>("removeEdge", v, w, name) as IGraphLib;
    /// <inheritdoc/>
    public virtual async Task<IGraphLib> RemoveNodeAsync(string name) => await this._jsInstance.InvokeAsync<IJSObjectReference>("removeNode", name) as IGraphLib;
    /// <inheritdoc/>
    public virtual async Task<IGraphLib> SetDefaultEdgeLabelAsync(object label) => await this._jsInstance.InvokeAsync<IJSObjectReference>("setDefaultEdgeLabel", label) as IGraphLib;
    /// <inheritdoc/>
    public virtual async Task<IGraphLib> SetDefaultEdgeLabelAsync(Func<string, object> labelFn) => await this._jsInstance.InvokeAsync<IJSObjectReference>("setDefaultEdgeLabel", labelFn) as IGraphLib;
    /// <inheritdoc/>
    public virtual async Task<IGraphLib> SetDefaultNodeLabelAsync(object label) => await this._jsInstance.InvokeAsync<IJSObjectReference>("setDefaultNodeLabel", label) as IGraphLib;
    /// <inheritdoc/>
    public virtual async Task<IGraphLib> SetDefaultNodeLabelAsync(Func<string, object> labelFn) => await this._jsInstance.InvokeAsync<IJSObjectReference>("setDefaultNodeLabel", labelFn) as IGraphLib;
    /// <inheritdoc/>
    public virtual async Task<IGraphLib> SetGraphAsync(IDagreGraphConfig label) => await this._jsInstance.InvokeAsync<IJSObjectReference>("setGraph", label) as IGraphLib;
    /// <inheritdoc/>
    public virtual async Task<IGraphLib> SetEdgeAsync(string v, string w) => await this._jsInstance.InvokeAsync<IJSObjectReference>("setEdge", v, w) as IGraphLib;
    /// <inheritdoc/>
    public virtual async Task<IGraphLib> SetEdgeAsync(string v, string w, object label) => await this._jsInstance.InvokeAsync<IJSObjectReference>("setEdge", v, w, label) as IGraphLib;
    /// <inheritdoc/>
    public virtual async Task<IGraphLib> SetEdgeAsync(string v, string w, object label, string name) => await this._jsInstance.InvokeAsync<IJSObjectReference>("setEdge", v, w, label, name) as IGraphLib;
    /// <inheritdoc/>
    public virtual async Task<IGraphLib> SetEdgeAsync(GraphLibEdge edge, object label) => await this._jsInstance.InvokeAsync<IJSObjectReference>("setEdge", edge, label) as IGraphLib;
    /// <inheritdoc/>
    public virtual async Task<IGraphLib> SetNodeAsync(string name, object label) => await this._jsInstance.InvokeAsync<IJSObjectReference>("setNode", name, label) as IGraphLib;
    /// <inheritdoc/>
    public virtual async Task<IGraphLib> SetNodesAsync(string[] names, object label) => await this._jsInstance.InvokeAsync<IJSObjectReference>("setNodes", names, label) as IGraphLib;
    /// <inheritdoc/>
    public virtual async Task<IGraphLib> SetParentAsync(string v, string p) => await this._jsInstance.InvokeAsync<IJSObjectReference>("setParent", v, p) as IGraphLib;
    /// <inheritdoc/>
    public virtual async Task<IGraphLib> SetPathAsync(string[] nodes, object label) => await this._jsInstance.InvokeAsync<IJSObjectReference>("setPath", nodes, label) as IGraphLib;
    /// <inheritdoc/>
    public virtual async Task<string[]> SinksAsync() => await this._jsInstance.InvokeAsync<string[]>("sinks");
    /// <inheritdoc/>
    public virtual async Task<string[]> SourcesAsync() => await this._jsInstance.InvokeAsync<string[]>("sources");
    /// <inheritdoc/>
    public virtual async Task<string[]?> SuccessorsAsync(string v) => await this._jsInstance.InvokeAsync<string[]?>("successors", v);

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            (this._jsInstance as IDisposable)?.Dispose();
        }
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (this._jsInstance is not null)
        {
            await this._jsInstance.DisposeAsync().ConfigureAwait(false);
        }
    }

}
