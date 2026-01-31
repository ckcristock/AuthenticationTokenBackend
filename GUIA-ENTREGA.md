# 📦 Guía de Entrega - Actividad 3

## Backend: Authentication Token Backend

---

## 📋 ENTREGABLES INCLUIDOS

### ✅ 1. Repositorio del Backend

**Ubicación:** `AuthenticationTokenBackend/`

**Estructura del proyecto:**

```
AuthenticationTokenBackend/
├── Controllers/
│   ├── AuthController.cs        # Login, Register, Logout
│   └── TareasController.cs      # CRUD de tareas
├── Models/
│   ├── User.cs                  # Modelo de usuario
│   ├── Tarea.cs                 # Modelo de tarea
│   └── DTOs/                    # Data Transfer Objects
├── Data/
│   └── ApplicationDbContext.cs  # DbContext de Entity Framework
├── Services/
│   └── TokenService.cs          # Generación y validación JWT
├── Database/
│   ├── setup-evaluador.sql      # ⭐ Script principal (USAR ESTE)
│   ├── setup.sql                # Script original
│   └── update-password.sql      # Script de actualización
├── Documentation/
│   ├── README.md                # Documentación completa
│   ├── INSTALACION.md           # Guía de instalación
│   ├── VERIFICACION-REQUISITOS.md # Verificación de cumplimiento
│   └── INSTRUCCIONES-EVALUADOR.md # Guía para el evaluador
├── Program.cs                   # Configuración principal
├── appsettings.json             # Configuración
└── AuthenticationTokenBackend.csproj # Proyecto .NET
```

---

### ✅ 2. Scripts de Base de Datos MySQL

**Ubicación:** `Database/`

#### 📄 Script Principal: `setup-evaluador.sql`

**⭐ Este es el script que debe usar el evaluador**

**Contenido:**

- Creación completa de la base de datos `autenticacion_db`
- Tabla `usuarios` con índices optimizados
- Tabla `tareas` con relaciones
- 3 usuarios de prueba con contraseñas correctas
- Tareas profesionales pre-cargadas
- Consultas de verificación
- Información de credenciales

**Credenciales incluidas:**

- `testadmin` / `admin123` - 6 tareas
- `usuario1` / `admin123` - 3 tareas
- `evaluador` / `admin123` - 2 tareas

**Cómo ejecutarlo:**

```bash
# Opción 1: Línea de comandos
mysql -u root -p < Database/setup-evaluador.sql

# Opción 2: phpMyAdmin
# - Ir a pestaña "Importar"
# - Seleccionar setup-evaluador.sql
# - Ejecutar
```

---

### ✅ 3. Repositorio del Frontend

**Ubicación:** Proyecto Angular separado `AuthenticationTokenFrontend`

**Nota:** El frontend está en un repositorio/carpeta independiente que consume este backend mediante API REST.

---

### ✅ 4. README con Flujo de Autenticación

Ver archivo completo en: [README.md](README.md)

---

## 🔐 FLUJO DE AUTENTICACIÓN Y EJECUCIÓN

### 📌 Flujo de Autenticación Completo

