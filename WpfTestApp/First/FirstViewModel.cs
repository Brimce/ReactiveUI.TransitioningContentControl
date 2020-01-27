using System;
using System.Reactive.Disposables;
using ReactiveUI;
using Splat;

namespace WpfTestApp.First
{
    public class FirstViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
    {
        public string UrlPathSegment => "First";

        public IScreen HostScreen { get; }

        public FirstViewModel(IScreen screen = null)
        {
            HostScreen = screen ?? Locator.Current.GetService<IScreen>();
            this.WhenActivated(disposable =>
            {
                if (App.ShowFirstViewLog)
                    Console.WriteLine("Active FirstViewModel");
                Disposable.Create(() =>
                    {
                        if (App.ShowFirstViewLog)
                            Console.WriteLine("Dispose FirstViewModel");
                    })
                    .DisposeWith(disposable);
            });
        }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}