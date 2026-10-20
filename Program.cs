using HeroesFluent.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<HeroesDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("HeroesFluent")));
var app = builder.Build();

// INICIO: bloque agregado para la carga inicial de héroes con Bogus.
var cantidadHeroes = app.Configuration.GetValue<int>("DatosIniciales:CantidadHeroes");
ArgumentOutOfRangeException.ThrowIfNegative(cantidadHeroes);

if (cantidadHeroes > 0)
{
    await using var scope = app.Services.CreateAsyncScope();
    var context = scope.ServiceProvider.GetRequiredService<HeroesDbContext>();
    await context.Database.MigrateAsync();
    await HeroesSeeder.InicializarAsync(context, cantidadHeroes);
}
// FIN: bloque agregado para la carga inicial de héroes con Bogus.

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute("default", "{controller=Heroes}/{action=Index}/{id?}");
app.Run();
