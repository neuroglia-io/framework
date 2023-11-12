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

namespace Neuroglia.Data;

/// <summary>
/// Represents a <see href="https://en.wikipedia.org/wiki/Nested_set_model">Nested Set Model (NSM)</see> tree
/// </summary>
/// <typeparam name="T">The type of data to build a new <see cref="NsmTree{T}"/> for</typeparam>
public class NsmTree<T>
{

    protected List<NsmTreeNode<T>>? _rootNodes;
    /// <summary>
    /// Gets a list containing the tree's root nodes, if any
    /// </summary>
    public virtual IReadOnlyCollection<NsmTreeNode<T>>? RootNodes => this._rootNodes?.AsReadOnly();

    /// <summary>
    /// Flattens the <see cref="NsmTree{T}"/>
    /// </summary>
    /// <returns>A new <see cref="IEnumerable{T}"/> containing all the <see cref="NsmTreeNode{T}"/> the tree is made out of</returns>
    public virtual IEnumerable<NsmTreeNode<T>>? Flatten() => this.RootNodes?.SelectMany(n => n.Flatten());

    /// <summary>
    /// Builds a new <see cref="NsmTree{T}"/> using the specified hierarchy root items
    /// </summary>
    /// <param name="rootItems">The root items of the hierarchy to build a new <see cref="NsmTree{T}"/> for</param>
    /// <param name="childrenResolver">A function used to resolve the children of a given item</param>
    /// <returns>A new <see cref="NsmTree{T}"/></returns>
    public static NsmTree<T> BuildFor(IEnumerable<T> rootItems, Func<T, IEnumerable<T>> childrenResolver)
    {
        if (rootItems == null || !rootItems.Any()) throw new ArgumentNullException(nameof(rootItems));
        if (childrenResolver == null) throw new ArgumentNullException(nameof(childrenResolver));

        var rootNode = new NsmTreeNode<T>(0, 0, 0, null, default!);
        Populate(rootNode, rootItems, childrenResolver);

        var tree = new NsmTree<T> { _rootNodes = rootNode.ChildNodes?.ToList() };
        return tree;
    }

    /// <summary>
    /// Populates a <see cref="NsmTreeNode{T}"/> using the specified collection of items
    /// </summary>
    /// <param name="parent">The <see cref="NsmTreeNode{T}"/> to populate</param>
    /// <param name="items">The collection of items to populate the <see cref="NsmTreeNode{T}"/> with</param>
    /// <param name="childrenResolver">A function used to resolve the children of a given item</param>
    protected static void Populate(NsmTreeNode<T> parent, IEnumerable<T>? items, Func<T, IEnumerable<T>> childrenResolver)
    {
        if (parent == null) throw new ArgumentNullException(nameof(parent));
        if (childrenResolver == null) throw new ArgumentNullException(nameof(childrenResolver));
        if (items == null || !items.Any()) return;

        var lastRight = parent.Left;
        foreach (var item in items)
        {
            var left = lastRight + 1;
            var right = left + 1;
            var depth = parent.Left == 0 ? 0 : parent.Depth + 1;
            var lineage = depth == 0 ? null : parent.Lineage + parent.Left.ToString("X8");
            var node = new NsmTreeNode<T>(left, right, depth, lineage, item, parent);
            parent.Append(node);
            Populate(node, childrenResolver.Invoke(item), childrenResolver);
            lastRight = node.Right;
        }
        parent.Freeze();

    }

}
