using System;
using System.Linq;
using System.Reactive.Disposables;
using ReactiveUI;

namespace WpfTestApp
{
    public partial class MainWindow : ReactiveWindow<MainViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MainViewModel();
            ComboBoxTransitionType.ItemsSource = ViewModel.TransitionTypes;
            ComboBoxTransitionType.SelectedItem = ViewModel.TransitionTypes.First();

            this.WhenActivated(disposables =>
            {
                // Bind the view model router to RoutedViewHost.Router property.
                this.OneWayBind(ViewModel, x => x.Router1, x => x.RoutedViewHost1.Router)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, x => x.Router2, x => x.RoutedViewHost2.Router)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, x => x.RouterOriginal, x => x.OriginalRoutedViewHost.Router)
                    .DisposeWith(disposables);
                this.BindCommand(ViewModel, x => x.GoNext, x => x.GoNextButton)
                    .DisposeWith(disposables);
                this.BindCommand(ViewModel, x => x.GoBack, x => x.GoBackButton)
                    .DisposeWith(disposables);
            });

            RoutedViewHost2.ContentChanged += (sender, args) => Console.WriteLine("RoutedViewHost2 content changed");
        }
    }
}