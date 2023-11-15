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
/// Represents a <see cref="NsmTree"/> node
/// </summary>
public class NsmTreeNode<T>
{

    /// <summary>
    /// Initializes a new <see cref="NsmTreeNode{T}"/>
    /// </summary>
    /// <param name="left">The <see cref="NsmTreeNode{T}"/>'s left number</param>
    /// <param name="right">The <see cref="NsmTreeNode{T}"/>'s right number</param>
    /// <param name="depth">The <see cref="NsmTreeNode{T}"/>'s depth</param>
    /// <param name="lineage">The <see cref="NsmTreeNode{T}"/>'s lineage, which is a concatenation of the hexadecimal representation of the node's anecestors left</param>
    /// <param name="value">The <see cref="NsmTreeNode{T}"/>'s value</param>
    public NsmTreeNode(uint left, uint right, uint depth, string? lineage, T value, NsmTreeNode<T>? parent = null)
    {
        this.Left = left;
        this.Right = right;
        this.Depth = depth;
        this.Lineage = lineage;
        this.Value = value;
        this.Parent = parent;
    }

    /// <summary>
    /// Gets the <see cref="NsmTreeNode{T}"/>'s left number
    /// </summary>
    public virtual uint Left { get; protected set; }

    /// <summary>
    /// Gets the <see cref="NsmTreeNode{T}"/>'s right number
    /// </summary>
    public virtual uint Right { get; protected set; }

    /// <summary>
    /// Gets the <see cref="NsmTreeNode{T}"/>'s depth
    /// </summary>
    public virtual uint Depth { get; protected set; }

    /// <summary>
    /// Gets the <see cref="NsmTreeNode{T}"/>'s lineage, which is a concatenation of the hexadecimal representation of the node's anecestors left
    /// </summary>
    public virtual string? Lineage { get; protected set; }

    /// <summary>
    /// Gets the <see cref="NsmTreeNode{T}"/>'s value
    /// </summary>
    public virtual T Value { get; protected set; }

    /// <summary>
    /// Gets the <see cref="NsmTreeNode{T}"/>'s parent, if any
    /// </summary>
    public virtual NsmTreeNode<T>? Parent { get; protected set; }

    /// <summary>
    /// Gets a value indicating whether or not the <see cref="NsmTreeNode{T}"/> is read-only
    /// </summary>
    public virtual bool ReadOnly { get; protected set; }

    protected List<NsmTreeNode<T>>? _childNodes;
    /// <summary>
    /// Gets a list containing the node's children, if any
    /// </summary>
    public virtual IReadOnlyCollection<NsmTreeNode<T>>? ChildNodes => this._childNodes?.AsReadOnly();

    /// <summary>
    /// Appends the specified <see cref="NsmTreeNode{T}"/>
    /// </summary>
    /// <param name="node">The <see cref="NsmTreeNode{T}"/> to append</param>
    public virtual void Append(NsmTreeNode<T> node)
    {
        if (node == null) throw new ArgumentNullException(nameof(node));
        if (this.ReadOnly) throw new InvalidOperationException("Cannot update a node that is read-only");

        this._childNodes ??= new();
        this._childNodes.Add(node);

        this.Right = node.Right + 1;
        if(this.Parent != null) this.Parent.Right = this.Right + 1;
    }

    /// <summary>
    /// Freezes the <see cref="NsmTreeNode{T}"/> by marking it as read-only
    /// </summary>
    public virtual void Freeze() => this.ReadOnly = true;

    /// <summary>
    /// Flattens the <see cref="NsmTree{T}"/>
    /// </summary>
    /// <returns>A new <see cref="IEnumerable{T}"/> containing all the <see cref="NsmTreeNode{T}"/> the tree is made out of</returns>
    public virtual IEnumerable<NsmTreeNode<T>> Flatten()
    {
        yield return this;
        if (this.ChildNodes == null) yield break;
        foreach (var child in ChildNodes.SelectMany(n => n.Flatten())) yield return child;
    }

}