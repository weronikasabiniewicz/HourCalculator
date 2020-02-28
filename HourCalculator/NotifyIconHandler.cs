using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HourCalculator
{
    public class NotifyIconHandler
    {
        public System.Windows.Forms.NotifyIcon NotifyIcon = new System.Windows.Forms.NotifyIcon();
        private readonly MainWindow _window;

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
            NotifyIcon.Icon = Properties.Resources.if_time_173116;
            NotifyIcon.Visible = true;

            CreateContextMenu();

            ConfigureOnClickNotifyIcon();

            ShowNotification += (title, message) => NotifyIcon.ShowBalloonTip(500, title, message, System.Windows.Forms.ToolTipIcon.Info);
        }

        private void ConfigureOnClickNotifyIcon()
        {
            NotifyIcon.Click += (sender, eventArgs) =>
            {
                if (((System.Windows.Forms.MouseEventArgs)eventArgs).Button == System.Windows.Forms.MouseButtons.Right)
                {
                    return;
                }
                var message = OnNotifyIconClicked();

                NotifyIcon.BalloonTipTitle = @"Spent time";
                NotifyIcon.BalloonTipText = message;
                NotifyIcon.ShowBalloonTip(500);

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
                OnStartClicked?.Invoke();
                _window.Hide();
            });
            contextMenu.MenuItems.Add(startMenuItem);

            NotifyIcon.ContextMenu = contextMenu;
        }


        private static System.Windows.Forms.MenuItem CreateMenuItem(string menuItemText, Action onClick)
        {
            var menuItem = new System.Windows.Forms.MenuItem(menuItemText);
            menuItem.Click += (sender, eventArgs) => { onClick(); };
                       
            return menuItem;
        }

    }
}
