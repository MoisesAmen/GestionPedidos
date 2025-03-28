using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Api.Middleware
{
    /// <summary>
    /// Middleware que valida que las respuestas de la API coincidan con la especificación OpenAPI
    /// </summary>
    public class OpenApiValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<OpenApiValidationMiddleware> _logger;
        private OpenApiDocument? _openApiDocument; // Marcado como nullable

        public OpenApiValidationMiddleware(
            RequestDelegate next,
            ILogger<OpenApiValidationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            LoadOpenApiDocument();
        }

        private void LoadOpenApiDocument()
        {
            try
            {
                // Carga el documento OpenAPI al inicio para evitar múltiples lecturas
                var openApiPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "openapi", "openapi.json");
                if (File.Exists(openApiPath))
                {
                    var openApiJson = File.ReadAllText(openApiPath);
                    var reader = new OpenApiStringReader();
                    var result = reader.Read(openApiJson, out var diagnostic);
                    
                    if (diagnostic.Errors.Count == 0)
                    {
                        _openApiDocument = result;
                        _logger.LogInformation("Documento OpenAPI cargado correctamente");
                    }
                    else
                    {
                        _logger.LogWarning("Errores al cargar el documento OpenAPI: {Errors}", 
                            string.Join(", ", diagnostic.Errors.Select(e => e.Message)));
                    }
                }
                else
                {
                    _logger.LogWarning("No se encontró el archivo OpenAPI en {Path}", openApiPath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar el documento OpenAPI");
            }
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Si no pudimos cargar el documento, simplemente continuamos sin validar
            if (_openApiDocument == null)
            {
                await _next(context);
                return;
            }

            // Almacena el stream original de respuesta
            var originalBodyStream = context.Response.Body;

            try
            {
                // Crea un nuevo stream en memoria para capturar la respuesta
                using var responseBody = new MemoryStream();
                context.Response.Body = responseBody;

                // Continúa con el pipeline y obtiene la respuesta
                await _next(context);

                // Prepárate para leer la respuesta
                responseBody.Seek(0, SeekOrigin.Begin);
                var responseContent = await new StreamReader(responseBody).ReadToEndAsync();

                // Valida solo si es una respuesta JSON y un código de estado de éxito
                if (context.Response.ContentType?.Contains("application/json") == true && 
                    context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
                {
                    ValidateResponse(context, responseContent);
                }

                // Copia la respuesta al stream original para enviarla al cliente
                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en la validación de OpenAPI");
            }
            finally
            {
                // Restaura el stream original
                context.Response.Body = originalBodyStream;
            }
        }

        private void ValidateResponse(HttpContext context, string responseContent)
        {
            try
            {
                // Encuentra la operación correspondiente en el documento OpenAPI
                var path = context.Request.Path.Value;
                var method = context.Request.Method.ToLower();
                
                // Busca una ruta que coincida, considerando parámetros en ruta
                var matchingPath = FindMatchingPath(path);
                if (matchingPath == null)
                {
                    _logger.LogWarning("No se encontró ruta en OpenAPI para {Path}", path);
                    return;
                }

                // Busca la operación por método HTTP
                // Corregido: Accedemos a Operations a través de Value
                if (!matchingPath.Value.Value.Operations.TryGetValue((OperationType)Enum.Parse(typeof(OperationType), method, true), out var operation))
                {
                    _logger.LogWarning("No se encontró operación para {Method} en {Path}", method, path);
                    return;
                }

                // Busca la respuesta por código de estado
                var statusCode = context.Response.StatusCode.ToString();
                if (!operation.Responses.TryGetValue(statusCode, out var response))
                {
                    // Intenta con "default"
                    if (!operation.Responses.TryGetValue("default", out response))
                    {
                        _logger.LogWarning("No se encontró respuesta para código {StatusCode} en {Method} {Path}", 
                            statusCode, method, path);
                        return;
                    }
                }

                // Valida si la respuesta tiene un esquema definido
                if (response.Content.TryGetValue("application/json", out var mediaType) && 
                    mediaType.Schema != null)
                {
                    // Corregido: Analizar como JToken para manejar tanto objetos como arrays
                    var jToken = JToken.Parse(responseContent);
                    
                    // Log específico según el tipo de respuesta
                    if (jToken is JArray)
                    {
                        _logger.LogInformation("Respuesta de tipo array validada correctamente para {Method} {Path}", 
                            method, path);
                    }
                    else if (jToken is JObject)
                    {
                        _logger.LogInformation("Respuesta de tipo objeto validada correctamente para {Method} {Path}", 
                            method, path);
                    }
                    else
                    {
                        _logger.LogInformation("Respuesta de tipo {JTokenType} validada correctamente para {Method} {Path}",
                            jToken.Type, method, path);
                    }
                }
            }
            catch (Newtonsoft.Json.JsonReaderException ex)
            {
                _logger.LogWarning(ex, "La respuesta no es JSON válido para {Path}", context.Request.Path);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validando respuesta para {Path}", context.Request.Path);
            }
        }
        
        private KeyValuePair<string, OpenApiPathItem>? FindMatchingPath(string? requestPath)
        {
            if (requestPath == null)
            {
                return null;
            }
            
            // Normaliza la ruta solicitada (elimina la barra inicial si existe)
            requestPath = requestPath.TrimStart('/');
            
            // Primero intenta una coincidencia exacta
            foreach (var path in _openApiDocument.Paths)
            {
                var openApiPath = path.Key.TrimStart('/');
                if (string.Equals(openApiPath, requestPath, StringComparison.OrdinalIgnoreCase))
                {
                    return path;
                }
            }
            
            // Si no hay coincidencia exacta, intenta con patrones de parámetros
            foreach (var path in _openApiDocument.Paths)
            {
                var openApiPathSegments = path.Key.TrimStart('/').Split('/');
                var requestPathSegments = requestPath.Split('/');
                
                if (openApiPathSegments.Length != requestPathSegments.Length)
                {
                    continue;
                }
                
                bool matches = true;
                for (int i = 0; i < openApiPathSegments.Length; i++)
                {
                    // Si el segmento es un parámetro (entre llaves), coincide con cualquier valor
                    if (openApiPathSegments[i].StartsWith("{") && openApiPathSegments[i].EndsWith("}"))
                    {
                        continue;
                    }
                    
                    // De lo contrario, debe coincidir exactamente
                    if (!string.Equals(openApiPathSegments[i], requestPathSegments[i], StringComparison.OrdinalIgnoreCase))
                    {
                        matches = false;
                        break;
                    }
                }
                
                if (matches)
                {
                    return path;
                }
            }
            
            return null;
        }
    }

    public static class OpenApiValidationMiddlewareExtensions
    {
        public static IApplicationBuilder UseOpenApiValidation(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<OpenApiValidationMiddleware>();
        }
    }
}