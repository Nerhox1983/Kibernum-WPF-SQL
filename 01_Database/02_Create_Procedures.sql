USE UserM;
GO

DROP PROCEDURE IF EXISTS sp_RegistrarAuditoria;
GO

CREATE PROCEDURE sp_RegistrarAuditoria
    @TablaAfectada NVARCHAR(100),
    @IdRegistroAfectado INT,
    @Operacion NVARCHAR(20),
    @DetalleOperacion NVARCHAR(MAX),
    @UsuarioResponsable NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;    
    
    INSERT INTO Auditoria (
        TablaAfectada, 
        IdRegistroAfectado, 
        Operacion, 
        DetalleOperacion, 
        UsuarioResponsable, 
        FechaAccion
    )
    VALUES (
        @TablaAfectada,
        @IdRegistroAfectado,
        UPPER(@Operacion),
        @DetalleOperacion,
        @UsuarioResponsable,
        GETDATE()
    );
END;
GO

DROP PROCEDURE IF EXISTS sp_InsertarArea;
GO

CREATE PROCEDURE sp_InsertarArea
    @Nombre NVARCHAR(50),    
    @UsuarioResponsable NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO dbo.Areas (Nombre)
        VALUES (@Nombre);

        DECLARE @NuevoId INT = SCOPE_IDENTITY();
       
        DECLARE @Detalle NVARCHAR(MAX) = CONCAT('Registro de Area: ', @Nombre);

        EXEC sp_RegistrarAuditoria 
            @TablaAfectada = 'Areas', 
            @IdRegistroAfectado = @NuevoId, 
            @Operacion = 'INSERT', 
            @DetalleOperacion = @Detalle, 
            @UsuarioResponsable = @UsuarioResponsable;
        
        COMMIT TRANSACTION;
        
        SELECT 'Area registrada con éxito' AS Mensaje, @NuevoId AS IdGenerado;        

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 
            ROLLBACK TRANSACTION;
        
        THROW;
    END CATCH
END;
GO

DROP PROCEDURE IF EXISTS sp_ActualizarArea;
GO

CREATE PROCEDURE sp_ActualizarArea
    @IdArea INT,
    @Nombre NVARCHAR(50),
    @UsuarioResponsable NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM dbo.Areas WHERE IdArea = @IdArea)
        BEGIN
            RAISERROR('El área especificada no existe.', 16, 1);
            RETURN;
        END

        BEGIN TRANSACTION;
        
        UPDATE dbo.Areas 
        SET Nombre = @Nombre
        WHERE IdArea = @IdArea;

        DECLARE @Detalle NVARCHAR(MAX) = CONCAT('Actualizacion de Area: ', @Nombre);

        EXEC sp_RegistrarAuditoria 
            @TablaAfectada = 'Areas', 
            @IdRegistroAfectado = @IdArea, 
            @Operacion = 'UPDATE', 
            @DetalleOperacion = @Detalle, 
            @UsuarioResponsable = @UsuarioResponsable;


        COMMIT TRANSACTION;
        SELECT 'Área actualizada con éxito' AS Mensaje;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

DROP PROCEDURE IF EXISTS sp_BorrarArea;
GO

CREATE PROCEDURE sp_BorrarArea
    @IdArea INT,
    @UsuarioResponsable NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM dbo.Areas WHERE IdArea = @IdArea)
        BEGIN
            RAISERROR('El área no existe.', 16, 1);
            RETURN;
        END

        BEGIN TRANSACTION;
        DELETE FROM dbo.Areas WHERE IdArea = @IdArea;

        DECLARE @Detalle NVARCHAR(MAX) = CONCAT('Borrado de Area: ', @IdArea);

        EXEC sp_RegistrarAuditoria 
            @TablaAfectada = 'Areas', 
            @IdRegistroAfectado = @IdArea, 
            @Operacion = 'DELETE', 
            @DetalleOperacion = @Detalle, 
            @UsuarioResponsable = @UsuarioResponsable;

        COMMIT TRANSACTION;
        SELECT 'Área eliminada con éxito' AS Mensaje;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        
        IF ERROR_NUMBER() = 547 
            RAISERROR('No se puede eliminar el área porque tiene usuarios o registros asociados.', 16, 1);
        ELSE
            THROW;
    END CATCH
END;
GO


DROP PROCEDURE IF EXISTS sp_ListarAreas;
GO
CREATE PROCEDURE sp_ListarAreas 
AS 
BEGIN
    SELECT 
    IdArea, 
    Nombre 
    FROM Areas    
