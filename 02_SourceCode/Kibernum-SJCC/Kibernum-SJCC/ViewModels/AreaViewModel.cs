using Kibernum_SJCC.Data;
using Kibernum_SJCC.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Kibernum_SJCC.ViewModels
{
    public class AreaViewModel : INotifyPropertyChanged
    {
        private readonly UsuarioRepository _repo = new UsuarioRepository();
        private int _idAreaSeleccionada = 0;
        private string _nombreArea;
        private Area _areaSeleccionada;

        public string NombreArea
        {
            get => _nombreArea;
            set { _nombreArea = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Area> Areas { get; set; }

        public Area AreaSeleccionada
        {
            get => _areaSeleccionada;
            set
            {
                _areaSeleccionada = value;
                OnPropertyChanged();

                if (_areaSeleccionada != null)
                {
                    NombreArea = _areaSeleccionada.Nombre;
                    _idAreaSeleccionada = _areaSeleccionada.IdArea;
                }
            }
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="AreaViewModel"/>.
        /// </summary>
        /// <remarks>
        /// Este constructor prepara la colección observable de áreas y realiza la carga inicial 
        /// de datos mediante el método <see cref="RefrescarLista"/>.
        /// </remarks>
        public AreaViewModel()
        {
            Areas = new ObservableCollection<Area>();
            RefrescarLista();
        }

        /// <summary>
        /// Procesa la persistencia de un área en la base de datos.
        /// </summary>
        /// <remarks>
        /// Este método realiza una operación dual:
        /// <list type="bullet">
        /// <item><description><b>Inserción:</b> Si <c>_idAreaSeleccionada</c> es 0.</description></item>
        /// <item><description><b>Actualización:</b> Si <c>_idAreaSeleccionada</c> es distinto de 0.</description></item>
        /// </list>
        /// Tras una operación exitosa, se limpia el formulario y se refresca la lista visual.
        /// </remarks>
        public void Guardar()
        {
            if (string.IsNullOrWhiteSpace(NombreArea))
            {
                MessageBox.Show("El nombre del área no puede estar vacío.");
                return;
            }

            try
            {
                Area areaModificada = new Area { IdArea = _idAreaSeleccionada, Nombre = NombreArea };

                if (_idAreaSeleccionada == 0)
                    _repo.InsertarArea(areaModificada);
                else
                    _repo.ActualizarArea(areaModificada);

                MessageBox.Show("Operación exitosa.");
                RefrescarLista();
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar: " + ex.Message);
            }
        }

        /// <summary>
        /// Elimina el área actualmente seleccionada del sistema tras una confirmación del usuario.
        /// </summary>
        /// <remarks>
        /// El método realiza las siguientes acciones:
        /// <list type="number">
        /// <item><description>Verifica si existe un área seleccionada mediante <c>_idAreaSeleccionada</c>.</description></item>
        /// <item><description>Solicita confirmación mediante un cuadro de diálogo de advertencia.</description></item>
        /// <item><description>Ejecuta el borrado en el repositorio y actualiza la interfaz.</description></item>
        /// </list>
        /// Si el borrado es exitoso, se invocan <see cref="RefrescarLista"/> y <see cref="LimpiarCampos"/>.
        /// </remarks>
        public void Borrar()
        {
            if (_idAreaSeleccionada == 0)
            {
                MessageBox.Show("Por favor, seleccione un área de la lista para borrar.");
                return;
            }

            var resultado = MessageBox.Show($"¿Está seguro que desea borrar el área '{NombreArea}'?",
                                          "Confirmar Borrado",
                                          MessageBoxButton.YesNo,
                                          MessageBoxImage.Warning);

            if (resultado == MessageBoxResult.Yes)
            {
                try
                {                    
                    _repo.BorrarArea(_idAreaSeleccionada);

                    MessageBox.Show("Área borrada correctamente.");
                    RefrescarLista();
                    LimpiarCampos();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al borrar: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Sincroniza la colección local de áreas con la información almacenada en el repositorio.
        /// </summary>
        /// <remarks>
        /// Este método vacía la colección <see cref="Areas"/> y la repuebla con los datos actualizados.
        /// Al utilizar una <see cref="ObservableCollection{T}"/>, la interfaz de usuario se actualizará 
        /// automáticamente para reflejar los cambios.
        /// </remarks>
        public void RefrescarLista()
        {
            Areas.Clear();
            var lista = _repo.ListarAreas();
            foreach (var a in lista) Areas.Add(a);
        }

        /// <summary>
        /// Restablece el estado de las propiedades del ViewModel a sus valores predeterminados.
        /// </summary>
        /// <remarks>
        /// Este método se utiliza para limpiar el formulario de edición y resetear las referencias 
        /// al área seleccionada, notificando a la interfaz de usuario mediante <see cref="OnPropertyChanged"/>.
        /// </remarks>
        public void LimpiarCampos()
        {
            NombreArea = string.Empty;
            _idAreaSeleccionada = 0;
            _areaSeleccionada = null;
            OnPropertyChanged(nameof(AreaSeleccionada));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifica a la vista que el valor de una propiedad ha cambiado para actualizar el enlace de datos (Data Binding).
        /// </summary>
        /// <param name="name">
        /// Nombre de la propiedad que cambió. Gracias al atributo <see cref="CallerMemberNameAttribute"/>, 
        /// este valor se asigna automáticamente si se omite al llamar al método.
        /// </param>
        /// <remarks>
        /// Este método dispara el evento <see cref="PropertyChanged"/>. Es esencial para que la interfaz 
        /// de usuario reaccione a los cambios en el ViewModel de manera reactiva.
        /// </remarks>
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}