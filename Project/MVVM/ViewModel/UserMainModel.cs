using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project.Core;

namespace Project.MVVM.ViewModel
{
    class UserMainModel : ObservableObject
    {
       
        public RelayCommand HomeViewCommand { get; set; }
       
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
          
            HomeVM = new HomeViewModel();
           
            CurrentView = HomeVM;
            HomeViewCommand = new RelayCommand(o =>
            {
                CurrentView = HomeVM;
            });
        }
    }
}
