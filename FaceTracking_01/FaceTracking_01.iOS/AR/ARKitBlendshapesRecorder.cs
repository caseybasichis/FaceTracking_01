using System;
using System.Collections.Generic;
using System.IO;
using ARKit;
using Foundation;
using Newtonsoft.Json;

namespace FaceTracking_01.iOS.AR
{
    public class ARKitBlendshapeRecorder
    {
        private DateTime _startTime;
        private bool _isRecording;
        private List<Dictionary<string, float>> _blendShapesData = new List<Dictionary<string, float>>();
        private string _fileName;
        private string _recordingPath;

        public ARKitBlendshapeRecorder()
        {
            _isRecording = false;
        }

        public void DebugWriteBlendShapesToFile(string fileName, string content)
        {
            string filePath = Path.Combine(_recordingPath, $"{fileName}.json");
            File.WriteAllText(filePath, content);
        }
        
        public void WriteBlendShapesToJson(ARKit.ARBlendShapeLocationOptions blendShapesDict)
        {
            if (!_isRecording)
            {
                return;
            }

            Console.WriteLine("Writing blendshapes data...");

            var blendShapes = new Dictionary<string, float>();
            foreach (NSString key in blendShapesDict.Dictionary.Keys)
            {
                NSNumber value = (NSNumber)blendShapesDict.Dictionary[key];
                blendShapes.Add(key.ToString(), value.FloatValue);
            }
            string jsonString = JsonConvert.SerializeObject(blendShapes);

            Console.WriteLine("Blendshapes data: " + jsonString);
            
            // Debug: Write individual blendshapes to files
            DebugWriteBlendShapesToFile(_fileName + "_individual_blendshapes", jsonString);
            _blendShapesData.Add(blendShapes);
        }

        public void StartRecording(string fileName, string recordingsPath)
        {
            _isRecording = true;
            Console.WriteLine("StartRecording: _isRecording is now " + _isRecording);
            _fileName = fileName;
            _recordingPath = recordingsPath;
            _startTime = DateTime.Now;
            _blendShapesData.Clear();
        }

        public void StopRecording()
        {
            _isRecording = false;
            Console.WriteLine("StopRecording: _isRecording is now " + _isRecording);
            string json = JsonConvert.SerializeObject(_blendShapesData);

            string filePath = Path.Combine(_recordingPath, $"{_fileName}_blendshapesdata.json");

            File.WriteAllText(filePath, json);
        }
    }
}