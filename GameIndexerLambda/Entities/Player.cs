namespace GameIndexerLambda.Entities;
public class Player : User
{
    public string Cpf { get; set; }
    public List<Guid> Library { get; set; } = new();
    public List<Guid> Cart { get; set; } = new();
    public List<Guid> Wishlist { get; set; } = new();
}
