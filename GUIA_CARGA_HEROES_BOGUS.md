# Guía: cargar héroes al iniciar con Bogus

Esta guía explica cómo reproducir los cambios sobre el proyecto original y cómo
explicar su funcionamiento. En el proyecto actual estos cambios ya están aplicados.

## 1. Qué queremos conseguir

Queremos indicar una cantidad de héroes en la configuración y que, al iniciar la
aplicación, se generen y guarden los que falten para alcanzar ese total.

Por ejemplo, las migraciones del proyecto incluyen cuatro héroes. Si configuramos
100, debemos generar **96 nuevos**, para tener 100 en total.

La solución tiene tres responsabilidades:

- **`HeroeFaker`**: define cómo generar un héroe ficticio con Bogus.
- **`HeroesSeeder`**: consulta la base, calcula cuántos faltan y los guarda.
- **`Program.cs`**: lee la cantidad y ejecuta la inicialización antes de atender solicitudes.

## 2. Instalar el paquete Bogus

Bogus es una biblioteca que genera datos ficticios, como nombres, números y fechas.
Entity Framework Core sigue siendo el encargado de guardar esos datos en SQL Server.

Abre una terminal en la carpeta que contiene `HeroesFluent.csproj` y ejecuta:

```powershell
dotnet add HeroesFluent.csproj package Bogus --version 35.6.5
```

Si utilizas la **Consola del Administrador de paquetes** de Visual Studio, puedes
usar esta alternativa, seleccionando `HeroesFluent` como proyecto predeterminado:

```powershell
Install-Package Bogus -Version 35.6.5
```

**Elige uno de los dos comandos.** Ambos instalan el mismo paquete.

El comando agrega esta referencia dentro de un `<ItemGroup>` del archivo
`HeroesFluent.csproj`:

```xml
<PackageReference Include="Bogus" Version="35.6.5" />
```

No necesitas escribir esa línea manualmente si ya ejecutaste el comando. Conserva
las referencias a Entity Framework Core que el proyecto ya tiene.

## 3. Configurar la cantidad en `appsettings.json`

Agrega la sección `DatosIniciales` al mismo nivel que `ConnectionStrings` y
`Logging`. El archivo de este proyecto queda así:

```json
{
  "ConnectionStrings": {
    "HeroesFluent": "Server=localhost;Database=HeroesFluent;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "DatosIniciales": {
    "CantidadHeroes": 100
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

Conserva tu cadena de conexión si tu servidor es diferente. Presta atención a las
comas entre las secciones: el archivo debe seguir siendo JSON válido.

`CantidadHeroes` representa el **total mínimo deseado**, incluyendo los héroes
que ya existen. No representa cuántos agregar en cada inicio.

| Héroes existentes | Cantidad configurada | Nuevos héroes | Total final |
| --- | --- | --- | --- |
| 4 | 100 | 96 | 100 |
| 100 | 100 | 0 | 100 |
| 100 | 150 | 50 | 150 |
| 150 | 50 | 0 | 150 |
| 150 | 0 | 0 | 150 |

Con `0` se desactiva esta inicialización. Una cantidad negativa produce un error de
validación al iniciar. En esta implementación, si se omite la clave, la lectura
como `int` devuelve `0`.

## 4. Crear `Data/HeroeFaker.cs`

Crea el archivo `HeroeFaker.cs` dentro de la carpeta `Data` y agrega este código:

```csharp
using Bogus;
using HeroesFluent.Models;

namespace HeroesFluent.Data;

