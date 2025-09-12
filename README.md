# Arquitectura Hexagonal con Generación de Código

Este proyecto implementa una arquitectura hexagonal (Ports and Adapters) con una herramienta de generación de código que analiza las tablas de una base de datos SQL Server y genera automáticamente todo el código necesario para implementar la arquitectura.

## 🏗️ Arquitectura

El proyecto sigue los principios de la arquitectura hexagonal:

- **Domain**: Contiene las entidades de negocio, value objects y reglas de dominio
- **Application**: Contiene los casos de uso, comandos, queries y handlers (CQRS)
- **Infrastructure**: Implementa los puertos definidos en Domain y Application
- **WebApi**: Capa de presentación con controladores REST
- **Database**: Scripts SQL para crear y configurar la base de datos
- **CodeGenerator**: Herramienta para generar código automáticamente

## 🚀 Características Principales

### Base de Datos
- ✅ Proyecto SQL Server (.sqlproj)
- ✅ Scripts de creación de tablas
- ✅ Datos de inicialización (seed data)
- ✅ Índices y restricciones optimizadas
- ✅ Columnas de auditoría (CreatedAt, UpdatedAt, IsDeleted)

### Generador de Código
- ✅ Análisis automático de estructura de base de datos
- ✅ Generación de entidades de dominio
- ✅ Creación de value objects automática
- ✅ Generación de repositorios
- ✅ Implementación de CQRS (Commands, Queries, Handlers)
- ✅ Controladores REST API
- ✅ Seguimiento de principios DDD

## 📁 Estructura del Proyecto

```
src/
├── Domain/                    # Capa de dominio
│   ├── Companies/            # Agregado Company
│   ├── Common/               # Clases base y abstracciones
│   └── Abstractions/         # Interfaces de puertos
├── Application/              # Capa de aplicación
│   ├── Companies/            # Casos de uso para Company
│   └── Abstractions/         # Interfaces de aplicación
├── Infrastructure/           # Capa de infraestructura
│   ├── Caching/             # Implementación de caché
│   ├── Logging/             # Implementación de logging
│   ├── Persistence/         # Implementación de repositorios
│   └── Services/            # Servicios de dominio
├── DataAccess.Ef/           # Acceso a datos con Entity Framework
├── WebApi/                  # API REST
├── Database/                # Scripts de base de datos
│   ├── Tables/              # Scripts de tablas
│   ├── Data/                # Datos de inicialización
│   └── Scripts/             # Scripts de configuración
└── CodeGenerator/           # Herramienta de generación de código
    ├── Models/              # Modelos para análisis
    └── Services/            # Servicios de generación

scripts/                     # Scripts de automatización
├── setup-database.ps1       # Configurar base de datos
└── generate-code.ps1        # Generar código automáticamente

tests/                       # Pruebas unitarias e integración
├── UnitTests/
└── IntegrationTests/
```

## 🛠️ Configuración Inicial

### Prerequisitos

- .NET 9.0 SDK
- SQL Server (LocalDB, Express o Developer)
- PowerShell (para scripts de automatización)

### 1. Configurar la Base de Datos

```powershell
# Ejecutar el script de configuración
.\scripts\setup-database.ps1 -Server "localhost" -Database "HexagonalArchitectureDB"
```

O manualmente:

```bash
# Crear la base de datos
sqlcmd -S localhost -i src/Database/Scripts/CreateDatabase.sql

# Crear las tablas
sqlcmd -S localhost -d HexagonalArchitectureDB -i src/Database/Tables/Companies.sql
sqlcmd -S localhost -d HexagonalArchitectureDB -i src/Database/Tables/Employees.sql

# Insertar datos de ejemplo
sqlcmd -S localhost -d HexagonalArchitectureDB -i src/Database/Data/SeedData.sql
```

### 2. Compilar la Solución

```bash
cd src
dotnet build
```

### 3. Ejecutar la Aplicación

```bash
cd src/WebApi
dotnet run
```

## 🔧 Generación de Código

### Usar el Generador Automáticamente

```powershell
# Generar código para todas las entidades
.\scripts\generate-code.ps1 -Server "localhost" -Database "HexagonalArchitectureDB"
```

### Usar el Generador Manualmente

```bash
cd src/CodeGenerator
dotnet run -- "Server=localhost;Database=HexagonalArchitectureDB;Trusted_Connection=true;"
```

### Personalizar la Generación

El generador crea automáticamente:

- **Entidades de dominio** con validaciones
- **Value objects** para campos como Email, TaxId
- **Repositorios** con operaciones CRUD
- **Comandos y queries** siguiendo CQRS
- **Handlers** para procesar comandos y queries
- **Controladores REST** con endpoints completos

## 📊 Entidades Incluidas

### Company (Agregado Raíz)
- **Propiedades**: Id, Name, TaxId, Address
- **Value Objects**: TaxId
- **Relaciones**: Tiene muchos Employees

### Employee (Entidad)
- **Propiedades**: Id, FullName, Email, Phone, CompanyId
- **Value Objects**: Email
- **Relaciones**: Pertenece a una Company

## 🧪 Testing

```bash
# Ejecutar pruebas unitarias
cd tests/UnitTests
dotnet test

# Ejecutar pruebas de integración
cd tests/IntegrationTests
dotnet test
```

## 📚 Documentación Adicional

- [Documentación de la Base de Datos](src/Database/README.md)
- [Documentación del Generador de Código](src/CodeGenerator/README.md)
- [Comandos de la Aplicación](docs/commands/)

## 🔄 Flujo de Trabajo Recomendado

1. **Diseñar la base de datos** creando/modificando scripts SQL
2. **Configurar la BD** ejecutando los scripts
3. **Generar código** usando la herramienta automática
4. **Revisar y personalizar** el código generado
5. **Integrar** en el proyecto existente
6. **Configurar** inyección de dependencias
7. **Probar** la funcionalidad

## 🚀 Próximas Mejoras

- [ ] Soporte para PostgreSQL y MySQL
- [ ] Generación de tests unitarios
- [ ] Interfaz gráfica para el generador
- [ ] Soporte para relaciones complejas
- [ ] Generación de documentación automática
- [ ] Integración con Entity Framework Migrations

## 🤝 Contribuir

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## 📄 Licencia

Este proyecto está bajo la Licencia MIT. Ver el archivo `LICENSE` para más detalles.

## 📞 Soporte

Si tienes preguntas o necesitas ayuda:

1. Revisa la documentación en las carpetas `docs/`
2. Consulta los README específicos de cada componente
3. Abre un issue en el repositorio



