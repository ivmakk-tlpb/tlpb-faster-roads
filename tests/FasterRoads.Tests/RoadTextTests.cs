using FasterRoads;
using Xunit;

public class RoadTextTests
{
    [Fact]
    public void The_title_and_the_description_are_fixed()
    {
        Assert.Equal("On the Road", RoadText.Title);
        Assert.Equal("Roads make travel easier.", RoadText.Description);
    }

    [Theory]
    [InlineData(1.33f, "Movement speed is 33% faster")]
    [InlineData(1.5f, "Movement speed is 50% faster")]
    [InlineData(1.257f, "Movement speed is 26% faster")]
    [InlineData(3f, "Movement speed is 200% faster")]
    public void The_speed_line_gives_the_whole_percent(float multiplier, string line)
    {
        Assert.Equal(line, RoadText.SpeedLine(multiplier));
    }

    [Theory]
    [InlineData(0.5f, "Stamina depletes 50% slower")]
    [InlineData(0f, "Stamina depletes 100% slower")]
    [InlineData(0.875f, "Stamina depletes 13% slower")]
    public void The_drain_line_gives_the_whole_percent(float multiplier, string line)
    {
        Assert.Equal(line, RoadText.DrainLine(multiplier));
    }

    [Fact]
    public void A_multiplier_of_one_gives_no_line()
    {
        Assert.Null(RoadText.SpeedLine(1f));
        Assert.Null(RoadText.DrainLine(1f));
    }

    [Fact]
    public void A_value_outside_the_range_gives_the_line_of_the_limit()
    {
        Assert.Equal("Movement speed is 200% faster", RoadText.SpeedLine(10f));
        Assert.Null(RoadText.DrainLine(4f));
    }
}
