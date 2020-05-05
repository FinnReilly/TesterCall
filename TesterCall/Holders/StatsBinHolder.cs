using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TesterCall.Models;
using TesterCall.Models.Interfaces;
using TesterCall.Models.Stats;

namespace TesterCall.Holders
{
    public static class StatsBinHolder
    {
        private static Dictionary<string, int> _currentNameIncrementStore { get; set; } = new Dictionary<string, int>();
        private static bool _recording { get; set; }
        private static StatsBin _activeBin { get; set; }

        public static bool Recording => _recording;
        public static List<StatsBin> Bins { get; set; } = new List<StatsBin>();
        public static StatsBin ActiveBin => _activeBin;

        public static void StartRecording(string binName)
        {
            var start = DateTime.Now;
            var currentIteration = 0;

            if (!string.IsNullOrEmpty(binName))
            {
                //prevent duplicate names in store
                if (!_currentNameIncrementStore.TryGetValue(binName, out var existing))
                {
                    _currentNameIncrementStore[binName] = 0;
                }
                else
                {
                    _currentNameIncrementStore[binName] = existing + 1;
                }

                currentIteration = _currentNameIncrementStore[binName];
            }

            var key = new StatsBin(binName,
                                        currentIteration,
                                        start);
            _activeBin = key;
            Bins.Add(_activeBin);
            _recording = true;
        }

        public static void StopRecording()
        {
            _activeBin.RecordingFinished = DateTime.Now;
            _recording = false;
            _activeBin = null;
        }

        public static void Add(IHasResponseTime responseContent)
        {
            if (_activeBin != null)
            {
                _activeBin.Responses.Add(responseContent);
            }
        }

        public static void FlushAll()
        {
            _activeBin = null;
            Bins = new List<StatsBin>();
            _recording = false;
            _currentNameIncrementStore = new Dictionary<string, int>();
        }
    }
}
