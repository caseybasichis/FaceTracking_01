using Foundation;
using AVFoundation;
using CoreMedia;
using FaceTracking_01.iOS.AR;
using System;

public class AVCaptureVideoDataOutputSampleBufferDelegate : AVCaptureDataOutputSampleBufferDelegateBase, IAVCaptureVideoDataOutputSampleBufferDelegate
{
    // Constructor
    public AVCaptureVideoDataOutputSampleBufferDelegate(FaceTrackingARView faceTrackingARView)
        : base(faceTrackingARView)
    {
        Console.WriteLine("AVCaptureVideoDataOutputSampleBufferDelegate created");
    }

    // Override DidOutputSampleBuffer method
    public override void DidOutputSampleBuffer(AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection)
    {
        base.DidOutputSampleBuffer(captureOutput, sampleBuffer, connection);
    }
}
// end
