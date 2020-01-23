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
            Console.WriteLine("Create FirstBisView");

            this.WhenActivated(disposables =>
            {
                Console.WriteLine("Active FirstBisView");

                this.OneWayBind(ViewModel, x => x.UrlPathSegment, x => x.PathTextBlock.Text)
                    .DisposeWith(disposables);
            });
        }
    }
}