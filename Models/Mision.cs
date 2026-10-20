namespace HeroesFluent.Models;

public class Mision
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public Dificultad Dificultad { get; set; }
    public DateTime? FechaLimite { get; set; }

    public ICollection<HeroeMision> HeroeMisiones { get; set; } = new List<HeroeMision>();
}