END;
GO
------------------------------------------------
DROP PROCEDURE IF EXISTS sp_InsertarRol;
GO

CREATE PROCEDURE sp_InsertarRol
    @Nombre NVARCHAR(50),
    @UsuarioResponsable NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        INSERT INTO dbo.Roles 
            (Nombre)
        VALUES (@Nombre);

        DECLARE @NuevoId INT = SCOPE_IDENTITY();

        DECLARE @Detalle NVARCHAR(MAX) = CONCAT('Registro de Rol: ', @Nombre);

        EXEC sp_RegistrarAuditoria 
            @TablaAfectada = 'Rol', 
            @IdRegistroAfectado = @NuevoId, 
            @Operacion = 'INSERT', 
            @DetalleOperacion = @Detalle, 
            @UsuarioResponsable = @UsuarioResponsable;
       
        COMMIT TRANSACTION;
       
        SELECT 'Rol registrado con éxito' AS Mensaje, @NuevoId AS IdGenerado;
    END TRY
    BEGIN CATCH
       
        IF @@TRANCOUNT > 0 
            ROLLBACK TRANSACTION;        
       
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();

        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;
GO

DROP PROCEDURE IF EXISTS sp_ActualizarRol;
GO

CREATE PROCEDURE sp_ActualizarRol
    @IdRol INT,
    @Nombre NVARCHAR(50),
    @UsuarioResponsable NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE IdRol = @IdRol)
        BEGIN
            RAISERROR('El Rol especificado no existe.', 16, 1);
            RETURN;
        END

        BEGIN TRANSACTION;
        
        UPDATE dbo.Roles 
        SET Nombre = @Nombre
        WHERE IdRol = @IdRol;
        
        DECLARE @Detalle NVARCHAR(MAX) = CONCAT('Actualizacion de Area: ', @IdRol);

        EXEC sp_RegistrarAuditoria 
            @TablaAfectada = 'Roles', 
            @IdRegistroAfectado = @IdRol,
            @Operacion = 'UPDATE', 
            @DetalleOperacion = @Detalle, 
            @UsuarioResponsable = @UsuarioResponsable;

        COMMIT TRANSACTION;
        SELECT 'Rol actualizado con éxito' AS Mensaje;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

DROP PROCEDURE IF EXISTS sp_BorrarRol;
GO

CREATE PROCEDURE sp_BorrarRol
    @IdRol INT,
    @UsuarioResponsable NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE IdRol = @IdRol)
        BEGIN
            RAISERROR('El Rol no existe.', 16, 1);
            RETURN;
        END

        BEGIN TRANSACTION;
        DELETE FROM dbo.Roles WHERE IdRol = @IdRol;
         
        DECLARE @Detalle NVARCHAR(MAX) = CONCAT('Borrado de Rol: ', @IdRol);

        EXEC sp_RegistrarAuditoria 
            @TablaAfectada = 'Roles', 
            @IdRegistroAfectado = @IdRol, 
            @Operacion = 'DELETE', 
            @DetalleOperacion = @Detalle, 
            @UsuarioResponsable = @UsuarioResponsable;
        COMMIT TRANSACTION;

        SELECT 'Rol eliminado con éxito' AS Mensaje;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        
        IF ERROR_NUMBER() = 547 
            RAISERROR('No se puede eliminar el Rol porque tiene usuarios o registros asociados.', 16, 1);
        ELSE
            THROW;
    END CATCH
END;
GO

DROP PROCEDURE IF EXISTS sp_ListarRoles;
GO

CREATE PROCEDURE sp_ListarRoles 
AS 
BEGIN
    SELECT IdRol, Nombre 
    FROM Roles
    
END;
GO
------------------------------------------------
DROP PROCEDURE IF EXISTS sp_InsertarUsuario;
GO

