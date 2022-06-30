using Caliburn.Micro;
using Notifications.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.MVVM.ViewModel
{
    /*Klasa stworzona do Notifikacji z modułu notifications.wpf */
    class NotificationsControlViewModel : PropertyChangedBase
    {
        private readonly INotificationManager nManger;

        public string TitleText { get; set; }
        public string MessageText { get; set; }

        public NotificationsControlViewModel(INotificationManager manager)
        {
            nManger = manager;
        }
    }
}
