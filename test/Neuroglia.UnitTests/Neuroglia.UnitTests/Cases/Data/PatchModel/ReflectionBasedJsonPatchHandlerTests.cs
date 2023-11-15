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

using Json.Patch;
using Json.Pointer;
using Microsoft.Extensions.DependencyInjection;
using Neuroglia.Data;
using Neuroglia.Data.PatchModel;
using Neuroglia.Data.PatchModel.Services;
using Neuroglia.Serialization.Json;

namespace Neuroglia.UnitTests.Cases.Data.PatchModel;

public class ReflectionBasedJsonPatchHandlerTests
{

    protected IPatchHandler PatchHandler { get; } = new ReflectionBasedJsonPatchHandler(new ServiceCollection().BuildServiceProvider());

    [Fact]
    public async Task Patch_Aggregate_Should_Work()
    {
        //arrange
        var tagToRemove = "Old Fake Tag To Remove";
        var product = new ProductAggregate("Fake Name", "Fake Description", tags: new string[] { tagToRemove });
        var updatedName = "Updated Fake Product Name";
        var updatedDescription = "Updated Fake Product Description";
        var newTag = "New Fake Tag";
        var patchOperations = new List<PatchOperation>()
        {
            PatchOperation.Replace(JsonPointer.Create<ProductState>(p => p.Name), JsonSerializer.Default.SerializeToNode(updatedName)),
            PatchOperation.Replace(JsonPointer.Create<ProductState>(p => p.Description!), JsonSerializer.Default.SerializeToNode(updatedDescription)),
            PatchOperation.Add(JsonPointer.Create<ProductState>(p => p.Tags), JsonSerializer.Default.SerializeToNode(newTag)),
            PatchOperation.Remove(JsonPointer.Create<ProductState>(p => p.Tags).Combine(JsonPointer.Parse("/0"))),
            PatchOperation.Test(JsonPointer.Create<ProductState>(p => p.StateVersion), JsonSerializer.Default.SerializeToNode(0))
        };
        var patch = new JsonPatch(patchOperations);

        //act
        product = await this.PatchHandler.ApplyPatchAsync(patch, product);

        //assert
        product!.State.Name.Should().Be(updatedName);
        product.State.Description.Should().Be(updatedDescription);
        product.State.Tags.Should().ContainSingle();
        product.State.Tags.Should().Contain(newTag);
    }

    class Product
    {

        public Product()
        {
            this.Name = "Unamed";
            this._activeTags.Add("active");
            this.Contacts.Add("contact-to-remove-1");
            this.Contacts.Add("contact-to-remove-2");
            this.Contacts.Add("contact-to-remove-3");
        }

        public string Name { get; protected set; } = null!;

        public string? Description { get; set; }

        private List<Category> _categories = new();
        public IReadOnlyCollection<Category> Categories => this._categories.AsReadOnly();

        private List<string> _activeTags = new();
        public IReadOnlyCollection<string> ActiveTags => this._activeTags.AsReadOnly();

        private List<string> _retiredTags = new();
        public IReadOnlyCollection<string> RetiredTags => this._retiredTags.AsReadOnly();

        public List<string> Contacts { get; set; } = new();

        public List<string>? ContactsToRemove { get; set; }

        [JsonPatchOperation(JsonPatchOperationType.Add | JsonPatchOperationType.Replace, nameof(Name))]
        public virtual void SetName(string name)
        {
            this.Name = name;
        }

        [JsonPatchOperation(JsonPatchOperationType.Add, nameof(Categories))]
        public virtual void AddCategory(Category category, int? index = null)
        {
            if (index.HasValue) this._categories.Insert(index.Value, category);
            else this._categories.Add(category);
        }

        [JsonPatchOperation(JsonPatchOperationType.Replace, nameof(Categories))]
        public virtual void ReplaceCategory(Category category, int index)
        {

        }

        [JsonPatchOperation(JsonPatchOperationType.Remove, nameof(Categories))]
        public virtual void RemoveCategory(int index)
        {
            this._categories.RemoveAt(index);
        }

        [JsonPatchOperation(JsonPatchOperationType.Copy, nameof(ActiveTags))]
        public void AddRetiredTags(IEnumerable<string>? tags)
        {
            if (tags == null) return;
            this._retiredTags.AddRange(tags);
        }

    }

    class Category
        : Entity<string>
    {

        public Category(string name)
            : base(Guid.NewGuid().ToString("N"))
        {
            this.Name = name;
        }

        public string Name { get; set; }

    }

    class ProductAggregate
        : AggregateRoot<string, ProductState>
    {

        public ProductAggregate(string name, string? description = null, IEnumerable<string>? tags = null)
        {
            this.State.Name = name;
            this.State.Description = description;
            if (tags != null) this.State._tags = tags.ToList();
        }

        [JsonPatchOperation(JsonPatchOperationType.Replace, nameof(ProductState.Name))]
        public void SetName(string name)
        {
            this.State.Name = name;
        }

        [JsonPatchOperation(JsonPatchOperationType.Replace, nameof(ProductState.Description))]
        public void SetDescription(string description)
        {
            this.State.Description = description;
        }

        [JsonPatchOperation(JsonPatchOperationType.Add, nameof(ProductState.Categories), ReferencedType = typeof(Category))]
        public void AddCategory(Category category)
        {
            this.State._categories.Add(category.Id);
        }

        [JsonPatchOperation(JsonPatchOperationType.Add, nameof(ProductState.Categories), ReferencedType = typeof(Category))]
        public void RemoveCategory(int index)
        {
            this.State._categories.RemoveAt(index);
        }

        [JsonPatchOperation(JsonPatchOperationType.Add, nameof(ProductState.Tags))]
        public void AddTag(string tag, int? index = null)
        {
            if (index.HasValue) this.State._tags.Insert(index.Value, tag);
            else this.State._tags.Add(tag);
        }

        [JsonPatchOperation(JsonPatchOperationType.Remove, nameof(ProductState.Tags))]
        public void RemoveTag(int index)
        {
            this.State._tags.RemoveAt(index);
        }

    }

    record ProductState
        : AggregateState<string>
    {

        public string Name { get; internal protected set; }

        public string? Description { get; internal protected set; }

        internal List<string> _categories = new();
        public IReadOnlyCollection<string> Categories => this._categories.AsReadOnly();

        internal List<string> _tags = new();
        public IReadOnlyCollection<string> Tags => this._tags.AsReadOnly();

    }

}
