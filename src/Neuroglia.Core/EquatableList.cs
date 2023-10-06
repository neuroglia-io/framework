using System.Collections;
using System.Runtime.Serialization;

namespace Neuroglia;

/// <summary>
/// Represents an <see cref="IList{T}"/> 
/// </summary>
/// <typeparam name="T">The type of the items the list is made out of</typeparam>
[CollectionDataContract]
public record EquatableList<T>
    : IList<T>
{

    /// <summary>
    /// Initializes a new <see cref="EquatableList{T}"/>
    /// </summary>
    public EquatableList() : this(new List<T>()) { }

    /// <summary>
    /// Initializes a new <see cref="EquatableList{T}"/>
    /// </summary>
    /// <param name="items">The underlying <see cref="EquatableList{T}"/> that contains the items the <see cref="EquatableList{T}"/> is made out of</param>
    public EquatableList(IEnumerable<T> items)
    {
        this.Items = items.ToList();
    }

    /// <summary>
    /// Gets the underlying <see cref="EquatableList{T}"/> that contains the items the <see cref="EquatableList{T}"/> is made out of
    /// </summary>
    protected List<T> Items { get; set; }

    /// <inheritdoc/>
    public virtual T this[int index]
    {
        get
        {
            return this.Items[index];
        }
        set
        {
            this.Items[index] = value;
        }
    }

    /// <inheritdoc/>
    public virtual int Count => this.Items.Count;

    /// <inheritdoc/>
    public virtual bool IsReadOnly => ((IList<T>)this.Items).IsReadOnly;

    /// <inheritdoc/>
    public virtual void Add(T item) => this.Items.Add(item);

    /// <inheritdoc/>
    public virtual bool Contains(T item) => this.Items.Contains(item);

    /// <inheritdoc/>
    public virtual void CopyTo(T[] array, int arrayIndex) => this.Items.CopyTo(array, arrayIndex);

    /// <inheritdoc/>
    public virtual IEnumerator<T> GetEnumerator() => this.Items.GetEnumerator();

    /// <inheritdoc/>
    public virtual int IndexOf(T item) => this.Items.IndexOf(item);

    /// <inheritdoc/>
    public virtual void Insert(int index, T item) => this.Items.Insert(index, item);

    /// <inheritdoc/>
    public virtual bool Remove(T item) => this.Items.Remove(item);

    /// <inheritdoc/>
    public virtual void RemoveAt(int index) => this.Items.RemoveAt(index);

    /// <inheritdoc/>
    public virtual void Clear() => this.Items.Clear();

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    /// <inheritdoc />
    public override int GetHashCode()
    {
        int someHashValue = -234897289;
        foreach (var item in this.Items)
        {
            someHashValue ^= item!.GetHashCode();
        }
        return someHashValue;
    }

    /// <inheritdoc/>
    public virtual bool Equals(EquatableList<T>? other)
    {
        if (other == null || other.Count != this.Count) return false;
        for (int i = 0; i < this.Count; i++)
        {
            var record = other[i];
            if (record == null || !record.Equals(this[i])) return false;
        }
        return true;
    }

}
