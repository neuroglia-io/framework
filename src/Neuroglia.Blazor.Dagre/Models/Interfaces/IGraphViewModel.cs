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

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Neuroglia.Blazor.Dagre.Models;

public delegate Task MouseEventHandler(GraphEventArgs<MouseEventArgs> e);
public delegate Task WheelEventHandler(GraphEventArgs<WheelEventArgs> e);

public interface IGraphViewModel
    : IIdentifiable, ILabeled, IDimension, IPosition, ICssClass, IMetadata
{
    decimal Scale { get; set; }
    bool EnableProfiling { get; set; }
    IReadOnlyDictionary<Guid, INodeViewModel> Nodes { get; }
    IReadOnlyDictionary<Guid, INodeViewModel> AllNodes { get; }
    IReadOnlyDictionary<Guid, IEdgeViewModel> Edges { get; }
    IReadOnlyDictionary<Guid, IClusterViewModel> Clusters { get; }
    IReadOnlyDictionary<Guid, IClusterViewModel> AllClusters { get; }
    IReadOnlyCollection<Type> SvgDefinitionComponents { get; }
    IGraphLib? DagreGraph {  get; set; }

    event MouseEventHandler? MouseMove;
    event MouseEventHandler? MouseDown;
    event MouseEventHandler? MouseUp;
    event MouseEventHandler? MouseEnter;
    event MouseEventHandler? MouseLeave;
    event WheelEventHandler? Wheel;
    event Action? Changed;

    Task RegisterComponentTypeAsync<TElement, TComponent>()
        where TElement : IGraphElement
        where TComponent : ComponentBase;

    Type GetComponentTypeAsync<TElement>(TElement node)
        where TElement : IGraphElement;

    Task AddElementAsync(IGraphElement element);

    Task AddElementsAsync(IEnumerable<IGraphElement> elements);

    Task OnMouseMoveAsync(ElementReference sender, MouseEventArgs e, IGraphElement? element);

    Task OnMouseDownAsync(ElementReference sender, MouseEventArgs e, IGraphElement? element);

    Task OnMouseUpAsync(ElementReference sender, MouseEventArgs e, IGraphElement? element);

    Task OnMouseEnterAsync(ElementReference sender, MouseEventArgs e, IGraphElement? element);

    Task OnMouseLeaveAsync(ElementReference sender, MouseEventArgs e, IGraphElement? element);

    Task OnWheelAsync(ElementReference sender, WheelEventArgs e, IGraphElement? element);

}