```
┌─────────────────────────────────────────────────────────────┐
│                    FLUJO DE AUTENTICACIÓN                    │
└─────────────────────────────────────────────────────────────┘

1. REGISTRO (Opcional)
   ┌──────────────┐
   │   Cliente    │
   │  (Angular)   │
   └──────┬───────┘
          │ POST /api/auth/register
          │ { usuario, contrasena }
          ▼
   ┌──────────────┐
   │   Backend    │──────► Hash SHA256 de contraseña
   │  (ASP.NET)   │──────► Crear usuario en BD
   └──────┬───────┘──────► Generar token JWT
          │
          ▼ Response
   ┌──────────────────────────────────────────┐
   │ { success, message, data: {              │
   │   id, usuario, token, fechaExpiracion    │
   │ }}                                       │
   └──────────────────────────────────────────┘


2. LOGIN
   ┌──────────────┐
   │   Cliente    │
   └──────┬───────┘
          │ POST /api/auth/login
          │ { usuario: "testadmin", contrasena: "admin123" }
          ▼
   ┌──────────────┐
   │   Backend    │──────► Hash contraseña con SHA256
   │              │──────► Buscar usuario en BD
   │              │──────► Validar hash coincide
   └──────┬───────┘──────► Generar token JWT (firma HS256)
          │               ├─ Claims: UserId, Username, Jti
          │               ├─ Expiración: 24 horas
          │               └─ Firmado con secret configurable
          │
          │ Guardar token en BD (campo usuarios.token)
          │ Actualizar fecha_ultimo_login
          │
          ▼ Response 200 OK
   ┌──────────────────────────────────────────┐
   │ { "success": true,                       │
   │   "message": "Login exitoso",            │
   │   "data": {                              │
   │     "id": 3,                             │
   │     "usuario": "testadmin",              │
   │     "token": "eyJhbGciOiJI...",          │
   │     "fechaExpiracion": "2026-02-01..."   │
   │   }                                      │
   │ }                                        │
   └──────────────────────────────────────────┘
          │
          ▼
   ┌──────────────┐
   │   Cliente    │──────► Guarda token en localStorage
   │  (Angular)   │──────► Guarda info de usuario
   └──────────────┘


3. ACCESO A RECURSOS PROTEGIDOS
   ┌──────────────┐
   │   Cliente    │
   └──────┬───────┘
          │ GET /api/tareas
          │ Headers: { Authorization: "Bearer eyJhbGciOiJI..." }
          ▼
   ┌──────────────────────────────────────────┐
   │     Middleware de Autenticación JWT      │
   │  (app.UseAuthentication())               │
   └──────┬───────────────────────────────────┘
          │
          ├─► Validar firma del token
          ├─► Validar expiración (24h)
          ├─► Validar Issuer
          ├─► Validar Audience
          │
          ▼ Token válido
   ┌──────────────────────────────────────────┐
   │  Extrae Claims del token:                │
   │  - UserId (ClaimTypes.NameIdentifier)    │
   │  - Username (ClaimTypes.Name)            │
   └──────┬───────────────────────────────────┘
          │
          ▼
   ┌──────────────┐
   │ [Authorize]  │──────► Verifica que usuario esté autenticado
   │ Attribute    │
   └──────┬───────┘
          │
          ▼
   ┌──────────────┐
   │ Controller   │──────► Obtiene userId del token
   │ TareasCtrl   │──────► Busca tareas WHERE usuario_id = userId
   └──────┬───────┘──────► Retorna solo las tareas del usuario
          │
          ▼ Response 200 OK
   ┌──────────────────────────────────────────┐
   │ { "success": true,                       │
   │   "message": "Tareas obtenidas",         │
   │   "data": [                              │
   │     { id, titulo, descripcion, ...},     │
   │     { id, titulo, descripcion, ...}      │
   │   ]                                      │
   │ }                                        │
   └──────────────────────────────────────────┘


4. OPERACIONES CRUD
   Todas requieren el mismo flujo de autenticación:

   POST   /api/tareas        → Crear tarea (asociada al userId del token)
   GET    /api/tareas/{id}   → Obtener tarea (solo si pertenece al usuario)
   PUT    /api/tareas/{id}   → Actualizar tarea (solo si pertenece al usuario)
   DELETE /api/tareas/{id}   → Eliminar tarea (solo si pertenece al usuario)


5. LOGOUT
   ┌──────────────┐
   │   Cliente    │
   └──────┬───────┘
          │ POST /api/auth/logout
          │ Headers: { Authorization: "Bearer eyJhbGciOiJI..." }
          ▼
   ┌──────────────┐
   │   Backend    │──────► Valida token JWT
   │              │──────► Extrae userId del token
   │              │──────► Busca usuario en BD
   └──────┬───────┘──────► Pone token = NULL en BD
          │               (Invalida el token)
          │
          ▼ Response 200 OK
   ┌──────────────────────────────────────────┐
   │ { "success": true,                       │
   │   "message": "Sesión cerrada",           │
   │   "data": null                           │
   │ }                                        │
   └──────────────────────────────────────────┘
          │
          ▼
   ┌──────────────┐
   │   Cliente    │──────► Elimina token de localStorage
   │  (Angular)   │──────► Redirige a /login
   └──────────────┘


6. ERRORES DE AUTENTICACIÓN

   401 Unauthorized:
   - Token inválido o expirado
   - Token no proporcionado
   - Usuario o contraseña incorrectos

   403 Forbidden:
   - Token válido pero sin permisos

   400 Bad Request:
   - Datos de entrada inválidos
   - Validaciones fallidas
```

---

## 🚀 INSTRUCCIONES DE EJECUCIÓN PARA EL EVALUADOR

### Paso 1: Configurar la Base de Datos

```bash
# Opción 1: MySQL desde terminal
mysql -u root -p < Database/setup-evaluador.sql

# Opción 2: phpMyAdmin
# 1. Abrir phpMyAdmin
# 2. Click en pestaña "Importar"
# 3. Seleccionar "Database/setup-evaluador.sql"
# 4. Click en "Continuar"
```

**Resultado esperado:**

- Base de datos `autenticacion_db` creada
- Tabla `usuarios` con 3 usuarios
- Tabla `tareas` con 11 tareas de ejemplo
- Credenciales listas para usar

---

### Paso 2: Configurar el Backend

