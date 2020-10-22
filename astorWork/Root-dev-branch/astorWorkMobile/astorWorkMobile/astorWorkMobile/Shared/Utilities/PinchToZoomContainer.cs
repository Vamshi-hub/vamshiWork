using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace astorWorkMobile.Shared.Utilities
{
    public class PinchToZoomContainer : ContentView
    {
        double currentScale = 1;
        double startScale = 1;
        double xOffset = 0;
        double yOffset = 0;
        double originalWidth;
        double originalHeight;

        double ScreenWidth;
        double ScreenHeight;
        public PinchToZoomContainer()
        {
            GestureRecognizers.Clear();
            PinchGestureRecognizer pinchGesture = new PinchGestureRecognizer();
            pinchGesture.PinchUpdated += PinchGesture_PinchUpdated;
            GestureRecognizers.Add(pinchGesture);

            var panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += OnPanUpdated;
            GestureRecognizers.Add(panGesture);
        }
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height); //must be called

            if (ScreenWidth != width || ScreenHeight != height)
            {
                originalWidth = Content.Width / Content.Scale;
                originalHeight = Content.Height / Content.Scale;

                ScreenWidth = width;
                ScreenHeight = height;

                xOffset = Content.TranslationX;
                yOffset = Content.TranslationY;

                currentScale = Content.Scale;
            }
        }
        private void PinchGesture_PinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {
            if (e.Status == GestureStatus.Started)
            {
                // Store the current scale factor applied to the wrapped user interface element,
                // and zero the components for the center point of the translate transform.
                startScale = Content.Scale;
                Content.AnchorX = 0;
                Content.AnchorY = 0;
            }

            if (e.Status == GestureStatus.Running)
            {
                // Calculate the scale factor to be applied.
                currentScale += (e.Scale - 1) * startScale;
                currentScale = Math.Max(1, currentScale);

                // The ScaleOrigin is in relative coordinates to the wrapped user interface element,
                // so get the X pixel coordinate.
                double renderedX = Content.X + xOffset;
                double deltaX = renderedX / Width;
                double deltaWidth = Width / (Content.Width * startScale);
                double originX = (e.ScaleOrigin.X - deltaX) * deltaWidth;

                // The ScaleOrigin is in relative coordinates to the wrapped user interface element,
                // so get the Y pixel coordinate.
                double renderedY = Content.Y + yOffset;
                double deltaY = renderedY / Height;
                double deltaHeight = Height / (Content.Height * startScale);
                double originY = (e.ScaleOrigin.Y - deltaY) * deltaHeight;

                // Calculate the transformed element pixel coordinates.
                double targetX = xOffset - (originX * Content.Width) * (currentScale - startScale);
                double targetY = yOffset - (originY * Content.Height) * (currentScale - startScale);

                // Apply translation based on the change in origin.
                Content.TranslationX = targetX.Clamp(-Content.Width * (currentScale - 1), 0);
                Content.TranslationY = targetY.Clamp(-Content.Height * (currentScale - 1), 0);

                // Apply scale factor.
                Content.Scale = currentScale;
            }

            if (e.Status == GestureStatus.Completed)
            {
                // Store the translation delta's of the wrapped user interface element.
                xOffset = Content.TranslationX;
                yOffset = Content.TranslationY;
            }
        }

        public void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            var s = (ContentView)sender;
            if (Content.Scale == 1)
            {
                return;
            }

            switch (e.StatusType)
            {
                case GestureStatus.Running:
                    // Translate and ensure we don't pan beyond the wrapped user interface element bounds.
                    double newX = e.TotalX + xOffset;
                    double newY = e.TotalY + yOffset;

                    //double minX = Math.Min(0, 0 - ((Content.Width * Content.Scale) / 2));
                    //double maxX = Math.Max(0, App.ScreenWidth - ((Content.Width * Content.Scale) / 2));

                    //if (newX < minX)
                    //{
                    //    newX = minX;
                    //}

                    //if (newX > maxX)
                    //{
                    //    newX = maxX;
                    //}

                    //double minY = Math.Min(0, 0 - ((Content.Height * Content.Scale) / 2));
                    //double maxY = Math.Max(0, App.ScreenHeight - ((Content.Height * Content.Scale) / 2));

                    //if (newY < minY)
                    //{
                    //    newY = minY;
                    //}

                    //if (newY > maxY)
                    //{
                    //    newY = maxY;
                    //}

                    Content.TranslationX = newX;
                    Content.TranslationY = newY;
                  //  Content.TranslateTo(newX, newY, 0, Easing.Linear);
                    break;

                case GestureStatus.Completed:
                    // Store the translation applied during the pan
                    xOffset = Content.TranslationX;
                    yOffset = Content.TranslationY;
                    //if (originalWidth * currentScale < App.ScreenWidth && App.ScreenWidth > App.ScreenHeight)
                    //    xOffset = (App.ScreenWidth - originalWidth * currentScale) / 2 - Content.X;
                    //else
                    //    xOffset = Math.Max(Math.Min(0, xOffset), -Math.Abs(originalWidth * currentScale - App.ScreenWidth));

                    //// center the image if the height of the image is smaller than the screen height
                    //if (originalHeight * currentScale < App.ScreenHeight && App.ScreenHeight > App.ScreenWidth)
                    //    yOffset = (App.ScreenHeight - originalHeight * currentScale) / 2 - Content.Y;
                    //else
                    //    yOffset = Math.Max(Math.Min((originalHeight - App.ScreenHeight) / 2, yOffset), -Math.Abs(originalHeight * currentScale - App.ScreenHeight - (originalHeight - App.ScreenHeight) / 2));
                    //Content.TranslateTo(xOffset, yOffset, 500, Easing.BounceOut);
                    if (originalWidth * currentScale < ScreenWidth && ScreenWidth > ScreenHeight)
                        xOffset = (ScreenWidth - originalWidth * currentScale) / 2 - s.Content.X;
                    else
                        xOffset = Math.Max(Math.Min(0, xOffset), -Math.Abs(originalWidth * currentScale - ScreenWidth));

                    // center the image if the height of the image is smaller than the screen height
                    if (originalHeight * currentScale < ScreenHeight && ScreenHeight > ScreenWidth)
                        yOffset = (ScreenHeight - originalHeight * currentScale) / 2 - s.Content.Y;
                    else
                        yOffset = Math.Max(Math.Min((originalHeight - ScreenHeight) / 2, yOffset), -Math.Abs(originalHeight * currentScale - ScreenHeight - (originalHeight - ScreenHeight) / 2));
                    s.Content.TranslateTo(xOffset, yOffset, 500, Easing.BounceOut);
                    break;
            }
        }
    }
}