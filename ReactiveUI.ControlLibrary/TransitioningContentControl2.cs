using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using ReactiveUI.ControlLibrary.Storyboards;

namespace ReactiveUI.ControlLibrary
{
    /// <summary>
    /// A Control that animates the transition when its content is changed.
    /// </summary>
    [TemplatePart(Name = "PART_Container", Type = typeof(FrameworkElement))]
    public class TransitioningContentControl2 : Control
    {
        private bool _isTransitioning;

        #region Content

        /// <summary>
        /// <see cref="DependencyProperty"/> for the <see cref="Content"/> property.
        /// </summary>
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
            "Content",
            typeof(object),
            typeof(TransitioningContentControl2),
            new PropertyMetadata(ContentChangedCallback));

        private static void ContentChangedCallback(DependencyObject obj,
            DependencyPropertyChangedEventArgs args)
        {
            var transitioningContentControl = (TransitioningContentControl2) obj;

            // Call QueueTransition to change ValueStates.
            transitioningContentControl.QueueTransition(args.NewValue);

            // Call OnValueChanged to raise the ValueChanged event.
            transitioningContentControl.OnValueChanged(
                new ContentChangedEventArgs(ContentChangedEvent, args.NewValue)
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
                typeof(ContentChangedEventHandler), typeof(TransitioningContentControl2));

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
            typeof(ReactiveUI.TransitioningContentControl.TransitionType),
            typeof(TransitioningContentControl2),
            new PropertyMetadata(ReactiveUI.TransitioningContentControl.TransitionType.Fade));

        /// <summary>
        /// <see cref="DependencyProperty"/> for the <see cref="TransitionPart"/> property.
        /// </summary>
        public static readonly DependencyProperty TransitionPartProperty =
            DependencyProperty.RegisterAttached(
                "TransitionPart",
                typeof(TransitionPartType),
                typeof(TransitioningContentControl2),
                new PropertyMetadata(TransitionPartType.OutIn));

        private Grid _container;
        private Storyboard _storyboard;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransitioningContentControl2"/> class.
        /// </summary>
        public TransitioningContentControl2()
        {
            DefaultStyleKey = typeof(TransitioningContentControl2);
        }

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
        public ReactiveUI.TransitioningContentControl.TransitionType Transition
        {
            get => (ReactiveUI.TransitioningContentControl.TransitionType) GetValue(TransitionProperty);
            set => SetValue(TransitionProperty, value);
        }

        /// <summary>
        /// Gets or sets the transition part.
        /// </summary>
        /// <value>The transition part.</value>
        public TransitionPartType TransitionPart
        {
            get => (TransitionPartType) GetValue(TransitionPartProperty);
            set => SetValue(TransitionPartProperty, value);
        }

        /// <inheritdoc/>
        public override void OnApplyTemplate()
        {
            // Wire up all of the various control parts.
            _container = (Grid) GetTemplateChild("PART_Container") ?? throw new ArgumentException("PART_Container not found.");

            // Set the current content site to the first piece of content.
            _container.Children.Add(
                new ContentPresenter
                {
                    Content = Content
                }
            );
        }

        private void QueueTransition(object newContent)
        {
            //not null if is playing
            if (_isTransitioning)
            {
                //change the content 
                GetNewContentPresenter().Content = newContent;
                return;
            }

            var contentPresenter = new ContentPresenter
            {
                Content = newContent
            };

            _container.Children.Add(contentPresenter);

            // Create the storyboard
            _storyboard = CreateStoryboard(
                Transition,
                TransitionPart,
                GetOldContentPresenter(),
                contentPresenter,
                _container
            );

            // Remove old content
            _storyboard.Completed += OnStoryboardCompleted;

            _storyboard.Begin(_container);
            _isTransitioning = true;
        }

        private void OnStoryboardCompleted(object sender, EventArgs args)
        {
            _container.Children.RemoveAt(0);
            _storyboard.Completed -= OnStoryboardCompleted;
            _storyboard = null;
            _isTransitioning = false;
        }

        /// <summary>
        /// Get 
        /// </summary>
        /// <returns></returns>
        private ContentPresenter GetOldContentPresenter()
        {
            return _container.Children.Count == 1 ? null : (ContentPresenter) _container.Children[0];
        }

