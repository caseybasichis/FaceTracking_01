using Foundation;
using AVFoundation;
using CoreMedia;
using FaceTracking_01.iOS.AR;
using System;

public class AVCaptureAudioDataOutputSampleBufferDelegate : AVCaptureDataOutputSampleBufferDelegateBase, IAVCaptureAudioDataOutputSampleBufferDelegate
{
    // Constructor
    public AVCaptureAudioDataOutputSampleBufferDelegate(FaceTrackingARView faceTrackingARView)
        : base(faceTrackingARView)
    {
        Console.WriteLine("AVCaptureAudioDataOutputSampleBufferDelegate created");
    }
}
// end
