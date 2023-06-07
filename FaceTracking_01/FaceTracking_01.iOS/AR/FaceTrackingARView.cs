// using System;
// using ARKit;
// using AVFoundation;
// using Foundation;
// using SceneKit;
// using UIKit;
// using CoreFoundation;
// using FaceTracking_01.iOS.AR; // Add this at the top

using System;
using ARKit;
using SceneKit;
using UIKit;
using Xamarin.Forms.Platform.iOS;
using System.IO;
using Newtonsoft.Json;
using Foundation;
using AVFoundation;
using CoreMedia;
using CoreVideo;
using CoreFoundation;
using System.Runtime.InteropServices;
using AudioToolbox;
using System.Linq;
using MetalKit;
using CoreAnimation;
using CoreImage;
using CoreGraphics;
using FaceTracking_01.iOS.AR;

namespace FaceTracking_01.iOS.AR
{
    public class FaceTrackingARView : ARSCNView, IARSCNViewDelegate
    {
        private ARFaceTrackingConfiguration _configuration;
        private AVCaptureSession _captureSession;
        private AudioRecorder _audioRecorder;
        private VideoRecorder _videoRecorder;
        private ARKitBlendshapeRecorder _blendshapeRecorder;

        public bool IsRecording { get; private set; }
        public string CurrentImageName { get; set; }

        public FaceTrackingARView()
        {
            _configuration = new ARFaceTrackingConfiguration();
            if (!ARFaceTrackingConfiguration.IsSupported)
            {
                Console.WriteLine("Your device does not support AR Face Tracking.");
                return;
            }

            Delegate = this;
            Session.Run(_configuration, ARSessionRunOptions.ResetTracking);
            SetupAudioCaptureSession();
            _audioRecorder = new AudioRecorder(this);
            _videoRecorder = new VideoRecorder();
            _blendshapeRecorder = new ARKitBlendshapeRecorder();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                Session.Pause();
                _captureSession.StopRunning();
            }
        }

        /*
        public override void WillMoveToWindow(UIWindow window)
        {
            base.WillMoveToWindow(window);

            if (window == null)
            {
                Session.Pause();
                _captureSession.StopRunning();
            }
            else
            {
                _captureSession.StartRunning();
                Session.Run(_configuration, ARSessionRunOptions.ResetTracking);
            }
        }
        */

        [Export("renderer:didUpdateNode:forAnchor:")]
        public void DidUpdateNode(ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor)
        {
            if (anchor is ARFaceAnchor faceAnchor)
            {
                var blendShapes = faceAnchor.BlendShapes as ARKit.ARBlendShapeLocationOptions;
                Console.WriteLine("DidUpdateNode called for ARFaceAnchor. Updating blendshapes...");
                _blendshapeRecorder.WriteBlendShapesToJson(blendShapes);
            }
        }

        [Export("session:didUpdateFrame:")]
        public void DidUpdateFrame(ARSession session, ARFrame frame)
        {
            if (_videoRecorder != null)
            {
                frame.CapturedImage.YCbCrToRGBAPixelBuffer(out CVPixelBuffer pixelBuffer);
                _videoRecorder.WriteVideoFrame(pixelBuffer, frame.Timestamp);
            }
        }

        private void SetupAudioCaptureSession()
        {
            _captureSession = new AVCaptureSession();
            _captureSession.SessionPreset = AVCaptureSession.PresetHigh;

            // Add audio input
            var audioDevice = AVCaptureDevice.GetDefaultDevice(AVMediaTypes.Audio);
            
            NSError audioError;
            var audioInput = AVCaptureDeviceInput.FromDevice(audioDevice, out audioError);

            if (audioInput == null || audioError != null)
            {
                Console.WriteLine("Error adding audio input: " + audioError?.LocalizedDescription);
                return;
            }

            if (_captureSession.CanAddInput(audioInput))
            {
                _captureSession.AddInput(audioInput);
            }
            else
            {
                Console.WriteLine("Cannot add audio input to capture session.");
                return;
            }

            if (_audioRecorder == null)
            {
                _audioRecorder = new AudioRecorder(this);
            }

            // Add audio data output
            var audioDataOutput = new AVCaptureAudioDataOutput();
            audioDataOutput.SetSampleBufferDelegateQueue(_audioRecorder, DispatchQueue.MainQueue);

            if (_captureSession.CanAddOutput(audioDataOutput))
            {
                _captureSession.AddOutput(audioDataOutput);
                _audioRecorder.SetAudioDataOutput(audioDataOutput);
            }
            else
            {
                Console.WriteLine("Error adding audio data output");
                return;
            }

            _captureSession.StartRunning();
        }

        public void StartRecording(string fileName, string recordingsPath)
        {
            _blendshapeRecorder.StartRecording(fileName, recordingsPath);
            _audioRecorder.StartRecordingAudio(fileName, recordingsPath);
            _videoRecorder.StartRecordingVideo(fileName, recordingsPath, Bounds.Width, Bounds.Height);
            IsRecording = true;
        }

        public void StopRecording()
        {
            _blendshapeRecorder.StopRecording();
            _audioRecorder.StopRecordingAudio();
            _videoRecorder.StopRecordingVideo(CurrentImageName);
            IsRecording = false;
        }
    }
}