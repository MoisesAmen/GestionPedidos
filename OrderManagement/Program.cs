using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Api.Data;
using System.Reflection;
using MediatR;
using OrderManagement.Api.Services;
//using OrderManagement.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configurar CORS para permitir peticiones desde Swagger UI
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Agregar servicios al contenedor
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configurar Swagger para usar el archivo JSON estático en lugar de generarlo dinámicamente
builder.Services.AddSwaggerGen();

// Agregar MediatR
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
});
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
    // Aplicar política CORS
    app.UseCors("AllowAll");

    // Configura Swagger para usar un archivo JSON estático
    app.UseSwagger(c =>
    {
        c.RouteTemplate = "api-docs/{documentName}/swagger.json";
    });
    
    app.UseSwaggerUI(c =>
    {
        // Usar el archivo JSON estático en lugar del generado
        c.SwaggerEndpoint("/openapi/openapi.json", "API de Gestión de Pedidos v1");
        c.RoutePrefix = "api-docs";
        c.DocumentTitle = "Documentación API - Sistema de Gestión de Pedidos";
        c.DefaultModelsExpandDepth(2);
        c.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Model);
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
        c.EnableDeepLinking();
        c.DisplayRequestDuration();
    });
}

// Habilitar el servicio de archivos estáticos para servir el archivo openapi.json
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
    RequestPath = ""
});

// Agrega la validación OpenAPI (opcional pero recomendado)
//app.UseOpenApiValidation(); 

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();