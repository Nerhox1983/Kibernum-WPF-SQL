using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Kibernum_SJCC.Models;

namespace Kibernum_SJCC.Data
{
    public class UsuarioRepository
    {
        private readonly string cnnStr = ConfigurationManager.ConnectionStrings["CnnUserM"].ConnectionString;
        const string USUARIO_RESPONSABLE = "App_WPF_User";

        /// <summary>
        /// Realiza la persistencia de un nuevo registro de usuario en la base de datos.
        /// Valida la integridad de los campos obligatorios antes de ejecutar la transacción.
        /// </summary>
        /// <param name="user">Objeto de tipo <see cref="Usuario"/> que contiene la información del perfil a registrar.</param>
        /// <exception cref="SqlException">Se lanza si ocurre un error durante la ejecución del procedimiento almacenado.</exception>        
        public void InsertarUsuario(Usuario user)
        {
            using (SqlConnection con = new SqlConnection(cnnStr))
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarUsuario", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Nombre", user.Nombre);
                cmd.Parameters.AddWithValue("@Apellido", user.Apellido);
                cmd.Parameters.AddWithValue("@Correo", user.Correo);
                cmd.Parameters.AddWithValue("@Telefono", (object)user.Telefono ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdArea", user.IdArea);
                cmd.Parameters.AddWithValue("@IdRol", user.IdRol);
                cmd.Parameters.AddWithValue("@UsuarioResponsable", USUARIO_RESPONSABLE);
                cmd.Parameters.AddWithValue("@Activo", user.Activo);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// realiza la actualización de un registro de usuario existente en la base de datos.
        /// </summary>
        /// <param name="user">Objeto de tipo <see cref="Usuario"/> que contiene la información del perfil a actualizar.</param>
        public void ActualizarUsuario(Usuario user)
        {
            using (SqlConnection con = new SqlConnection(cnnStr))
            {
                SqlCommand cmd = new SqlCommand("sp_ActualizarUsuario", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdUsuario", user.IdUsuario); // Clave para la edición
                cmd.Parameters.AddWithValue("@Nombre", user.Nombre);
                cmd.Parameters.AddWithValue("@Apellido", user.Apellido);
                cmd.Parameters.AddWithValue("@Correo", user.Correo);
                cmd.Parameters.AddWithValue("@Telefono", (object)user.Telefono ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdArea", user.IdArea);
                cmd.Parameters.AddWithValue("@IdRol", user.IdRol);
                cmd.Parameters.AddWithValue("@UsuarioResponsable", USUARIO_RESPONSABLE);
                cmd.Parameters.AddWithValue("@Activo", user.Activo);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Recupera el listado de los últimos diez usuarios registrados en el sistema.
        /// Utiliza el procedimiento almacenado <c>sp_ConsultarUltimosUsuarios</c> para garantizar la eficiencia en la consulta.
        /// </summary>
        /// <returns>Una lista de objetos <see cref="Usuario"/>; si no hay registros, devuelve una lista vacía.</returns>
        /// <remarks>
        /// Este método realiza una lectura secuencial a través de un SqlDataReader y mapea cada registro manualmente 
        /// para evitar dependencias de ORMs externos en la prueba técnica.
        /// </remarks>
        public List<Usuario> ObtenerUltimosDiez()
        {
            var lista = new List<Usuario>();
            using (SqlConnection con = new SqlConnection(cnnStr))
            {
                SqlCommand cmd = new SqlCommand("sp_ConsultarUltimosUsuarios", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        lista.Add(new Usuario
                        {
                            IdUsuario = (int)rdr["IdUsuario"],
                            Nombre = rdr["Nombre"].ToString(),
                            Apellido = rdr["Apellido"].ToString(),
                            Telefono = rdr["Telefono"].ToString(),
                            Correo = rdr["Correo"].ToString(),
                            Area = rdr["Area"].ToString(),
                            Rol = rdr["Rol"].ToString(),
                            Activo = (bool)rdr["Activo"],
                            FechaRegistro = Convert.ToDateTime(rdr["FechaRegistro"])
                        });
                    }
                }
            }
            return lista;
        }

        /// <summary>
        /// Registra una nueva área funcional en el sistema.
        /// Este método permite expandir el catálogo de departamentos disponibles para la asignación de usuarios.
        /// </summary>
        /// <param name="area">Objeto de tipo <see cref="Area"/> que contiene la descripción y estado del nuevo departamento.</param>
        /// <exception cref="SqlException">Se lanza si ocurre un error de duplicidad o de conexión con el servidor de base de datos.</exception>
        public void InsertarArea(Area area)
        {
            using (SqlConnection con = new SqlConnection(cnnStr))
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarArea", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Nombre", area.Nombre);                
                cmd.Parameters.AddWithValue("@UsuarioResponsable", USUARIO_RESPONSABLE);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Actualiza la información de un área existente en la base de datos.
        /// Utiliza el identificador del objeto <paramref name="area"/> para localizar el registro y aplicar los cambios.
        /// </summary>
        /// <param name="area">Objeto de tipo <see cref="Area"/> con los datos actualizados.</param>
        /// <exception cref="SqlException">Se lanza si el área no existe o si ocurre un error de concurrencia en la base de datos.</exception>
        public void ActualizarArea(Area area)
        {
            using (SqlConnection con = new SqlConnection(cnnStr))
            {
                SqlCommand cmd = new SqlCommand("sp_ActualizarArea", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdArea", area.IdArea); // Clave para la edición
                cmd.Parameters.AddWithValue("@Nombre", area.Nombre);
                cmd.Parameters.AddWithValue("@UsuarioResponsable", USUARIO_RESPONSABLE);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Obtiene la colección completa de áreas funcionales registradas en el sistema.
        /// Este método es ideal para poblar controles de selección o listados maestros en la interfaz de usuario.
        /// </summary>
        /// <returns>Una lista de objetos <see cref="Area"/> con todos los registros encontrados; si no hay datos, retorna una lista vacía.</returns>
        /// <remarks>
        /// La consulta recupera todas las áreas para permitir la gestión integral del catálogo desde la capa de presentación.
        /// </remarks>        
        public List<Area> ListarAreas()
        {
            var lista = new List<Area>();
            using (SqlConnection con = new SqlConnection(cnnStr))
            {
                SqlCommand cmd = new SqlCommand("sp_ListarAreas", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        lista.Add(new Area { IdArea = (int)rdr["IdArea"], Nombre = rdr["Nombre"].ToString() });
                    }
                }
            }
            return lista;
        }

        /// <summary>
        /// Obtiene la colección completa de roles funcionales registrados en el sistema.
        /// Este método es ideal para poblar controles de selección o listados maestros en la interfaz de usuario.
        /// </summary>
        /// <returns>Una lista de objetos <see cref="Rol"/> con todos los registros encontrados; si no hay datos, retorna una lista vacía.</returns>
        /// <remarks>
        /// La consulta recupera todos los Roles para permitir la gestión integral del catálogo desde la capa de presentación.
        /// </remarks>
        
        public List<Rol> ListarRoles()
        { 
            var lista = new List<Rol>();
            using (SqlConnection con = new SqlConnection(cnnStr))
            {
                SqlCommand cmd = new SqlCommand("sp_ListarRoles", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        lista.Add(new Rol { IdRol = (int)rdr["IdRol"], Nombre = rdr["Nombre"].ToString() });
                    }
                }
            }
            return lista;
        }

        /// <summary>
        /// Elimina un registro de área de la base de datos utilizando su identificador único.
        /// </summary>
        /// <param name="id">Identificador primario (ID) del área que se desea remover.</param>        
        /// <remarks>
        /// Se recomienda verificar la existencia de dependencias (usuarios vinculados) 
        /// antes de invocar este método para evitar excepciones de integridad referencial.
        /// </remarks>
        public void BorrarArea(int id)
        {
            using (SqlConnection con = new SqlConnection(cnnStr))
            {
                SqlCommand cmd = new SqlCommand("sp_BorrarArea", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdArea", id);
                cmd.Parameters.AddWithValue("@UsuarioResponsable", USUARIO_RESPONSABLE);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }        
}