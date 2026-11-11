using FasterRoads;
using Xunit;

// The patch classes and the fake game types hold static state, so their tests do not run in parallel.
[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class GameState
{
    public const string Name = "Game state";
}

// Each test starts from a clean state with a loaded plugin.
[Collection(GameState.Name)]
public abstract class GameTestBase
{
    protected GameTestBase()
    {
        FakeState.Reset();
        new Plugin().Load();
    }
}
