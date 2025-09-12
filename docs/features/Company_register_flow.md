# Plan Técnico: Company Register Flow

## Descripción
Feature para permitir el registro completo de empresas desde una landing page, incluyendo múltiples direcciones, emails, teléfonos y redes sociales diferenciados por tipo. La empresa debe tener al menos un elemento de cada categoría.

## Archivos y Funciones a Modificar/Crear

### 1. Base de Datos - Nuevas Entidades

#### 1.1 Scripts SQL
**Archivo:** `src/backend/Dualcomp.Auth.Database/Scripts/003_CompanyExtendedEntities.sql`
- Crear tabla `AddressTypes` con seed data (Principal, Sucursal, Facturación, Envío)
- Crear tabla `EmailTypes` con seed data (Principal, Facturación, Soporte, Comercial)
- Crear tabla `PhoneTypes` con seed data (Principal, Móvil, Fax, WhatsApp)
- Crear tabla `SocialMediaTypes` con seed data (Facebook, Instagram, LinkedIn, Twitter, YouTube)
- Crear tabla `CompanyAddresses` (CompanyId, AddressTypeId, Address, IsPrimary)
- Crear tabla `CompanyEmails` (CompanyId, EmailTypeId, Email, IsPrimary)
- Crear tabla `CompanyPhones` (CompanyId, PhoneTypeId, Phone, IsPrimary)
- Crear tabla `CompanySocialMedias` (CompanyId, SocialMediaTypeId, Url, IsPrimary)

#### 1.2 Script de Alter para Company
**Archivo:** `src/backend/Dualcomp.Auth.Database/Scripts/004_AlterCompanyTable.sql`
- Remover columna `Address` de tabla `Companies` (ya no será necesaria)

### 2. Backend - Modelos de Dominio

#### 2.1 Nuevos Value Objects
**Archivo:** `src/backend/Dualcomp.Auth.Domain/Companies/ValueObjects/AddressType.cs`
- Value object para tipos de dirección (Principal, Sucursal, Facturación, Envío)

**Archivo:** `src/backend/Dualcomp.Auth.Domain/Companies/ValueObjects/EmailType.cs`
- Value object para tipos de email (Principal, Facturación, Soporte, Comercial)

**Archivo:** `src/backend/Dualcomp.Auth.Domain/Companies/ValueObjects/PhoneType.cs`
- Value object para tipos de teléfono (Principal, Móvil, Fax, WhatsApp)

**Archivo:** `src/backend/Dualcomp.Auth.Domain/Companies/ValueObjects/SocialMediaType.cs`
- Value object para tipos de redes sociales (Facebook, Instagram, LinkedIn, Twitter, YouTube)

#### 2.2 Nuevas Entidades
**Archivo:** `src/backend/Dualcomp.Auth.Domain/Companies/CompanyAddress.cs`
- Entidad para direcciones de empresa con AddressType

**Archivo:** `src/backend/Dualcomp.Auth.Domain/Companies/CompanyEmail.cs`
- Entidad para emails de empresa con EmailType

**Archivo:** `src/backend/Dualcomp.Auth.Domain/Companies/CompanyPhone.cs`
- Entidad para teléfonos de empresa con PhoneType

**Archivo:** `src/backend/Dualcomp.Auth.Domain/Companies/CompanySocialMedia.cs`
- Entidad para redes sociales de empresa con SocialMediaType

#### 2.3 Modificar Entidad Company
**Archivo:** `src/backend/Dualcomp.Auth.Domain/Companies/Company.cs`
- Agregar colecciones privadas para Addresses, Emails, Phones, SocialMedias
- Agregar métodos para agregar/remover cada tipo de contacto
- Modificar constructor para no requerir Address simple
- Agregar validación para asegurar al menos un elemento de cada tipo

### 3. Backend - Configuraciones EF Core

#### 3.1 Nuevas Configuraciones
**Archivo:** `src/backend/Dualcomp.Auth.DataAccess.EntityFramework/Configurations/AddressTypeConfiguration.cs`
- Configuración EF Core para AddressType

**Archivo:** `src/backend/Dualcomp.Auth.DataAccess.EntityFramework/Configurations/EmailTypeConfiguration.cs`
- Configuración EF Core para EmailType

**Archivo:** `src/backend/Dualcomp.Auth.DataAccess.EntityFramework/Configurations/PhoneTypeConfiguration.cs`
- Configuración EF Core para PhoneType

