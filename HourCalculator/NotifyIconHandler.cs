using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HourCalculator
{
    public class NotifyIconHandler
    {
        public System.Windows.Forms.NotifyIcon notifyIcon = new System.Windows.Forms.NotifyIcon();
        private ViewModel _vm;
        private MainWindow _window;

        public NotifyIconHandler(ViewModel viewModel, MainWindow window)
        {
            _vm = viewModel;
            _window = window;
            _vm.NotifIcon = this;
            CreateNotifyIcon();
        }

        public void CreateNotifyIcon()
        {
            notifyIcon.Icon = Properties.Resources.if_time_173116;
            notifyIcon.Visible = true;

            CreateContextMenu();

            ConfigureOnClickNotifyIcon();
        }

        private void ConfigureOnClickNotifyIcon()
        {
            notifyIcon.Click += (sender, eventArgs) =>
            {
                notifyIcon.BalloonTipTitle = "Spent time";

                if (((System.Windows.Forms.MouseEventArgs)eventArgs).Button == System.Windows.Forms.MouseButtons.Right)
                {
                    return;
                }

                notifyIcon.BalloonTipText = PrepareBallonTextMessage();
                notifyIcon.ShowBalloonTip(500);

            };
        }

        private string PrepareBallonTextMessage()
        {
            var ballonTipText = new StringBuilder("Please press start"); 
                
                if (_vm.SpendTime.HasValue)
                {
                    ballonTipText.Clear();
                    ballonTipText.Append(_vm.SpendTime.Value.Hours + "h " + _vm.SpendTime.Value.Minutes + "m");
                }
                
                if (_vm.IsOverTime)
                {
                    ballonTipText.AppendLine();
                    ballonTipText.Append("Overtime: ");
                    ballonTipText.Append(_vm.OverTime.Value.Hours + "h " + _vm.OverTime.Value.Minutes + "m");
                }
            return ballonTipText.ToString();
        }

        private void CreateContextMenu()
        {
            var contextMenu = new System.Windows.Forms.ContextMenu();

            var showMenuItem = CreateMenuItem("Show", () =>
            {
                _window.Show();
                _window.WindowState = System.Windows.WindowState.Normal;
            });
            contextMenu.MenuItems.Add(showMenuItem);

            var startMenuItem = CreateMenuItem("Start", () =>
            {
                _vm.Start();
                _window.Hide();
            });
            contextMenu.MenuItems.Add(startMenuItem);

            notifyIcon.ContextMenu = contextMenu;
        }


        private System.Windows.Forms.MenuItem CreateMenuItem(string menuItemText, Action onClick)
        {
            var menuItem = new System.Windows.Forms.MenuItem(menuItemText);
            menuItem.Click += (sender, eventArgs) => { onClick(); };

            return menuItem;
        }

    }
}
