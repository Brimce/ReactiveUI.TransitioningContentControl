using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace ReactiveUI.ControlLibrary
{
    /// <summary>
    /// TransitioningContentControl2 inherits Control instead ContentControl.
    /// and add a Content property
    /// A Control that animates the transition when its content is changed.
    /// </summary>
    [TemplatePart(Name = "PART_Container", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PART_PreviousContentPresentationSite", Type = typeof(ContentPresenter))]
    [TemplatePart(Name = "PART_CurrentContentPresentationSite", Type = typeof(ContentPresenter))]
    [TemplateVisualState(Name = NormalState, GroupName = PresentationGroup)]
    public class TransitioningContentControl1 : Control
    {
        #region Content

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
            "Content", 
            typeof(object), 
            typeof(TransitioningContentControl1), 
            new PropertyMetadata(ContentChangedCallback));

        private static void ContentChangedCallback(DependencyObject obj,
            DependencyPropertyChangedEventArgs args)
        {
            var transitioningContentControl = (TransitioningContentControl1) obj;
            var newContent = args.NewValue;

            // Call QueueTransition to change ValueStates.
            transitioningContentControl.QueueTransition(transitioningContentControl.Content, newContent);

            // Call OnValueChanged to raise the ValueChanged event.
            transitioningContentControl.OnValueChanged(
                new ContentChangedEventArgs(ContentChangedEvent, newContent)
            );
        }

        public object Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }
        

        #endregion
        
        #region ContentChanged

        public static readonly RoutedEvent ContentChangedEvent =
            EventManager.RegisterRoutedEvent("ContentChanged", RoutingStrategy.Direct,
                typeof(ContentChangedEventHandler), typeof(TransitioningContentControl1));

        public event ContentChangedEventHandler ContentChanged
        {
            add => AddHandler(ContentChangedEvent, value);
            remove => RemoveHandler(ContentChangedEvent, value);
        }

        protected virtual void OnValueChanged(ContentChangedEventArgs e)
        {
            // Raise the ContentChanged event so applications can be alerted
            // when Value changes.
            RaiseEvent(e);
        }

        public delegate void ContentChangedEventHandler(object sender, ContentChangedEventArgs e);

        public class ContentChangedEventArgs : RoutedEventArgs
        {
            public ContentChangedEventArgs(RoutedEvent id, object content)
            {
                Content = content;
                RoutedEvent = id;
            }

            public object Content { get; }
        }

        #endregion

        /// <summary>
        /// <see cref="DependencyProperty"/> for the <see cref="Transition"/> property.
        /// </summary>
        public static readonly DependencyProperty TransitionProperty = DependencyProperty.RegisterAttached(
            "Transition",
            typeof(TransitioningContentControl.TransitionType),
            typeof(TransitioningContentControl1),
            new PropertyMetadata(TransitioningContentControl.TransitionType.Fade, OnTransitionChanged));

        /// <summary>
        /// <see cref="DependencyProperty"/> for the <see cref="TransitionPart"/> property.
        /// </summary>
        public static readonly DependencyProperty TransitionPartProperty =
            DependencyProperty.RegisterAttached(
                "TransitionPart",
                typeof(TransitionPartType),
                typeof(TransitioningContentControl1),
                new PropertyMetadata(TransitionPartType.OutIn, OnTransitionPartChanged));

        private const string PresentationGroup = "PresentationStates";
        private const string NormalState = "Normal";
        private bool _isTransitioning;
        private bool _canSplitTransition;
        private Storyboard _startingTransition;
        private Storyboard _completingTransition;
        private Grid _container;
        private ContentPresenter _previousContentPresentationSite;
        private ContentPresenter _currentContentPresentationSite;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransitioningContentControl1"/> class.
        /// </summary>
        public TransitioningContentControl1()
        {
            DefaultStyleKey = typeof(TransitioningContentControl1);
        }

        /// <summary>
        /// Occurs when a transition has completed.
        /// </summary>
        public event RoutedEventHandler TransitionCompleted;

        /// <summary>
        /// Occurs when a transition has started.
        /// </summary>
        public event RoutedEventHandler TransitionStarted;

        
        /// <summary>
        /// Represents the part of the transition that the developer would like the TransitioningContentControl to perform.
        /// </summary>
        /// <remarks>This only applies to certain TransitionTypes. An InvalidOperationException will be thrown if the TransitionType does not support the TransitionPartType. Default is OutIn.</remarks>
        public enum TransitionPartType
        {
            /// <summary>
            /// Transitions out only.
            /// </summary>
            Out,

            /// <summary>
            /// Transitions in only.
            /// </summary>
            In,

            /// <summary>
            /// Transitions in and out.
            /// </summary>
            OutIn
        }

        /// <summary>
        /// Gets or sets the transition.
        /// </summary>
        /// <value>The transition.</value>
        public TransitioningContentControl.TransitionType Transition { get => (TransitioningContentControl.TransitionType)GetValue(TransitionProperty); set => SetValue(TransitionProperty, value); }

        /// <summary>
        /// Gets or sets the transition part.
        /// </summary>
        /// <value>The transition part.</value>
        public TransitionPartType TransitionPart { get => (TransitionPartType)GetValue(TransitionPartProperty); set => SetValue(TransitionPartProperty, value); }

        private Storyboard StartingTransition
        {
            get => _startingTransition;
            set
            {
                _startingTransition = value;
                if (_startingTransition != null) SetTransitionDefaultValues();
            }
        }

        private Storyboard CompletingTransition
        {
            get => _completingTransition;
            set
            {
                // Decouple transition.
                if (_completingTransition != null) _completingTransition.Completed -= OnTransitionCompleted;

                _completingTransition = value;

                if (_completingTransition == null) 
                    return;

                _completingTransition.Completed += OnTransitionCompleted;
                SetTransitionDefaultValues();
            }
        }

        /// <inheritdoc/>
        public override void OnApplyTemplate()
        {
            // Wire up all of the various control parts.
            _container = (Grid)GetTemplateChild("PART_Container");
            if (_container == null)
            {
                throw new ArgumentException("PART_Container not found.");
            }

            _currentContentPresentationSite =
                (ContentPresenter)GetTemplateChild("PART_CurrentContentPresentationSite");
            if (_currentContentPresentationSite == null)
            {
                throw new ArgumentException("PART_CurrentContentPresentationSite not found.");
            }

            _previousContentPresentationSite =
                (ContentPresenter)GetTemplateChild("PART_PreviousContentPresentationSite");

            // Set the current content site to the first piece of content.
            _currentContentPresentationSite.Content = Content;
            VisualStateManager.GoToState(this, NormalState, false);
        }

        private static void OnTransitionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var transitioningContentControl = (TransitioningContentControl1)d;
            var transition = (TransitioningContentControl.TransitionType)e.NewValue;

            transitioningContentControl._canSplitTransition = VerifyCanSplitTransition(
                transition,
                transitioningContentControl.TransitionPart);
        }

        private static void OnTransitionPartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var transitioningContentControl = (TransitioningContentControl1)d;
            var transitionPart = (TransitionPartType)e.NewValue;

            transitioningContentControl._canSplitTransition =
                VerifyCanSplitTransition(transitioningContentControl.Transition, transitionPart);
        }

        private static bool VerifyCanSplitTransition(TransitioningContentControl.TransitionType transition, TransitionPartType transitionPart)
        {
            // Check whether the TransitionPart is compatible with the current transition.
            if (transition != TransitioningContentControl.TransitionType.Fade && transition != TransitioningContentControl.TransitionType.FadeDown) 
                return true;
            
            if (transitionPart != TransitionPartType.OutIn)
                throw new InvalidOperationException("Cannot split this transition.");

            return false;
        }

        /// <summary>
        /// Aborts the transition.
        /// </summary>
        private void AbortTransition()
        {
            // Go to a normal state and release our hold on the old content.
            VisualStateManager.GoToState(this, NormalState, false);
            
            _isTransitioning = false;
            if (_previousContentPresentationSite != null)
            {
                _previousContentPresentationSite.Content = null;
            }
        }

        private void OnTransitionCompleted(object sender, EventArgs e)
        {
            AbortTransition();

            TransitionCompleted?.Invoke(this, new RoutedEventArgs());
        }

        private void RaiseTransitionStarted()
        {
            TransitionStarted?.Invoke(this, new RoutedEventArgs());
        }

        private void QueueTransition(object oldContent, object newContent)
        {
            // Both ContentPresenters must be available, otherwise a transition is useless.
            if (_currentContentPresentationSite != null && _previousContentPresentationSite != null)
            {
                _currentContentPresentationSite.Content = newContent;
                _previousContentPresentationSite.Content = oldContent;

                if (_isTransitioning) 
                    return;

                // Determine the TransitionPart that is associated with this transition and either set up a single part transition, or a queued transition.
                string startingTransitionName;
                if (TransitionPart == TransitionPartType.OutIn && _canSplitTransition)
                {
                    // Wire up the completion transition.
                    var transitionInName = Transition + "Transition_" + TransitionPartType.In;
                    CompletingTransition = GetTransitionStoryboardByName(transitionInName);

                    // Wire up the first transition to start the second transition when it's complete.
                    startingTransitionName = Transition + "Transition_" + TransitionPartType.Out;
                    var transitionOut = GetTransitionStoryboardByName(startingTransitionName);
                    transitionOut.Completed += (sender, args) => 
                        VisualStateManager.GoToState(this, transitionInName, false);
                    StartingTransition = transitionOut;
                }
                else
                {
                    startingTransitionName = Transition + "Transition_" + TransitionPart;
                    CompletingTransition = GetTransitionStoryboardByName(startingTransitionName);
                }

                // Start the transition.
                _isTransitioning = true;
                RaiseTransitionStarted();
                VisualStateManager.GoToState(this, startingTransitionName, false);
            }
            else
            {
                if (_currentContentPresentationSite != null)
                {
                    _currentContentPresentationSite.Content = newContent;
                }
            }
        }

        private Storyboard GetTransitionStoryboardByName(string transitionName)
        {
            // Hook up the CurrentTransition.
            var presentationGroup =
                ((IEnumerable<VisualStateGroup>)VisualStateManager.GetVisualStateGroups(_container)).FirstOrDefault(o => o.Name == PresentationGroup);
            if (presentationGroup == null)
            {
                throw new ArgumentException("Invalid VisualStateGroup.");
            }

            var transition =
                ((IEnumerable<VisualState>)presentationGroup.States).Where(o => o.Name == transitionName).Select(
                    o => o.Storyboard).FirstOrDefault();
            if (transition == null)
            {
                throw new ArgumentException("Invalid transition");
            }

            return transition;
        }

        /// <summary>
        /// Sets default values for certain transition types.
        /// </summary>
        private void SetTransitionDefaultValues()
        {
            switch (Transition)
            {
                // Do some special handling of particular transitions so that we get nice smooth transitions that utilise the size of the content.
                case TransitioningContentControl.TransitionType.FadeDown:
                {
                    var completingDoubleAnimation = (DoubleAnimation)CompletingTransition.Children[0];
                    completingDoubleAnimation.From = -ActualHeight;

                    var startingDoubleAnimation = (DoubleAnimation)CompletingTransition.Children[1];
                    startingDoubleAnimation.To = ActualHeight;

                    return;
                }
                case TransitioningContentControl.TransitionType.SlideLeft:
                {
                    if (CompletingTransition != null)
                    {
                        if (CompletingTransition.Children[0] is DoubleAnimationUsingKeyFrames animationUsingKeyFrames)
                            animationUsingKeyFrames.KeyFrames[1].Value = -ActualWidth;

                        if (CompletingTransition.Children[0] is DoubleAnimation doubleAnimation)
                            doubleAnimation.To = -ActualWidth;
                    }

                    if (StartingTransition == null) 
                        return;
                    
                    var startingDoubleAnimation = (DoubleAnimation)StartingTransition.Children[0];
                    startingDoubleAnimation.To = -ActualWidth;

                    return;
                }

                case TransitioningContentControl.TransitionType.Fade:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}