# Frontend Brief: CRM Multi-tenant Angular

## 1. Descripción del proyecto
Frontend Angular para plataforma SaaS multi-tenant con login centralizado (SSO) para los productos de la empresa. El frontend consumirá los endpoints del backend .NET y proporcionará una interfaz intuitiva y moderna para la gestión de empresas, empleados y autenticación. Diseñado para ser responsive y optimizado para tablets y móviles con funciones adaptadas según el dispositivo.

## 2. Público objetivo
- **Administradores de empresas** que gestionan usuarios, permisos y configuraciones
- **Empleados de empresas clientes** que utilizan las aplicaciones asignadas
- **Usuarios multi-empresa** que necesitan cambiar de contexto de trabajo
- **Dispositivos móviles y tablets** con interfaces adaptadas

## 3. Beneficios / funcionalidades principales
- **Interfaz moderna e intuitiva** con AdminLTE
- **Autenticación segura** con JWT y refresh token automático
- **Gestión completa de empresas** (registro, listado, actualización, detalle)
- **Gestión de empleados** por empresa con CRUD completo
- **Diseño responsive** optimizado para desktop, tablet y móvil
- **Navegación fluida** con guards y interceptors automáticos
- **Manejo de errores** centralizado con modales informativos
- **Multi-tenant** con cambio de contexto mediante logout/login
- **Validaciones en tiempo real** en formularios
- **Paginación y búsqueda** en listados
- **Testing completo** (unitarios e integración)

## 4. Arquitectura / tecnología (alta nivel)
- **Framework**: Angular 17+ con Signals para gestión de estado
- **UI Components**: Usaremos AdminLTE para reutilizar componentes y diseños
- **State Management**: Angular Signals (no NgRx por simplicidad)
- **HTTP Client**: RxJS con interceptors para manejo automático de tokens
- **Authentication**: JWT con refresh token automático, guards para rutas protegidas
- **Testing**: Jest para unitarios, Angular Testing Library para integración
- **Code Quality**: ESLint + Prettier para consistencia
- **Build**: Angular CLI con configuración optimizada
- **PWA**: Configurable como parámetro para futuras implementaciones

## 5. Estructura modular por dominios
- **Core Module**: Servicios singleton, guards, interceptors, configuración
- **Auth Module**: Login, logout, cambio de contraseña, refresh token
- **Companies Module**: CRUD de empresas, listado con paginación y búsqueda
- **Employees Module**: Gestión de empleados por empresa, asignación de roles
- **Shared Module**: Componentes reutilizables, pipes, directivas, modelos
- **Layout Module**: Navegación, headers, sidebars, layouts responsive

## 6. Endpoints del backend a consumir
- **Auth**: POST /api/auth/login, POST /api/auth/logout, POST /api/auth/change-password, POST /api/auth/refresh-token
- **Companies**: GET /api/companies, POST /api/companies, GET /api/companies/{id}, PUT /api/companies/{id}
- **Employees**: GET /api/employees, POST /api/employees, GET /api/employees/{id}, PUT /api/employees/{id}

## 7. Modelos de datos principales
- **User**: id, email, fullName, companyId, isCompanyAdmin
- **Company**: id, name, taxId, address, employeeCount
- **Employee**: id, fullName, email, phone, companyId, position, hireDate, isActive, userId
- **AuthResponse**: accessToken, refreshToken, expiresAt, user
- **Pagination**: page, pageSize, totalCount, totalPages

## 8. Diseño responsive y UX
- **Desktop**: Funcionalidad completa con sidebar y navegación expandida
- **Tablet**: Layout adaptado con navegación colapsable, funcionalidad completa
- **Móvil**: Navegación simplificada, funciones reducidas, interfaz touch-friendly
- **Tema**: Diseño limpio e intuitivo, colores corporativos, tipografía legible
- **Accesibilidad**: Cumplimiento de estándares WCAG, navegación por teclado

## 9. Criterios de aceptación (resumen)
- Usuarios pueden autenticarse con email/password y recibir tokens JWT
- Refresh token funciona automáticamente sin intervención del usuario
- Empresas pueden registrarse, listarse, actualizarse y visualizarse en detalle
- Empleados pueden gestionarse por empresa con CRUD completo
- Interfaz es completamente responsive en desktop, tablet y móvil
- Errores se muestran en modales informativos y amigables
- Validaciones funcionan en tiempo real en formularios
- Navegación está protegida por guards de autenticación
- Cambio de empresa se realiza mediante logout/login
- Testing cubre funcionalidades críticas y casos edge

## 10. Consideraciones técnicas
- **Performance**: Lazy loading de módulos, OnPush change detection, optimización de bundles
- **Security**: Tokens seguros, sanitización de inputs, validación client-side
- **Maintainability**: Código limpio siguiendo principios SOLID, documentación inline
- **Scalability**: Arquitectura modular que permite agregar nuevos dominios fácilmente
- **Internationalization**: Preparado para múltiples idiomas (inicialmente español)
- **Browser Support**: Compatibilidad con navegadores modernos (Chrome, Firefox, Safari, Edge)

## 11. Configuración y deployment
- **Environment**: Configuración para desarrollo, staging y producción
- **API Base URL**: Configurable por ambiente
- **PWA**: Parámetro configurable para habilitar/deshabilitar
- **Build**: Optimización automática para producción
- **CORS**: Backend ya configurado para aceptar requests del frontend
