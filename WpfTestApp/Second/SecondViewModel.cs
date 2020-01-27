using System;
using System.Reactive.Disposables;
using ReactiveUI;
using Splat;

namespace WpfTestApp.Second
{
    public class SecondViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
    {
        public string UrlPathSegment => "Second";

        public IScreen HostScreen { get; }

        public SecondViewModel(IScreen screen = null)
        {
            HostScreen = screen ?? Locator.Current.GetService<IScreen>();
            this.WhenActivated(disposable =>
            {
                if (App.ShowSecondViewLog)
                    Console.WriteLine("Active SecondViewModel");
                Disposable.Create(() =>
                    {
                        if (App.ShowSecondViewLog)
                            Console.WriteLine("Dispose SecondViewModel");
                    })
                    .DisposeWith(disposable);
            });
        }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}