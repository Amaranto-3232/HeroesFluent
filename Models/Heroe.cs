namespace HeroesFluent.Models;

public class Heroe
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Alias { get; set; } = string.Empty;
    public int NivelPoder { get; set; }
    public DateTime FechaRegistro { get; set; }
    public int? EquipoId { get; set; }

    public Equipo? Equipo { get; set; }
    public ICollection<HeroeMision> HeroeMisiones { get; set; } = new List<HeroeMision>();
}