public sealed class HeroeFaker : Faker<Heroe>
{
    public HeroeFaker(IReadOnlyList<int> equipoIds) : base("es")
    {
        RuleFor(h => h.Nombre, f => f.Name.FullName());
        RuleFor(h => h.Alias, f => $"Heroe-{f.Random.Guid():N}");
        RuleFor(h => h.NivelPoder, f => f.Random.Int(1, 100));
        RuleFor(h => h.FechaRegistro, f => f.Date.Past(2, DateTime.UtcNow));
        RuleFor(h => h.EquipoId, f => equipoIds.Count > 0 ? f.PickRandom<int>(equipoIds) : (int?)null);
    }
}
```

### Cómo explicar esta clase

La biblioteca que usamos es **Bogus**. Su documentación principal está en el
[repositorio oficial de Bogus](https://github.com/bchavez/Bogus).

`Faker<Heroe>` es una clase de Bogus especializada en generar objetos del tipo
`Heroe`. Al heredar de ella, agrupamos todas las reglas en una clase reutilizable.
`sealed` indica que esta clase no se puede heredar.

El constructor recibe `equipoIds`, una lista de identificadores de equipos que
existen en la base. `base("es")` selecciona el conjunto de datos en español de
Bogus; no cambia el idioma de la aplicación.

Cada llamada a `RuleFor` establece el valor que se generará para una propiedad:

| Regla | Qué hace y por qué |
| --- | --- |
| `f.Name.FullName()` | Genera un nombre completo ficticio. |
| `$"Heroe-{f.Random.Guid():N}"` | Forma un alias con un GUID de 32 caracteres sin guiones. El alias completo mide 38 caracteres, dentro del máximo de 60 del modelo. La probabilidad de repetición es extremadamente baja y la base mantiene su índice único. |
| `f.Random.Int(1, 100)` | Genera un poder entre 1 y 100, incluyendo ambos extremos, para respetar la restricción de la tabla. |
| `f.Date.Past(2, DateTime.UtcNow)` | Genera una fecha de registro de los dos años anteriores al momento actual en UTC. |
| `f.PickRandom<int>(equipoIds)` | Selecciona un equipo existente; si la lista está vacía, asigna `null`. |

En `RuleFor(h => h.Nombre, f => f.Name.FullName())`, `h => h.Nombre` identifica la
propiedad que vamos a llenar y `f => f.Name.FullName()` indica cómo generar su valor.

No asignamos `Id`: SQL Server lo genera al insertar porque la columna es
`IDENTITY`. Tampoco creamos misiones ni equipos nuevos.

**Esta clase solamente crea objetos en memoria.** Por ejemplo,
`faker.Generate(10)` devuelve diez objetos `Heroe`; todavía no los guarda.

### Documentación oficial para consultar

- [Ejemplos de uso de Bogus](https://github.com/bchavez/Bogus#usage): muestran
  `Faker<T>`, `RuleFor`, `Generate` y `PickRandom`, que usamos en `HeroeFaker`.
- [Métodos disponibles de Bogus](https://github.com/bchavez/Bogus#bogus-api-support):
  consulta las categorías `Name`, `Date` y `Random` para conocer los generadores
  de nombres, fechas, números y GUID.
- [Idiomas y configuraciones regionales](https://github.com/bchavez/Bogus#locales):
  explica los códigos de idioma, incluido `es`, que usamos en `base("es")`.
- [Paquete Bogus 35.6.5 en NuGet](https://www.nuget.org/packages/Bogus/35.6.5):
  contiene la versión instalada y los comandos para agregarla al proyecto.

Para relacionar la documentación con nuestro código: `equipoIds` es el nombre
del parámetro que definimos nosotros para recibir los identificadores de equipos;
no es una función de Bogus. La operación de Bogus es `PickRandom<int>(equipoIds)`,
que elige un identificador de esa lista. La condición que asigna `null` cuando
no hay equipos también pertenece a nuestro código.

## 5. Crear `Data/HeroesSeeder.cs`

Crea el archivo `HeroesSeeder.cs` dentro de `Data` y agrega:

```csharp
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
```

### Cómo explicar el método

1. **Valida la cantidad.** `ThrowIfNegative` rechaza números negativos. Esta
   validación también protege llamadas al método desde otros lugares.
2. **Sale si la carga está desactivada.** Con `0`, no consulta ni modifica la base.
3. **Cuenta los héroes existentes.** `CountAsync` realiza el conteo en la base sin
   traer todos los héroes a memoria.
4. **Calcula lo que falta.** Resta la cantidad actual del total deseado. Si el
   resultado es cero o negativo, termina sin borrar ni insertar registros.
5. **Obtiene los equipos válidos.** `Select(e => e.Id)` recupera únicamente sus
   identificadores. `AsNoTracking` expresa que es una lectura; en esta proyección
   de enteros EF Core ya no materializa entidades que deba seguir.
6. **Genera y prepara los registros.** `Generate(cantidadFaltante)` crea los objetos
   y `AddRange` los marca como nuevos dentro del contexto.
7. **Guarda los cambios.** `SaveChangesAsync` ejecuta las inserciones en la base.

`async` y `await` permiten esperar las operaciones de base de datos sin bloquear
el hilo durante la espera. `CancellationToken` permite que quien invoque el método
solicite cancelar esas operaciones; es opcional en la llamada actual.

La clase es `static` porque se usa como una rutina de inicialización y no necesita
mantener estado propio entre llamadas.

## 6. Modificar `Program.cs`

El bloque nuevo debe ir **después de `var app = builder.Build();` y antes de
`app.Run();`**. En este proyecto se colocó justo después de construir la aplicación.

Los `using` de `HeroesFluent.Data` y `Microsoft.EntityFrameworkCore` ya existían.
El archivo completo queda así:

```csharp
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
```

### Cómo explicar el bloque nuevo

- **`GetValue<int>("DatosIniciales:CantidadHeroes")`** lee la cantidad configurada.
  Los dos puntos permiten acceder a una propiedad dentro de una sección.
- **`ThrowIfNegative`** detiene el arranque si la cantidad es inválida.
- **`if (cantidadHeroes > 0)`** evita ejecutar esta inicialización cuando está
  desactivada. En este código, con `0` también se omite `MigrateAsync`.
- **`CreateAsyncScope()`** crea un ámbito temporal para usar servicios con tiempo
  de vida limitado, como el contexto de EF Core registrado con `AddDbContext`.
- **`GetRequiredService<HeroesDbContext>()`** obtiene el contexto desde el contenedor
  de dependencias, con la conexión SQL Server ya configurada.
- **`MigrateAsync()`** aplica las migraciones pendientes, incluyendo los datos de
  ejemplo que contienen. Si la base no existe, puede crearla si el usuario de la
  conexión tiene permisos. Esto ocurre antes de contar los héroes.
- **`InicializarAsync(...)`** completa y guarda la cantidad de héroes faltantes.
- **`await using`** libera el ámbito y su contexto al terminar el bloque.

Los dos `await` de las operaciones hacen que la aplicación espere a que finalicen
las migraciones y la carga antes de ejecutar `app.Run()` y comenzar a atender
solicitudes. El `await` de `await using` tiene otra función: permite esperar la
liberación asíncrona de recursos al salir del bloque.

### Primera línea: crear un ámbito y asegurar su limpieza

```csharp
await using var scope = app.Services.CreateAsyncScope();
```

Un **ámbito**, llamado `scope` en inglés, delimita cuánto tiempo se usan ciertos
servicios de la aplicación. Puedes imaginarlo como una sesión temporal de trabajo:
se abre para inicializar los datos y se cierra al terminar.

En este proyecto, `AddDbContext<HeroesDbContext>(...)` registra el contexto con
tiempo de vida **scoped** de forma predeterminada. Esto significa que, dentro de
un mismo ámbito, las solicitudes de ese servicio reciben la misma instancia del
contexto. Otro ámbito obtiene su propia instancia.

Cuando un usuario visita una página, ASP.NET Core crea un ámbito para esa
solicitud. Pero este código se ejecuta durante el arranque, antes de recibir una
solicitud. Por eso creamos un ámbito explícitamente.

La línea tiene tres partes:

- `app.Services` permite acceder al contenedor donde registramos los servicios
  con `builder.Services`.
- `CreateAsyncScope()` crea el ámbito temporal. Crear el ámbito por sí solo no
  consulta SQL Server ni crea las tablas.
- `await using` indica que, al salir del bloque `if`, se debe liberar ese ámbito
  de forma asíncrona y esperar a que termine su limpieza. El contenedor también
  libera los servicios desechables que creó dentro del ámbito, incluido el contexto.

La creación del ámbito es síncrona; el `await` de esta línea corresponde a su
**liberación al final**, no a una espera para crearlo. Esta limpieza también se
intenta si una de las instrucciones del bloque lanza una excepción.

**Para explicarlo:** «Creo una sesión temporal de servicios para trabajar con la
base durante el arranque y asegurar que sus recursos se liberen al terminar».

### Segunda línea: obtener el contexto de base de datos

```csharp
var context = scope.ServiceProvider.GetRequiredService<HeroesDbContext>();
```

`HeroesDbContext` es la clase de EF Core que permite consultar `Heroes` y `Equipos`
y guardar cambios. Esta línea obtiene una instancia preparada para trabajar con
la configuración que ya registramos en `Program.cs`.

- `scope.ServiceProvider` busca servicios dentro del ámbito recién creado.
- `GetRequiredService<HeroesDbContext>()` solicita el servicio de ese tipo. Si no
  está registrado, lanza una excepción; no devuelve `null`.
- `var context` guarda la referencia al contexto para usarla en las dos líneas
  siguientes. El compilador deduce que su tipo es `HeroesDbContext`.

El contenedor construye el contexto cuando se necesita, proporcionándole sus
opciones: en este caso, el proveedor SQL Server y la cadena de conexión que se
definieron con `AddDbContext` y `UseSqlServer`.

Obtener el contexto **no significa que ya se consultó la base**. Las operaciones
siguientes son las que necesitan comunicarse con SQL Server. Tampoco necesitamos
crear otro contexto dentro del seeder: le pasamos este mismo.

**Para explicarlo:** «Pido al contenedor el contexto de EF Core, ya configurado
para acceder a SQL Server, y lo reutilizo durante toda la inicialización».

### Tercera línea: preparar la base con las migraciones existentes

```csharp
await context.Database.MigrateAsync();
```

`context.Database` da acceso a operaciones relacionadas con la base de datos.
`MigrateAsync()` consulta el historial de migraciones y aplica las del proyecto
que todavía no se hayan ejecutado en esa base.

En este proyecto, esas migraciones crean las tablas e incluyen equipos, misiones,
cuatro héroes de ejemplo y sus asignaciones. Por eso deben ejecutarse **antes**
de que el seeder cuente héroes o consulte equipos.

Si todas las migraciones ya están aplicadas, no vuelve a ejecutar sus operaciones
ni inserta otra vez sus datos de ejemplo. Si la base no existe, puede crearla,
siempre que el servidor esté disponible y la conexión tenga permisos suficientes.

El `await` indica que la siguiente línea debe esperar a que termine esta operación.
Durante las esperas de entrada/salida no necesita mantener bloqueado el hilo que
inició la operación. Esto no lanza la carga de héroes en paralelo.

`MigrateAsync()` **aplica migraciones ya creadas**; no genera archivos de migración.
Tampoco invoca Bogus. Los héroes ficticios se generan en el paso siguiente.

**Para explicarlo:** «Primero actualizo la base usando las migraciones pendientes,
para asegurar que existan las tablas y los datos de ejemplo antes de cargar más héroes».

### Cuarta línea: completar y guardar los héroes faltantes

```csharp
await HeroesSeeder.InicializarAsync(context, cantidadHeroes);
```

Esta línea llama al método estático que creamos en `HeroesSeeder`. Le entrega:

- `context`: el mismo contexto obtenido del ámbito, para consultar y guardar.
- `cantidadHeroes`: el total deseado que leímos de la configuración.

El método cuenta los héroes existentes, calcula cuántos faltan, consulta los
identificadores de equipos y usa `HeroeFaker` para generar únicamente esa diferencia.
Después llama a `SaveChangesAsync` para guardar los nuevos registros.

Por ejemplo, con `cantidadHeroes = 100` y cuatro héroes existentes, genera y guarda
96. Con 100 o más existentes, termina sin agregar registros.

El `await` espera a que finalice el método, incluyendo el guardado. Cuando termina,
se sale del `if`, se libera el ámbito y continúa la configuración de la aplicación
hasta llegar a `app.Run()`.

**Para explicarlo:** «Con la base preparada, completo la cantidad configurada con
héroes generados por Bogus y espero a que EF Core termine de guardarlos».

### Las cuatro líneas juntas, comentadas

```csharp
// Crea un ámbito temporal; al salir del bloque se libera con sus servicios.
await using var scope = app.Services.CreateAsyncScope();

