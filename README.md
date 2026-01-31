# 🔐 Authentication Token Backend

**Backend BaaS desarrollado con ASP.NET Core 8.0 para gestión de autenticación con JWT y CRUD de tareas**

---

## 📋 Actividad 3 - Consumo tipo BaaS con autenticación simulada (token)

### 🎯 Objetivo

Verificar la implementación de un flujo de autenticación, autorización y protección de rutas, utilizando un backend que centralice la lógica de seguridad y persistencia.

---

## ✅ Cumplimiento de Requisitos

### Requerimientos Funcionales Implementados (100%)

| #   | Requisito                              | Estado | Implementación                                 |
| --- | -------------------------------------- | ------ | ---------------------------------------------- |
| 1   | Login con usuario y contraseña         | ✅     | `AuthController.cs` - Login con SHA256         |
| 2   | Emisión y almacenamiento de token JWT  | ✅     | `TokenService.cs` - JWT con 24h de expiración  |
| 3   | Módulo de tareas (CRUD completo)       | ✅     | `TareasController.cs` - 5 endpoints protegidos |
| 4   | Protección de rutas y cierre de sesión | ✅     | `[Authorize]` + Logout endpoint                |

### Requerimientos Técnicos Implementados (100%)

| #   | Requisito                          | Estado | Tecnología                     |
| --- | ---------------------------------- | ------ | ------------------------------ |
| 1   | Backend en ASP.NET Core            | ✅     | .NET 8.0                       |
| 2   | Base de datos MySQL                | ✅     | MySQL + EF Core + Pomelo       |
| 3   | Endpoints autenticados             | ✅     | JWT Bearer Authentication      |
| 4   | Interceptor Authorization Bearer   | ✅     | Middleware JWT configurado     |
| 5   | Manejo de errores de autenticación | ✅     | `ApiResponseDto` estandarizado |

---

## 🚀 Inicio Rápido

### 📋 Requisitos Previos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MySQL Server 8.0+](https://dev.mysql.com/downloads/mysql/)

### ⚡ Instalación en 4 Pasos

#### 1️⃣ Configurar Base de Datos

Ejecuta el script SQL (incluye 3 usuarios y tareas de prueba):

```bash
mysql -u root -p < Database/setup-evaluador.sql
```

O desde phpMyAdmin: Importar → `Database/setup-evaluador.sql` → Continuar

#### 2️⃣ Configurar Conexión

Edita `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=autenticacion_db;User=root;Password=TU_PASSWORD;"
}
```

#### 3️⃣ Instalar Dependencias

```bash
dotnet restore
```

#### 4️⃣ Ejecutar

```bash
dotnet run
```

✅ Backend corriendo en: `http://localhost:7000`  
✅ Swagger UI en: `http://localhost:7000/swagger`

---

## 👤 Credenciales de Prueba

| Usuario     | Contraseña | Tareas | Descripción                                |
| ----------- | ---------- | ------ | ------------------------------------------ |
| `testadmin` | `admin123` | 6      | Usuario administrador con tareas completas |
| `usuario1`  | `admin123` | 3      | Usuario estándar con tareas en progreso    |
| `evaluador` | `admin123` | 2      | Usuario para evaluación                    |

---

## 🔐 Flujo de Autenticación Completo

