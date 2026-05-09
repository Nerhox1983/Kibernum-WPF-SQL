using Kibernum_SJCC;
using System.Windows;

namespace Kibernum_SJCC
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Instanciamos la ventana usando su namespace completo
            var window = new Kibernum_SJCC.Views.MainWindow();
            window.Show();
        }
    }
}
