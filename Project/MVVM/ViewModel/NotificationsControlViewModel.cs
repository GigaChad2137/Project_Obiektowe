using Caliburn.Micro;
using Notifications.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.MVVM.ViewModel
{
    class NotificationsControlViewModel : PropertyChangedBase
    {
        private readonly INotificationManager nManger;

        public string TitleText { get; set; }
        public string MessageText { get; set; }

        public NotificationsControlViewModel(INotificationManager manager)
        {
            nManger = manager;
        }

        public void Okay()
        {
            nManger.Show(new NotificationContent { Title = "Succes", Message = "okay" });
        }
    }
}
