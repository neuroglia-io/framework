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

using Neuroglia.Data;

namespace Neuroglia.UnitTests.Cases.Data;

public class NsmTreeTests
{

    [Fact]
    public void Build_Tree_Should_Work()
    {
        //arrange
        var categories = BuildCategories();

        //act
        var tree = NsmTree<Category>.BuildFor(categories.Where(c => string.IsNullOrWhiteSpace(c.ParentId)), c => GetChildrenOf(c, categories));

        //assert
        tree.Should().NotBeNull();
        tree.RootNodes.Should().ContainSingle();
    }

    [Fact]
    public void Flatten_Tree_Should_Work()
    {
        //arrange
        var categories = BuildCategories();
        var tree = NsmTree<Category>.BuildFor(categories.Where(c => string.IsNullOrWhiteSpace(c.ParentId)), c => GetChildrenOf(c, categories));

        //act
        var nodes = tree.Flatten();

        //assert
        nodes.Should().NotBeNull();
        nodes.Should().HaveSameCount(categories);
        nodes!.Select(n => n.Value).Should().BeEquivalentTo(categories);
    }

    [Fact]
    public void Find_Node_Ancestor_Should_Work()
    {
        //arrange
        var categories = BuildCategories();
        var tree = NsmTree<Category>.BuildFor(categories.Where(c => string.IsNullOrWhiteSpace(c.ParentId)), c => GetChildrenOf(c, categories));
        var nodes = tree.Flatten()!;
        var fishNode = nodes.First(c => c.Value.Id == "fish");

        //act
        var ancestors = nodes.Where(n => !string.IsNullOrWhiteSpace(n.Lineage) && n.Lineage.StartsWith(fishNode.Lineage![..((int)(fishNode.Depth - 1) * 8)]) && n.Left < fishNode.Left && n.Right > fishNode.Right);

        //assert
        ancestors.Should().NotBeNull();
        ancestors.Should().ContainSingle();
        ancestors.First().Value.Id.Should().Be(fishNode.Value.ParentId);
    }

    [Fact]
    public void Find_Node_Ancestors_Should_Work()
    {
        //arrange
        var categories = BuildCategories();
        var tree = NsmTree<Category>.BuildFor(categories.Where(c => string.IsNullOrWhiteSpace(c.ParentId)), c => GetChildrenOf(c, categories));
        var nodes = tree.Flatten()!;
        var node = nodes.First(c => c.Value.Id == "fish");

        //act
        var ancestors = nodes.Where(n => (string.IsNullOrWhiteSpace(n.Lineage) || n.Lineage.StartsWith(node.Lineage![..8])) && n.Left < node.Left && n.Right > node.Right);

        //assert
        ancestors.Should().NotBeNull();
        ancestors.Should().HaveCount((int)node.Depth);
    }

    [Fact]
    public void Find_Node_Direct_Descendants_Should_Work()
    {
        //arrange
        var categories = BuildCategories();
        var tree = NsmTree<Category>.BuildFor(categories.Where(c => string.IsNullOrWhiteSpace(c.ParentId)), c => GetChildrenOf(c, categories));
        var nodes = tree.Flatten()!;
        var node = nodes.First(c => c.Value.Id == "food");

        //act
        var descendants = nodes.Where(n => !string.IsNullOrWhiteSpace(n.Lineage) && n.Lineage.Length / 8 == node.Depth + 1 && n.Left > node.Left && n.Right < node.Right);

        //assert
        descendants.Should().NotBeNull();
        descendants.Should().HaveSameCount(categories.Where(c => c.ParentId == node.Value.Id));
    }

    [Fact]
    public void Find_Node_All_Descendants_Should_Work()
    {
        //arrange
        var categories = BuildCategories();
        var tree = NsmTree<Category>.BuildFor(categories.Where(c => string.IsNullOrWhiteSpace(c.ParentId)), c => GetChildrenOf(c, categories));
        var nodes = tree.Flatten()!;
        var node = nodes.First(c => c.Value.Id == "food");

        //act
        var descendants = nodes.Where(n => !string.IsNullOrWhiteSpace(n.Lineage) && n.Lineage.StartsWith(node.Lineage!) && n.Left > node.Left && n.Right < node.Right);

        //assert
        descendants.Should().NotBeNull();
    }

    static IEnumerable<Category> GetChildrenOf(Category category, IEnumerable<Category> categories) => categories.Where(c => c.ParentId == category.Id);

    static IEnumerable<Category> BuildCategories()
    {
        var products = new Category("products");
        var drinks = new Category("drinks", products.Id);
        var soda = new Category("soda", drinks.Id);
        var juices = new Category("juices", drinks.Id);
        var freshJuices = new Category("fresh juices", juices.Id);
        var bottledJuices = new Category("bottled juices", juices.Id);
        var waters = new Category("waters", drinks.Id);
        var alcohol = new Category("alcohol", drinks.Id);
        var food = new Category("food", products.Id);
        var starters = new Category("starters", food.Id);
        var mainCourses = new Category("main courses", food.Id);
        var meat = new Category("meat", mainCourses.Id);
        var fish = new Category("fish", mainCourses.Id);
        var vegetarian = new Category("vegetarian", mainCourses.Id);
        var desserts = new Category("desserts", food.Id);
        return new List<Category>() { products, drinks, soda, juices, freshJuices, bottledJuices, waters, alcohol, food, starters, mainCourses, meat, fish, vegetarian, desserts };
    }

    class Category
    {

        public Category(string id, string? parentId = null)
        {
            this.Id = id;
            this.ParentId = parentId;
        }

        public string Id { get; }

        public string? ParentId { get; }

    }

}
