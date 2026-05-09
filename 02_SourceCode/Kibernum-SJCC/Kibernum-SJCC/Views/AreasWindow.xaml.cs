using System.Windows;
using Kibernum_SJCC.ViewModels;

namespace Kibernum_SJCC.Views
{
    public partial class AreasWindow : Window
    {
        private AreaViewModel _viewModel;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="AreasWindow"/>.
        /// Configura los componentes visuales y establece el ViewModel como contexto de datos.
        /// </summary>
        public AreasWindow()
        {
            InitializeComponent();
            _viewModel = new AreaViewModel();
            this.DataContext = _viewModel;
        }

        /// <summary>
        /// Gestiona el evento de clic del botón guardar para procesar el almacenamiento de la información.
        /// </summary>
        /// <param name="sender">El objeto que originó el evento (botón guardar).</param>
        /// <param name="e">Información detallada sobre el evento de enrutamiento.</param>        
        private void btnGuardar_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _viewModel.Guardar();
        }

        /// <summary>
        /// Controla el evento de clic del botón limpiar para restablecer los campos de entrada.
        /// </summary>
        /// <param name="sender">El objeto que activa el evento.</param>
        /// <param name="e">Argumentos de evento que contienen los datos de la ruta.</param>
        private void btnLimpiar_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _viewModel.LimpiarCampos();
        }

        /// <summary>
        /// Gestiona el evento de clic para eliminar el registro seleccionado.
        /// Obtiene el contexto de datos actual y ejecuta la lógica de borrado en el ViewModel.
        /// </summary>
        /// <param name="sender">El botón que disparó el evento.</param>
        /// <param name="e">Datos del evento de enrutamiento.</param>
        private void btnBorrar_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (AreaViewModel)this.DataContext;
            viewModel.Borrar();
        }
    }
}