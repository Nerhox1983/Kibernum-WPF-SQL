using Kibernum_SJCC.Data;
using Kibernum_SJCC.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;

namespace Kibernum_SJCC.ViewModels
{
    public class UsuarioViewModel : INotifyPropertyChanged
    {
        private readonly UsuarioRepository _repo = new UsuarioRepository();
        private int _idUsuarioSeleccionado = 0;

        private string _nombre;
        public string Nombre { get => _nombre; set { _nombre = value; OnPropertyChanged(); } }

        private string _apellido;
        public string Apellido { get => _apellido; set { _apellido = value; OnPropertyChanged(); } }

        private string _correo;
        public string Correo { get => _correo; set { _correo = value; OnPropertyChanged(); } }

        private string _telefono;
        public string Telefono { get => _telefono; set { _telefono = value; OnPropertyChanged(); } }

        private int? _idArea;
        public int? IdArea { get => _idArea; set { _idArea = value; OnPropertyChanged(); } }

        private int? _idRol;
        public int? IdRol { get => _idRol; set { _idRol = value; OnPropertyChanged(); } }

        // Colecciones para los controles de lista
        public ObservableCollection<Usuario> Usuarios { get; set; }
        public ObservableCollection<Area> Areas { get; set; }
        public ObservableCollection<Rol> Roles { get; set; }

        public bool EsEdicion => _idUsuarioSeleccionado != 0;
        private bool _activo = true; 
        public bool Activo { get => _activo; set { _activo = value; OnPropertyChanged(); } }
        
