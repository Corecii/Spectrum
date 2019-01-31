using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using System.Windows;

namespace Spectrum.Resonator.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string TestBindableString { get; set; } = "This is a test.";

        [Command]
        public void ClickityClick()
        {
            MessageBox.Show("Yay.");
        }
    }
}
