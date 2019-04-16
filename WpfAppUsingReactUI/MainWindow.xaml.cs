using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Legacy;
using DynamicData;
using DynamicData.Binding;
using WpfAppUsingReactUI.Services;
using restTest.Models;
using System.Windows;

namespace WpfAppUsingReactUI
{
    /// <summary>
    /// ReactiveCommand(IObservable <bool>  canExecute = null, IScheduler scheduler = null, bool initialCondition = true)
    /// </summary>
    public partial class MainWindow : ReactiveWindow<SearchViewModel>
    {

        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new SearchViewModel();
            DataContext = ViewModel;
        }

    }




}
