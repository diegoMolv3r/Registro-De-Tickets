USE RegistroDeTicketsPW3;
GO

INSERT INTO TicketEstado (Nombre) VALUES ('Iniciado'), ('Asignado'), ('Respondido'), ('Finalizado');
GO

INSERT INTO TicketPrioridad (Nombre) VALUES ('Baja'), ('Media'), ('Alta');
GO

INSERT INTO Usuario (Username, Email, PasswordHash, Estado) 
VALUES ('cliente_prueba', 'cliente@test.com', 'hash_de_prueba_123','Activo'),
	   ('admin_prueba', 'admin@test.com', 'hash_de_prueba_123', 'Activo'),
	   ('tecnico_prueba', 'tecnico@test.com', 'hash_de_prueba_123', 'Activo');

INSERT INTO Cliente (Id) 
VALUES (1);
GO

INSERT INTO Administrador (Id) 
VALUES (2);
GO

INSERT INTO Tecnico (Id) 
VALUES (3);
GO


DECLARE @NuevoUserId INT = 102;
DECLARE @AdminRoleId INT = 1; 



SET IDENTITY_INSERT Usuario ON;
INSERT INTO Usuario (
    Id, Username, Email, PasswordHash, Estado, 
    NormalizedUserName, NormalizedEmail, EmailConfirmed, SecurityStamp, 
    AccessFailedCount, LockoutEnabled, PhoneNumberConfirmed, TwoFactorEnabled
) 
VALUES
(
    @NuevoUserId, 
    'super_admin', 
    'superadmin@test.com', 
    'hash_de_prueba_123', 
    'Activo', 
    'SUPER_ADMIN', 
    'SUPERADMIN@TEST.COM', 
    0, 
    NEWID(), 
    0, 
    1, 
    0, 
    0
);
SET IDENTITY_INSERT Usuario OFF;


INSERT INTO AspNetUserRoles (UserId, RoleId) 
VALUES (@NuevoUserId, @AdminRoleId);


INSERT INTO AspNetUserClaims (UserId, ClaimType, ClaimValue) 
VALUES (
    @NuevoUserId, 
    'Permiso',        
    'Acciones_de_Alto_Riesgo'
);

INSERT INTO Administrador (Id) VALUES (@NuevoUserId);
GO