```
┌─────────┐         ┌─────────┐         ┌─────────┐
│Frontend │         │Backend  │         │MySQL DB │
└────┬────┘         └────┬────┘         └────┬────┘
     │                   │                   │
     │ POST /auth/login  │                   │
     ├──────────────────>│                   │
     │ {usuario,pass}    │                   │
     │                   │ Hash SHA256       │
     │                   ├──────────┐        │
     │                   │          │        │
     │                   │<─────────┘        │
     │                   │                   │
     │                   │ SELECT usuario    │
     │                   ├──────────────────>│
     │                   │  WHERE hash       │
     │                   │<──────────────────┤
     │                   │ Usuario válido    │
     │                   │                   │
     │                   │ Generar JWT       │
     │                   ├──────────┐        │
     │                   │ (24h)    │        │
     │                   │<─────────┘        │
     │                   │                   │
     │                   │ UPDATE token      │
     │                   ├──────────────────>│
     │                   │<──────────────────┤
     │                   │                   │
     │<──────────────────┤                   │
     │ {token, user}     │                   │
     │                   │                   │
     │ GET /tareas       │                   │
     ├──────────────────>│                   │
     │ Auth: Bearer JWT  │                   │
     │                   │ Validar JWT       │
     │                   ├──────────┐        │
     │                   │          │        │
     │                   │<─────────┘        │
     │                   │                   │
     │                   │ SELECT tareas     │
     │                   ├──────────────────>│
     │                   │ WHERE user_id     │
     │                   │<──────────────────┤
     │                   │                   │
     │<──────────────────┤                   │
     │ Lista tareas      │                   │
     │                   │                   │
     │ POST /auth/logout │                   │
     ├──────────────────>│                   │
     │ Auth: Bearer JWT  │                   │
     │                   │ UPDATE token=NULL │
     │                   ├──────────────────>│
     │                   │<──────────────────┤
     │<──────────────────┤                   │
     │ Sesión cerrada    │                   │
     │                   │                   │
```

---

## 📡 API Endpoints

### 🔓 Autenticación (Sin token requerido)

#### **POST** `/api/auth/register`

Registra un nuevo usuario.

**Request:**

```json
{
  "usuario": "nuevo_usuario",
  "contrasena": "password123"
}
```

**Response:**

```json
{
  "success": true,
  "message": "Usuario registrado exitosamente",
  "data": {
    "id": 4,
    "usuario": "nuevo_usuario",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "fechaExpiracion": "2026-02-01T12:00:00Z"
  }
}
```

#### **POST** `/api/auth/login`

Inicia sesión y obtiene token JWT.

**Request:**

```json
{
  "usuario": "testadmin",
  "contrasena": "admin123"
}
```

**Response:**

```json
{
  "success": true,
  "message": "Login exitoso",
  "data": {
    "id": 1,
    "usuario": "testadmin",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "fechaExpiracion": "2026-02-01T12:00:00Z"
  }
}
```

#### **POST** `/api/auth/logout`

Cierra la sesión del usuario (requiere token).

**Headers:**

```
Authorization: Bearer {token}
```

**Response:**

```json
{
  "success": true,
  "message": "Sesión cerrada exitosamente",
  "data": null
}
```

---

### 🔒 Tareas - CRUD (Token Requerido)

> **⚠️ Importante:** Todos los endpoints de tareas requieren el header:  
> `Authorization: Bearer {tu_token_jwt}`

#### **GET** `/api/tareas`

Lista todas las tareas del usuario autenticado.

**Response:**

```json
{
  "success": true,
  "message": "Tareas obtenidas exitosamente",
  "data": [
    {
      "id": 1,
      "titulo": "Implementar sistema de autenticación JWT",
      "descripcion": "Desarrollar endpoints de login, logout y validación de tokens",
      "completada": true,
      "fechaCreacion": "2026-01-30T08:00:00Z",
      "fechaActualizacion": "2026-01-31T10:30:00Z"
    },
    {
      "id": 2,
      "titulo": "Crear módulo CRUD de tareas",
      "descripcion": "Endpoints para crear, leer, actualizar y eliminar tareas",
      "completada": false,
      "fechaCreacion": "2026-01-30T09:15:00Z",
      "fechaActualizacion": null
    }
  ]
}
```

#### **GET** `/api/tareas/{id}`

Obtiene una tarea específica por ID (solo si pertenece al usuario autenticado).

**Response:**

