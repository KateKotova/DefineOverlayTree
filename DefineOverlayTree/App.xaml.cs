using System;
using System.Windows;

namespace DefineOverlayTree
{
    public partial class App
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            // Hook on error before app really starts.
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_OnUnhandledException;

            var main = new MainWindow();
            main.Show();
        }

        private static void CurrentDomain_OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // Last resort error catch.
            MessageBox.Show(e.ExceptionObject.ToString());
        }
    }
}