**2.1. Editar `appsettings.json`**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=autenticacion_db;User=root;Password=TU_PASSWORD_MYSQL;"
  }
}
```

**2.2. Restaurar paquetes (si es necesario)**

```bash
dotnet restore
```

**2.3. Ejecutar el backend**

```bash
dotnet run
```

**Resultado esperado:**

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:7000
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

---

### Paso 3: Verificar que el Backend Funciona

**3.1. Acceder a Swagger**

Abrir en el navegador: `http://localhost:7000/swagger`

**3.2. Probar el endpoint de Login**

```bash
POST http://localhost:7000/api/auth/login
Content-Type: application/json

{
  "usuario": "testadmin",
  "contrasena": "admin123"
}
```

**Respuesta esperada:**

```json
{
  "success": true,
  "message": "Login exitoso",
  "data": {
    "id": 3,
    "usuario": "testadmin",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "fechaExpiracion": "2026-02-01T00:00:00Z"
  }
}
```

**3.3. Copiar el token y probar endpoint protegido**

```bash
GET http://localhost:7000/api/tareas
Authorization: Bearer {PEGAR_TOKEN_AQUI}
```

**Respuesta esperada:**

```json
{
  "success": true,
  "message": "Tareas obtenidas exitosamente",
  "data": [
    {
      "id": 1,
      "titulo": "Implementar autenticación JWT",
      "descripcion": "Desarrollar sistema de autenticación...",
      "completada": true,
      "fechaCreacion": "2026-01-29T10:00:00",
      "fechaActualizacion": null
    }
    // ... más tareas
  ]
}
```

---

### Paso 4: Ejecutar el Frontend (si se proporciona)

```bash
# En el directorio del frontend Angular
npm install
ng serve

# O si usa otro puerto
ng serve --port 4200
```

Abrir navegador en: `http://localhost:4200`

---

## 🧪 PRUEBAS SUGERIDAS

### 1. Flujo Completo de Autenticación

**Test 1: Login exitoso**

```http
POST /api/auth/login
{
  "usuario": "testadmin",
  "contrasena": "admin123"
}

Resultado esperado: 200 OK con token
```

**Test 2: Login fallido (credenciales incorrectas)**

```http
POST /api/auth/login
{
  "usuario": "testadmin",
  "contrasena": "incorrecta"
}

Resultado esperado: 401 Unauthorized
```

**Test 3: Acceso sin token**

```http
GET /api/tareas

Resultado esperado: 401 Unauthorized
```

**Test 4: Acceso con token válido**

```http
GET /api/tareas
Authorization: Bearer {token_del_login}

Resultado esperado: 200 OK con lista de tareas
```

---

### 2. CRUD de Tareas

**Test 5: Crear tarea**

```http
POST /api/tareas
Authorization: Bearer {token}
{
  "titulo": "Nueva tarea de prueba",
  "descripcion": "Descripción de prueba",
  "completada": false
}

Resultado esperado: 201 Created
```

**Test 6: Listar tareas**

```http
GET /api/tareas
Authorization: Bearer {token}

Resultado esperado: 200 OK con todas las tareas del usuario
```

**Test 7: Actualizar tarea**

```http
PUT /api/tareas/1
Authorization: Bearer {token}
{
  "titulo": "Tarea actualizada",
  "descripcion": "Nueva descripción",
  "completada": true
}

Resultado esperado: 200 OK
```

**Test 8: Eliminar tarea**

```http
DELETE /api/tareas/1
Authorization: Bearer {token}

Resultado esperado: 200 OK
```

---

### 3. Protección de Rutas

**Test 9: Acceso a tarea de otro usuario**

```
Hacer login con usuario1
Intentar acceder a tarea de testadmin

Resultado esperado: 404 Not Found (no encuentra la tarea)
```

**Test 10: Logout**

```http
POST /api/auth/logout
Authorization: Bearer {token}

Resultado esperado: 200 OK
```

---

## 📊 TECNOLOGÍAS UTILIZADAS

| Tecnología                       | Versión | Propósito                |
| -------------------------------- | ------- | ------------------------ |
| ASP.NET Core                     | 8.0     | Framework del backend    |
| Entity Framework Core            | 8.0.1   | ORM para base de datos   |
| Pomelo.EntityFrameworkCore.MySql | 8.0.0   | Provider MySQL           |
| MySQL                            | 8.0+    | Base de datos            |
| JWT Bearer                       | 8.0.1   | Autenticación con tokens |
| Swagger/OpenAPI                  | -       | Documentación de API     |

---

## 🎯 CUMPLIMIENTO DE REQUISITOS

Ver documento completo: [VERIFICACION-REQUISITOS.md](VERIFICACION-REQUISITOS.md)

### Resumen:

- ✅ Login con usuario y contraseña
- ✅ Emisión y almacenamiento de token JWT
- ✅ Módulo de tareas (CRUD completo)
- ✅ Protección de rutas con [Authorize]
- ✅ Cierre de sesión
- ✅ Backend ASP.NET Core
- ✅ Base de datos MySQL
- ✅ Endpoints autenticados mediante token
- ✅ Manejo centralizado de errores