```json
{
  "success": true,
  "message": "Tarea obtenida exitosamente",
  "data": {
    "id": 1,
    "titulo": "Implementar sistema de autenticación JWT",
    "descripcion": "Desarrollar endpoints de login, logout y validación de tokens",
    "completada": true,
    "fechaCreacion": "2026-01-30T08:00:00Z",
    "fechaActualizacion": "2026-01-31T10:30:00Z"
  }
}
```

#### **POST** `/api/tareas`

Crea una nueva tarea asociada al usuario autenticado.

**Request:**

```json
{
  "titulo": "Configurar base de datos MySQL",
  "descripcion": "Crear tablas usuarios y tareas con relaciones",
  "completada": false
}
```

**Response:**

```json
{
  "success": true,
  "message": "Tarea creada exitosamente",
  "data": {
    "id": 8,
    "titulo": "Configurar base de datos MySQL",
    "descripcion": "Crear tablas usuarios y tareas con relaciones",
    "completada": false,
    "fechaCreacion": "2026-01-31T14:20:00Z",
    "fechaActualizacion": null
  }
}
```

#### **PUT** `/api/tareas/{id}`

Actualiza una tarea existente (solo si pertenece al usuario autenticado).

**Request:**

```json
{
  "titulo": "Configurar base de datos MySQL - Actualizado",
  "descripcion": "Crear tablas con índices optimizados",
  "completada": true
}
```

**Response:**

```json
{
  "success": true,
  "message": "Tarea actualizada exitosamente",
  "data": {
    "id": 8,
    "titulo": "Configurar base de datos MySQL - Actualizado",
    "descripcion": "Crear tablas con índices optimizados",
    "completada": true,
    "fechaCreacion": "2026-01-31T14:20:00Z",
    "fechaActualizacion": "2026-01-31T15:45:00Z"
  }
}
```

#### **DELETE** `/api/tareas/{id}`

Elimina una tarea (solo si pertenece al usuario autenticado).

**Response:**

```json
{
  "success": true,
  "message": "Tarea eliminada exitosamente",
  "data": null
}
```

---

## 🗂️ Estructura del Proyecto

```
AuthenticationTokenBackend/
├── Controllers/
│   ├── AuthController.cs           # Endpoints: Login, Register, Logout
│   └── TareasController.cs         # Endpoints: CRUD de tareas
├── Models/
│   ├── User.cs                     # Entidad Usuario (BD)
│   ├── Tarea.cs                    # Entidad Tarea (BD)
│   └── DTOs/
│       ├── LoginRequestDto.cs      # DTO para login
│       ├── LoginResponseDto.cs     # DTO respuesta login
│       ├── TareaDto.cs             # DTO para tareas
│       └── ApiResponseDto.cs       # Wrapper de respuestas
├── Data/
│   └── ApplicationDbContext.cs     # DbContext de EF Core
├── Services/
│   ├── ITokenService.cs            # Interfaz servicio JWT
│   └── TokenService.cs             # Generación y validación JWT
├── Database/
│   ├── setup-evaluador.sql         # ⭐ Script principal (USAR ESTE)
│   ├── setup.sql                   # Script original
│   ├── update-password.sql         # Script actualización passwords
│   └── INSTRUCCIONES-EVALUADOR.md  # Guía para evaluador
├── Program.cs                      # Configuración principal de la app
├── appsettings.json                # Configuración (conexión DB, JWT)
├── appsettings.Development.json    # Configuración desarrollo
├── README.md                       # 📄 Esta documentación
├── INSTALACION.md                  # Guía de instalación paso a paso
├── VERIFICACION-REQUISITOS.md      # Verificación de cumplimiento 100%
└── GUIA-ENTREGA.md                 # Guía para entrega académica
```

---

## 🗄️ Base de Datos

### Tablas Implementadas

#### **usuarios**

