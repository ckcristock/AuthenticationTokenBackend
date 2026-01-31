-- =====================================================
-- Script de Base de Datos - Authentication Token Backend
-- Actividad 3: Consumo tipo BaaS con autenticación (token)
-- =====================================================
-- Este script configura completamente la base de datos
-- para que el evaluador pueda probar el sistema
-- =====================================================

-- Crear y usar la base de datos
CREATE DATABASE IF NOT EXISTS autenticacion_db
CHARACTER SET utf8mb4
COLLATE utf8mb4_unicode_ci;

USE autenticacion_db;

-- =====================================================
-- ELIMINAR TABLAS EXISTENTES (si existen)
-- =====================================================
DROP TABLE IF EXISTS tareas;
DROP TABLE IF EXISTS usuarios;

-- =====================================================
-- CREAR TABLA DE USUARIOS
-- =====================================================
CREATE TABLE usuarios (
    id INT AUTO_INCREMENT PRIMARY KEY,
    usuario VARCHAR(50) NOT NULL UNIQUE,
    contrasena VARCHAR(255) NOT NULL COMMENT 'Hash SHA256 de la contraseña',
    token VARCHAR(500) NULL COMMENT 'Token JWT actual del usuario',
    fecha_creacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    fecha_ultimo_login TIMESTAMP NULL,
    INDEX idx_usuario (usuario),
    INDEX idx_token (token(255))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
COMMENT='Tabla de usuarios del sistema';

-- =====================================================
-- CREAR TABLA DE TAREAS
-- =====================================================
CREATE TABLE tareas (
    id INT AUTO_INCREMENT PRIMARY KEY,
    titulo VARCHAR(200) NOT NULL,
    descripcion VARCHAR(1000) NULL,
    completada BOOLEAN DEFAULT FALSE,
    fecha_creacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    fecha_actualizacion TIMESTAMP NULL,
    usuario_id INT NOT NULL,
    FOREIGN KEY (usuario_id) REFERENCES usuarios(id) ON DELETE CASCADE,
    INDEX idx_usuario_id (usuario_id),
    INDEX idx_completada (completada)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
COMMENT='Tabla de tareas del sistema CRUD';

-- =====================================================
-- INSERTAR USUARIOS DE PRUEBA
-- =====================================================
-- Hash SHA256 de "admin123": JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3T9J0o=
-- Nota: El backend hashea las contraseñas con SHA256 antes de compararlas

INSERT INTO usuarios (usuario, contrasena, fecha_creacion) VALUES 
('testadmin', 'JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3T9J0o=', NOW()),
('usuario1', 'JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3T9J0o=', NOW()),
('evaluador', 'JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3T9J0o=', NOW());

-- =====================================================
-- INSERTAR TAREAS PROFESIONALES PARA testadmin
-- =====================================================
INSERT INTO tareas (titulo, descripcion, completada, usuario_id, fecha_creacion) VALUES 
(
    'Implementar autenticación JWT',
    'Desarrollar sistema de autenticación basado en tokens JWT con emisión, validación y expiración automática de 24 horas.',
    TRUE,
    (SELECT id FROM usuarios WHERE usuario = 'testadmin'),
    '2026-01-29 10:00:00'
),
(
    'Configurar Entity Framework Core',
    'Configurar el ORM Entity Framework Core con Pomelo MySQL provider para gestionar las operaciones de base de datos.',
    TRUE,
    (SELECT id FROM usuarios WHERE usuario = 'testadmin'),
    '2026-01-29 11:30:00'
),
(
    'Desarrollar endpoints CRUD de tareas',
    'Implementar los endpoints RESTful para crear, leer, actualizar y eliminar tareas con validaciones completas.',
    TRUE,
    (SELECT id FROM usuarios WHERE usuario = 'testadmin'),
    '2026-01-30 09:15:00'
),
(
    'Implementar middleware de autorización',
    'Configurar middleware para proteger rutas y validar tokens JWT en cada petición autenticada.',
    TRUE,
    (SELECT id FROM usuarios WHERE usuario = 'testadmin'),
    '2026-01-30 14:20:00'
),
(
    'Documentar API con Swagger',
    'Generar documentación interactiva de la API utilizando Swagger/OpenAPI con ejemplos de uso.',
    FALSE,
    (SELECT id FROM usuarios WHERE usuario = 'testadmin'),
    '2026-01-31 08:00:00'
),
(
    'Optimizar consultas de base de datos',
    'Revisar y optimizar las consultas SQL generadas por Entity Framework, agregar índices necesarios.',
    FALSE,
    (SELECT id FROM usuarios WHERE usuario = 'testadmin'),
    '2026-01-31 10:45:00'
);

-- =====================================================
-- INSERTAR TAREAS PARA usuario1
-- =====================================================
INSERT INTO tareas (titulo, descripcion, completada, usuario_id, fecha_creacion) VALUES 
(
    'Aprender ASP.NET Core 8.0',
    'Estudiar los fundamentos de ASP.NET Core, incluyendo dependency injection, middleware y configuración.',
    TRUE,
    (SELECT id FROM usuarios WHERE usuario = 'usuario1'),
    '2026-01-28 09:00:00'
),
(
    'Practicar consultas SQL avanzadas',
    'Realizar ejercicios de consultas complejas con JOIN, subconsultas y optimización de índices.',
    FALSE,
    (SELECT id FROM usuarios WHERE usuario = 'usuario1'),
    '2026-01-30 15:30:00'
),
(
    'Implementar validaciones con Data Annotations',
    'Aplicar validaciones en los DTOs utilizando Data Annotations de ASP.NET Core.',
    FALSE,
    (SELECT id FROM usuarios WHERE usuario = 'usuario1'),
    '2026-01-31 11:00:00'
);

-- =====================================================
-- INSERTAR TAREAS PARA evaluador
-- =====================================================
INSERT INTO tareas (titulo, descripcion, completada, usuario_id, fecha_creacion) VALUES 
(
    'Revisar arquitectura del backend',
    'Evaluar la estructura del proyecto, separación de responsabilidades y patrones implementados.',
    FALSE,
    (SELECT id FROM usuarios WHERE usuario = 'evaluador'),
    NOW()
),
(
    'Probar endpoints de autenticación',
    'Verificar funcionamiento de login, registro y logout con diferentes casos de prueba.',
    FALSE,
    (SELECT id FROM usuarios WHERE usuario = 'evaluador'),
    NOW()
);

-- =====================================================
-- CONSULTAS DE VERIFICACIÓN
-- =====================================================

-- Ver todos los usuarios
SELECT 
    id,
    usuario,
    LEFT(contrasena, 20) as contrasena_hash,
    fecha_creacion,
    fecha_ultimo_login
FROM usuarios
ORDER BY id;

-- Ver todas las tareas con información del usuario
SELECT 
    t.id,
    t.titulo,
    LEFT(t.descripcion, 50) as descripcion_corta,
    t.completada,
    t.fecha_creacion,
    u.usuario as creado_por
FROM tareas t
INNER JOIN usuarios u ON t.usuario_id = u.id
ORDER BY t.fecha_creacion DESC;

-- Estadísticas por usuario
SELECT 
    u.usuario,
    COUNT(t.id) as total_tareas,
    SUM(CASE WHEN t.completada = TRUE THEN 1 ELSE 0 END) as completadas,
    SUM(CASE WHEN t.completada = FALSE THEN 1 ELSE 0 END) as pendientes
FROM usuarios u
LEFT JOIN tareas t ON u.id = t.usuario_id
GROUP BY u.id, u.usuario
ORDER BY u.usuario;

-- =====================================================
-- INFORMACIÓN IMPORTANTE PARA EL EVALUADOR
-- =====================================================

SELECT '========================================' as '';
SELECT 'CREDENCIALES DE ACCESO' as '';
SELECT '========================================' as '';
SELECT '' as '';
SELECT 'Usuario: testadmin | Contraseña: admin123' as credenciales;
SELECT 'Usuario: usuario1   | Contraseña: admin123' as credenciales;
SELECT 'Usuario: evaluador  | Contraseña: admin123' as credenciales;
SELECT '' as '';
SELECT '========================================' as '';
SELECT 'ENDPOINTS DISPONIBLES' as '';
SELECT '========================================' as '';
SELECT '' as '';
SELECT 'POST /api/auth/login    - Iniciar sesión' as endpoint;
SELECT 'POST /api/auth/register - Registrar usuario' as endpoint;
SELECT 'POST /api/auth/logout   - Cerrar sesión (requiere token)' as endpoint;
SELECT 'GET  /api/tareas        - Listar todas las tareas (requiere token)' as endpoint;
SELECT 'GET  /api/tareas/{id}   - Obtener tarea específica (requiere token)' as endpoint;
SELECT 'POST /api/tareas        - Crear nueva tarea (requiere token)' as endpoint;
SELECT 'PUT  /api/tareas/{id}   - Actualizar tarea (requiere token)' as endpoint;
SELECT 'DELETE /api/tareas/{id} - Eliminar tarea (requiere token)' as endpoint;
SELECT '' as '';
SELECT '========================================' as '';
SELECT 'Base de datos configurada correctamente!' as resultado;
SELECT '========================================' as '';
