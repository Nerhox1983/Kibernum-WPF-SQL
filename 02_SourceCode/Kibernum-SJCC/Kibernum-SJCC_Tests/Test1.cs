using Kibernum_SJCC.ViewModels;

namespace Kibernum_SJCC_Tests
{
    [TestClass]
    public class UsuarioTests
    {
        [TestMethod]
        public void CorreoInvalido_DebeRetornarFalse()
        {
            // Arrange (Preparar)
            var viewModel = new UsuarioViewModel();
            string correoErroneo = "usuario@dominio"; // Falta el .com

            // Act (Actuar)
            bool resultado = viewModel.EsCorreoValido(correoErroneo);

            // Assert (Afirmar)
            Assert.IsFalse(resultado, "Un correo sin extensión de dominio debería ser inválido.");
        }

        [TestMethod]
        public void TelefonoConLetras_DebeRetornarFalse()
        {
            // Arrange
            var viewModel = new UsuarioViewModel();
            string telefonoConLetras = "12345abc";

            // Act
            bool resultado = viewModel.EsTelefonoValido(telefonoConLetras);

            // Assert
            Assert.IsFalse(resultado);
        }
    }
}
