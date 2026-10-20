namespace HeroesFluent.Models;

public class HeroeMision
{
    public int HeroeId { get; set; }
    public int MisionId { get; set; }
    public string Rol { get; set; } = string.Empty;
    public DateTime FechaAsignacion { get; set; }

    public Heroe Heroe { get; set; } = null!;
    public Mision Mision { get; set; } = null!;
}