| Campo                | Tipo         | Descripción               |
| -------------------- | ------------ | ------------------------- |
| `id`                 | INT          | PK, Auto-increment        |
| `usuario`            | VARCHAR(50)  | Usuario único (índice)    |
| `contrasena`         | VARCHAR(255) | Hash SHA256               |
| `token`              | VARCHAR(500) | JWT almacenado (nullable) |
| `fecha_creacion`     | TIMESTAMP    | Fecha de registro         |
| `fecha_ultimo_login` | TIMESTAMP    | Último acceso             |

#### **tareas**

| Campo                 | Tipo          | Descripción           |
| --------------------- | ------------- | --------------------- |
| `id`                  | INT           | PK, Auto-increment    |
| `titulo`              | VARCHAR(200)  | Título de la tarea    |
| `descripcion`         | VARCHAR(1000) | Descripción detallada |
| `completada`          | BOOLEAN       | Estado (true/false)   |
| `fecha_creacion`      | TIMESTAMP     | Fecha de creación     |
| `fecha_actualizacion` | TIMESTAMP     | Última modificación   |
| `usuario_id`          | INT           | FK → usuarios(id)     |

**Relación:** Usuario 1:N Tareas (con eliminación en cascada)

---

## 🔒 Seguridad Implementada

### 🔐 Autenticación

- **Hash de Contraseñas:** SHA256 (Base64)
- **Tokens JWT:** Firmados con HS256
- **Expiración:** 24 horas configurable
- **Claims incluidos:** UserId, Username, Jti (ID único del token)
- **Validación:** Issuer, Audience, Lifetime, Signature

### 🛡️ Autorización

- **Middleware JWT:** Validación automática en cada request
- **Atributo `[Authorize]`:** Protege todos los endpoints de tareas
- **Scope de Usuario:** Cada usuario solo ve/modifica sus propias tareas
- **Logout:** Invalidación de token en BD (token = NULL)

### 🌐 CORS

- **Configurado para desarrollo:** AllowAnyOrigin + AllowAnyHeader
- **⚠️ Producción:** Configurar origins específicos en `Program.cs`

---

## 🧪 Pruebas de la API

### Opción 1: Swagger UI (Recomendado)

1. Ejecuta el backend: `dotnet run`
2. Abre: `http://localhost:7000/swagger`
3. Prueba el login con `testadmin` / `admin123`
4. Copia el token recibido
5. Haz clic en **"Authorize"** (botón candado)
6. Pega: `Bearer {tu_token}`
7. Prueba los endpoints protegidos

### Opción 2: Archivo HTTP (VS Code + REST Client)

```http
### 1. Login
POST http://localhost:7000/api/auth/login
Content-Type: application/json

{
  "usuario": "testadmin",
  "contrasena": "admin123"
}

### 2. Obtener tareas (reemplaza TOKEN)
GET http://localhost:7000/api/tareas
Authorization: Bearer {TOKEN}
```

### Opción 3: cURL

```bash
# Login
curl -X POST http://localhost:7000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"usuario":"testadmin","contrasena":"admin123"}'

# Obtener tareas
curl -X GET http://localhost:7000/api/tareas \
  -H "Authorization: Bearer {tu_token}"
```

### Opción 4: Postman

Importa la colección desde `AuthenticationTokenBackend.http` o crea manualmente los endpoints.

---

## 🛠️ Stack Tecnológico

| Componente         | Tecnología                       | Versión |
| ------------------ | -------------------------------- | ------- |
| **Framework**      | ASP.NET Core                     | 8.0     |
| **Lenguaje**       | C#                               | 12.0    |
| **Base de Datos**  | MySQL                            | 8.0+    |
| **ORM**            | Entity Framework Core            | 8.0.1   |
| **Provider MySQL** | Pomelo.EntityFrameworkCore.MySql | 8.0.0   |
| **Autenticación**  | JWT Bearer                       | 8.0.1   |
| **Documentación**  | Swagger/OpenAPI                  | 6.4.0   |

