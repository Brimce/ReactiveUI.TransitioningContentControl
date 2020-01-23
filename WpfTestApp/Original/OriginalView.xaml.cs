using System;
using System.Reactive.Disposables;
using ReactiveUI;

namespace WpfTestApp.Original
{
    public partial class OriginalView : ReactiveUserControl<OriginalViewModel>
    {
        public OriginalView()
        {
            InitializeComponent();
            Console.WriteLine("Create OriginalView");

            this.WhenActivated(disposables =>
            {
                Console.WriteLine("Active OriginalView");

                this.OneWayBind(ViewModel, x => x.UrlPathSegment, x => x.PathTextBlock.Text)
                    .DisposeWith(disposables);
            });
        }
    }
}