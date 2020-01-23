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
            Console.WriteLine("Create OriginalBisView");

            this.WhenActivated(disposables =>
            {
                Console.WriteLine("Active OriginalBisView");

                this.OneWayBind(ViewModel, x => x.UrlPathSegment, x => x.PathTextBlock.Text)
                    .DisposeWith(disposables);
            });
        }
    }
}