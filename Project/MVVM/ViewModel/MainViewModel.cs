﻿using Project.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Project.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {
        public RelayCommand PracownicyViewCommand { get; set; }
        public RelayCommand HomeViewCommand { get; set; }
        public RelayCommand DiscoveryViewCommand { get; set; }

        public PracownicyViewModel PracownicyVM { get; set; }

        public HomeViewModel HomeVM { get; set; }
        public DodajPracownikaViewModel DiscoveryVM { get; set; }
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
        public MainViewModel()
        {
            PracownicyVM = new PracownicyViewModel();
            HomeVM = new HomeViewModel();
            DiscoveryVM = new DodajPracownikaViewModel();
            CurrentView = HomeVM;
            HomeViewCommand = new RelayCommand(o =>
            {
                CurrentView = HomeVM;
            });
            DiscoveryViewCommand = new RelayCommand(o =>
            {
                CurrentView = DiscoveryVM;
            });
            PracownicyViewCommand = new RelayCommand(o =>
            {
                CurrentView = PracownicyVM;
            });
        }
    }
}
