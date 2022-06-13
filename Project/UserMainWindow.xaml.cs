﻿using System;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Project
{
    /// <summary>
    /// Logika interakcji dla klasy UserMainWindow.xaml
    /// </summary>
    public partial class UserMainWindow : Window
    {
        public UserMainWindow()
        {
            InitializeComponent();
        }
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            LoginScreen dashboard = new LoginScreen();
            dashboard.Show();
            this.Close();

        }
    }
}
