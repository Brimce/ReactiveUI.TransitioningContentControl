using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using Splat;
using WpfTestApp.First;
using WpfTestApp.FirstBis;
using WpfTestApp.Original;
using WpfTestApp.OriginalBis;
using WpfTestApp.Second;
using WpfTestApp.SecondBis;

namespace WpfTestApp
{
    public class MainViewModel : ReactiveObject //, IScreen
    {
        // The Router associated with this Screen.
        // Required by the IScreen interface.
        public RoutingState Router1 { get; }
        public RoutingState Router2 { get; }

        public RoutingState RouterOriginal { get; }

        // The command that navigates a user to first view model.
        public ReactiveCommand<Unit, IRoutableViewModel> GoNext { get; }

        // The command that navigates a user back.
        public ReactiveCommand<Unit, Unit> GoBack { get; }

        public List<TransitioningContentControl.TransitionType> TransitionTypes { get; } = Enum.GetValues(typeof(TransitioningContentControl.TransitionType)).Cast<TransitioningContentControl.TransitionType>().ToList();

        private readonly ObservableAsPropertyHelper<IRoutableViewModel> _currentVM;
        public IRoutableViewModel CurrentVM => _currentVM.Value;

        public MainViewModel()
        {
            // Initialize the Router.
            Router1 = new RoutingState();
            Router2 = new RoutingState();
            RouterOriginal = new RoutingState();

            _currentVM = RouterOriginal.CurrentViewModel.ToProperty(this, nameof(CurrentVM));
            // Router uses Splat.Locator to resolve views for
            // view models, so we need to register our views
            // using Locator.CurrentMutable.Register* methods.
            //
            // Instead of registering views manually, you 
            // can use custom IViewLocator implementation,
            // see "View Location" section for details.
            //
            Locator.CurrentMutable.Register(() => new FirstView(), typeof(IViewFor<FirstViewModel>));
            Locator.CurrentMutable.Register(() => new SecondView(), typeof(IViewFor<SecondViewModel>));
            Locator.CurrentMutable.Register(() => new FirstBisView(), typeof(IViewFor<FirstBisViewModel>));
            Locator.CurrentMutable.Register(() => new SecondBisView(), typeof(IViewFor<SecondBisViewModel>));
            Locator.CurrentMutable.Register(() => new OriginalView(), typeof(IViewFor<OriginalViewModel>));
            Locator.CurrentMutable.Register(() => new OriginalBisView(), typeof(IViewFor<OriginalBisViewModel>));

            // Manage the routing state. Use the Router.Navigate.Execute
            // command to navigate to different view models. 
            //
            // Note, that the Navigate.Execute method accepts an instance 
            // of a view model, this allows you to pass parameters to 
            // your view models, or to reuse existing view models.
            //
            GoNext = ReactiveCommand.CreateFromObservable(() =>
                CurrentVM is OriginalViewModel
                    ? Observable.Merge(
                        RouterOriginal.Navigate.Execute(new OriginalBisViewModel()),
                        Router1.Navigate.Execute(new FirstBisViewModel()),
                        Router2.Navigate.Execute(new SecondBisViewModel())
                    )
                    : Observable.Merge(
                        RouterOriginal.Navigate.Execute(new OriginalViewModel()),
                        Router1.Navigate.Execute(new FirstViewModel()),
                        Router2.Navigate.Execute(new SecondViewModel())
                    )
            );

            // You can also ask the router to go back.
            GoBack = ReactiveCommand.CreateFromObservable(() =>
                    Observable.Merge(
                        Router1.NavigateBack.Execute(),
                        Router2.NavigateBack.Execute(),
                        RouterOriginal.NavigateBack.Execute()
                    ),
                Observable.Merge(
                    RouterOriginal.NavigateBack.CanExecute,
                    Router1.NavigateBack.CanExecute,
                    Router2.NavigateBack.CanExecute
                )
            );
        }
    }
}