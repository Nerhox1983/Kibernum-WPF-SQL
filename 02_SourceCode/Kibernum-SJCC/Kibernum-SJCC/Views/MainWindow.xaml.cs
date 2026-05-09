using System.Windows;
using Kibernum_SJCC.ViewModels;

namespace Kibernum_SJCC.Views
{
    public partial class MainWindow : Window
    {
        private UsuarioViewModel _viewModel;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="MainWindow"/>.
        /// Configura la interfaz de usuario y establece el contexto de datos (DataContext) 
        /// vinculando la vista con su respectivo ViewModel.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();            
            _viewModel = new UsuarioViewModel();
            this.DataContext = _viewModel;
        }

        /// <summary>
        /// Maneja el evento de clic del botón Guardar.
        /// Despacha la instrucción de persistencia de datos al ViewModel correspondiente.
        /// </summary>
        /// <param name="sender">El objeto que originó el evento (usualmente <see cref="Button"/>).</param>
        /// <param name="e">Información sobre el evento de enrutado.</param>
        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {            
            _viewModel.Guardar();
        }

        /// <summary>
        /// Maneja el evento de clic del botón Limpiar.
        /// Solicita al ViewModel que restablezca los valores de las propiedades vinculadas a la interfaz.
        /// </summary>
        /// <param name="sender">El objeto que activa el evento (típicamente el <see cref="Button"/>).</param>
        /// <param name="e">Datos del evento de enrutado.</param>
        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.LimpiarCampos();
        }

        /// <summary>
        /// Gestiona la apertura de la ventana de áreas y actualiza los catálogos locales al finalizar.
        /// </summary>
        /// <remarks>
        /// Este método abre <see cref="Views.AreasWindow"/> de forma modal. 
        /// Una vez que el usuario cierra la ventana de gestión, se invoca la actualización 
        /// de los catálogos en el ViewModel para reflejar posibles cambios.
        /// </remarks>
        /// <param name="sender">El objeto que dispara el evento.</param>
        /// <param name="e">Argumentos del evento de clic.</param>
        private void btnGestionAreas_Click(object sender, RoutedEventArgs e)
        {
            Views.AreasWindow ventanaAreas = new Views.AreasWindow();
            ventanaAreas.ShowDialog();            
            _viewModel.RefrescarCatalogos();
        }

    }
}