# 📋 Sistema de Gestión de Tickets 📋

Este proyecto es un Sistema de Gestión de Tickets de Soporte desarrollado como parte de la materia Programación Web III. El sistema centraliza y organiza las solicitudes de los clientes (tickets) y las asigna a los técnicos correspondientes para su resolución.

---

## 🛠️ Stack de Tecnologías

* **Backend:** ASP.NET Core (.NET)
* **Lenguaje:** C#
* **Base de Datos:** SQL Server
* **ORM:** Entity Framework Core (EF Core)
* **Frontend:** Bootstrap 5 y CSS
* **Arquitectura:** MVC (Modelo-Vista-Controlador)

---

##  Kanban (Gestión del Proyecto)

El seguimiento de tareas, *backlog* y *sprints* del proyecto se gestiona a través de nuestro tablero de Trello:

* **[Ver Tablero de Trello](https://trello.com/b/Wm6pSIJF/appticket-progra-web-iii)**

---

## 👤 Roles del Sistema

La aplicación tiene una arquitectura de tres roles:

* **Cliente:** Puede crear nuevos tickets de soporte y ver el estado de sus reclamos anteriores.
* **Técnico:** Ve un tablero con los tickets que le fueron asignados y puede actualizar su estado (ej. "En Progreso", "Resuelto").
* **Administrador:** Tiene una vista de todos los tickets. Es el encargado de validar los tickets nuevos y asignarlos al técnico correspondiente.

## 🏛️ Arquitectura de Base de Datos

* **Herencia de Usuarios:** El sistema utiliza un modelo de herencia **Table-Per-Type (TPT)** para la gestión de usuarios.
* **Tabla Base:** `Usuario` (contiene datos comunes como Email y PasswordHash).
* **Tablas Derivadas:** `Admin`, `Tecnico`, `Cliente` (conectadas por FK a `Usuario.Id`).
* **Tablas de Catálogo:** Se usan `TicketEstado` y `TicketPrioridad` para normalizar los datos y evitar "magic strings".

---

## 🛠️ Generar Clases de EF Core (Scaffolding)

El proyecto utiliza un enfoque **Database-First**. Una vez creada la base de datos, necesitas generar las clases de entidad de C# usando EF Core.

> **Importante:** Asegúrate de cambiar `DESKTOP-7IS0SRC\SQLEXPRESS` por el nombre de tu instancia local de SQL Server.

---

### Consola de Administrador de Paquetes (PMC)
```powershell
Scaffold-DbContext "Server=DESKTOP-7IS0SRC\SQLEXPRESS;Database=RegistroDeTicketsPW3;Trusted_Connection=True;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer -Project RegistroDeTickets.Data -OutputDir Entidades -Force
```

---

## 📧 Configuración de Credenciales de Email (User Secrets)

Para que el servicio de envío de correos electrónicos funcione, configurar las credenciales SMTP en los User Secrets:
1. __Crear User Secrets__ (secrets.json): Clic derecho en el proyecto (RegistroDeTickets.web) $\rightarrow$ "Administrar secretos de usuario" o "Manage User Secrets"
2. __Agregar la configuración__ siguiente al archivo creado y reemplazar los valores que faltan:

```json
{
  "CONFIGURACIONES_EMAIL": {
    "EMAIL": "xxxx xxxx xxxxx",
    "PASSWORD": "xx xxx xxx",
    "HOST": "smtp.gmail.com",
    "PUERTO": 587
  }
}
```
