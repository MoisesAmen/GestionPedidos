# GestionPedidos

## Sistema de Gestión de Pedidos con Arquitectura CQRS

Este proyecto implementa una API REST para la gestión de pedidos utilizando arquitectura CQRS (Command Query Responsibility Segregation), que separa las operaciones de lectura y escritura para optimizar el rendimiento y la escalabilidad del sistema.

### Tecnologías Utilizadas

- **.NET 8**: Framework base de desarrollo
- **ASP.NET Core**: Para la creación de APIs REST
- **Entity Framework Core**: ORM para la persistencia de datos en PostgreSQL
- **PostgreSQL**: Base de datos relacional para operaciones de escritura
- **MongoDB**: Base de datos NoSQL para optimizar las consultas
- **MediatR**: Implementación del patrón mediador para CQRS
- **Swagger/OpenAPI**: Documentación interactiva de la API
- **Docker**: Contenedores para facilitar el despliegue

### Arquitectura del Sistema

La aplicación sigue una arquitectura CQRS con las siguientes características:

- **Comandos**: Manejan todas las operaciones de escritura y se almacenan en PostgreSQL
- **Consultas**: Optimizadas para lectura rápida desde MongoDB
- **Sincronización**: Sistema automático para mantener sincronizadas ambas bases de datos
- **Validación**: Implementada para todas las operaciones de comando
- **Documentación**: API documentada con Swagger y anotaciones XML

### Estructura del Proyecto

- **OrderManagement**: Proyecto principal con la API
  - **Api**: Contiene controladores, comandos y consultas
  - **Data**: Configuración de acceso a datos y migraciones
  - **Services**: Servicios de sincronización y otros
  - **Migrations**: Migraciones de Entity Framework Core

### Requisitos Previos

- .NET 8 SDK
- Docker y Docker Compose
- PostgreSQL (opcional si se usa Docker)
- MongoDB (opcional si se usa Docker)

### Configuración y Despliegue

#### Usando Docker

1. Clone el repositorio:
   ```bash
   git clone https://github.com/tuusuario/GestionPedidos.git
   cd GestionPedidos
   ```

2. Ejecute la aplicación con Docker Compose:
   ```bash
   docker-compose -f docker-compose.development.yml up -d
   ```

3. Acceda a la API en: http://localhost:5000

#### Desarrollo Local

1. Restaure las dependencias:
   ```bash
   dotnet restore
   ```

2. Configure las cadenas de conexión en `appsettings.Development.json`

3. Aplique las migraciones:
   ```bash
   dotnet ef database update -p OrderManagement
   ```

4. Ejecute la aplicación:
   ```bash
   dotnet run --project OrderManagement
   ```

5. Acceda a la API en: https://localhost:5001

### Gestión de Migraciones de Base de Datos

Para crear una nueva migración:
```bash
dotnet ef migrations add NombreMigracion -p OrderManagement
```

Para actualizar la base de datos:
```bash
dotnet ef database update -p OrderManagement
```

### Documentación de la API

La documentación Swagger está disponible en:
- Desarrollo: https://localhost:5001/swagger
- Docker: http://localhost:5000/swagger

### Características Principales

- Gestión completa del ciclo de vida de pedidos
- Separación de operaciones de lectura/escritura
- Alto rendimiento para consultas
- Escalabilidad horizontal
- Documentación interactiva
- Validaciones robustas

