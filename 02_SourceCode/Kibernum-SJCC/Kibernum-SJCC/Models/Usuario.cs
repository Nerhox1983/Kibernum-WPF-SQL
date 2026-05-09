using System;

public class Usuario
{
    public int IdUsuario { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string Correo { get; set; }
    public string Telefono { get; set; }
    public int IdArea { get; set; }
    public int IdRol { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaRegistro { get; set; }
    public string Area { get; set; } // Propiedad para mostrar el nombre del Área
    public string Rol { get; set; }  // Propiedad para mostrar el nombre del Rol
}
