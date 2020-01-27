using System;
using System.Reactive.Disposables;
using ReactiveUI;
using Splat;

namespace WpfTestApp.SecondBis
{
    public class SecondBisViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
    {
        public string UrlPathSegment => "Second Bis";

        public IScreen HostScreen { get; }

        public SecondBisViewModel(IScreen screen = null)
        {
            HostScreen = screen ?? Locator.Current.GetService<IScreen>();
            this.WhenActivated(disposable =>
            {
                if (App.ShowSecondViewLog)
                    Console.WriteLine("Active SecondBisViewModel");
                Disposable.Create(() =>
                    {
                        if (App.ShowSecondViewLog)
                            Console.WriteLine("Dispose SecondBisViewModel");
                    })
                    .DisposeWith(disposable);
            });
        }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}