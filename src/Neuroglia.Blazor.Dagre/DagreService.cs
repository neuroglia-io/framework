// Copyright © 2021-Present Neuroglia SRL. All rights reserved.
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
/// Wraps Dagre js library as an injectable service
/// https://github.com/dagrejs/dagre
/// </summary>
/// <remarks>
/// Creates a new instance of DagreService
/// </remarks>
/// <param name="jSRuntime"></param>
public class DagreService(IJSRuntime jSRuntime)
    : IDagreService
{
    /// <summary>
    /// The JS Runtime instance
    /// </summary>
    protected readonly IJSRuntime jsRuntime = jSRuntime;

    /// <summary>
    /// Computes the nodes and edges position of the provided <see cref="IGraphViewModel"/>
    /// </summary>
    /// <param name="graphViewModel"></param>
    /// <param name="options"></param>
    /// <returns>The updated <see cref="IGraphViewModel"/></returns>
    public virtual async Task<IGraphViewModel> ComputePositionsAsync(IGraphViewModel graphViewModel, IDagreGraphOptions? options = null)
    {
        ProfilingTimer? profilingTimer = null;
        if (graphViewModel.EnableProfiling)
        {
            profilingTimer = new("ComputePositionsAsync");
            profilingTimer.Start();
        }
        // build the dagre/graphlib graph
        var graph = await this.GraphAsync(options);
        var nodes = graphViewModel.AllNodes.Values.Concat( graphViewModel.AllClusters.Values);
        foreach (var node in nodes)
        {
            await graph.SetNodeAsync(node.Id.ToString(), node);
            if (node.ParentId != null) await graph.SetParentAsync(node.Id.ToString(), node.ParentId.ToString()!);
        }
        foreach(var edge in graphViewModel.Edges.Values)
        {
            if (options?.Multigraph == true) await graph.SetEdgeAsync(edge.SourceId.ToString(), edge.TargetId.ToString(), edge, edge.Id.ToString());
            else await graph.SetEdgeAsync(edge.SourceId.ToString(), edge.TargetId.ToString(), edge);
        }
        await this.LayoutAsync(graph);
        // update our view models with the computed values
        foreach (var node in nodes)
        {
            var graphNode = await graph.NodeAsync(node.Id.ToString());
            node.SetGeometry(graphNode.X, graphNode.Y, graphNode.Width, graphNode.Height);
        }
        foreach (var edge in graphViewModel.Edges.Values)
        {
            GraphLibEdge graphEdge;
            if (options?.Multigraph == true)  graphEdge = await graph.EdgeAsync(edge.SourceId.ToString(), edge.TargetId.ToString(), edge.Id.ToString());
            else graphEdge = await graph.EdgeAsync(edge.SourceId.ToString(), edge.TargetId.ToString());
            if (graphEdge?.Points != null) edge.Points = [.. graphEdge.Points];
        }
        graphViewModel.DagreGraph = graph;
        if (profilingTimer != null)
        {
            profilingTimer.Stop();
            profilingTimer.Dispose();
        }
        return graphViewModel;
    }

    /// <inheritdoc/>
    public virtual async Task<IGraphLib> DeserializeAsync(string json) => await this.jsRuntime.InvokeAsync<IGraphLib>("neuroglia.blazor.dagre.read", json);

    /// <summary>
    /// Returns a new <see cref="IGraphLib"/> instance
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public virtual async Task<IGraphLib> GraphAsync(IDagreGraphOptions? options = null)
    {
        var graphLibOptions = new GraphLibOptions(options);
        graphLibOptions.Multigraph ??= true;
        graphLibOptions.Compound ??= true;
        var jsInstance = await this.jsRuntime.InvokeAsync<IJSObjectReference>("neuroglia.blazor.dagre.graph", graphLibOptions);
        var graph = new GraphLib(jsInstance);
        var dagreGraphConfig = new DagreGraphConfig(options);
        dagreGraphConfig.Direction ??= DagreGraphDirection.LeftToRight;
        await graph.SetGraphAsync(dagreGraphConfig);
        return graph;
    }

    /// <summary>
    /// Computes the graph layout
    /// </summary>
    /// <param name="graph"></param>
    /// <returns></returns>
    public virtual async Task<IGraphLib?> LayoutAsync(IGraphLib graph) => await this.jsRuntime.InvokeAsync<IJSObjectReference>("neuroglia.blazor.dagre.layout", await graph.InstanceAsync()) as IGraphLib;
    
    /// <inheritdoc/>
    public virtual async Task<string> SerializeAsync(IGraphLib graph) => await this.jsRuntime.InvokeAsync<string>("neuroglia.blazor.dagre.write", await graph.InstanceAsync());

}