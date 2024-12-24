using Soenneker.Tests.FixturedUnit;
using Xunit;

namespace Soenneker.Runners.ytdlp.Tests;

[Collection("Collection")]
public class ConsoleHostedServiceTests : FixturedUnitTest
{
    public ConsoleHostedServiceTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
    }

    [Fact]
    public void Default()
    {

    }
}