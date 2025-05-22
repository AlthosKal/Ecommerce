using BACK_END.Data;
using BACK_END.Service;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

// Carga del archivo .env
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(json =>
        json.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// Configuración de Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

// Inyección de servicios
builder.Services.AddSingleton<CloudinaryService>();

// Configuración de la base de datos
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnection"));
});
builder.Services.AddTransient<SeeDB>();

// ✅ Agrega CORS ANTES de builder.Build()
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMauiApp", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// ✅ Usa la política CORS registrada
app.UseCors("AllowMauiApp");

// ✅ Seed de datos (si lo necesitas)
SeedData(app);

void SeedData(WebApplication app)
{
    IServiceScopeFactory? scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using (IServiceScope? scope = scopedFactory!.CreateScope())
    {
        SeeDB? service = scope.ServiceProvider.GetService<SeeDB>();
        service!.SeedAsync().Wait();
    }
}

// Middleware del pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
