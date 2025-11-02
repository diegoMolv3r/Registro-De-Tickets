
--CREATE DATABASE RegistroDeTicketsPW3;
USE RegistroDeTicketsPW3;
GO

CREATE TABLE TicketEstado (
    Id INT IDENTITY(1,1) NOT NULL,
    Nombre NVARCHAR(30) NOT NULL,
    CONSTRAINT PK_TicketEstado PRIMARY KEY (Id),
    CONSTRAINT UQ_TicketEstado_Nombre UNIQUE (Nombre)
);
GO

CREATE TABLE TicketPrioridad (
    Id INT IDENTITY(1,1) NOT NULL,
    Nombre NVARCHAR(30) NOT NULL,
    CONSTRAINT PK_TicketPrioridad PRIMARY KEY (Id),
    CONSTRAINT UQ_TicketPrioridad_Nombre UNIQUE (Nombre)
);
GO


CREATE TABLE Usuario(
	Id INT NOT NULL UNIQUE IDENTITY,
	Username NVARCHAR(20) NOT NULL UNIQUE,
	Email NVARCHAR(255) NOT NULL UNIQUE,
	PasswordHash NVARCHAR(MAX) NOT NULL,
    Estado NVARCHAR(20) NOT NULL,
	CONSTRAINT PK_usuario PRIMARY KEY (Id)
);

CREATE TABLE Administrador (
    Id INT NOT NULL,
    -- Aquí puedes añadir columnas específicas de un Admin

    CONSTRAINT PK_Admin PRIMARY KEY (Id),
    CONSTRAINT FK_Admin_Usuario FOREIGN KEY (Id) REFERENCES Usuario(Id) ON DELETE CASCADE
);
GO
CREATE TABLE Tecnico (
    Id INT NOT NULL,
    -- Columnas específicas eliminadas según solicitud

    CONSTRAINT PK_Tecnico PRIMARY KEY (Id),
    CONSTRAINT FK_Tecnico_Usuario FOREIGN KEY (Id) REFERENCES Usuario(Id) ON DELETE CASCADE
);
GO
CREATE TABLE Cliente (
    Id INT NOT NULL,
    Domicilio NVARCHAR(MAX),
    -- Columnas específicas eliminadas según solicitud

    CONSTRAINT PK_Cliente PRIMARY KEY (Id),
    CONSTRAINT FK_Cliente_Usuario FOREIGN KEY (Id) REFERENCES Usuario(Id) ON DELETE CASCADE
);
GO

CREATE TABLE Ticket(
	Id INT NOT NULL IDENTITY,
    Motivo NVARCHAR(50) NOT NULL,
	Descripcion NVARCHAR(MAX) NOT NULL,
    FechaCreacion DATETIME DEFAULT (GETDATE()),
    FechaActualizacion DATETIME NULL,
	
    Id_cliente INT NOT NULL, -- ID del cliente que solicita el ticket
    Id_tecnico INT NULL, -- ID del tecnico al cual le asignaron dicho ticket, hasta que el admin se lo asigne a alguien es NULL.

	EstadoId INT NOT NULL DEFAULT 1, -- Normalizacion de Estado
    PrioridadId INT NOT NULL, -- Normalizacion de Prioridad

	CONSTRAINT PK_Ticket PRIMARY KEY (Id),
    CONSTRAINT FK_Ticket_Estado FOREIGN KEY (EstadoId) REFERENCES TicketEstado(Id),
    CONSTRAINT FK_Ticket_Prioridad FOREIGN KEY (PrioridadId) REFERENCES TicketPrioridad(Id),
	CONSTRAINT FK_Ticket_Cliente FOREIGN KEY (Id_cliente) REFERENCES Cliente(Id),
    CONSTRAINT FK_Ticket_Tecnico FOREIGN KEY (Id_tecnico) REFERENCES Tecnico(Id)
);
GO

CREATE TABLE ReporteTecnico(
    IdReporte INT IDENTITY(1,1) NOT NULL,
    Descripcion NVARCHAR(MAX) NOT NULL,
    FechaCreacion DATE NOT NULL,
    IdTicket INT NOT NULL,
    CONSTRAINT PK_ReporteTecnico PRIMARY KEY (IdReporte),
    CONSTRAINT FK_Ticket FOREIGN KEY (IdTicket) REFERENCES Ticket(Id),
);
GO
-- No se olviden de cambiar el servidor por el que tengan en su SqlServer

-- Comando para la consola del Administrador de paquetes de NuGet.
--Scaffold-DbContext "Server=DESKTOP-7IS0SRC\SQLEXPRESS;Database=RegistroDeTicketsPW3;Trusted_Connection=True;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer -Project RegistroDeTickets.Data -OutputDir Entidades -Force

-- Comando para PowerShell, CMD, etc.
-- dotnet ef dbcontext scaffold "Server=DESKTOP-7IS0SRC\SQLEXPRESS;Database=RegistroDeTicketsPW3;Trusted_Connection=True;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer --project RegistroDeTickets.Data --output-dir Entidades --force