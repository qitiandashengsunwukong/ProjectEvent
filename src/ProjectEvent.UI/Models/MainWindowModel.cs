﻿using Microsoft.Extensions.DependencyInjection;
using ProjectEvent.UI.Controls;
using ProjectEvent.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectEvent.UI.Models
{
    public class MainWindowModel : UINotifyPropertyChanged
    {
        private IServiceProvider ServiceProvider_;
        public IServiceProvider ServiceProvider
        {
            get { return ServiceProvider_; }
            set { ServiceProvider_ = value; OnPropertyChanged(); }
        }

        private string Uri_;
        public string Uri
        {
            get { return Uri_; }
            set { Uri_ = value; OnPropertyChanged(); }
        }
       
    }
}