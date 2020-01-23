using System;
using System.Reactive.Disposables;
using ReactiveUI;

namespace WpfTestApp.SecondBis
{
    public partial class SecondBisView : ReactiveUserControl<SecondBisViewModel>
    {
        public SecondBisView()
        {
            InitializeComponent();
            Console.WriteLine("Create SecondBisView");
            this.WhenActivated(disposables =>
            {
                Console.WriteLine("Active SecondBisView");

                this.OneWayBind(ViewModel, x => x.UrlPathSegment, x => x.PathTextBlock.Text)
                    .DisposeWith(disposables);
            });
        }
    }
}