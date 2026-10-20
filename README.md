# HeroesFluent

Para reproducir y explicar los cambios paso a paso, consulta la
[guía de carga inicial con Bogus](GUIA_CARGA_HEROES_BOGUS.md).

## Carga inicial de héroes

Al iniciar, la aplicación aplica las migraciones pendientes y utiliza Bogus para
completar la cantidad total de héroes configurada en `appsettings.json`:

```json
"DatosIniciales": {
  "CantidadHeroes": 100
}
```

La cantidad incluye los cuatro héroes de ejemplo de las migraciones y cualquier
héroe existente. Por ejemplo, si hay 4 y configuras 100, se generan 96. Los reinicios
no agregan más registros si ya se alcanzó la cantidad; reducirla no elimina datos.
Usa `0` para desactivar esta inicialización. Los valores negativos se rechazan.

También puedes indicar la cantidad al ejecutar:

```powershell
dotnet run -- --DatosIniciales:CantidadHeroes=250
```

`Data/HeroeFaker.cs` define los datos generados: nombres en español, alias únicos,
poder entre 1 y 100, fechas de registro pasadas y equipos existentes (o sin equipo
si no hay ninguno). `Data/HeroesSeeder.cs` guarda solamente los héroes faltantes.

SQL Server debe estar en ejecución y la conexión `ConnectionStrings:HeroesFluent`
debe apuntar a la base de datos deseada antes de iniciar la aplicación.
