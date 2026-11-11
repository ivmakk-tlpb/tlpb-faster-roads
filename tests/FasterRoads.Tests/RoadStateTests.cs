using FasterRoads;
using Xunit;

public class RoadStateTests
{
    private readonly RoadState state = new RoadState();

    [Fact]
    public void The_boost_is_off_at_start()
    {
        Assert.False(state.IsOn);
    }

    [Fact]
    public void A_road_result_turns_the_boost_on_at_once()
    {
        state.Update(10f, true);

        Assert.True(state.IsOn);
    }

    [Fact]
    public void A_non_road_result_keeps_the_boost_on_for_half_a_second()
    {
        state.Update(10f, true);
        state.Update(10.1f, false);
        state.Update(10.49f, false);

        Assert.True(state.IsOn);
    }

    [Fact]
    public void A_non_road_result_turns_the_boost_off_half_a_second_after_the_last_road_result()
    {
        state.Update(10f, true);
        state.Update(10.2f, false);
        state.Update(10.5f, false);

        Assert.False(state.IsOn);
    }

    [Fact]
    public void A_road_result_within_the_grace_time_keeps_the_boost_on_with_no_gap()
    {
        state.Update(10f, true);
        state.Update(10.3f, false);
        state.Update(10.4f, true);
        state.Update(10.8f, false);

        Assert.True(state.IsOn);
    }

    [Fact]
    public void A_non_road_result_with_the_boost_off_keeps_it_off()
    {
        state.Update(10f, false);

        Assert.False(state.IsOn);
    }

    [Fact]
    public void The_first_check_is_due_at_once()
    {
        Assert.True(state.ShouldCheck(0f));
    }

    [Fact]
    public void A_check_is_due_at_most_every_tenth_of_a_second()
    {
        state.Update(10f, false);

        Assert.False(state.ShouldCheck(10.05f));
        Assert.True(state.ShouldCheck(10.1f));
    }

    [Fact]
    public void A_change_of_the_state_is_reported_once()
    {
        Assert.True(state.Update(10f, true));
        Assert.False(state.Update(10.1f, true));
        Assert.False(state.Update(10.2f, false));
        Assert.True(state.Update(10.6f, false));
        Assert.False(state.Update(10.7f, false));
    }
}
