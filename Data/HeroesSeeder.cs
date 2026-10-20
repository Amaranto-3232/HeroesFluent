using Microsoft.EntityFrameworkCore;

namespace HeroesFluent.Data;

public static class HeroesSeeder
{
    public static async Task InicializarAsync(
        HeroesDbContext context,
        int cantidadHeroes,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(cantidadHeroes);

        if (cantidadHeroes == 0)
        {
            return;
        }

        var cantidadActual = await context.Heroes.CountAsync(cancellationToken);
        var cantidadFaltante = cantidadHeroes - cantidadActual;

        if (cantidadFaltante <= 0)
        {
            return;
        }

        var equipoIds = await context.Equipos.AsNoTracking()
            .Select(e => e.Id)
            .ToListAsync(cancellationToken);

        var faker = new HeroeFaker(equipoIds);
        context.Heroes.AddRange(faker.Generate(cantidadFaltante));
        await context.SaveChangesAsync(cancellationToken);
    }
}
