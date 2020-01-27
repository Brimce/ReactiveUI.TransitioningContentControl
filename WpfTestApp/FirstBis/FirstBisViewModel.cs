using System;
using System.Reactive.Disposables;
using ReactiveUI;
using Splat;

namespace WpfTestApp.FirstBis
{
    public class FirstBisViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
    {
        public string UrlPathSegment => "First Bis";

        public IScreen HostScreen { get; }

        public FirstBisViewModel(IScreen screen = null)
        {
            HostScreen = screen ?? Locator.Current.GetService<IScreen>();
            this.WhenActivated(disposable =>
            {
                if (App.ShowFirstViewLog)
                    Console.WriteLine("Active FirstBisViewModel");
                Disposable.Create(() =>
                    {
                        if (App.ShowFirstViewLog)
                            Console.WriteLine("Dispose FirstBisViewModel");
                    })
                    .DisposeWith(disposable);
            });
        }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}