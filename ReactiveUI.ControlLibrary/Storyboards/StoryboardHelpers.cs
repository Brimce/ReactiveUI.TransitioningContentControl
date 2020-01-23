using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Animation;

namespace ReactiveUI.ControlLibrary.Storyboards
{
    /// <summary>
    /// Animation helpers for <see cref="Storyboard"/>
    /// https://github.com/taisetna/Faseto.Word/blob/5b8eebb5e995a606dd77a8240203bfb74779e63d/Faseto.Word/Faseto.Word/Animation/StoryboardHelpers.cs
    /// </summary>
    public static class StoryboardHelpers
    {
        #region Sliding To/From Left

        /// <summary>
        /// Adds a slide from left animation to the storyboard
        /// </summary>
        /// <param name="storyboard">The storyboard to add the animation to</param>
        /// <param name="seconds">The time the animation will take</param>
        /// <param name="offset">The distance to the left to start from</param>
        /// <param name="decelerationRatio">The rate of deceleration</param>
        /// <param name="keepMargin">Whether to keep the element at the same width during animation</param>
        /// <param name="target">The framework element targeted by the fade in</param>
        /// <param name="beginTime"></param>
        public static Storyboard AddSlideFromLeft(this Storyboard storyboard, float seconds, double offset, float decelerationRatio = 0.9f, bool keepMargin = true, DependencyObject target = null, TimeSpan? beginTime = null)
        {
            // Create the margin animate from right 
            var animation = new ThicknessAnimation
            {
                BeginTime = beginTime ?? new TimeSpan(0),
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
                From = new Thickness(-offset, 0, keepMargin ? offset : 0, 0),
                To = new Thickness(0),
                DecelerationRatio = decelerationRatio
            };

            // Set the target property name
            Storyboard.SetTargetProperty(animation, new PropertyPath("Margin"));

            // Set the target if specified
            if(target != null)
                Storyboard.SetTarget(animation, target);

            // Add this to the storyboard
            storyboard.Children.Add(animation);

            return storyboard;
        }

        public static Storyboard AddSlideFromLeft(this Storyboard storyboard, IEnumerable<KeyFrame> keyFrameCollection, DependencyObject target = null, TimeSpan? beginTime = null)
        {
            var easingThicknessKeyFrames = keyFrameCollection.Select(frame =>
                new EasingThicknessKeyFrame(
                    new Thickness(-frame.Offset, 0, frame.Offset, 0),
                    KeyTime.FromTimeSpan(TimeSpan.FromSeconds(frame.Seconds)),
                    frame.EasingFunction)
            );

            var keyFrames = new ThicknessKeyFrameCollection();
            foreach (var keyFrame in easingThicknessKeyFrames) keyFrames.Add(keyFrame);

            // Create the margin animate from right 
            var animation = new ThicknessAnimationUsingKeyFrames
            {
                BeginTime = beginTime ?? new TimeSpan(0),
                KeyFrames = keyFrames
            };

            // Set the target property name
            Storyboard.SetTargetProperty(animation, new PropertyPath("Margin"));

            // Set the target if specified
            if(target != null)
                Storyboard.SetTarget(animation, target);

            // Add this to the storyboard
            storyboard.Children.Add(animation);

            return storyboard;
        }

        public struct KeyFrame
        {
            public double Offset { get; }
            public float Seconds { get; }
            public IEasingFunction EasingFunction { get; }

            public KeyFrame(double offset, float seconds, IEasingFunction easingFunction = null)
            {
                Offset = offset;
                Seconds = seconds;
                EasingFunction = easingFunction;
            }
        }

        /// <summary>
        /// Adds a slide to left animation to the storyboard
        /// </summary>
        /// <param name="storyboard">The storyboard to add the animation to</param>
        /// <param name="seconds">The time the animation will take</param>
        /// <param name="offset">The distance to the left to end at</param>
        /// <param name="decelerationRatio">The rate of deceleration</param>
        /// <param name="keepMargin">Whether to keep the element at the same width during animation</param>
        /// <param name="target"></param>
        /// <param name="beginTime"></param>
        public static Storyboard AddSlideToLeft(this Storyboard storyboard, float seconds, double offset, float decelerationRatio = 0.9f, bool keepMargin = true, DependencyObject target = null, TimeSpan? beginTime = null)
        {
            // Create the margin animate from right 
            var animation = new ThicknessAnimation
            {
                BeginTime = beginTime ?? new TimeSpan(0),
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
                From = new Thickness(0),
                To = new Thickness(-offset, 0, keepMargin ? offset : 0, 0),
                DecelerationRatio = decelerationRatio
            };

            // Set the target property name
            Storyboard.SetTargetProperty(animation, new PropertyPath("Margin"));
            
            // Set the target if specified
            if(target != null)
                Storyboard.SetTarget(animation, target);

            // Add this to the storyboard
            storyboard.Children.Add(animation);

            return storyboard;
        }

