using System;
using System.IO;
using AVFoundation;
using CoreAnimation;
using CoreFoundation;
using CoreGraphics;
using CoreImage;
using CoreMedia;
using CoreVideo;
using Foundation;
using UIKit;

namespace FaceTracking_01.iOS.AR
{
    public class VideoRecorder
    {
        private AVAssetWriter _videoWriter;
        private AVAssetWriterInput _videoWriterInput;
        private AVAssetWriterInputPixelBufferAdaptor _pixelBufferAdaptor;
        private DispatchQueue _videoWritingQueue = new DispatchQueue("videoWritingQueue");

        public void StartRecordingVideo(string fileName, string recordingsPath, nfloat width, nfloat height)
        {
            NSError error;
            var videoOutputSettings = new NSMutableDictionary();
            videoOutputSettings.Add(AVVideo.WidthKey, NSNumber.FromNFloat(width));
            videoOutputSettings.Add(AVVideo.HeightKey, NSNumber.FromNFloat(height));
            videoOutputSettings.Add(AVVideo.CodecKey, AVVideo.CodecH264);

            _videoWriterInput = new AVAssetWriterInput(AVMediaType.Video, new AVVideoSettingsCompressed(videoOutputSettings));

            _videoWriterInput.ExpectsMediaDataInRealTime = true;

            var pixelBufferAttributes = new NSMutableDictionary();

            pixelBufferAttributes.Add(CVPixelBuffer.PixelFormatTypeKey, NSNumber.FromInt64((int)CVPixelFormatType.CV32BGRA));
            pixelBufferAttributes.Add(CVPixelBuffer.WidthKey, NSNumber.FromNFloat(width));
            pixelBufferAttributes.Add(CVPixelBuffer.HeightKey, NSNumber.FromNFloat(height));

            _pixelBufferAdaptor = new AVAssetWriterInputPixelBufferAdaptor(_videoWriterInput, pixelBufferAttributes);

            var videoFileUrl = NSUrl.FromFilename(Path.Combine(recordingsPath, $"{fileName}.mov"));

            _videoWriter = new AVAssetWriter(videoFileUrl, AVFileType.QuickTimeMovie, out error);

            if (error == null)
            {
                _videoWriter.AddInput(_videoWriterInput);
                _videoWriter.StartWriting();
                _videoWriter.StartSessionAtSourceTime(CoreMedia.CMTime.Zero);
            }
            else
            {
                Console.WriteLine("Error creating AVAssetWriter: " + error.LocalizedDescription);
            }
        }

        public void StopRecordingVideo(string imageName)
        {
            if (_videoWriter != null)
            {
                if (_videoWriter.Status == AVAssetWriterStatus.Writing)
                {
                    _videoWriterInput.MarkAsFinished();
                    _videoWriter.FinishWriting(() =>
                    {
                        Console.WriteLine($"Video writing finished for: {imageName}.mov");
                        if (_videoWriter.Status == AVAssetWriterStatus.Failed)
                        {
                            Console.WriteLine("Video writing failed: " + _videoWriter.Error.LocalizedDescription);
                        }
                        else
                        {
                            Console.WriteLine("Video writing succeeded.");
                        }
                        _videoWriter.Dispose();
                        _videoWriter = null;
                    });
                }
                else
                {
                    Console.WriteLine("Video writer status is not 'Writing', cannot finish writing.");
                }
            }
        }

        public void WriteVideoFrame(CVPixelBuffer pixelBuffer, double timestamp)
        {
            if (_videoWriter != null && _videoWriter.Status == AVAssetWriterStatus.Writing)
            {
                _videoWritingQueue.DispatchAsync(() =>
                {
                    if (_videoWriterInput.ReadyForMoreMediaData)
                    {
                        _pixelBufferAdaptor.AppendPixelBufferWithPresentationTime(pixelBuffer, new CMTime((long)timestamp, 1));
                        pixelBuffer.Dispose();
                    }
                });
            }
        }
    }
}
// end