### Paquetes NuGet Instalados

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.1" />
<PackageReference Include="Pomelo.EntityFrameworkCore.MySql" Version="8.0.0" />
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.1" />
<PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="7.2.0" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.4.0" />
```

---

## ⚙️ Configuración Avanzada

### JWT Settings (appsettings.json)

```json
"JwtSettings": {
  "Secret": "MiClaveSecretaSuperSeguraParaJWT2026ConMinimoDeCaracteres",
  "Issuer": "AuthenticationTokenBackend",
  "Audience": "AuthenticationTokenBackend",
  "ExpirationInHours": 24
}
```

**⚠️ Cambiar para producción:**

- Generar secret de 256+ bits
- Configurar issuer/audience específicos
- Reducir tiempo de expiración (ej: 1-2 horas)

### Variables de Entorno (Producción)

```bash
export ConnectionStrings__DefaultConnection="Server=prod-server;..."
export JwtSettings__Secret="produccion_secret_super_seguro_256bits"
```

---

## 🚨 Solución de Problemas

### ❌ Error: "Unable to connect to MySQL"

**Solución:**

```bash
# Verificar que MySQL esté corriendo
mysql -u root -p

# Verificar puerto en appsettings.json (default 3306)
"Server=localhost;Port=3306;Database=autenticacion_db;..."
```

### ❌ Error: "Unknown database 'autenticacion_db'"

**Solución:**

```bash
# Ejecutar script SQL
mysql -u root -p < Database/setup-evaluador.sql
```

### ❌ Error: 401 Unauthorized al llamar /api/tareas

**Solución:**

1. Verifica que el token esté en el header: `Authorization: Bearer {token}`
2. Verifica que el token no haya expirado (24h de vida)
3. Verifica que el formato sea correcto (incluye "Bearer " antes del token)

### ❌ Error: "Usuario o contraseña incorrectos"

**Solución:**

```sql
-- Verificar hash en BD
SELECT usuario, contrasena FROM usuarios;

-- Hash correcto para "admin123":
-- JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3T9J0o=
```

---

## 📚 Documentación Adicional

- **[INSTALACION.md](INSTALACION.md)** - Guía paso a paso de instalación
- **[VERIFICACION-REQUISITOS.md](VERIFICACION-REQUISITOS.md)** - Verificación 100% de cumplimiento
- **[GUIA-ENTREGA.md](GUIA-ENTREGA.md)** - Guía para entrega académica
- **[Database/INSTRUCCIONES-EVALUADOR.md](Database/INSTRUCCIONES-EVALUADOR.md)** - Instrucciones para evaluador

---

## 📦 Para Desplegar en Producción

### Checklist de Seguridad

- [ ] Cambiar `JwtSettings:Secret` a valor seguro (256+ bits)
- [ ] Configurar CORS con origins específicos (no `AllowAnyOrigin`)
- [ ] Usar variables de entorno para datos sensibles
- [ ] Habilitar HTTPS obligatorio
- [ ] Implementar rate limiting
- [ ] Agregar logging estructurado (Serilog)
- [ ] Considerar usar ASP.NET Core Identity
- [ ] Implementar refresh tokens
- [ ] Configurar respaldo automático de BD

---

## 📄 Licencia

Este proyecto es de uso académico para la **Actividad 3 - Consumo tipo BaaS con autenticación simulada (token)**.

---

## 👨‍💻 Autor

Desarrollado como parte de la evaluación de conceptos de autenticación JWT, protección de rutas y desarrollo de APIs RESTful con ASP.NET Core.

---

## 📞 Soporte

Para problemas o dudas sobre la implementación, revisar:

1. Esta documentación completa
2. Logs del backend (ejecutar con `dotnet run`)
3. Logs de MySQL (`/var/log/mysql/error.log`)
4. Swagger UI para probar endpoints interactivamente

---

**✅ Proyecto 100% funcional y listo para evaluación**
