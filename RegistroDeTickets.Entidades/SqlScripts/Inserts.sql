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