**Archivo:** `src/backend/Dualcomp.Auth.DataAccess.EntityFramework/Configurations/SocialMediaTypeConfiguration.cs`
- Configuración EF Core para SocialMediaType

**Archivo:** `src/backend/Dualcomp.Auth.DataAccess.EntityFramework/Configurations/CompanyAddressConfiguration.cs`
- Configuración EF Core para CompanyAddress

**Archivo:** `src/backend/Dualcomp.Auth.DataAccess.EntityFramework/Configurations/CompanyEmailConfiguration.cs`
- Configuración EF Core para CompanyEmail

**Archivo:** `src/backend/Dualcomp.Auth.DataAccess.EntityFramework/Configurations/CompanyPhoneConfiguration.cs`
- Configuración EF Core para CompanyPhone

**Archivo:** `src/backend/Dualcomp.Auth.DataAccess.EntityFramework/Configurations/CompanySocialMediaConfiguration.cs`
- Configuración EF Core para CompanySocialMedia

#### 3.2 Modificar Configuraciones Existentes
**Archivo:** `src/backend/Dualcomp.Auth.DataAccess.EntityFramework/Configurations/CompanyConfiguration.cs`
- Agregar configuraciones para las nuevas colecciones
- Remover configuración de Address simple

### 4. Backend - Repositorios

#### 4.1 Nuevos Repositorios
**Archivo:** `src/backend/Dualcomp.Auth.DataAccess.EntityFramework/Repositories/AddressTypeRepository.cs`
- Repositorio para AddressType con método GetAll()

**Archivo:** `src/backend/Dualcomp.Auth.DataAccess.EntityFramework/Repositories/EmailTypeRepository.cs`
- Repositorio para EmailType con método GetAll()

**Archivo:** `src/backend/Dualcomp.Auth.DataAccess.EntityFramework/Repositories/PhoneTypeRepository.cs`
- Repositorio para PhoneType con método GetAll()

**Archivo:** `src/backend/Dualcomp.Auth.DataAccess.EntityFramework/Repositories/SocialMediaTypeRepository.cs`
- Repositorio para SocialMediaType con método GetAll()

#### 4.2 Modificar Repositorio Existente
**Archivo:** `src/backend/Dualcomp.Auth.DataAccess.EntityFramework/Repositories/CompanyRepository.cs`
- Agregar métodos para incluir las nuevas colecciones en las consultas

### 5. Backend - Application Layer

#### 5.1 Nuevos Queries para Tipos
**Archivo:** `src/backend/Dualcomp.Auth.Application/AddressTypes/GetAddressTypes/GetAddressTypesQuery.cs`
- Query para obtener todos los tipos de dirección

**Archivo:** `src/backend/Dualcomp.Auth.Application/AddressTypes/GetAddressTypes/GetAddressTypesQueryHandler.cs`
- Handler para GetAddressTypesQuery

**Archivo:** `src/backend/Dualcomp.Auth.Application/AddressTypes/GetAddressTypes/GetAddressTypesResult.cs`
- Result para GetAddressTypesQuery

**Archivo:** `src/backend/Dualcomp.Auth.Application/EmailTypes/GetEmailTypes/GetEmailTypesQuery.cs`
- Query para obtener todos los tipos de email

**Archivo:** `src/backend/Dualcomp.Auth.Application/EmailTypes/GetEmailTypes/GetEmailTypesQueryHandler.cs`
- Handler para GetEmailTypesQuery

**Archivo:** `src/backend/Dualcomp.Auth.Application/EmailTypes/GetEmailTypes/GetEmailTypesResult.cs`
- Result para GetEmailTypesQuery

**Archivo:** `src/backend/Dualcomp.Auth.Application/PhoneTypes/GetPhoneTypes/GetPhoneTypesQuery.cs`
- Query para obtener todos los tipos de teléfono

**Archivo:** `src/backend/Dualcomp.Auth.Application/PhoneTypes/GetPhoneTypes/GetPhoneTypesQueryHandler.cs`
- Handler para GetPhoneTypesQuery

**Archivo:** `src/backend/Dualcomp.Auth.Application/PhoneTypes/GetPhoneTypes/GetPhoneTypesResult.cs`
- Result para GetPhoneTypesQuery

**Archivo:** `src/backend/Dualcomp.Auth.Application/SocialMediaTypes/GetSocialMediaTypes/GetSocialMediaTypesQuery.cs`
- Query para obtener todos los tipos de redes sociales

