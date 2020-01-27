using System;
using System.Reactive.Disposables;
using ReactiveUI;
using Splat;

namespace WpfTestApp.Original
{
    public class OriginalViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
    {
        public string UrlPathSegment => "Original";

        public IScreen HostScreen { get; }

        public OriginalViewModel(IScreen screen = null)
        {
            HostScreen = screen ?? Locator.Current.GetService<IScreen>();
            this.WhenActivated(disposable =>
            {
                if (App.ShowOriginaltViewLog)
                    Console.WriteLine("Active OriginalViewModel");
                Disposable.Create(() =>
                    {
                        if (App.ShowOriginaltViewLog)
                            Console.WriteLine("Dispose OriginalViewModel");
                    })
                    .DisposeWith(disposable);
            });
        }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}