using System;
using System.Reactive.Disposables;
using ReactiveUI;

namespace WpfTestApp.Second
{
    public partial class SecondView : ReactiveUserControl<SecondViewModel>
    {
        public SecondView()
        {
            InitializeComponent();
            Console.WriteLine("Create SecondView");
            this.WhenActivated(disposables =>
            {
                Console.WriteLine("Active SecondView");

                this.OneWayBind(ViewModel, x => x.UrlPathSegment, x => x.PathTextBlock.Text)
                    .DisposeWith(disposables);
            });
        }
    }
}