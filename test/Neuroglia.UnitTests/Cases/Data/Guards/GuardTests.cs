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

using Neuroglia.Data.Guards;

namespace Neuroglia.UnitTests.Cases.Data.Guards;

public class GuardTests
{

    [Fact]
    public void Guard_Against_Object_WhenNull_Should_Work()
    {
        //arrange
        var guardAgainstNullValue = () => Guard.Against((object)null!).WhenNull();
        var guardAgainstValidValue = () => Guard.Against(new{ }).WhenNull();

        //act
        guardAgainstNullValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Object_WhenNotNull_Should_Work()
    {
        //arrange
        var guardAgainstNullValue = () => Guard.Against((object)null!).WhenNotNull();
        var guardAgainstValidValue = () => Guard.Against(new { }).WhenNotNull();

        //act
        guardAgainstNullValue.Should().NotThrow();
        guardAgainstValidValue.Should().Throw<GuardException>();
    }

    [Fact]
    public void Guard_Against_Object_WhenNullReference_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against((object?)null).WhenNullReference("fake-id");
        var guardAgainstValidValue = () => Guard.Against(new { }).WhenNullReference("fake-id");

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Guid_WhenEmpty_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against(Guid.Empty).WhenEmpty();
        var guardAgainstValidValue = () => Guard.Against(Guid.NewGuid()).WhenEmpty();

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_TimeSpan_WhenLowerThan_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against(TimeSpan.FromSeconds(30)).WhenLowerThan(TimeSpan.FromMinutes(1));
        var guardAgainstValidValue = () => Guard.Against(TimeSpan.FromMinutes(1)).WhenLowerThan(TimeSpan.FromSeconds(30));

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_TimeSpan_WhenLowerOrEqualTo_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against(TimeSpan.FromMinutes(1)).WhenLowerOrEqualsTo(TimeSpan.FromSeconds(60));
        var guardAgainstValidValue = () => Guard.Against(TimeSpan.FromMinutes(2)).WhenLowerOrEqualsTo(TimeSpan.FromSeconds(60));

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_TimeSpan_WhenHigherThan_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against(TimeSpan.FromMinutes(1)).WhenHigherThan(TimeSpan.FromSeconds(30));
        var guardAgainstValidValue = () => Guard.Against(TimeSpan.FromSeconds(1)).WhenHigherThan(TimeSpan.FromSeconds(30));

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_TimeSpan_WhenHigherOrEqualTo_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against(TimeSpan.FromSeconds(30)).WhenHigherOrEqualsTo(TimeSpan.FromSeconds(30));
        var guardAgainstValidValue = () => Guard.Against(TimeSpan.FromSeconds(10)).WhenHigherOrEqualsTo(TimeSpan.FromSeconds(30));

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_DateTime_WhenBefore_Should_Work()
    {
        //arrange
        var before = DateTime.Now;
        var guardAgainstInvalidValue = () => Guard.Against(before.Subtract(TimeSpan.FromMinutes(1))).WhenBefore(before);
        var guardAgainstValidValue = () => Guard.Against(before).WhenBefore(before);

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_DateTime_WhenAfter_Should_Work()
    {
        //arrange
        var before = DateTime.Now;
        var guardAgainstInvalidValue = () => Guard.Against(before.Add(TimeSpan.FromMinutes(1))).WhenAfter(before);
        var guardAgainstValidValue = () => Guard.Against(before).WhenAfter(before);

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_String_WhenNullOrWhitespace_Should_Work()
    {
        //arrange
        var guardAgainstEmptyValue = () => Guard.Against(string.Empty).WhenNullOrWhitespace();
        var guardAgainstNullValue = () => Guard.Against((string)null!).WhenNullOrWhitespace();
        var guardAgainstWhitespaceValue = () => Guard.Against("  ").WhenNullOrWhitespace();
        var guardAgainstValidValue = () => Guard.Against("fake value").WhenNullOrWhitespace();

        //assert
        guardAgainstEmptyValue.Should().Throw<GuardException>();
        guardAgainstNullValue.Should().Throw<GuardException>();
        guardAgainstWhitespaceValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_String_WhenStartsWith_Should_Work()
    {
        //arrange
        var guardAgainstValidValue = () => Guard.Against("Fake value").WhenStartsWith("Fake");
        var guardAgainstInvalidValue = () => Guard.Against("value").WhenStartsWith("Fake");

        //assert
        guardAgainstValidValue.Should().Throw<GuardException>();
        guardAgainstInvalidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_String_WhenNotStartsWith_Should_Work()
    {
        //arrange
        var guardAgainstValidValue = () => Guard.Against("value").WhenNotStartsWith("fake");
        var guardAgainstInvalidValue = () => Guard.Against("Fake value").WhenNotStartsWith("Fake");

        //assert
        guardAgainstValidValue.Should().Throw<GuardException>();
        guardAgainstInvalidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_String_WhenEndsWith_Should_Work()
    {
        //arrange
        var guardAgainstValidValue = () => Guard.Against("Fake value").WhenEndsWith("value");
        var guardAgainstInvalidValue = () => Guard.Against("Fake").WhenEndsWith("value");

        //assert
        guardAgainstValidValue.Should().Throw<GuardException>();
        guardAgainstInvalidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_String_WhenNotEndsWith_Should_Work()
    {
        //arrange
        var guardAgainstValidValue = () => Guard.Against("Fake").WhenNotEndsWith("value");
        var guardAgainstInvalidValue = () => Guard.Against("Fake value").WhenNotEndsWith("value");

        //assert
        guardAgainstValidValue.Should().Throw<GuardException>();
        guardAgainstInvalidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_String_WhenContains_Should_Work()
    {
        //arrange
        var guardAgainstValidValue = () => Guard.Against("Fake value").WhenContains("Fake");
        var guardAgainstInvalidValue = () => Guard.Against("value").WhenContains("Fake");

        //assert
        guardAgainstValidValue.Should().Throw<GuardException>();
        guardAgainstInvalidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_String_WhenNotContains_Should_Work()
    {
        //arrange
        var guardAgainstValidValue = () => Guard.Against("value").WhenNotContains("Fake");
        var guardAgainstInvalidValue = () => Guard.Against("Fake value").WhenNotContains("Fake");

        //assert
        guardAgainstValidValue.Should().Throw<GuardException>();
        guardAgainstInvalidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Equatable_WhenEquals_Should_Work()
    {
        //arrange
        var guardAgainstEqualValue = () => Guard.Against("John").WhenEquals("John");
        var guardAgainstNonEqualValue = () => Guard.Against("John").WhenEquals("Jane");

        //act
        guardAgainstEqualValue.Should().Throw<GuardException>();
        guardAgainstNonEqualValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Equatable_WhenNotEquals_Should_Work()
    {
        //arrange
        var guardAgainstNonEqualValue = () => Guard.Against("John").WhenNotEquals("Jane");
        var guardAgainstEqualValue = () => Guard.Against("John").WhenNotEquals("John");

        //act
        guardAgainstNonEqualValue.Should().Throw<GuardException>();
        guardAgainstEqualValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Enumerable_WhenNullOrEmpty_Should_Work()
    {
        //arrange
        var guardAgainstEmptyValue = () => Guard.Against(string.Empty).WhenNullOrEmpty();
        var guardAgainstNullValue = () => Guard.Against((string)null!).WhenNullOrEmpty();
        var guardAgainstValidValue = () => Guard.Against("fake value").WhenNullOrEmpty();

        //assert
        guardAgainstEmptyValue.Should().Throw<GuardException>();
        guardAgainstNullValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Enumerable_WhenCountLowerThan_Should_Work()
    {
        //arrange
        var guardAgainstEmptyValue = () => Guard.Against(string.Empty).WhenCountLowerThan(10);
        var guardAgainstNullValue = () => Guard.Against((string)null!).WhenCountLowerThan(10);
        var guardAgainstTooShortValue = () => Guard.Against("fake valu").WhenCountLowerThan(10);
        var guardAgainstValidValue = () => Guard.Against("fake value").WhenCountLowerThan(10);

        //assert
        guardAgainstEmptyValue.Should().Throw<GuardException>();
        guardAgainstNullValue.Should().Throw<GuardException>();
        guardAgainstTooShortValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Enumerable_WhenCountHigherThan_Should_Work()
    {
        //arrange
        var guardAgainstTooLongValue = () => Guard.Against("fake value 123").WhenCountHigherThan(10);
        var guardAgainstValidValue = () => Guard.Against("fake value").WhenCountHigherThan(10);

        //assert
        guardAgainstTooLongValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Enumerable_WhenCountEquals_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against("fake value").WhenCountEquals(10);
        var guardAgainstValidValue = () => Guard.Against("fake value 123").WhenCountEquals(10);

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Enumerable_WhenCountNotEquals_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against("fake value 123").WhenCountNotEquals(10);
        var guardAgainstValidValue = () => Guard.Against("fake value").WhenCountNotEquals(10);

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Enumerable_WhenContains_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against(new int[] { 0, 1, 2, 3 }).WhenContains(0);
        var guardAgainstValidValue = () => Guard.Against(new int[] { 1, 2, 3 }).WhenContains(0);

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Enumerable_WhenNotContains_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against(new int[] { 1, 2, 3 }).WhenNotContains(0);
        var guardAgainstValidValue = () => Guard.Against(new int[] { 0, 1, 2, 3 }).WhenNotContains(0);

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Enumerable_WhenAny_Should_Work()
    {
        //arrange
        var predicate = (int i) => i == 0;
        var message = "Fake Message";
        var guardAgainstInvalidValue = () => Guard.Against(new int[] { 0, 1, 2, 3 }).WhenAny(predicate, message);
        var guardAgainstValidValue = () => Guard.Against(new int[] { 1, 2, 3 }).WhenAny(predicate, message);

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Enumerable_WhenNone_Should_Work()
    {
        //arrange
        var predicate = (int i) => i == 0;
        var message = "Fake Message";
        var guardAgainstInvalidValue = () => Guard.Against(new int[] { 1, 2, 3 }).WhenNone(predicate, message);
        var guardAgainstValidValue = () => Guard.Against(new int[] { 0, 1, 2, 3 }).WhenNone(predicate, message);

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Enumerable_WhenAll_Should_Work()
    {
        //arrange
        var predicate = (int i) => i < 10;
        var message = "Fake Message";
        var guardAgainstInvalidValue = () => Guard.Against(new int[] { 1, 2, 3 }).WhenAll(predicate, message);
        var guardAgainstValidValue = () => Guard.Against(new int[] { 1, 2, 3, 11 }).WhenAll(predicate, message);

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Enumerable_WhenNotAll_Should_Work()
    {
        //arrange
        var predicate = (int i) => i < 10;
        var message = "Fake Message";
        var guardAgainstInvalidValue = () => Guard.Against(new int[] { 1, 2, 3, 11 }).WhenNotAll(predicate, message);
        var guardAgainstValidValue = () => Guard.Against(new int[] { 0, 1, 2, 3 }).WhenNotAll(predicate, message);

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Boolean_WhenTrue_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against(true).WhenTrue();
        var guardAgainstValidValue = () => Guard.Against(false).WhenTrue();

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Boolean_WhenFalse_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against(false).WhenFalse();
        var guardAgainstValidValue = () => Guard.Against(true).WhenFalse();

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Integer_WhenNegative_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against((int)-1).WhenNegative();
        var guardAgainstValidValue = () => Guard.Against((int)0).WhenNegative();

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Integer_WhenPositive_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against((int)1).WhenPositive();
        var guardAgainstValidValue = () => Guard.Against((int)0).WhenPositive();

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Integer_WhenLowerThan_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against((int)0).WhenLowerThan(1);
        var guardAgainstValidValue = () => Guard.Against((int)1).WhenLowerThan(1);

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Integer_WhenLowerOrEqualTo_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against((int)0).WhenLowerOrEqualTo(0);
        var guardAgainstValidValue = () => Guard.Against((int)1).WhenLowerOrEqualTo(0);

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Integer_WhenHigherThan_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against((int)2).WhenHigherThan(1);
        var guardAgainstValidValue = () => Guard.Against((int)1).WhenHigherThan(1);

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Integer_WhenHigherOrEqual_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against((int)2).WhenHigherOrEqualTo(1);
        var guardAgainstValidValue = () => Guard.Against((int)0).WhenHigherOrEqualTo(1);

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Integer_WhenWithinRange_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against((int)1).WhenWithinRange(0, 10);
        var guardAgainstValidValue = () => Guard.Against((int)0).WhenWithinRange(0, 10);

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Integer_WhenNotWithinRange_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against((int)0).WhenNotWithinRange(1, 10);
        var guardAgainstValidValue = () => Guard.Against((int)1).WhenNotWithinRange(1, 10);

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Decimal_WhenNegative_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against((decimal)-1).WhenNegative();
        var guardAgainstValidValue = () => Guard.Against((decimal)0).WhenNegative();

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Decimal_WhenPositive_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against((decimal)1).WhenPositive();
        var guardAgainstValidValue = () => Guard.Against((decimal)0).WhenPositive();

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Decimal_WhenLowerThan_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against((decimal)0).WhenLowerThan(1);
        var guardAgainstValidValue = () => Guard.Against((decimal)1).WhenLowerThan(1);

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Decimal_WhenLowerOrEqualTo_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against((decimal)0).WhenLowerOrEqualTo(0);
        var guardAgainstValidValue = () => Guard.Against((decimal)1).WhenLowerOrEqualTo(0);

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Decimal_WhenHigherThan_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against((decimal)2).WhenHigherThan(1);
        var guardAgainstValidValue = () => Guard.Against((decimal)1).WhenHigherThan(1);

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Decimal_WhenHigherOrEqual_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against((decimal)2).WhenHigherOrEqualTo(1);
        var guardAgainstValidValue = () => Guard.Against((decimal)0).WhenHigherOrEqualTo(1);

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Decimal_WhenWithinRange_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against((decimal)1).WhenWithinRange(0, 10);
        var guardAgainstValidValue = () => Guard.Against((decimal)0).WhenWithinRange(0, 10);

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Decimal_WhenNotWithinRange_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against((decimal)0).WhenNotWithinRange(1, 10);
        var guardAgainstValidValue = () => Guard.Against((decimal)1).WhenNotWithinRange(1, 10);

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Double_WhenNegative_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against((double)-1).WhenNegative();
        var guardAgainstValidValue = () => Guard.Against((double)0).WhenNegative();

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Double_WhenPositive_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against((double)1).WhenPositive();
        var guardAgainstValidValue = () => Guard.Against((double)0).WhenPositive();

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Double_WhenLowerThan_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against((double)0).WhenLowerThan(1);
        var guardAgainstValidValue = () => Guard.Against((double)1).WhenLowerThan(1);

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Double_WhenLowerOrEqualTo_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against((double)0).WhenLowerOrEqualTo(0);
        var guardAgainstValidValue = () => Guard.Against((double)1).WhenLowerOrEqualTo(0);

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Double_WhenHigherThan_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against((double)2).WhenHigherThan(1);
        var guardAgainstValidValue = () => Guard.Against((double)1).WhenHigherThan(1);

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Double_WhenHigherOrEqualTo_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against((double)2).WhenHigherOrEqualTo(1);
        var guardAgainstValidValue = () => Guard.Against((double)0).WhenHigherOrEqualTo(1);

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Double_WhenWithinRange_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against((double)1).WhenWithinRange(0, 10);
        var guardAgainstValidValue = () => Guard.Against((double)0).WhenWithinRange(0, 10);

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Double_WhenNotWithinRange_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against((double)0).WhenNotWithinRange(1, 10);
        var guardAgainstValidValue = () => Guard.Against((double)1).WhenNotWithinRange(1, 10);

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Short_WhenNegative_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against((short)-1).WhenNegative();
        var guardAgainstValidValue = () => Guard.Against((short)0).WhenNegative();

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Short_WhenPositive_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against((short)1).WhenPositive();
        var guardAgainstValidValue = () => Guard.Against((short)0).WhenPositive();

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Short_WhenLowerThan_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against((short)0).WhenLowerThan(1);
        var guardAgainstValidValue = () => Guard.Against((short)1).WhenLowerThan(1);

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Short_WhenLowerOrEqualTo_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against((short)0).WhenLowerOrEqualTo(0);
        var guardAgainstValidValue = () => Guard.Against((short)1).WhenLowerOrEqualTo(0);

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Short_WhenHigherThan_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against((short)2).WhenHigherThan(1);
        var guardAgainstValidValue = () => Guard.Against((short)1).WhenHigherThan(1);

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Short_WhenHigherOrEqualTo_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against((short)2).WhenHigherOrEqualTo(1);
        var guardAgainstValidValue = () => Guard.Against((short)0).WhenHigherOrEqualTo(1);

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Short_WhenWithinRange_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against((short)1).WhenWithinRange(0, 10);
        var guardAgainstValidValue = () => Guard.Against((short)0).WhenWithinRange(0, 10);

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Short_WhenNotWithinRange_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against((short)0).WhenNotWithinRange(1, 10);
        var guardAgainstValidValue = () => Guard.Against((short)1).WhenNotWithinRange(1, 10);

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Long_WhenNegative_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against((long)-1).WhenNegative();
        var guardAgainstValidValue = () => Guard.Against((long)0).WhenNegative();

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Long_WhenPositive_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against((long)1).WhenPositive();
        var guardAgainstValidValue = () => Guard.Against((long)0).WhenPositive();

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Long_WhenLowerThan_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against((long)0).WhenLowerThan(1);
        var guardAgainstValidValue = () => Guard.Against((long)1).WhenLowerThan(1);

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Long_WhenLowerOrEqualTo_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against((long)0).WhenLowerOrEqualTo(0);
        var guardAgainstValidValue = () => Guard.Against((long)1).WhenLowerOrEqualTo(0);

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Long_WhenHigherThan_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against((long)2).WhenHigherThan(1);
        var guardAgainstValidValue = () => Guard.Against((long)1).WhenHigherThan(1);

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Long_WhenHigherOrEqual_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against((long)2).WhenHigherOrEqualTo(1);
        var guardAgainstValidValue = () => Guard.Against((long)0).WhenHigherOrEqualTo(1);

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Long_WhenWithinRange_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against((long)1).WhenWithinRange(0, 10);
        var guardAgainstValidValue = () => Guard.Against((long)0).WhenWithinRange(0, 10);

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

    [Fact]
    public void Guard_Against_Long_WhenNotWithinRange_Should_Work()
    {
        //arrange
        var guardAgainstInvalidValue = () => Guard.Against((long)0).WhenNotWithinRange(1, 10);
        var guardAgainstValidValue = () => Guard.Against((long)1).WhenNotWithinRange(1, 10);

        //assert
        guardAgainstInvalidValue.Should().Throw<GuardException>();
        guardAgainstValidValue.Should().NotThrow();
    }

}