// Obtiene el contexto de EF Core con la configuración de SQL Server.
var context = scope.ServiceProvider.GetRequiredService<HeroesDbContext>();

// Espera a que se apliquen las migraciones pendientes de la base.
await context.Database.MigrateAsync();

// Espera a que se generen y guarden solamente los héroes que faltan.
await HeroesSeeder.InicializarAsync(context, cantidadHeroes);
```

El orden completo es: **crear el ámbito → obtener el contexto → aplicar
migraciones → completar héroes → liberar el ámbito → continuar el arranque**.

## 7. Qué archivos no necesitas modificar

Para agregar esta funcionalidad no es necesario cambiar:

- `Controllers/HeroesController.cs`: ya consulta los héroes de la base. La carga se
  ejecuta al iniciar y no depende de que alguien abra la página de héroes.
- `Models/Heroe.cs`: ya contiene las propiedades necesarias.
- `Data/HeroesDbContext.cs`: ya expone `Heroes` y `Equipos`.
- Las configuraciones de entidades y las migraciones existentes.
- Las vistas: mostrarán los registros guardados mediante la consulta existente.

**No necesitas crear una migración nueva** porque no cambiamos la estructura de
las tablas. Bogus genera registros en tiempo de ejecución; no lo agregamos a
`HasData` ni a `OnModelCreating`.

Se conservan los cuatro héroes y las relaciones de ejemplo de las migraciones.
El archivo `README.md` también recibió una explicación breve de uso; ese cambio
es documentación y no participa en la ejecución.

## 8. Compilar y probarlo tú mismo

Antes de ejecutar, inicia SQL Server y comprueba que la conexión de
`appsettings.json` apunta al servidor y la base correctos. Con esta implementación,
si falla la conexión o una migración, la inicialización falla y la aplicación no
termina de arrancar.

Desde la carpeta del proyecto:

```powershell
dotnet build HeroesFluent.csproj
dotnet run --project HeroesFluent.csproj
```

Abre la dirección local que muestre la terminal y entra a `/Heroes`. La ruta por
defecto también abre ese listado.

Puedes comprobar los registros en SQL Server Management Studio con:

```sql
USE [HeroesFluent];

