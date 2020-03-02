using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace victoria
{
    /// <summary>
    /// Log the camera transform and interaction events to csv file.
    /// </summary>
    public class TourLog : MonoBehaviour
    {
        [SerializeField] float _logRate = 0.05f;

        public void StartLog(Transform transformToLog, TourController.TourMode mode)
        {
            _shouldCompleteLogForTour = false;

            var pathBase = GeneratePathBase(mode);
            var transformFile = pathBase + $"{1f / _logRate}sps.csv";
            var eventFile = pathBase + "events.csv";

            StartCoroutine(WriteTransformIntoFile(transformToLog, transformFile));
            StartCoroutine(WriteReceivedEventsIntoFile(eventFile));
        }

        public void CompleteLog()
        {
            _shouldCompleteLogForTour = true;
        }

        public void LogEvent(TourEvent eventType, InteractiveSegment.SegmentType segment)
        {
            _receivedTourEvents.Enqueue((eventType, segment));
        }

        public enum TourEvent
        {
            Play,
            Complete
        }

        private static string SampleToCSV(Transform t)
        {
            var pos = t.position;
            var rot = t.rotation.eulerAngles;
            return $"{pos.x}\t {pos.y}\t {pos.z}\t {rot.x}\t {rot.y}\t {rot.z}\n";
        }

        private static string EventToCSV((TourEvent eventType, InteractiveSegment.SegmentType segment) eventData)
        {
            var now = DateTime.Now;
            return
                $"{now.Year}_{now.Month}_{now.Day}T{now.Hour}h{now.Minute}m{now.Second}s\t {eventData.eventType.ToString()}\t {eventData.segment.ToString()}\n";
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

        private IEnumerator WriteReceivedEventsIntoFile(string filename)
        {
            var p = Path.Combine(Application.persistentDataPath, filename);
            using (var file = new FileStream(p, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                using (var writer = new StreamWriter(file, Encoding.UTF8))
                {
                    while (true)
                    {
                        while (_receivedTourEvents.Count > 0)
                            writer.Write(EventToCSV(_receivedTourEvents.Dequeue()));

                        if (_shouldCompleteLogForTour)
                            break;

                        yield return null;
                    }
                }
            }
        }

        private IEnumerator WriteTransformIntoFile(Transform transformToLog, string filename)
        {
            var p = Path.Combine(Application.persistentDataPath, filename);
            using (var file = new FileStream(p, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                using (var writer = new StreamWriter(file, Encoding.UTF8))
                {
                    writer.Write(GenerateHeader());
                    while (true)
                    {
                        _timeSinceLastLog += Time.deltaTime;
                        if (_timeSinceLastLog > _logRate)
                        {
                            _timeSinceLastLog -= _logRate;
                            var csvLine = SampleToCSV(transformToLog);
                            writer.Write(csvLine);
                        }

                        if (_shouldCompleteLogForTour)
                            break;

                        yield return null;
                    }
                }
            }
        }

        private static string GeneratePathBase(TourController.TourMode mode)
        {
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

            var filenamePrefix =
                $"{modeString}_{now.Year}_{now.Month}_{now.Day}T{now.Hour}h{now.Minute}m{now.Second}s_";
            return filenamePrefix;
        }

        private readonly Queue<(TourEvent eventType, InteractiveSegment.SegmentType segment)> _receivedTourEvents =
            new Queue<(TourEvent, InteractiveSegment.SegmentType)>();

        private float _timeSinceLastLog;
        private bool _shouldCompleteLogForTour;
        private StreamWriter _eventLogger;
    }
}