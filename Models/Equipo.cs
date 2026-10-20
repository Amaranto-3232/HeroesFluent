namespace HeroesFluent.Models;

public class Equipo
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public DateTime FechaFundacion { get; set; }
    public decimal Presupuesto { get; set; }

    public ICollection<Heroe> Heroes { get; set; } = new List<Heroe>();
}
