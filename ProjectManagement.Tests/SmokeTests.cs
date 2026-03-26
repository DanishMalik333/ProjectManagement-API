using System;
using Xunit;
using FluentAssertions;

namespace ProjectManagement.Tests.Basic;

public class SmokeTests
{
    [Fact]
    public void Test_ThatCanRun()
    {
        var value = 1 + 1;
        value.Should().Be(2);
    }

    [Fact]
    public void Test_StringComparison()
    {
        var text = "hello";
        text.Should().Be("hello");
    }
}
