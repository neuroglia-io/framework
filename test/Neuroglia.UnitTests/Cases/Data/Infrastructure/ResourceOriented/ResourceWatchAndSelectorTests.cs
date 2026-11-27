// Copyright © 2021-Present Neuroglia SRL. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Neuroglia.Data.Infrastructure.ResourceOriented;
using Neuroglia.Data.PatchModel.Services;
using System.Reactive.Subjects;

namespace Neuroglia.UnitTests.Cases.Data.Infrastructure.ResourceOriented;

/// <summary>
/// Covers the disposal of typed watches, the formatting of label selectors and the strategic merge patch handler
/// </summary>
public class ResourceWatchAndSelectorTests
{

    sealed class DisposableSubject
        : IObservable<IResourceWatchEvent>, IDisposable
    {

        readonly Subject<IResourceWatchEvent> _subject = new();

        public bool Disposed { get; private set; }

        public IDisposable Subscribe(IObserver<IResourceWatchEvent> observer) => this._subject.Subscribe(observer);

        public void Dispose() => this.Disposed = true;

    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task A_Typed_Watch_Should_Dispose_The_Watch_It_Converts_Only_If_It_Owns_It(bool owned)
    {
        //arrange
        var asynchronous = new DisposableSubject();
        var synchronous = new DisposableSubject();

        //act
        await new ResourceWatch<FakeNamespacedResource>(asynchronous, owned).DisposeAsync();
        new ResourceWatch<FakeNamespacedResource>(synchronous, owned).Dispose();

        //assert
        asynchronous.Disposed.Should().Be(owned);
        synchronous.Disposed.Should().Be(owned);
    }

    [Theory]
    [InlineData(LabelSelectionOperator.Contains, null, null, "tier")]
    [InlineData(LabelSelectionOperator.NotContains, null, null, "!tier")]
    [InlineData(LabelSelectionOperator.Contains, null, "web,api", "tier in (web,api)")]
    [InlineData(LabelSelectionOperator.NotContains, null, "web", "tier notin (web)")]
    [InlineData(LabelSelectionOperator.Contains, "web", null, "tier in (web)")]
    [InlineData(LabelSelectionOperator.Equals, "web", null, "tier=web")]
    public void A_Label_Selector_Should_Be_Formatted_As_Kubernetes_Does(LabelSelectionOperator selectionOperator, string? value, string? values, string expected)
    {
        var selector = new LabelSelector() { Key = "tier", Operator = selectionOperator, Value = value, Values = values == null ? null : [.. values.Split(',')] };
        selector.ToString().Should().Be(expected);
    }

    [Fact]
    public async Task A_Strategic_Merge_Patch_Should_Keep_The_Fields_Of_The_Target_It_Does_Not_Set()
    {
        //arrange
        var target = new FakeResourceSpec() { FakeProperty1 = "kept", FakeProperty2 = 1 };

        //act
        var patched = await new JsonStrategicMergePatchHandler().ApplyPatchAsync(new { fakeProperty2 = 2 }, target);

        //assert
        patched!.FakeProperty1.Should().Be("kept");
        patched.FakeProperty2.Should().Be(2);
    }

}
