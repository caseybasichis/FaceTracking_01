using AVFoundation;
using CoreMedia;
using FaceTracking_01.iOS.AR;
using Foundation;
using System;

public abstract class AVCaptureDataOutputSampleBufferDelegateBase : NSObject, IAVCaptureDataOutputSampleBufferDelegate
{
    protected FaceTrackingARView _faceTrackingARView;

    // Constructor
    protected AVCaptureDataOutputSampleBufferDelegateBase(FaceTrackingARView faceTrackingARView)
    {
        _faceTrackingARView = faceTrackingARView;
        Console.WriteLine("AVCaptureDataOutputSampleBufferDelegateBase created"); // Add this line

    }

    // DidOutputSampleBuffer method
    [Export("captureOutput:didOutputSampleBuffer:fromConnection:")]
    public virtual void DidOutputSampleBuffer(AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection)
    {
    }

    public void HandleSampleBuffer(object sampleBuffer)
    {
        DidOutputSampleBuffer(null, sampleBuffer as CMSampleBuffer, null);
    }
}
// end
