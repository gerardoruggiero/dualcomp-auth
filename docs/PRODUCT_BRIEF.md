# Product Brief: Plataforma de Login Centralizado y Gestión de Productos (Argentina)

## 1. Descripción del proyecto
Plataforma SaaS multi‑tenant con login centralizado (SSO) para los productos de la empresa (ej.: CRM, Ventas, Compras, Stock, Postventa). El acceso de usuarios está gobernado por el estado de cuenta de cada empresa cliente. Incluye registro de empresas (self‑service), gestión de empleados y permisos por producto, y facturación con pagos recurrentes integrados con Mercado Pago. Orientado inicialmente al mercado de Argentina.

## 2. Público objetivo
- Empresas de distintos tamaños en Argentina que requieran un conjunto de aplicaciones de back‑office (CRM, Ventas, Compras, Stock, Postventa).
- Administradores de empresas (dueños/gerentes) que contratan y configuran productos, usuarios y permisos.
- Empleados de las empresas clientes que utilizan las aplicaciones asignadas.

## 3. Beneficios / funcionalidades principales
- Login centralizado (SSO) para todos los productos de la suite.
- Multi‑empresa para usuarios: un usuario puede pertenecer a múltiples empresas y elegir el contexto de trabajo.
- Control por estado de cuenta: bloqueo/desbloqueo automático del acceso según morosidad o regularización.
- Registro self‑service de empresas desde landing page: datos fiscales/personales, selección de productos, alta de usuario administrador, dirección mínima, contactos, credenciales seguras.
- Integración de pagos recurrentes con Mercado Pago (tokenización y suscripciones) con opción de tarjeta en alta.
- Flujo de aprobación de empresa: estado “pendiente de aprobación” previo a habilitación total.
- Gestión de empleados por empresa: altas/bajas, asignación de roles y permisos por producto adquirido.
- Catálogo de productos administrable: nombre, descripción, permisos (scopes/roles), fecha de vigencia, documentación técnica/funcional adjunta.
- Seguridad: claves generadas automáticamente y encriptadas, auditoría de accesos y cambios.

## 4. Requisitos de datos clave
- Entidades: Empresa (cliente), Usuario, Membresía Usuario‑Empresa, Producto, Permiso (por producto), Suscripción/Plan, Estado de cuenta, Dirección, Medios de pago.
- Registro mínimo obligatorio: al menos una dirección; usuario admin con nombre completo, teléfono, email; contraseña generada y encriptada; datos públicos para validación (sitio web, LinkedIn, documento). Tarjeta opcional.
- Estados de empresa: Pendiente de aprobación, Activa, Suspendida por morosidad, Inactiva.

## 5. Arquitectura / tecnología (alta nivel)
- Multi‑tenant: modelo de Organización/Empresa con membresías de usuario; segregación por `tenant_id` a nivel de datos.
- Identidad y Acceso: Las claves de los usuarios se guardarán de forma segura en una base de datos propia. No se debe poder iniciar sesión con cuentas externas.
- Facturación y pagos: integración con Mercado Pago para suscripciones/pagos recurrentes; webhooks para sincronizar estados (aprobado, rechazado, vencido) y disparar (des)bloqueos. El sistema deberá saber el importe mensual para cada empresa, el cual es un monto por usuario y por sistema. Ejemplo: 5 USD por usuario por sistema. Si 3 usuarios usa CRM y ventas: 3 usuarios x 2 módulos cada uno: 3 * 5 * 2 = 30 USD.
- Orquestación de estados: eventos de ciclo de vida (empresa aprobada, morosidad, regularización) que actualizan accesos.
- Datos: base relacional con SQL Server con índices por `tenant_id`; auditoría y trazabilidad.
- Backend: API modular, desarrollada con en .NET, por dominios (Empresas, Usuarios, Productos, Suscripciones, Billing); colas/eventos para webhooks.
- Frontend: En Angular con un diseño amigable y legible, utilizando AdminLTE, con un panel de administración para el cliente (gestión de empleados, permisos, productos) y consola interna para aprobación.
- Infraestructura: contenedores, CI/CD, monitoreo, logs centralizados, backups; localización y zona horaria de Argentina.
- Será montado sobre Azure.

## 6. Criterios de aceptación (resumen)
- Empresas pueden registrarse, seleccionar productos y crear un usuario administrador con datos mínimos.
- Estado “pendiente de aprobación” bloquea parcialmente la operación hasta aprobación interna.
- Usuarios pueden pertenecer a múltiples empresas y cambiar de contexto.
- Morosidad de una empresa bloquea el acceso de todos sus empleados a todos los productos; al regularizar, se rehabilita automáticamente.
- Catálogo de productos configurable con permisos y documentación.
- Pagos recurrentes operativos vía Mercado Pago con webhooks funcionando y conciliación básica.
- Un usuario no puede estar logueado en más de un dispositivo al mismo tiempo.

## 7. Consideraciones regulatorias y locales
- Cumplir buenas prácticas de seguridad (encriptación de contraseñas, manejo seguro de tokens de pago).
- Ajustes de localización (idioma español, moneda ARS, impuestos locales si aplica) y soporte a CUIT/DNI.


