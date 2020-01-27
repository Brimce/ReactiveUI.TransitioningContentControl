using System;
using System.Reactive.Disposables;
using ReactiveUI;
using Splat;

namespace WpfTestApp.OriginalBis
{
    public class OriginalBisViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
    {
        public string UrlPathSegment => "Original Bis";

        public IScreen HostScreen { get; }

        public OriginalBisViewModel(IScreen screen = null)
        {
            HostScreen = screen ?? Locator.Current.GetService<IScreen>();
            this.WhenActivated(disposable =>
            {
                if (App.ShowOriginaltViewLog)
                    Console.WriteLine("Active OriginalBisViewModel");
                Disposable.Create(() =>
                    {
                        if (App.ShowOriginaltViewLog)
                            Console.WriteLine("Dispose OriginalBisViewModel");
                    })
                    .DisposeWith(disposable);
            });
        }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}