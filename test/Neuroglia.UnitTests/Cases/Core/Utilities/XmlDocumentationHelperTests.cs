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

namespace Neuroglia.UnitTests.Cases.Core.Utilities;

public class XmlDocumentationHelperTests
{

    [Fact]
    public void Get_Summary_Of_Type_Should_Work()
    {
        //act
        var summary = XmlDocumentationHelper.SummaryOf(typeof(User));

        //assert
        summary.Should().Be("Description of documented type with a Code Reference and an Hyperlink Reference (http://google.com)");
    }

    [Fact]
    public void Get_Summary_Of_Property_Should_Work()
    {
        //act
        var summary = XmlDocumentationHelper.SummaryOf(typeof(User).GetProperty(nameof(User.Address))!);

        //assert
        summary.Should().Be("Gets the user's address");
    }

    [Fact]
    public void Get_Summary_Of_Method_Should_Work()
    {
        //act
        var summary = XmlDocumentationHelper.SummaryOf(typeof(User).GetMethod(nameof(User.Login))!);

        //assert
        summary.Should().Be("Logs the user in");
    }

    [Fact]

    public void Get_Summary_Of_Parameter_Should_Work()
    {
        //act
        var summary = XmlDocumentationHelper.SummaryOf(typeof(User).GetMethod(nameof(User.Login))!.GetParameters()[0]);

        //assert
        summary.Should().Be("The scheme to use to log the user in");
    }

    /// <summary>
    /// Description of documented type with a <see cref="Address">Code Reference</see> and an <see href="http://google.com">Hyperlink Reference</see>
    /// </summary>
    class User
    {

        /// <summary>
        /// Gets the <see cref="User"/>'s <see cref="Address"/>
        /// </summary>
        public Address? Address { get; }

        /// <summary>
        /// Logs the <see cref="User"/> in
        /// </summary>
        /// <param name="scheme">The scheme to use to log the <see cref="User"/> in</param>
        public void Login(string scheme) => throw new NotImplementedException();

    }

}