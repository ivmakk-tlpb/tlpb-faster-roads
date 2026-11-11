using FasterRoads;
using Xunit;

public class BoostTests
{
    [Theory]
    [InlineData(1.33f, 0.33f)]
    [InlineData(1f, 0f)]
    [InlineData(3f, 2f)]
    [InlineData(10f, 2f)]
    [InlineData(0.5f, 0f)]
    public void The_speed_change_is_the_multiplier_minus_one_within_the_range(float multiplier, float change)
    {
        Assert.Equal(change, Boost.SpeedChange(multiplier), 4);
    }

    [Theory]
    [InlineData(0.5f, -0.5f)]
    [InlineData(1f, 0f)]
    [InlineData(0f, -1f)]
    [InlineData(-2f, -1f)]
    [InlineData(4f, 0f)]
    public void The_drain_change_is_the_multiplier_minus_one_within_the_range(float multiplier, float change)
    {
        Assert.Equal(change, Boost.DrainChange(multiplier), 4);
    }

    [Theory]
    [InlineData(1.33f, 0.5f, true, true)]
    [InlineData(1.33f, 1f, true, true)]
    [InlineData(1f, 0.5f, true, true)]
    [InlineData(1f, 1f, true, false)]
    [InlineData(1.33f, 0.5f, false, false)]
    public void The_tile_shows_only_with_a_boost_and_the_icon_setting_on(float speed, float drain, bool showIcon, bool shows)
    {
        Assert.Equal(shows, Boost.ShowTile(speed, drain, showIcon));
    }

    [Theory]
    [InlineData(17, true)]
    [InlineData(18, true)]
    [InlineData(6, false)]
    [InlineData(0, false)]
    public void Only_the_road_surfaces_count_as_road(int surface, bool road)
    {
        Assert.Equal(road, Boost.IsRoadSurface(surface));
    }
}
