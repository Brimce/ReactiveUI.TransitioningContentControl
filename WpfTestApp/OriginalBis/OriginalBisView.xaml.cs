using System;
using System.Reactive.Disposables;
using ReactiveUI;

namespace WpfTestApp.OriginalBis
{
    public partial class OriginalBisView : ReactiveUserControl<OriginalBisViewModel>
    {
        public OriginalBisView()
        {
            InitializeComponent();
            if (App.ShowOriginaltViewLog)
                Console.WriteLine("Create OriginalBisView");

            this.WhenActivated(disposables =>
            {
                if (App.ShowOriginaltViewLog)
                    Console.WriteLine("Active OriginalBisView");

                this.OneWayBind(ViewModel, x => x.UrlPathSegment, x => x.PathTextBlock.Text)
                    .DisposeWith(disposables);
            });
        }
    }
}