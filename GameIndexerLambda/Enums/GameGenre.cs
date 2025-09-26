using System.Text.Json.Serialization;

namespace GameIndexerLambda.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GameGenre
{
    Action,
    Shooter,
    Fighting,
    Platformer,
    Adventure,
    RPG,
    MMORPG,
    Strategy,
    RTS,
    TurnBasedStrategy,
    Simulation,
    Sports,
    Racing,
    Puzzle,
    Horror,
    Survival,
    Sandbox,
    CardGame
}