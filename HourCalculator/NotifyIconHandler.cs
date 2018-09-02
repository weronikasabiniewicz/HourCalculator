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
        private MainWindow _window;

        public NotifyIconHandler(MainWindow window)
        {
            _window = window;
            CreateNotifyIcon();
        }

        public Action OnStartClicked { get; set; }
        public Func<string> OnNotifyIconClicked { get; set; }

        public Action<string, string> ShowNotification { get; set; }

        private void CreateNotifyIcon()
        {
            notifyIcon.Icon = Properties.Resources.if_time_173116;
            notifyIcon.Visible = true;

            CreateContextMenu();

            ConfigureOnClickNotifyIcon();

            ShowNotification += (title, message) => notifyIcon.ShowBalloonTip(500, title, message, System.Windows.Forms.ToolTipIcon.Info);
        }

        private void ConfigureOnClickNotifyIcon()
        {
            notifyIcon.Click += (sender, eventArgs) =>
            {
                if (((System.Windows.Forms.MouseEventArgs)eventArgs).Button == System.Windows.Forms.MouseButtons.Right)
                {
                    return;
                }
                var message = OnNotifyIconClicked();

                notifyIcon.BalloonTipTitle = "Spent time";
                notifyIcon.BalloonTipText = message;
                notifyIcon.ShowBalloonTip(500);

            };
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
                if(OnStartClicked != null)
                {
                    OnStartClicked();
                };
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
