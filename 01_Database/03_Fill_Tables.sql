USE UserM;
GO

-- 1. Población de Áreas
EXEC sp_InsertarArea @Nombre = 'Nómina', @UsuarioResponsable = 'App_WPF_User';
EXEC sp_InsertarArea @Nombre = 'Facturación', @UsuarioResponsable = 'App_WPF_User';
EXEC sp_InsertarArea @Nombre = 'Servicio al cliente', @UsuarioResponsable = 'App_WPF_User';
EXEC sp_InsertarArea @Nombre = 'IT', @UsuarioResponsable = 'App_WPF_User';
GO

-- 2. Población de Roles
EXEC sp_InsertarRol @Nombre = 'Administrador', @UsuarioResponsable = 'App_WPF_User';
EXEC sp_InsertarRol @Nombre = 'Operador', @UsuarioResponsable = 'App_WPF_User';
EXEC sp_InsertarRol @Nombre = 'Auditor', @UsuarioResponsable = 'App_WPF_User';
EXEC sp_InsertarRol @Nombre = 'Consultor', @UsuarioResponsable = 'App_WPF_User';
EXEC sp_InsertarRol @Nombre = 'Gerente', @UsuarioResponsable = 'App_WPF_User';
GO

-- 4. Verificación
SELECT * FROM Areas;
SELECT * FROM Roles;
SELECT * FROM Auditoria;
GO