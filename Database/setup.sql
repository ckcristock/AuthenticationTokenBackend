-- Script de base de datos MySQL para Authentication Token Backend
-- Actividad 3 - Consumo tipo BaaS con autenticación simulada (token)

-- Crear la base de datos
CREATE DATABASE IF NOT EXISTS autenticacion_db
CHARACTER SET utf8mb4
COLLATE utf8mb4_unicode_ci;

USE autenticacion_db;

-- Tabla de usuarios
CREATE TABLE IF NOT EXISTS usuarios (
    id INT AUTO_INCREMENT PRIMARY KEY,
    usuario VARCHAR(50) NOT NULL UNIQUE,
    contrasena VARCHAR(255) NOT NULL,
    token VARCHAR(500) NULL,
    fecha_creacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    fecha_ultimo_login TIMESTAMP NULL,
    INDEX idx_usuario (usuario),
    INDEX idx_token (token(255))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Tabla de tareas
CREATE TABLE IF NOT EXISTS tareas (
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
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Insertar datos de prueba
-- Contraseña: admin123 (hashed con SHA256: jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=)
INSERT INTO usuarios (usuario, contrasena) VALUES 
('admin', 'jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=');

-- Contraseña: user123 (hashed con SHA256: BPiZbadjt6lpsQKO4wB1aerzpjVIbdqyEdUSyFud+Ps=)
INSERT INTO usuarios (usuario, contrasena) VALUES 
('usuario1', 'BPiZbadjt6lpsQKO4wB1aerzpjVIbdqyEdUSyFud+Ps=');

-- Insertar tareas de ejemplo para el usuario admin (id=1)
INSERT INTO tareas (titulo, descripcion, completada, usuario_id) VALUES 
('Completar backend', 'Implementar todos los endpoints del backend con autenticación JWT', TRUE, 1),
('Crear frontend', 'Desarrollar el frontend que consume el backend', FALSE, 1),
('Documentar API', 'Crear documentación completa de la API', FALSE, 1);

-- Insertar tareas de ejemplo para usuario1 (id=2)
INSERT INTO tareas (titulo, descripcion, completada, usuario_id) VALUES 
('Aprender .NET', 'Estudiar ASP.NET Core y Entity Framework', TRUE, 2),
('Practicar MySQL', 'Practicar consultas SQL avanzadas', FALSE, 2);

-- Consultas de verificación
SELECT * FROM usuarios;
SELECT * FROM tareas;

-- Ver tareas con información de usuario
SELECT 
    t.id,
    t.titulo,
    t.descripcion,
    t.completada,
    t.fecha_creacion,
    u.usuario
FROM tareas t
INNER JOIN usuarios u ON t.usuario_id = u.id
ORDER BY t.fecha_creacion DESC;
