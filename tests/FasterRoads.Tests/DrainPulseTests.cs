using FasterRoads;
using Xunit;

public class DrainPulseTests
{
    private const float Frame = 0.01f;

    private readonly DrainPulse pulse = new DrainPulse();

    private float[] Frames(float multiplier, int count, float frameTime = Frame)
    {
        var changes = new float[count];
        for (int i = 0; i < count; i++) changes[i] = pulse.Next(multiplier, frameTime);
        return changes;
    }

    // The game drain of a frame is its rate times the frame time, so the kept drain is the time of the frames
    // with the full drain over the time of all frames.
    private float KeptDrain(float multiplier, float[] frameTimes)
    {
        float kept = 0f, total = 0f;
        foreach (float t in frameTimes)
        {
            kept += (1f + pulse.Next(multiplier, t)) * t;
            total += t;
        }
        return kept / total;
    }

    [Theory]
    [InlineData(0.5f)]
    [InlineData(0.75f)]
    [InlineData(0.33f)]
    [InlineData(0.9f)]
    public void The_mean_drain_of_the_frames_is_the_multiplier(float multiplier)
    {
        var changes = Frames(multiplier, 1000);

        float meanDrain = changes.Select(c => 1f + c).Average();
        Assert.Equal(multiplier, meanDrain, 2);
    }

    [Theory]
    [InlineData(0.01f, 0.03f)]
    [InlineData(0.03f, 0.01f)]
    public void Frames_of_alternating_length_keep_the_multiplier_of_the_drain(float first, float second)
    {
        var times = Enumerable.Range(0, 1000).Select(i => i % 2 == 0 ? first : second).ToArray();

        Assert.Equal(0.5f, KeptDrain(0.5f, times), 2);
    }

    [Theory]
    [InlineData(0.5f)]
    [InlineData(0.25f)]
    [InlineData(0.8f)]
    public void Frames_of_random_length_keep_the_multiplier_of_the_drain(float multiplier)
    {
        var random = new Random(22);
        var times = Enumerable.Range(0, 2000).Select(_ => 0.005f + (float)random.NextDouble() * 0.035f).ToArray();

        Assert.Equal(multiplier, KeptDrain(multiplier, times), 2);
    }

    [Fact]
    public void Each_frame_has_the_full_drain_or_no_drain()
    {
        Assert.All(Frames(0.6f, 50), c => Assert.True(c == 0f || c == -1f));
    }

    [Fact]
    public void A_multiplier_of_one_half_takes_the_drain_away_on_every_other_frame()
    {
        Assert.Equal(new[] { -1f, 0f, -1f, 0f }, Frames(0.5f, 4));
    }

    [Fact]
    public void A_multiplier_of_zero_takes_the_drain_away_on_each_frame()
    {
        Assert.All(Frames(0f, 10), c => Assert.Equal(-1f, c));
    }

    [Fact]
    public void A_multiplier_of_one_keeps_the_drain_on_each_frame()
    {
        Assert.All(Frames(1f, 10), c => Assert.Equal(0f, c));
    }

    [Fact]
    public void A_frame_with_no_time_changes_nothing()
    {
        Assert.All(Frames(0f, 5, 0f), c => Assert.Equal(0f, c));
    }

    [Fact]
    public void A_value_outside_the_range_is_limited_to_it()
    {
        Assert.All(Frames(-3f, 5), c => Assert.Equal(-1f, c));
        Assert.All(Frames(4f, 5), c => Assert.Equal(0f, c));
    }
}
