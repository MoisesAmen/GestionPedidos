using Microsoft.EntityFrameworkCore;
using OrderManagement.Api.Data;
using System.Reflection;
using MediatR;
using OrderManagement.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "API de Gestión de Pedidos",
        Version = "v1", 
        Description = "API para la gestión de pedidos con arquitectura CQRS"
    });
    
    // Especificar la versión OpenAPI
    c.DocInclusionPredicate((docName, apiDesc) => true);
    
    // Incluir los comentarios XML de documentación
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
    
    // Configurar para que busque controladores en todos los assemblies cargados
    c.EnableAnnotations();
});

// Agregar MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

// Configurar Entity Framework Core con PostgreSQL
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("PostgresConnection"),
        npgsqlOptionsAction: sqlOptions =>
        {
            sqlOptions.MigrationsAssembly("OrderManagement");
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorCodesToAdd: null);
        }
    ));

// Configurar MongoDB para consultas
builder.Services.AddSingleton<MongoDbContext>();

// Servicio de sincronización entre bases de datos
builder.Services.AddScoped<MongoDbSyncService>();

var app = builder.Build();

// Configurar el pipeline de HTTP request
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c =>
    {
        c.RouteTemplate = "swagger/{documentName}/swagger.json";
    });
    
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Gestión de Pedidos v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();










