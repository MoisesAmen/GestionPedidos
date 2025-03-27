# Instrucciones para Migraciones de Entity Framework Core

## Crear una migración inicial

Para crear la primera migración, ejecuta el siguiente comando en la consola:

```bash
dotnet ef migrations add InitialCreate -p OrderManagement -s OrderManagement
```

## Aplicar migraciones a la base de datos

Para actualizar la base de datos con las migraciones pendientes:

```bash
dotnet ef database update -p OrderManagement
```

## Crear una nueva migración

Para crear migraciones adicionales cuando cambies el modelo:

```bash
dotnet ef migrations add NombreDeLaMigracion -p OrderManagement/OrderManagement.Api
```

## Revertir migraciones

Para revertir la última migración aplicada:

```bash
dotnet ef database update NombreMigracionAnterior -p OrderManagement/OrderManagement.Api
```

## Eliminar la última migración

Si has creado una migración pero aún no la has aplicado:

```bash
dotnet ef migrations remove -p OrderManagement/OrderManagement.Api
```
