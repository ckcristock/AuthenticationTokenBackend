-- Actualizar contraseña del usuario admin
-- Hash SHA256 de "admin123": JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3T9J0o=

USE autenticacion_db;

-- Actualizar usuario admin
UPDATE usuarios 
SET contrasena = 'JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3T9J0o=' 
WHERE usuario = 'admin';

-- Actualizar usuario1 también con la misma contraseña para pruebas
UPDATE usuarios 
SET contrasena = 'JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3T9J0o=' 
WHERE usuario = 'usuario1';

-- Verificar los cambios
SELECT id, usuario, contrasena, fecha_creacion 
FROM usuarios;

-- Mensaje de confirmación
SELECT 'Contraseñas actualizadas correctamente!' AS resultado;
SELECT 'Usuario: admin | Contraseña: admin123' AS credenciales;
SELECT 'Usuario: usuario1 | Contraseña: admin123' AS credenciales2;
