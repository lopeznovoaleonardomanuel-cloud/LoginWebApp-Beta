CREATE DATABASE SistemaTarea;
GO
USE SistemaTarea;
GO

CREATE TABLE Usuarios (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Correo VARCHAR(100) UNIQUE NOT NULL,
    Clave VARCHAR(100) NOT NULL
);


INSERT INTO Usuarios (Correo, Clave) VALUES ('admin@tarea.com', '12345');