        #endregion

        #region Sliding To/From Top

        /// <summary>
        /// Adds a slide from top animation to the storyboard
        /// </summary>
        /// <param name="storyboard">The storyboard to add the animation to</param>
        /// <param name="seconds">The time the animation will take</param>
        /// <param name="offset">The distance to the top to start from</param>
        /// <param name="beginTime"></param>
        /// <param name="decelerationRatio">The rate of deceleration</param>
        /// <param name="keepMargin">Whether to keep the element at the same height during animation</param>
        /// <param name="target">The framework element targeted by the fade in</param>
        public static Storyboard AddSlideFromTop(this Storyboard storyboard, float seconds, double offset, float decelerationRatio = 0.9f, bool keepMargin = true, DependencyObject target = null, TimeSpan? beginTime = null)
        {
            // Create the margin animate from right 
            var animation = new ThicknessAnimation
            {
                BeginTime = beginTime ?? new TimeSpan(0),
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
                From = new Thickness(0, -offset, 0, keepMargin ? offset : 0),
                To = new Thickness(0),
                DecelerationRatio = decelerationRatio
            };

            // Set the target property name
            Storyboard.SetTargetProperty(animation, new PropertyPath("Margin"));

            // Set the target if specified
            if (target != null)
                Storyboard.SetTarget(animation, target);

            // Add this to the storyboard
            storyboard.Children.Add(animation);

            return storyboard;
        }

        #endregion

        #region Sliding To/From Bottom

        /// <summary>
        /// Adds a slide to bottom animation to the storyboard
        /// </summary>
        /// <param name="storyboard">The storyboard to add the animation to</param>
        /// <param name="seconds">The time the animation will take</param>
        /// <param name="offset">The distance to the bottom to end at</param>
        /// <param name="decelerationRatio">The rate of deceleration</param>
        /// <param name="keepMargin">Whether to keep the element at the same height during animation</param>
        /// <param name="target"></param>
        /// <param name="beginTime"></param>
        public static Storyboard AddSlideToBottom(this Storyboard storyboard, float seconds, double offset, float decelerationRatio = 0.9f, bool keepMargin = true, DependencyObject target = null, TimeSpan? beginTime = null)
        {
            // Create the margin animate from right 
            var animation = new ThicknessAnimation
            {
                BeginTime = beginTime ?? new TimeSpan(0),
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
                From = new Thickness(0),
                To = new Thickness(0, keepMargin ? offset : 0, 0, -offset),
                DecelerationRatio = decelerationRatio
            };

            // Set the target property name
            Storyboard.SetTargetProperty(animation, new PropertyPath("Margin"));

            // Set the target if specified
            if(target != null)
                Storyboard.SetTarget(animation, target);

            // Add this to the storyboard
            storyboard.Children.Add(animation);

            return storyboard;
        }

        #endregion

        #region Fade In/Out

        /// <summary>
        /// Adds a fade in animation to the storyboard
        /// </summary>
        /// <param name="storyboard">The storyboard to add the animation to</param>
        /// <param name="duration">The time the animation will take</param>
        /// <param name="target">The framework element targeted by the fade in</param>
        /// <param name="beginTime"></param>
        public static Storyboard AddFadeIn(this Storyboard storyboard, float duration, DependencyObject target = null, TimeSpan? beginTime = null)
        {
            // Create the fade in animation
            var animation = new DoubleAnimation
            {
                BeginTime = beginTime ?? new TimeSpan(0),
                Duration = new Duration(TimeSpan.FromSeconds(duration)),
                From = 0,
                To = 1,
            };

            // Set the target property name
            Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));
            
            // Set the target if specified
            if(target != null)
                Storyboard.SetTarget(animation, target);

            // Add this to the storyboard
            storyboard.Children.Add(animation);

            return storyboard;
        }

        /// <summary>
        /// Adds a fade out animation to the storyboard
        /// </summary>
        /// <param name="storyboard">The storyboard to add the animation to</param>
        /// <param name="seconds">The time the animation will take</param>
        /// <param name="target">The framework element targeted by the fade out</param>
        public static Storyboard AddFadeOut(this Storyboard storyboard, float seconds, DependencyObject target = null)
        {
            // Create the margin animate from right 
            var animation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
                To = 0,
            };

            // Set the target property name
            Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));

            // Set the target if specified
            if(target != null)
                Storyboard.SetTarget(animation, target);

            // Add this to the storyboard
            storyboard.Children.Add(animation);

            return storyboard;
        }
        

        #endregion
    }
}
