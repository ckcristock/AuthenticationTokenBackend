# Instrucciones para el Evaluador

## 📋 Configuración de la Base de Datos

### Opción 1: Ejecutar desde línea de comandos

```bash
mysql -u root -p < Database/setup-evaluador.sql
```

### Opción 2: Ejecutar desde MySQL Workbench

1. Abrir MySQL Workbench
2. Conectarse al servidor local
3. File > Open SQL Script
4. Seleccionar: `Database/setup-evaluador.sql`
5. Ejecutar el script (botón ⚡ o Ctrl+Shift+Enter)

### Opción 3: Ejecutar desde phpMyAdmin

1. Abrir phpMyAdmin
2. Ir a la pestaña "Importar"
3. Seleccionar el archivo `Database/setup-evaluador.sql`
4. Hacer clic en "Continuar"

## 🔐 Credenciales de Acceso

La base de datos incluye 3 usuarios de prueba:

| Usuario   | Contraseña | Tareas   | Descripción                               |
| --------- | ---------- | -------- | ----------------------------------------- |
| testadmin | admin123   | 6 tareas | Usuario principal con tareas del proyecto |
| usuario1  | admin123   | 3 tareas | Usuario secundario para pruebas           |
| evaluador | admin123   | 2 tareas | Usuario específico para evaluación        |

**Nota:** Todas las contraseñas están hasheadas con SHA256 en la base de datos.

## 🚀 Ejecutar el Backend

```bash
# Navegar al directorio del proyecto
cd AuthenticationTokenBackend

# Restaurar paquetes (si es necesario)
dotnet restore

# Ejecutar el backend
dotnet run
```

El backend estará disponible en:

- **HTTP:** `http://localhost:7000`
- **Swagger:** `http://localhost:7000/swagger`

## 📝 Estructura de las Tareas

El script crea tareas profesionales que reflejan el desarrollo real del proyecto:

### Usuario: testadmin (6 tareas)

- ✅ Implementar autenticación JWT
- ✅ Configurar Entity Framework Core
- ✅ Desarrollar endpoints CRUD de tareas
- ✅ Implementar middleware de autorización
- ⏳ Documentar API con Swagger
- ⏳ Optimizar consultas de base de datos

### Usuario: usuario1 (3 tareas)

- ✅ Aprender ASP.NET Core 8.0
- ⏳ Practicar consultas SQL avanzadas
- ⏳ Implementar validaciones con Data Annotations

### Usuario: evaluador (2 tareas)

- ⏳ Revisar arquitectura del backend
- ⏳ Probar endpoints de autenticación

## 🧪 Probar la API

### 1. Login

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
    "id": 1,
    "usuario": "testadmin",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "fechaExpiracion": "2026-02-01T00:00:00Z"
  }
}
```

### 2. Obtener Tareas (requiere token)

```bash
GET http://localhost:7000/api/tareas
Authorization: Bearer {token_obtenido_del_login}
```

### 3. Crear Tarea

```bash
POST http://localhost:7000/api/tareas
Authorization: Bearer {token_obtenido_del_login}
Content-Type: application/json

{
  "titulo": "Nueva tarea de prueba",
  "descripcion": "Esta es una tarea creada por el evaluador",
  "completada": false
}
```

## ✅ Verificación de la Base de Datos

Después de ejecutar el script, puedes verificar que todo está correcto:

```sql
-- Ver todos los usuarios
SELECT id, usuario, fecha_creacion FROM usuarios;

-- Ver estadísticas de tareas por usuario
SELECT
    u.usuario,
    COUNT(t.id) as total_tareas,
    SUM(CASE WHEN t.completada = TRUE THEN 1 ELSE 0 END) as completadas,
    SUM(CASE WHEN t.completada = FALSE THEN 1 ELSE 0 END) as pendientes
FROM usuarios u
LEFT JOIN tareas t ON u.id = t.usuario_id
GROUP BY u.id, u.usuario;
```

## 🔍 Características Implementadas

✅ Sistema de autenticación con JWT
✅ Hash de contraseñas con SHA256
✅ CRUD completo de tareas
✅ Validación de tokens en endpoints protegidos
✅ Relaciones entre usuarios y tareas
✅ Eliminación en cascada
✅ Índices para optimización
✅ Timestamps automáticos
✅ CORS configurado
✅ Documentación con Swagger

## 📊 Endpoints Protegidos

Todos los endpoints de tareas requieren el header:

```
Authorization: Bearer {token}
```

El token se obtiene al hacer login y expira en 24 horas.

## 🎯 Puntos de Evaluación Cubiertos

1. ✅ **Login con usuario y contraseña** - `/api/auth/login`
2. ✅ **Emisión y almacenamiento de token** - Token JWT guardado en BD
3. ✅ **Módulo de tareas (CRUD)** - Endpoints completos
4. ✅ **Protección de rutas** - Middleware de autorización
5. ✅ **Cierre de sesión** - `/api/auth/logout`
6. ✅ **Backend ASP.NET Core** - .NET 8.0
7. ✅ **Base de datos MySQL** - Entity Framework Core
8. ✅ **Autenticación JWT Bearer** - Configurado y funcionando

---

**¡El sistema está listo para ser evaluado!** 🎉
