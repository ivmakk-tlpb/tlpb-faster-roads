using UnityEngine;

// A world with the player's character, its views, and the HUD that shows it.
internal sealed class Scene
{
    public readonly World World = new World();
    public readonly Entity Player;
    public readonly EntityMovementView Movement = new EntityMovementView();
    public readonly StaminaView Stamina = new StaminaView();
    public readonly StatusEffectsView Status = new StatusEffectsView();
    public readonly HudUI Hud = new HudUI();
    public readonly GameObject Terrain = new GameObject { layer = ObjectLayer.terrainLayer };

    public Scene(int playerId = 7)
    {
        Player = new Entity { id = playerId, world = World };
        Movement.entity = Player;
        Stamina.entity = Player;
        Status.entity = Player;
        Player.Components.Add(Movement);
        Player.Components.Add(Stamina);
        Player.Components.Add(Status);
        Movement.groundObject = Terrain;
        Hud.player = new LocalPlayer { playerEntity = Player };
    }

    // A creature in the same world with its own views.
    public EntityMovementView CreatureMovement(int id = 50)
    {
        var creature = new Entity { id = id, world = World };
        var movement = new EntityMovementView { entity = creature, groundObject = Terrain };
        creature.Components.Add(movement);
        return movement;
    }

    public StaminaView CreatureStamina(int id = 50)
    {
        var creature = new Entity { id = id, world = World };
        var stamina = new StaminaView { entity = creature };
        creature.Components.Add(stamina);
        return stamina;
    }

    // Runs one HUD update at the given time.
    public void HudFrame(float time)
    {
        Time.time = time;
        FasterRoads.Patches.HudPatches.Postfix(Hud);
    }

    // Puts the player on a road and runs a HUD update, so the road boost is on.
    public void OnRoad(float time = 10f)
    {
        World.Surface = SurfaceType.ROAD;
        HudFrame(time);
    }
}
