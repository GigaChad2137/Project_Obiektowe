using Project.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Project.MVVM.ViewModel
{
    class UserMainModel : ObservableObject
    {
        public RelayCommand UserVievCommand { get; set; }
        public RelayCommand HomeViewCommand { get; set; }

        public UserViewModel UserVM { get; set; }
        public HomeViewModel HomeVM { get; set; }

        private object _currentView;
        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }
        public UserMainModel()
        {
            UserVM = new UserViewModel();
            HomeVM = new HomeViewModel();
           
            CurrentView = HomeVM;
            HomeViewCommand = new RelayCommand(o =>
            {
                CurrentView = HomeVM;
            });
            UserVievCommand = new RelayCommand(o =>
            {
                CurrentView = UserVM;
            });
        }
    }
}
