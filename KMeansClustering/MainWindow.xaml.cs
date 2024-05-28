using KMeansClustering.viewsmodel;
using System.Windows;

namespace KMeansClustering
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainVM mainVM = new MainVM();
            mainVM.RequestClose += (sender, e) =>
            {
                Close();
            };
            DataContext = mainVM;
        }

        private void Close()
        {
            Application.Current.Shutdown();
        }
    }
}
