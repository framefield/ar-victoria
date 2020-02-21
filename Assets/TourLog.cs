using System;
using System.Collections;
using System.IO;
using UnityEngine;
using victoria.controller;

namespace victoria.logging
{
    public class TourLog : MonoBehaviour
    {
        [SerializeField] float _logRate = 0.05f;

        [ContextMenu("Start")]
        public void StartLog(Transform transformToLog, TourController.TourMode mode)
        {
            _shouldCompleteLogging = false;
            StartCoroutine(WriteTimeIntoFile(transformToLog, mode));
        }

        [ContextMenu("Complete")]
        public void CompleteLog()
        {
            _shouldCompleteLogging = true;
        }

        private IEnumerator WriteTimeIntoFile(Transform transformToLog, TourController.TourMode mode)
        {
            using (var sw = new StreamWriter(GeneratePath(mode, _logRate)))
            {
                sw.Write(GenerateHeader());
                while (!_shouldCompleteLogging)
                {
                    _timeSinceLastLog += Time.deltaTime;
                    if (_timeSinceLastLog > _logRate)
                    {
                        _timeSinceLastLog -= _logRate;
                        var csvLine = SampleToCSV(transformToLog);
                        sw.Write(csvLine);
                    }

                    yield return null;
                }
            }
        }

        private static string SampleToCSV(Transform t)
        {
            var pos = t.position;
            var rot = t.rotation.eulerAngles;
            return $"{pos.x}, {pos.y}, {pos.z}, {rot.x}, {rot.y}, {rot.z}\n";
        }

        private static string GenerateHeader()
        {
            var header = "";
            header += "PosX\t";
            header += "PosY\t";
            header += "PosZ\t";
            header += "RotX\t";
            header += "RotY\t";
            header += "RotZ\t";
            header += "\n";
            return header;
        }

        private static string GeneratePath(TourController.TourMode mode, float rate)
        {
            var exists = Directory.Exists(SubfolderPath);
            if (!exists)
                Directory.CreateDirectory(SubfolderPath);

            var now = DateTime.Now;
            char modeString;
            switch (mode)
            {
                case TourController.TourMode.Guided:
                    modeString = 'A';
                    break;
                case TourController.TourMode.Unguided:
                    modeString = 'B';
                    break;
                case TourController.TourMode.Mixed:
                    modeString = 'C';
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }

            var filename =
                $"{modeString}_{now.Year}_{now.Month}_{now.Day}T{now.Hour}h{now.Minute}m{now.Second}s_{1f / rate}sps.csv";
            return SubfolderPath + filename;
        }

        private float _timeSinceLastLog;
        private bool _shouldCompleteLogging;
        private const string SubfolderPath = "logs/";
    }
}