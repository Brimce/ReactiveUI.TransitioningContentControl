using System;
using System.Reactive.Disposables;
using ReactiveUI;
using WpfTestApp.First;

namespace WpfTestApp.FirstBis
{
    public partial class FirstBisView : ReactiveUserControl<FirstBisViewModel>
    {
        public FirstBisView()
        {
            InitializeComponent();
            if (App.ShowFirstViewLog)
                Console.WriteLine("Create FirstBisView");

            this.WhenActivated(disposables =>
            {
                if (App.ShowFirstViewLog)
                    Console.WriteLine("Active FirstBisView");

                this.OneWayBind(ViewModel, x => x.UrlPathSegment, x => x.PathTextBlock.Text)
                    .DisposeWith(disposables);
            });
        }
    }
}