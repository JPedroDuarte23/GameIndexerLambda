using GameIndexerLambda.Enums;

namespace GameIndexerLambda.Entities;
public class Game
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public Guid Publisher { get; set; }
    public string Description { get; set; }
    public Double Price { get; set; }
    public DateTime ReleaseDate { get; set; }

    public List<GameGenre> Genres { get; set; }
    public List<GameTag> Tags { get; set; }

    public int OwnershipCount { get; set; }
}
