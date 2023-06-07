using System;
using System.IO;
using AudioToolbox;
using AVFoundation;
using CoreMedia;
using Foundation;
using UIKit;

namespace FaceTracking_01.iOS.AR
{
    public class AudioRecorder : AVCaptureAudioDataOutputSampleBufferDelegate
    {
        // Add the 'new' keyword here
        new private readonly FaceTrackingARView _faceTrackingARView;
        private AVAudioRecorder _audioRecorder;
        private NSUrl _audioFileUrl;
        private AVCaptureAudioDataOutput _audioDataOutput;
        private AVAssetWriterInput _audioWriterInput;

        // Create a public property for AudioDataOutput
        public AVCaptureAudioDataOutput AudioDataOutput
        {
            get { return _audioDataOutput; }
            set { _audioDataOutput = value; }
        }

        public AudioRecorder(FaceTrackingARView faceTrackingARView) : base(faceTrackingARView)
        {
            _faceTrackingARView = faceTrackingARView;
        }

        public void StartRecordingAudio(string fileName, string recordingsPath)
        {
            NSError error;
            _audioFileUrl = NSUrl.FromFilename(Path.Combine(recordingsPath, $"{fileName}.m4a"));

            var audioSettings = new AudioSettings
            {
                SampleRate = 44100,
                Format = AudioFormatType.MPEG4AAC,
                NumberChannels = 1
            };

            _audioRecorder = AVAudioRecorder.Create(_audioFileUrl, new AudioSettings(audioSettings.Dictionary), out error);

            if (error == null)
            {
                _audioRecorder.PrepareToRecord();
                _audioRecorder.Record();
            }
            else
            {
                Console.WriteLine("Error creating AVAudioRecorder: " + error.LocalizedDescription);
            }
        }

        public void StopRecordingAudio()
        {
            if (_audioRecorder != null)
            {
                _audioRecorder.Stop();
                _audioRecorder.Dispose();
                _audioRecorder = null;
            }
        }

        public override void DidOutputSampleBuffer(AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection)
        {
            if (_audioWriterInput != null && _audioWriterInput.ReadyForMoreMediaData)
            {
                _audioWriterInput.AppendSampleBuffer(sampleBuffer);
            }
        }

        public void SetAudioWriterInput(AVAssetWriterInput audioWriterInput)
        {
            _audioWriterInput = audioWriterInput;
        }

        public void SetAudioDataOutput(AVCaptureAudioDataOutput audioDataOutput)
        {
            _audioDataOutput = audioDataOutput;
        }
    }
}