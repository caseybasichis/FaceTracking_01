using FaceTracking_01.iOS.AR;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using FaceTracking_01;
using System.IO;
using System;
using System.Collections.Generic;

[assembly: ExportRenderer(typeof(FaceTracking_01.Views.FaceTrackingPage), typeof(FaceTrackingPageRenderer))]
namespace FaceTracking_01.iOS.AR
{
    public class FaceTrackingPageRenderer : PageRenderer
    {
        private FaceTrackingARView _arView;
        private UIImageView _imageView;
        private UIButton _imageCycleButton;
        private int _currentImageIndex = 0;

        // test file names
        private List<string> _imageFileNames = new List<string>
        {
            "alf01.jpg",
            "alf02.jpg",
            "alf03.jpg",
            "alf04.jpg",
            "alf05.jpg"
        };

        private void SetupUserInterface()
        {
            _imageCycleButton = new UIButton(UIButtonType.System)
            {
                Frame = new CoreGraphics.CGRect(20, 40, UIScreen.MainScreen.Bounds.Width - 40, 40),
                BackgroundColor = UIColor.FromRGBA(0, 0, 0, 128), // semi-transparent background color
            };
            _imageCycleButton.SetTitle("Cycle Image", UIControlState.Normal);
            _imageCycleButton.SetTitleColor(UIColor.White, UIControlState.Normal);
            _imageCycleButton.TouchUpInside += OnImageCycleButtonClicked;
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null)
            {
                return;
            }

            try
            {
                SetupUserInterface();
                _arView = new FaceTrackingARView
                {
                    Frame = UIScreen.MainScreen.Bounds
                };
                // Comment out this line:
                // View.AddSubview(_arView);

                // Load and display the image from the list
                UpdateImageView();
                View.AddSubview(_imageView);

                // Add the button to the view hierarchy
                View.AddSubview(_imageCycleButton);

                // Bring the image view and button to the front
                View.BringSubviewToFront(_imageView);
                View.BringSubviewToFront(_imageCycleButton);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }


        private void OnImageCycleButtonClicked(object sender, EventArgs e)
        {
            _currentImageIndex++;
            if (_currentImageIndex >= _imageFileNames.Count)
            {
                _currentImageIndex = 0;
            }
            UpdateImageView();

            string imageName = Path.GetFileNameWithoutExtension(_imageFileNames[_currentImageIndex]);

            if (_arView.IsRecording)
            {
                _arView.StopRecording();
            }

            string fileName = $"{DateTime.Now:yyyyMMdd_HHmmss}_{imageName}";
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string recordingsPath = Path.Combine(documentsPath, "Recordings");
            Directory.CreateDirectory(recordingsPath);
            // Add this line
            _arView.StartRecording(fileName, recordingsPath);
        }

        private void UpdateImageView()
        {
            string imagePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), _imageFileNames[_currentImageIndex]);
            UIImage image = UIImage.FromFile(imagePath);
            if (_imageView == null)
            {
                _imageView = new UIImageView(image)
                {
                    Frame = UIScreen.MainScreen.Bounds,
                    ContentMode = UIViewContentMode.ScaleAspectFill,
                    BackgroundColor = UIColor.FromRGBA(0, 0, 0, 128), // semi-transparent background color
                };
            }
            else
            {
                _imageView.Image = image;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _arView?.Dispose();
                _arView = null;
                _imageView?.Dispose();
                _imageView = null;
                _imageCycleButton?.Dispose();
                _imageCycleButton = null;
            }
        }
    }
}