        /// <summary>
        /// Get last added ContentPresenter
        /// </summary>
        /// <returns></returns>
        private ContentPresenter GetNewContentPresenter()
        {
            return (ContentPresenter) _container.Children[_container.Children.Count - 1];
        }

        private Storyboard CreateStoryboard(
            ReactiveUI.TransitioningContentControl.TransitionType transition, 
            TransitionPartType transitionPart, 
            ContentPresenter oldContent, 
            ContentPresenter newContent, 
            FrameworkElement container)
        {
            var sb = new Storyboard();
            switch (transition)
            {
                case ReactiveUI.TransitioningContentControl.TransitionType.Fade:
                    switch (transitionPart)
                    {
                        case TransitionPartType.Out:
                        case TransitionPartType.In:
                            throw new InvalidOperationException("Cannot split this transition.");

                        case TransitionPartType.OutIn:
                            sb
                                .AddFadeIn(0.3f, newContent)
                                .AddFadeOut(0.3f, oldContent);

                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(transitionPart), transitionPart, null);
                    }
                    break;

                case ReactiveUI.TransitioningContentControl.TransitionType.FadeDown:
                    switch (transitionPart)
                    {
                        case TransitionPartType.Out:
                        case TransitionPartType.In:
                            throw new InvalidOperationException("Cannot split this transition.");

                        case TransitionPartType.OutIn:
                            sb
                                .AddFadeIn(0.3f, newContent)
                                .AddSlideFromTop(0.3f, 30, 0, target: newContent)
                                .AddSlideToBottom(0.3f, 30, 0, target: oldContent)
                                .AddFadeOut(0.3f, oldContent);

                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(transitionPart), transitionPart, null);
                    }
                    break;

                case ReactiveUI.TransitioningContentControl.TransitionType.SlideLeft:
                    switch (transitionPart)
                    {
                        case TransitionPartType.Out:
                            sb
                                .AddSlideToLeft(0.3f, container.ActualWidth, target: oldContent);
                            
                            Panel.SetZIndex(oldContent, 1);
                            Panel.SetZIndex(newContent, 0);

                            break;

                        case TransitionPartType.In:
                            sb
                                .AddFadeOut(0.4f, oldContent)
                                .AddFadeIn(0.2f, newContent, TimeSpan.FromMilliseconds(400))
                                .AddSlideFromLeft(0.2f, container.ActualWidth, target: newContent, beginTime: TimeSpan.FromMilliseconds(400));

                            break;

                        case TransitionPartType.OutIn:

                            if (oldContent.Content == null)
                                sb
                                    .AddSlideFromLeft(
                                        new[]
                                        {
                                            new StoryboardHelpers.KeyFrame(_container.ActualWidth, 0),
                                            new StoryboardHelpers.KeyFrame(0, 0.3f, new CircleEase {EasingMode = EasingMode.EaseOut})
                                        },
                                        target: newContent
                                    );
                            else
                                sb
                                    .AddSlideToLeft(0.2f, _container.ActualWidth, target: oldContent)
                                    .AddSlideFromLeft(
                                        new[]
                                        {
                                            new StoryboardHelpers.KeyFrame(_container.ActualWidth, 0),
                                            new StoryboardHelpers.KeyFrame(_container.ActualWidth, 0.6f),
                                            new StoryboardHelpers.KeyFrame(0, 0.9f, new CircleEase {EasingMode = EasingMode.EaseOut})
                                        },
                                        target: newContent
                                    );

                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(transitionPart), transitionPart, null);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(transition), transition, null);
            }

            return sb;
        }
    }

    ///// <summary>
    ///// Represents the type of transition that a TransitioningContentControl will perform.
    ///// </summary>
    //public enum TransitionType
    //{
    //    /// <summary>
    //    /// A simple fading transition.
    //    /// </summary>
    //    Fade,

    //    /// <summary>
    //    /// A transition that fades the new element in from the top.
    //    /// </summary>
    //    FadeDown,

    //    /// <summary>
    //    /// A transition that slides old content left and out of view, then slides new content back in from the same direction.
    //    /// </summary>
    //    SlideLeft
    //}
}