**Archivo:** `src/backend/Dualcomp.Auth.Application/SocialMediaTypes/GetSocialMediaTypes/GetSocialMediaTypesQueryHandler.cs`
- Handler para GetSocialMediaTypesQuery

**Archivo:** `src/backend/Dualcomp.Auth.Application/SocialMediaTypes/GetSocialMediaTypes/GetSocialMediaTypesResult.cs`
- Result para GetSocialMediaTypesQuery

#### 5.2 Modificar RegisterCompany
**Archivo:** `src/backend/Dualcomp.Auth.Application/Companies/RegisterCompany/RegisterCompanyCommand.cs`
- Agregar propiedades para Addresses, Emails, Phones, SocialMedias
- Agregar DTOs para cada tipo de contacto

**Archivo:** `src/backend/Dualcomp.Auth.Application/Companies/RegisterCompany/RegisterCompanyCommandHandler.cs`
- Modificar lógica para crear las nuevas entidades
- Agregar validación para asegurar al menos un elemento de cada tipo

**Archivo:** `src/backend/Dualcomp.Auth.Application/Companies/RegisterCompany/RegisterCompanyResult.cs`
- Agregar propiedades para devolver la información completa de la empresa

### 6. Backend - Web API

#### 6.1 Nuevos Controladores
**Archivo:** `src/backend/Dualcomp.Auth.WebApi/Controllers/AddressTypesController.cs`
- Endpoint GET /api/addresstypes para obtener todos los tipos

**Archivo:** `src/backend/Dualcomp.Auth.WebApi/Controllers/EmailTypesController.cs`
- Endpoint GET /api/emailtypes para obtener todos los tipos

**Archivo:** `src/backend/Dualcomp.Auth.WebApi/Controllers/PhoneTypesController.cs`
- Endpoint GET /api/phonetypes para obtener todos los tipos

**Archivo:** `src/backend/Dualcomp.Auth.WebApi/Controllers/SocialMediaTypesController.cs`
- Endpoint GET /api/socialmediatypes para obtener todos los tipos

#### 6.2 Modificar Controlador Existente
**Archivo:** `src/backend/Dualcomp.Auth.WebApi/Controllers/CompaniesController.cs`
- Modificar endpoint POST para aceptar la nueva estructura de datos
- Agregar validación de modelo

### 7. Frontend - Servicios

#### 7.1 Nuevos Servicios
**Archivo:** `src/frontend/src/app/company/services/AddressTypeService.ts`
- Servicio para obtener tipos de dirección

**Archivo:** `src/frontend/src/app/company/services/EmailTypeService.ts`
- Servicio para obtener tipos de email

**Archivo:** `src/frontend/src/app/company/services/PhoneTypeService.ts`
- Servicio para obtener tipos de teléfono

**Archivo:** `src/frontend/src/app/company/services/SocialMediaTypeService.ts`
- Servicio para obtener tipos de redes sociales

**Archivo:** `src/frontend/src/app/company/services/CompanyService.ts`
- Servicio para registro de empresa con nueva estructura

#### 7.2 Interfaces TypeScript
**Archivo:** `src/frontend/src/app/company/models/company.models.ts`
- Interfaces para Company, Address, Email, Phone, SocialMedia
- Interfaces para los tipos (AddressType, EmailType, etc.)

### 8. Frontend - Componentes

#### 8.1 Componente Principal
**Archivo:** `src/frontend/src/app/company/register/company-register.component.ts`
- Componente principal para el registro de empresa
- Diseño simple y no invasivo con secciones colapsables
- Validación básica de campos requeridos

**Archivo:** `src/frontend/src/app/company/register/company-register.component.html`
- Template con formulario reactivo para registro completo
- Interfaz limpia con secciones organizadas
- Botones de "Agregar más" para cada tipo de contacto

**Archivo:** `src/frontend/src/app/company/register/company-register.component.scss`
- Estilos minimalistas para el formulario de registro
- Diseño responsive y fácil de usar

#### 8.2 Componentes de Sección
**Archivo:** `src/frontend/src/app/company/register/sections/company-basic-info.component.ts`
- Componente para datos básicos de la empresa

**Archivo:** `src/frontend/src/app/company/register/sections/company-addresses.component.ts`
- Componente para gestión de direcciones

**Archivo:** `src/frontend/src/app/company/register/sections/company-emails.component.ts`
- Componente para gestión de emails

**Archivo:** `src/frontend/src/app/company/register/sections/company-phones.component.ts`
- Componente para gestión de teléfonos