SELECT COUNT(*) AS TotalHeroes FROM dbo.Heroes;

SELECT TOP (20) Id, Nombre, Alias, NivelPoder, FechaRegistro, EquipoId
FROM dbo.Heroes
ORDER BY Id DESC;
```

Para comprobar el comportamiento:

1. Configura un total mayor que el actual y ejecuta. Debe completar ese total.
2. Detén y vuelve a iniciar con la misma cantidad. El total debe mantenerse.
3. Aumenta la cantidad y reinicia. Debe agregar solamente la diferencia.
4. Reduce la cantidad y reinicia. Debe conservar los registros existentes.
5. Configura `0`. No debe ejecutar la carga inicial.

También puedes indicar una cantidad para una ejecución sin editar el JSON:

```powershell
dotnet run --project HeroesFluent.csproj -- --DatosIniciales:CantidadHeroes=250
```

El separador `--` envía el argumento siguiente a la aplicación. Esa configuración
por línea de comandos tiene prioridad sobre el valor de `appsettings.json`.

## 9. Explicación breve para una exposición

> Agregué Bogus para generar datos ficticios de héroes. La cantidad total deseada
> se define en appsettings.json. Al iniciar, Program lee ese valor, aplica las
> migraciones pendientes y llama a HeroesSeeder. El seeder cuenta los héroes que
> existen y calcula cuántos faltan. HeroeFaker genera esos objetos con reglas para
> nombres, alias, poder, fechas y equipos válidos. Finalmente, Entity Framework
> Core los guarda en SQL Server. Así, al reiniciar una sola instancia, no se siguen
> agregando héroes si ya se alcanzó el total configurado.

### Preguntas que puedes responder

**¿Bogus guarda los héroes?** No. Bogus crea objetos; EF Core los guarda con
`SaveChangesAsync`.

**¿Por qué usar dos clases?** Para separar las reglas de generación de datos de
las consultas y la persistencia.

**¿Qué pasa si ya existen más héroes que la cantidad configurada?** Se conservan.
La rutina completa faltantes, pero no elimina registros.

**¿Qué pasa si elimino héroes y reinicio?** Si el total quedó por debajo del
configurado, se crearán nuevos héroes para completar la diferencia. No se
recuperan los mismos datos eliminados.

**¿Por qué la carga está en Program y no en el controlador?** Porque se necesita
ejecutar al iniciar la aplicación, antes de recibir solicitudes.

**¿Funciona igual con varias instancias arrancando al mismo tiempo?** Este conteo
y posterior inserción está pensado para una instancia. Varias instancias podrían
contar al mismo tiempo y agregar más héroes del total deseado; esa coordinación
no forma parte del cambio actual.