        /// <summary>
        /// 
        /// </summary>
        private Usuario _usuarioSeleccionado;
        public Usuario UsuarioSeleccionado
        {
            get => _usuarioSeleccionado;
            set
            {
                _usuarioSeleccionado = value;
                OnPropertyChanged();
                if (_usuarioSeleccionado != null) CargarDatosSeleccionados();
            }
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="UsuarioViewModel"/>.
        /// </summary>
        /// <remarks>
        /// Durante la construcción, se realizan las siguientes operaciones:
        /// <list type="bullet">
        /// <item>
        /// <description>Se cargan de forma inmediata los catálogos de <see cref="Areas"/> y <see cref="Roles"/> desde el repositorio.</description>
        /// </item>
        /// <item>
        /// <description>Se inicializa la colección de <see cref="Usuarios"/> como una lista observable vacía.</description>
        /// </item>
        /// <item>
        /// <description>Se ejecuta <see cref="RefrescarGrilla"/> para poblar la lista principal de usuarios.</description>
        /// </item>
        /// </list>
        /// </remarks>
        public UsuarioViewModel()
        {
            Areas = new ObservableCollection<Area>(_repo.ListarAreas());
            Roles = new ObservableCollection<Rol>(_repo.ListarRoles());
            Usuarios = new ObservableCollection<Usuario>();
            RefrescarGrilla();
        }

        /// <summary>
        /// Procesa la persistencia de un usuario, validando la integridad de los datos antes de la operación.
        /// </summary>
        /// <remarks>
        /// El método realiza las siguientes acciones:
        /// <list type="number">
        /// <item><description>Valida el formato de correo y teléfono.</description></item>
        /// <item><description>Verifica que se hayan seleccionado claves foráneas válidas (Área y Rol).</description></item>
        /// <item><description>Determina si debe ejecutar una inserción o una actualización basada en <c>_idUsuarioSeleccionado</c>.</description></item>
        /// </list>
        /// </remarks>
        public void Guardar()
        {
            try
            {                
                if (!ValidarDatosEntrada()) return;
                
                var usuario = MapearUsuario();
                
                if (_idUsuarioSeleccionado == 0)
                    _repo.InsertarUsuario(usuario);
                else
                    _repo.ActualizarUsuario(usuario);

                MessageBox.Show("Operación exitosa", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                RefrescarGrilla();
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al procesar la solicitud: {ex.Message}", "Error Crítico", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Centraliza la lógica de validación de los campos del formulario.
        /// </summary>
        private bool ValidarDatosEntrada()
        {
            // 1. Validar que el Nombre no esté vacío (Causa del error en image_68aa6b.png)
            if (string.IsNullOrWhiteSpace(Nombre))
            {
                MessageBox.Show("El nombre es obligatorio.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // 2. Validar que el Apellido no esté vacío
            if (string.IsNullOrWhiteSpace(Apellido))
            {
                MessageBox.Show("El apellido es obligatorio.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // 3. Validar formato de correo
            if (!EsCorreoValido(Correo))
            {
                MessageBox.Show("El formato del correo electrónico no es válido.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // 4. Validar formato de teléfono
            if (!EsTelefonoValido(Telefono))
            {
                MessageBox.Show("El teléfono solo debe contener números.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // 5. Validar selección de combos (Área y Rol)
            if ((IdArea ?? 0) == 0 || (IdRol ?? 0) == 0)
            {
                MessageBox.Show("Por favor, seleccione un Área y un Rol.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Crea una instancia de <see cref="Usuario"/> con los datos actuales del ViewModel.
        /// </summary>
        private Usuario MapearUsuario() => new Usuario
        {
            IdUsuario = _idUsuarioSeleccionado,
            Nombre = Nombre,
            Apellido = Apellido,
            Correo = Correo,
            Telefono = Telefono,
            IdArea = IdArea.Value,
            IdRol = IdRol.Value,
            Activo = Activo
        };

        /// <summary>
        /// Actualiza la lista de usuarios mostrada en la interfaz, cargando únicamente los registros más recientes.
        /// </summary>
        /// <remarks>
        /// Este método realiza una limpieza completa de la colección <see cref="Usuarios"/> y la repuebla 
        /// con los últimos diez registros obtenidos desde el repositorio mediante <see cref="_repo.ObtenerUltimosDiez"/>.
        /// Se utiliza para mantener una vista ligera y optimizada de la actividad reciente.
        /// </remarks>        
        public void RefrescarGrilla()
        {
            Usuarios.Clear();
            var lista = _repo.ObtenerUltimosDiez();
            foreach (var u in lista) Usuarios.Add(u);
        }

        /// <summary>
        /// Transfiere la información del usuario seleccionado a las propiedades editables del ViewModel.
        /// </summary>
        /// <remarks>
        /// Este método actúa como un mapeador interno que prepara los campos del formulario para su edición.
        /// Al finalizar, se notifica el cambio de la propiedad <see cref="EsEdicion"/> para actualizar 
        /// el estado de la interfaz de usuario (por ejemplo, habilitar/deshabilitar botones o cambiar títulos).
        /// </remarks>
        private void CargarDatosSeleccionados()
        {
            _idUsuarioSeleccionado = UsuarioSeleccionado.IdUsuario;
            Nombre = UsuarioSeleccionado.Nombre;
            Apellido = UsuarioSeleccionado.Apellido;
            Correo = UsuarioSeleccionado.Correo;
            Telefono = UsuarioSeleccionado.Telefono;
            IdArea = UsuarioSeleccionado.IdArea;
            IdRol = UsuarioSeleccionado.IdRol;
            Activo = UsuarioSeleccionado.Activo;
            
            OnPropertyChanged(nameof(EsEdicion));
        }

        /// <summary>
        /// Restablece todas las propiedades del formulario de usuario a su estado inicial.
        /// </summary>
        /// <remarks>
        /// Este método realiza una limpieza integral que incluye:
        /// <list type="bullet">
        /// <item><description>Anulación de la selección actual (<see cref="UsuarioSeleccionado"/>).</description></item>
        /// <item><description>Reinicio de campos de texto y selectores de Area/Rol.</description></item>
        /// <item><description>Restablecimiento del estado <see cref="Activo"/> a su valor predeterminado (<c>true</c>).</description></item>
        /// <item><description>Notificación de cambio para <see cref="EsEdicion"/>, permitiendo que la vista se adapte al modo "Nuevo Registro".</description></item>
        /// </list>
        /// </remarks>
        public void LimpiarCampos()
        {
            _idUsuarioSeleccionado = 0;
            Nombre = string.Empty;
            Apellido = string.Empty;
            Correo = string.Empty;
            Telefono = string.Empty;
            IdArea = null;
            IdRol = null;
            UsuarioSeleccionado = null;
            Activo = true;            
            OnPropertyChanged(nameof(EsEdicion));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifica a la interfaz de usuario que el valor de una propiedad ha cambiado para actualizar el enlace de datos (Data Binding).
        /// </summary>
        /// <param name="name">
        /// Nombre de la propiedad que ha cambiado. Gracias al atributo <see cref="CallerMemberNameAttribute"/>, 
        /// este valor se asigna automáticamente con el nombre del método o propiedad desde donde se invoca.
        /// </param>
        /// <remarks>
        /// Este método dispara el evento <see cref="PropertyChanged"/>. Es el motor que permite la reactividad 
        /// en WPF/MAUI, asegurando que cualquier cambio en el ViewModel se refleje inmediatamente en la Vista.
        /// </remarks>
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// Valida si una cadena de texto tiene un formato de correo electrónico estructurado correctamente.
        /// </summary>
        /// <param name="email">La cadena de texto que representa el correo electrónico a evaluar.</param>
        /// <returns>
        /// <c>true</c> si el formato es válido (ej. usuario@dominio.com); de lo contrario, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// Esta validación comprueba la integridad sintáctica del correo mediante expresiones regulares (Regex), 
        /// pero no garantiza que la cuenta de correo exista realmente.
        /// </remarks>
        public bool EsCorreoValido(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            string patron = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, patron);
        }

        /// <summary>
        /// Determina si la cadena de texto proporcionada representa un número de teléfono válido.
        /// </summary>
        /// <param name="telefono">La cadena de caracteres que se desea validar.</param>
        /// <returns>
        /// <c>true</c> si la cadena es nula, vacía o contiene únicamente dígitos numéricos; 
        /// de lo contrario, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// Este método permite valores vacíos para dar soporte a campos opcionales. 
        /// No valida la longitud mínima o máxima, ni el código de país; 
        /// se limita a verificar que no existan caracteres alfabéticos o símbolos.
        /// </remarks>
        public bool EsTelefonoValido(string telefono)
        {            
            return string.IsNullOrEmpty(telefono) || telefono.All(char.IsDigit);
        }

        /// <summary>
        /// Actualiza las colecciones de referencia (Áreas y Roles) desde la fuente de datos.
        /// </summary>
        /// <remarks>
        /// Este método sincroniza los catálogos necesarios para el funcionamiento del formulario:
        /// <list type="bullet">
        /// <item><description>Limpia y repuebla la colección <see cref="Areas"/>.</description></item>
        /// <item><description>Limpia y repuebla la colección <see cref="Roles"/>.</description></item>
        /// </list>
        /// Es útil invocar este método cuando se sospecha que los datos maestros han cambiado en la base de datos 
        /// o después de una operación de mantenimiento.
        /// </remarks>
        public void RefrescarCatalogos()
        {            
            Areas.Clear();
            foreach (var a in _repo.ListarAreas()) Areas.Add(a);

            Roles.Clear();
            foreach (var r in _repo.ListarRoles()) Roles.Add(r);
        }
    }
}