**Archivo:** `src/frontend/src/app/company/register/sections/company-social-medias.component.ts`
- Componente para gestión de redes sociales

**Archivo:** `src/frontend/src/app/company/register/sections/company-employees.component.ts`
- Componente para gestión de empleados

### 9. Frontend - Routing

#### 9.1 Modificar Routing
**Archivo:** `src/frontend/src/app/app-routing.module.ts`
- Agregar ruta para `/company/register`

### 10. Backend - Tests Unitarios

#### 10.1 Tests para Nuevos Value Objects
**Archivo:** `src/tests/UnitTests/Companies/ValueObjects/AddressTypeTests.cs`
- Tests para validación de AddressType
- Tests para creación con valores válidos e inválidos

**Archivo:** `src/tests/UnitTests/Companies/ValueObjects/EmailTypeTests.cs`
- Tests para validación de EmailType
- Tests para creación con valores válidos e inválidos

**Archivo:** `src/tests/UnitTests/Companies/ValueObjects/PhoneTypeTests.cs`
- Tests para validación de PhoneType
- Tests para creación con valores válidos e inválidos

**Archivo:** `src/tests/UnitTests/Companies/ValueObjects/SocialMediaTypeTests.cs`
- Tests para validación de SocialMediaType
- Tests para creación con valores válidos e inválidos

#### 10.2 Tests para Nuevas Entidades
**Archivo:** `src/tests/UnitTests/Companies/CompanyAddressTests.cs`
- Tests para creación de CompanyAddress
- Tests para validaciones de AddressType
- Tests para IsPrimary

**Archivo:** `src/tests/UnitTests/Companies/CompanyEmailTests.cs`
- Tests para creación de CompanyEmail
- Tests para validaciones de EmailType
- Tests para IsPrimary

**Archivo:** `src/tests/UnitTests/Companies/CompanyPhoneTests.cs`
- Tests para creación de CompanyPhone
- Tests para validaciones de PhoneType
- Tests para IsPrimary

**Archivo:** `src/tests/UnitTests/Companies/CompanySocialMediaTests.cs`
- Tests para creación de CompanySocialMedia
- Tests para validaciones de SocialMediaType
- Tests para IsPrimary

#### 10.3 Tests para Company Modificada
**Archivo:** `src/tests/UnitTests/Companies/CompanyTests.cs`
- **Modificar tests existentes** para incluir nuevas colecciones
- Tests para agregar/remover Addresses, Emails, Phones, SocialMedias
- Tests para validación de "al menos un elemento de cada tipo"
- Tests para constructor sin Address simple

#### 10.4 Tests para Application Layer
**Archivo:** `src/tests/UnitTests/Application/AddressTypes/GetAddressTypesQueryHandlerTests.cs`
- Tests para GetAddressTypesQueryHandler
- Tests para retorno de tipos disponibles

**Archivo:** `src/tests/UnitTests/Application/EmailTypes/GetEmailTypesQueryHandlerTests.cs`
- Tests para GetEmailTypesQueryHandler
- Tests para retorno de tipos disponibles

**Archivo:** `src/tests/UnitTests/Application/PhoneTypes/GetPhoneTypesQueryHandlerTests.cs`
- Tests para GetPhoneTypesQueryHandler
- Tests para retorno de tipos disponibles

**Archivo:** `src/tests/UnitTests/Application/SocialMediaTypes/GetSocialMediaTypesQueryHandlerTests.cs`
- Tests para GetSocialMediaTypesQueryHandler
- Tests para retorno de tipos disponibles

**Archivo:** `src/tests/UnitTests/Application/Companies/RegisterCompanyCommandHandlerTests.cs`
- **Modificar tests existentes** para incluir nuevas entidades
- Tests para validación de "al menos un elemento de cada tipo"
- Tests para creación de empresa con múltiples contactos
- Tests para manejo de errores de validación

#### 10.5 Tests para Repositorios
**Archivo:** `src/tests/UnitTests/DataAccess/Repositories/AddressTypeRepositoryTests.cs`
- Tests para AddressTypeRepository
- Tests para método GetAll()

**Archivo:** `src/tests/UnitTests/DataAccess/Repositories/EmailTypeRepositoryTests.cs`
- Tests para EmailTypeRepository
- Tests para método GetAll()

**Archivo:** `src/tests/UnitTests/DataAccess/Repositories/PhoneTypeRepositoryTests.cs`
- Tests para PhoneTypeRepository
- Tests para método GetAll()

