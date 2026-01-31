# Authentication Token Backend

Backend BaaS desarrollado en ASP.NET Core para gestión de autenticación y tareas con tokens JWT.

## Actividad 3 - Consumo tipo BaaS con autenticación simulada (token)

### Objetivo

Verificar la implementación de un flujo de autenticación, autorización y protección de rutas, utilizando un backend que centralice la lógica de seguridad y persistencia.

## Características

### Requerimientos Funcionales Implementados

1. ✅ Login con usuario y contraseña
2. ✅ Emisión y almacenamiento de token JWT
3. ✅ Módulo de tareas (CRUD completo)
4. ✅ Protección de rutas y cierre de sesión

### Requerimientos Técnicos Implementados

1. ✅ Backend desarrollado en ASP.NET Core (.NET 8)
2. ✅ Base de datos MySQL para usuarios y tareas
3. ✅ Endpoints autenticados mediante token JWT
4. ✅ Interceptor Authorization Bearer
5. ✅ Manejo centralizado de errores de autenticación

## Stack Tecnológico

- **Framework**: ASP.NET Core 8.0
- **Base de Datos**: MySQL
- **ORM**: Entity Framework Core
- **Autenticación**: JWT (JSON Web Tokens)
- **Documentación**: Swagger/OpenAPI

## Estructura del Proyecto

```
AuthenticationTokenBackend/
├── Controllers/
│   ├── AuthController.cs       # Login, Register, Logout
│   └── TareasController.cs     # CRUD de tareas
├── Data/
│   └── ApplicationDbContext.cs # Contexto de Entity Framework
├── Database/
│   └── setup.sql               # Script de base de datos
├── Models/
│   ├── User.cs                 # Modelo de usuario
│   ├── Tarea.cs                # Modelo de tarea
│   └── DTOs/                   # Data Transfer Objects
├── Services/
│   └── TokenService.cs         # Servicio de generación JWT
├── Program.cs                  # Configuración principal
└── appsettings.json           # Configuración de la aplicación
```

## Configuración

### 1. Base de Datos

Edita `appsettings.json` con tus credenciales de MySQL:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=autenticacion_db;User=root;Password=tu_password;"
}
```

### 2. Crear la Base de Datos

Ejecuta el script SQL ubicado en `Database/setup.sql`:

```bash
mysql -u root -p < Database/setup.sql
```

### 3. Restaurar Paquetes

```bash
dotnet restore
```

### 4. Ejecutar el Proyecto

```bash
dotnet run
```

El servidor estará disponible en:

- HTTPS: `https://localhost:7XXX`
- HTTP: `http://localhost:5XXX`
- Swagger UI: `https://localhost:7XXX/swagger`

## Endpoints de la API

### Autenticación

#### POST /api/auth/register

Registra un nuevo usuario.

**Body:**

```json
{
  "usuario": "nombre_usuario",
  "contrasena": "password123"
}
```

**Response:**

```json
{
  "success": true,
  "message": "Usuario registrado exitosamente",
  "data": {
    "id": 1,
    "usuario": "nombre_usuario",
    "token": "eyJhbGciOiJIUzI1NiIs...",
    "fechaExpiracion": "2026-02-01T00:00:00Z"
  }
}
```

#### POST /api/auth/login

Inicia sesión y obtiene un token JWT.

**Body:**

```json
{
  "usuario": "admin",
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
    "usuario": "admin",
    "token": "eyJhbGciOiJIUzI1NiIs...",
    "fechaExpiracion": "2026-02-01T00:00:00Z"
  }
}
```

#### POST /api/auth/logout

Cierra la sesión del usuario autenticado.

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

### Tareas (Requiere Autenticación)

Todos los endpoints de tareas requieren el header:

```
Authorization: Bearer {token}
```

#### GET /api/tareas

Obtiene todas las tareas del usuario autenticado.

**Response:**

```json
{
  "success": true,
  "message": "Tareas obtenidas exitosamente",
  "data": [
    {
      "id": 1,
      "titulo": "Completar backend",
      "descripcion": "Implementar todos los endpoints...",
      "completada": true,
      "fechaCreacion": "2026-01-31T00:00:00Z",
      "fechaActualizacion": null
    }
  ]
}
```

#### GET /api/tareas/{id}

Obtiene una tarea específica por ID.

**Response:**

```json
{
  "success": true,
  "message": "Tarea obtenida exitosamente",
  "data": {
    "id": 1,
    "titulo": "Completar backend",
    "descripcion": "Implementar todos los endpoints...",
    "completada": true,
    "fechaCreacion": "2026-01-31T00:00:00Z",
    "fechaActualizacion": null
  }
}
```

#### POST /api/tareas

Crea una nueva tarea.

**Body:**

```json
{
  "titulo": "Nueva tarea",
  "descripcion": "Descripción de la tarea",
  "completada": false
}
```

**Response:**

```json
{
  "success": true,
  "message": "Tarea creada exitosamente",
  "data": {
    "id": 4,
    "titulo": "Nueva tarea",
    "descripcion": "Descripción de la tarea",
    "completada": false,
    "fechaCreacion": "2026-01-31T00:00:00Z"
  }
}
```

#### PUT /api/tareas/{id}

Actualiza una tarea existente.

**Body:**

```json
{
  "titulo": "Tarea actualizada",
  "descripcion": "Nueva descripción",
  "completada": true
}
```

**Response:**

```json
{
  "success": true,
  "message": "Tarea actualizada exitosamente",
  "data": {
    "id": 1,
    "titulo": "Tarea actualizada",
    "descripcion": "Nueva descripción",
    "completada": true,
    "fechaCreacion": "2026-01-31T00:00:00Z",
    "fechaActualizacion": "2026-01-31T12:00:00Z"
  }
}
```

#### DELETE /api/tareas/{id}

Elimina una tarea.

**Response:**

```json
{
  "success": true,
  "message": "Tarea eliminada exitosamente",
  "data": null
}
```

## Usuarios de Prueba

El script SQL incluye estos usuarios de prueba:

| Usuario  | Contraseña | Tareas   |
| -------- | ---------- | -------- |
| admin    | admin123   | 3 tareas |
| usuario1 | user123    | 2 tareas |

## Seguridad

- Las contraseñas se almacenan hasheadas con SHA256
- Los tokens JWT expiran en 24 horas
- Todos los endpoints de tareas requieren autenticación
- Las tareas están asociadas al usuario autenticado (no se pueden ver/editar tareas de otros usuarios)
- CORS habilitado para desarrollo (ajustar en producción)

## Swagger UI

La documentación interactiva está disponible en `/swagger` cuando el proyecto está corriendo.

Para probar endpoints protegidos:

1. Usa `/api/auth/login` para obtener un token
2. Haz clic en el botón "Authorize" en Swagger
3. Ingresa: `Bearer {tu_token}`
4. Prueba los endpoints protegidos

## Desarrollo

### Agregar Migraciones (opcional)

Si necesitas usar migraciones de Entity Framework:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Variables de Entorno

Puedes sobrescribir la configuración usando variables de entorno:

```bash
ConnectionStrings__DefaultConnection="Server=localhost;Database=autenticacion_db;..."
JwtSettings__Secret="tu_clave_secreta"
```

## Próximos Pasos

1. Crear el frontend que consuma este backend
2. Implementar refresh tokens para mayor seguridad
3. Agregar validaciones adicionales
4. Implementar rate limiting
5. Agregar logs y monitoreo

## Licencia

Este proyecto es de uso educativo para la Actividad 3.
