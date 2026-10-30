using System;
using System.Windows.Forms;

namespace Unidad3.Aplicacion;

internal static class Program
{
    // Windows Forms utiliza un hilo STA para controles y cuadros de diálogo.
    [STAThread]
    private static void Main()
    {
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new FormPrincipal());
    }
}
