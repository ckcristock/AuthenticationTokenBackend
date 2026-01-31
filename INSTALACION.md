# Guía de Instalación y Configuración

## Requisitos Previos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MySQL Server 8.0 o superior](https://dev.mysql.com/downloads/mysql/)
- Visual Studio Code o Visual Studio 2022 (opcional)

## Paso 1: Configurar MySQL

1. Asegúrate de que MySQL esté instalado y corriendo
2. Abre MySQL Workbench o la terminal de MySQL
3. Ejecuta el script de base de datos:

```bash
mysql -u root -p < Database/setup.sql
```

O desde MySQL Workbench, abre y ejecuta el archivo `Database/setup.sql`

## Paso 2: Configurar la Conexión a la Base de Datos

Edita el archivo `appsettings.json` y actualiza la cadena de conexión:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=autenticacion_db;User=root;Password=TU_PASSWORD_DE_MYSQL;"
}
```

**Importante**: Reemplaza `TU_PASSWORD_DE_MYSQL` con tu contraseña real de MySQL.

## Paso 3: Configurar JWT Secret (Opcional)

Para producción, cambia el secreto JWT en `appsettings.json`:

```json
"JwtSettings": {
  "Secret": "TuPropiaClaveSuperSecretaQueDebeSerMuyLarga",
  "Issuer": "AuthenticationTokenBackend",
  "Audience": "AuthenticationTokenBackend",
  "ExpirationInHours": 24
}
```

## Paso 4: Restaurar Paquetes

Abre una terminal en el directorio del proyecto y ejecuta:

```bash
dotnet restore
```

## Paso 5: Compilar el Proyecto

```bash
dotnet build
```

Si todo está correcto, deberías ver:

```
Compilación correcta.
    0 Advertencia(s)
    0 Errores
```

## Paso 6: Ejecutar el Proyecto

```bash
dotnet run
```

El servidor se iniciará y verás algo como:

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7XXX
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5XXX
```

## Paso 7: Probar la API

### Opción 1: Usar Swagger UI

Abre tu navegador y ve a:

```
https://localhost:7XXX/swagger
```

### Opción 2: Usar el archivo HTTP

Si usas VS Code con la extensión REST Client:

1. Abre `test-endpoints.http`
2. Reemplaza `7XXX` con tu puerto HTTPS real
3. Haz clic en "Send Request" sobre cada endpoint

### Opción 3: Usar cURL

```bash
# Login
curl -X POST https://localhost:7XXX/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"usuario":"admin","contrasena":"admin123"}'

# Obtener tareas (reemplaza TOKEN con el token obtenido del login)
curl -X GET https://localhost:7XXX/api/tareas \
  -H "Authorization: Bearer TOKEN"
```

### Opción 4: Usar Postman

1. Importa la colección desde el archivo `test-endpoints.http`
2. O crea las peticiones manualmente siguiendo la documentación en README.md

## Verificación de la Instalación

### 1. Verificar la Base de Datos

Conéctate a MySQL y verifica que las tablas fueron creadas:

```sql
USE autenticacion_db;
SHOW TABLES;
SELECT * FROM usuarios;
SELECT * FROM tareas;
```

Deberías ver:

- 2 usuarios (admin y usuario1)
- 5 tareas de ejemplo

### 2. Probar el Login

Desde Swagger o cURL, prueba hacer login con:

- Usuario: `admin`
- Contraseña: `admin123`

Deberías recibir un token JWT.

### 3. Probar un Endpoint Protegido

Usa el token obtenido para hacer una petición GET a `/api/tareas`

## Solución de Problemas

### Error: "Unable to connect to any of the specified MySQL hosts"

**Problema**: No se puede conectar a MySQL
**Solución**:

- Verifica que MySQL esté corriendo: `mysql -u root -p`
- Verifica la cadena de conexión en `appsettings.json`
- Verifica que el puerto sea el correcto (por defecto 3306)

### Error: "Access denied for user"

**Problema**: Credenciales incorrectas
**Solución**:

- Verifica que el usuario y contraseña en `appsettings.json` sean correctos
- Verifica los permisos del usuario en MySQL

### Error: "Unknown database 'autenticacion_db'"

**Problema**: La base de datos no existe
**Solución**:

- Ejecuta el script `Database/setup.sql`
- O crea la base de datos manualmente: `CREATE DATABASE autenticacion_db;`

### El puerto HTTPS muestra advertencia de certificado

**Problema**: Certificado de desarrollo no confiable
**Solución**:

```bash
dotnet dev-certs https --trust
```

### "JwtSettings:Secret no configurado"

**Problema**: Falta la configuración de JWT
**Solución**:

- Verifica que `appsettings.json` tenga la sección `JwtSettings`
- El archivo ya debería tener esta configuración

## Despliegue en Producción

Para despliegue en producción:

1. **Cambia el secreto JWT** en `appsettings.json`
2. **Configura CORS** apropiadamente en `Program.cs` (no uses "AllowAll")
3. **Usa variables de entorno** para datos sensibles
4. **Habilita HTTPS** obligatorio
5. **Considera usar ASP.NET Core Identity** para manejo de usuarios más robusto
6. **Implementa rate limiting** para prevenir abuso
7. **Agrega logging** apropiado

### Variables de Entorno (Recomendado para Producción)

En lugar de `appsettings.json`, usa variables de entorno:

```bash
export ConnectionStrings__DefaultConnection="Server=..."
export JwtSettings__Secret="..."
```

## Próximos Pasos

1. Revisa la documentación completa en `README.md`
2. Explora todos los endpoints en Swagger
3. Crea tu frontend que consuma esta API
4. Implementa funcionalidades adicionales según tus necesidades

## Soporte

Si encuentras problemas:

1. Revisa los logs de la aplicación
2. Verifica los logs de MySQL
3. Asegúrate de que todos los paquetes estén instalados correctamente
4. Verifica que la versión de .NET sea la 8.0

## Estructura de Respuestas de la API

Todas las respuestas siguen este formato:

```json
{
  "success": true/false,
  "message": "Mensaje descriptivo",
  "data": { ... },  // Solo si success es true
  "errors": [ ... ] // Solo si success es false
}
```

¡Listo! Tu backend está configurado y corriendo. 🚀
