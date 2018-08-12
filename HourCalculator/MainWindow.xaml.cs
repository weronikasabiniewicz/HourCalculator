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
            var vm = new ViewModel();
            DataContext = vm;
            
            System.Windows.Forms.NotifyIcon ni = new System.Windows.Forms.NotifyIcon();
            ni.Icon =  Properties.Resources.if_time_173116;
            ni.Visible = true;
            var contextMenu = new System.Windows.Forms.ContextMenu();
            var menuItem = new System.Windows.Forms.MenuItem("Start");
            menuItem.Click += (sender, eventArgs) =>
            {
                vm.Start();
                this.Hide();
            };
            contextMenu.MenuItems.Add(menuItem);
            ni.ContextMenu = contextMenu;
            ni.BalloonTipTitle = "Spend time";
          
            ni.MouseDown += (sender, eventArgs) =>
            {
                ni.BalloonTipText = vm.SpendTime.HasValue ? vm.SpendTime.Value.ToString(@"hh\:mm") : "Please press start";
                ni.ShowBalloonTip(1000);

            };

            ni.Click +=
                delegate(object sender, EventArgs args)
                {
                    if (this.WindowState == WindowState.Minimized)
                    {
                        this.Show();
                        this.WindowState = WindowState.Normal;
                    }
                    else
                    {
                        this.Hide();
                        this.WindowState = WindowState.Minimized;
                    }

                };
        }


        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == System.Windows.WindowState.Minimized)
                this.Hide();

            base.OnStateChanged(e);
        }

               
    }
}
