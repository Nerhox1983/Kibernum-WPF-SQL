CREATE DATABASE UserM;
GO
USE UserM;
GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Areas]') AND type in (N'U'))
BEGIN
    CREATE TABLE Areas (
        IdArea INT IDENTITY(1,1) PRIMARY KEY,
        Nombre NVARCHAR(50) NOT NULL UNIQUE        
    );
    PRINT 'Tabla Areas creada exitosamente.';
END
GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Roles]') AND type in (N'U'))
BEGIN
    CREATE TABLE Roles (
        IdRol INT IDENTITY(1,1) PRIMARY KEY,
        Nombre NVARCHAR(50) NOT NULL UNIQUE        
    );
    PRINT 'Tabla Roles creada exitosamente.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Usuarios]') AND type in (N'U'))
BEGIN
    CREATE TABLE Usuarios (
        IdUsuario INT IDENTITY(1,1) PRIMARY KEY,
        Nombre NVARCHAR(50) NOT NULL,
        Apellido NVARCHAR(50) NOT NULL,
        Correo NVARCHAR(100) UNIQUE NOT NULL,
        Telefono NVARCHAR(20),
        IdArea INT NOT NULL,
        IdRol INT NOT NULL,
        FechaRegistro DATETIME DEFAULT GETDATE(),
        Activo BIT DEFAULT 1,

        CONSTRAINT FK_Usuarios_Areas FOREIGN KEY (IdArea) REFERENCES Areas(IdArea),
        CONSTRAINT FK_Usuarios_Roles FOREIGN KEY (IdRol) REFERENCES Roles(IdRol)
    );
    PRINT 'Tabla Usuarios creada exitosamente.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Auditoria]') AND type in (N'U'))
BEGIN
    CREATE TABLE Auditoria (
        IdAuditoria INT IDENTITY(1,1) PRIMARY KEY,
        TablaAfectada NVARCHAR(50) NOT NULL,      
        IdRegistroAfectado INT NOT NULL,           
        Operacion NVARCHAR(10) NOT NULL,           
        DetalleOperacion NVARCHAR(MAX),            
        UsuarioResponsable NVARCHAR(100) NOT NULL, 
        FechaAccion DATETIME DEFAULT GETDATE()
    );
    PRINT 'Tabla Auditoria creada exitosamente.';
END
GO