**CUMPLIMIENTO TOTAL: 100%** ✅

---

## 📁 ARCHIVOS DE DOCUMENTACIÓN

| Archivo                                                                    | Descripción                              |
| -------------------------------------------------------------------------- | ---------------------------------------- |
| [README.md](README.md)                                                     | Documentación completa de la API         |
| [INSTALACION.md](INSTALACION.md)                                           | Guía paso a paso de instalación          |
| [VERIFICACION-REQUISITOS.md](VERIFICACION-REQUISITOS.md)                   | Verificación detallada de cumplimiento   |
| [Database/INSTRUCCIONES-EVALUADOR.md](Database/INSTRUCCIONES-EVALUADOR.md) | Instrucciones específicas para evaluador |
| [GUIA-ENTREGA.md](GUIA-ENTREGA.md)                                         | Este documento                           |

---

## 📞 CREDENCIALES DE PRUEBA

| Usuario   | Contraseña | Tareas | Descripción                               |
| --------- | ---------- | ------ | ----------------------------------------- |
| testadmin | admin123   | 6      | Usuario principal con tareas del proyecto |
| usuario1  | admin123   | 3      | Usuario secundario para pruebas           |
| evaluador | admin123   | 2      | Usuario específico para evaluación        |

**Nota:** Todas las contraseñas están hasheadas con SHA256 en la base de datos.

---

## ⚠️ NOTAS IMPORTANTES

1. **Puerto del backend:** Por defecto corre en `http://localhost:7000`
2. **CORS:** Está configurado en modo `AllowAll` para desarrollo. En producción debe ajustarse.
3. **Tokens JWT:** Expiran en 24 horas (configurable en `appsettings.json`)
4. **Hash de contraseñas:** SHA256 (implementado en `AuthController.HashPassword()`)
5. **Validación:** Todos los DTOs tienen Data Annotations para validación automática

---

## 🔧 SOLUCIÓN DE PROBLEMAS COMUNES

### Error: "Unable to connect to MySQL"

**Solución:** Verificar que MySQL esté corriendo y que las credenciales en `appsettings.json` sean correctas.

### Error: "JWT Secret no configurado"

**Solución:** Verificar que `appsettings.json` tenga la sección `JwtSettings` completa.

### Error: "Unknown database 'autenticacion_db'"

**Solución:** Ejecutar el script `Database/setup-evaluador.sql`

### Advertencia: Certificado SSL no confiable

**Solución:**

```bash
dotnet dev-certs https --trust
```

---

## 📦 ESTRUCTURA DE ENTREGA

```
ENTREGA-ACTIVIDAD-3/
├── AuthenticationTokenBackend/      # Backend ASP.NET Core
│   ├── Controllers/
│   ├── Models/
│   ├── Data/
│   ├── Services/
│   ├── Database/
│   │   └── setup-evaluador.sql     # ⭐ Script principal
│   ├── README.md
│   ├── INSTALACION.md
│   ├── VERIFICACION-REQUISITOS.md
│   └── GUIA-ENTREGA.md             # Este archivo
│
└── AuthenticationTokenFrontend/     # Frontend Angular (si aplica)
    ├── src/
    ├── package.json
    └── README.md
```

---

## ✅ CHECKLIST DE ENTREGA

- [x] Backend ASP.NET Core funcionando
- [x] Base de datos MySQL configurada
- [x] Scripts SQL incluidos
- [x] Usuarios de prueba configurados
- [x] Tareas de ejemplo cargadas
- [x] Documentación completa
- [x] README con flujo de autenticación
- [x] INSTALACION.md con guía paso a paso
- [x] VERIFICACION-REQUISITOS.md con análisis completo
- [x] Swagger UI funcionando
- [x] Todos los endpoints probados
- [x] CORS configurado
- [x] Manejo de errores implementado

---

## 🎓 INFORMACIÓN DEL PROYECTO

**Actividad:** 3 - Consumo tipo BaaS con autenticación simulada (token)

**Objetivo:** Verificar la implementación de un flujo de autenticación, autorización y protección de rutas, utilizando un backend que centralice la lógica de seguridad y persistencia.

**Estado:** ✅ COMPLETADO Y LISTO PARA EVALUACIÓN

**Fecha:** Enero 31, 2026

---

## 📧 CONTACTO Y SOPORTE

Para preguntas o problemas durante la evaluación, revisar:

1. Este documento (GUIA-ENTREGA.md)
2. INSTALACION.md para problemas de configuración
3. VERIFICACION-REQUISITOS.md para verificar cumplimiento
4. Swagger UI para documentación interactiva

---

**¡Gracias por evaluar este proyecto!** 🚀