**Archivo:** `src/tests/UnitTests/DataAccess/Repositories/SocialMediaTypeRepositoryTests.cs`
- Tests para SocialMediaTypeRepository
- Tests para método GetAll()

**Archivo:** `src/tests/UnitTests/DataAccess/Repositories/CompanyRepositoryTests.cs`
- **Modificar tests existentes** para incluir nuevas colecciones
- Tests para consultas con Includes de nuevas entidades

#### 10.6 Tests para Controladores
**Archivo:** `src/tests/UnitTests/WebApi/Controllers/AddressTypesControllerTests.cs`
- Tests para endpoint GET /api/addresstypes
- Tests para respuestas exitosas y errores

**Archivo:** `src/tests/UnitTests/WebApi/Controllers/EmailTypesControllerTests.cs`
- Tests para endpoint GET /api/emailtypes
- Tests para respuestas exitosas y errores

**Archivo:** `src/tests/UnitTests/WebApi/Controllers/PhoneTypesControllerTests.cs`
- Tests para endpoint GET /api/phonetypes
- Tests para respuestas exitosas y errores

**Archivo:** `src/tests/UnitTests/WebApi/Controllers/SocialMediaTypesControllerTests.cs`
- Tests para endpoint GET /api/socialmediatypes
- Tests para respuestas exitosas y errores

**Archivo:** `src/tests/UnitTests/WebApi/Controllers/CompaniesControllerTests.cs`
- **Modificar tests existentes** para incluir nueva estructura de datos
- Tests para endpoint POST con múltiples contactos
- Tests para validación de modelo

## Algoritmo de Validación

### Validación de Registro de Empresa:
1. **Validar datos básicos**: Nombre, TaxId (único), al menos un empleado
2. **Validar direcciones**: Al menos una dirección con tipo válido
3. **Validar emails**: Al menos un email con tipo válido
4. **Validar teléfonos**: Al menos un teléfono con tipo válido
5. **Validar redes sociales**: Al menos una red social con tipo válido
6. **Validar empleados**: Al menos un empleado con datos completos (sin rol)
7. **Crear empresa**: Con todas las entidades relacionadas en una transacción
8. **Enviar email de confirmación**: Con link para activar la cuenta

### Flujo de Registro:
1. Cargar tipos disponibles desde la API
2. Mostrar formulario simple con secciones organizadas
3. Validación básica de campos requeridos
4. Enviar datos completos al backend
5. Mostrar mensaje de confirmación con instrucciones de email
6. Usuario recibe email con link de confirmación
7. Al hacer clic en el link, la empresa se activa

## Fases de Implementación

### Fase 1: Backend - Base de Datos y Modelos
- Crear scripts SQL para nuevas entidades
- Crear modelos de dominio
- Configurar EF Core

### Fase 2: Backend - Application Layer
- Crear queries para tipos
- Modificar RegisterCompany
- Crear controladores para tipos

### Fase 3: Frontend - Servicios y Modelos
- Crear servicios para tipos
- Crear interfaces TypeScript
- Crear servicio de registro

### Fase 4: Frontend - Componentes
- Crear componente principal
- Crear componentes de sección
- Implementar validaciones

### Fase 5: Tests Unitarios
- Crear tests para nuevos Value Objects
- Crear tests para nuevas entidades
- Modificar tests existentes para Company
- Crear tests para Application Layer
- Crear tests para nuevos repositorios
- Crear tests para nuevos controladores

### Fase 6: Integración y Testing
- Conectar frontend con backend
- Probar flujo completo
- Ajustar validaciones y UX
- Implementar sistema de confirmación por email

## Detalles Técnicos Específicos

### Tipos de Datos Definidos:
- **AddressTypes**: Principal, Sucursal, Facturación, Envío
- **EmailTypes**: Principal, Facturación, Soporte, Comercial  
- **PhoneTypes**: Principal, Móvil, Fax, WhatsApp
- **SocialMediaTypes**: Facebook, Instagram, LinkedIn, Twitter, YouTube

### Empleados:
- Solo datos básicos del modelo existente (FullName, Email, Phone, Position, HireDate)
- Sin campo de rol
- Al menos un empleado requerido

### Validaciones:
- Validación básica de campos requeridos
- TaxId único en el sistema
- Al menos un elemento de cada tipo de contacto
- Formato de email válido
- Formato de teléfono válido

### Interfaz:
- Diseño simple y no invasivo
- Secciones colapsables para organizar la información
- Botones "Agregar más" para cada tipo de contacto
- Formulario responsive y fácil de usar