CREATE PROCEDURE sp_InsertarUsuario
    @Nombre NVARCHAR(50),
    @Apellido NVARCHAR(50),
    @Correo NVARCHAR(100),
    @Telefono NVARCHAR(20),
    @IdArea INT,
    @IdRol INT,
    @Activo BIT,
    @UsuarioResponsable NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO dbo.Usuarios (Nombre, Apellido, Correo, Telefono, IdArea, IdRol, Activo)
        VALUES (@Nombre, @Apellido, @Correo, @Telefono, @IdArea, @IdRol, @Activo);

        DECLARE @NuevoId INT = SCOPE_IDENTITY();
        DECLARE @Detalle NVARCHAR(MAX) = CONCAT('Registro de usuario: ', @Nombre, ' ', @Apellido);

        EXEC sp_RegistrarAuditoria 
            @TablaAfectada = 'Usuario', 
            @IdRegistroAfectado = @NuevoId, 
            @Operacion = 'INSERT', 
            @DetalleOperacion = @Detalle, 
            @UsuarioResponsable = @UsuarioResponsable;
       
        COMMIT TRANSACTION;        
        
        SELECT 'Usuario registrado con éxito' AS Mensaje, @NuevoId AS IdGenerado;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        
        THROW; 
    END CATCH
END;

GO

DROP PROCEDURE IF EXISTS sp_ActualizarUsuario;
GO

CREATE PROCEDURE sp_ActualizarUsuario
    @IdUsuario INT,
    @Nombre NVARCHAR(50),
    @Apellido NVARCHAR(50),
    @Correo NVARCHAR(100),
    @Telefono NVARCHAR(20),
    @IdArea INT,
    @IdRol INT,
    @Activo BIT,
    @UsuarioResponsable NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE IdUsuario = @IdUsuario)
        BEGIN
            RAISERROR('El usuario no existe.', 16, 1);
            RETURN;
        END

        BEGIN TRANSACTION;

        UPDATE dbo.Usuarios
        SET Nombre = @Nombre,
            Apellido = @Apellido,
            Correo = @Correo,
            Telefono = @Telefono,
            IdArea = @IdArea,
            IdRol = @IdRol,
            Activo = @Activo
        WHERE IdUsuario = @IdUsuario;

        -- Registro de Auditoría
        DECLARE @Detalle NVARCHAR(MAX) = CONCAT('Actualizacion de Usuario: ', @IdUsuario);

        EXEC sp_RegistrarAuditoria 
            @TablaAfectada = 'Usuarios', 
            @IdRegistroAfectado = @IdUsuario, 
            @Operacion = 'UPDATE', 
            @DetalleOperacion = @Detalle, 
            @UsuarioResponsable = @UsuarioResponsable;

        COMMIT TRANSACTION;
        SELECT 'Usuario actualizado con éxito' AS Mensaje;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

DROP PROCEDURE IF EXISTS sp_BorrarUsuario;
GO

CREATE PROCEDURE sp_BorrarUsuario
    @IdUsuario INT,
    @UsuarioResponsable NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        -- Obtener datos antes de borrar para la auditoría
        DECLARE @Detalle NVARCHAR(200);
        SELECT @Detalle = CONCAT('Borrado de usuario: ', Nombre, ' ', Apellido) 
        FROM dbo.Usuarios WHERE IdUsuario = @IdUsuario;

        IF @Detalle IS NULL
        BEGIN
            RAISERROR('El usuario no existe.', 16, 1);
            RETURN;
        END

        BEGIN TRANSACTION;

        DELETE FROM dbo.Usuarios WHERE IdUsuario = @IdUsuario;

        -- Registro de Auditoría del borrado
        DECLARE @DetalleAuditoria NVARCHAR(MAX) = CONCAT('Borrado de Usuario: ', @IdUsuario);

        EXEC sp_RegistrarAuditoria 
            @TablaAfectada = 'Usuarios', 
            @IdRegistroAfectado = @IdUsuario, 
            @Operacion = 'DELETE', 
            @DetalleOperacion = @DetalleAuditoria, 
            @UsuarioResponsable = @UsuarioResponsable;

        COMMIT TRANSACTION;
        SELECT 'Usuario eliminado con éxito' AS Mensaje;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
------------------------------------------------
DROP PROCEDURE IF EXISTS sp_ConsultarUltimosUsuarios;
GO

CREATE PROCEDURE sp_ConsultarUltimosUsuarios
AS
BEGIN
    SET NOCOUNT ON;

    -- Selecciona los últimos 10 usuarios registrados
    SELECT TOP 10 
        U.IdUsuario,
        U.Nombre,
        U.Apellido,
        U.Correo,
        U.Telefono,
        A.Nombre AS Area,
        R.Nombre AS Rol,
        U.FechaRegistro,
        U.Activo
    FROM Usuarios U
    INNER JOIN Areas A ON U.IdArea = A.IdArea
    INNER JOIN Roles R ON U.IdRol = R.IdRol
    ORDER BY U.FechaRegistro DESC;
END;
GO