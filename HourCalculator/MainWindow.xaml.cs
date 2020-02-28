using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HourCalculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var notifyIconService = new NotifyIconHandler(this);
            var scheduleService = new ScheduleService();
            DataContext = new ViewModel(notifyIconService, scheduleService);
            
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
                Hide();

            base.OnStateChanged(e);
        }

        private void TimeCalculator_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
           var result = MessageBox.Show(this, "Do you want close app? Data will be lost.", "Closing",MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if(result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

               
    }
}
