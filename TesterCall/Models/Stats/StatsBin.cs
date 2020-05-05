using System;
using System.Collections.Generic;
using System.Text;
using TesterCall.Models.Interfaces;

namespace TesterCall.Models.Stats
{
    public class StatsBin
    {
        private string _rootName;
        private int _nameIteration;
        private DateTime _start;

        public StatsBin(string name, 
                            int nameIteration,
                            DateTime start)
        {
            _rootName = name;
            _nameIteration = nameIteration;
            _start = start;
        }

        public DateTime RecordingStarted => _start;
        public DateTime? RecordingFinished { get; set; }
        public string SessionName => _nameIteration > 0 ?
                                        $"{_rootName}_{_nameIteration}" :
                                        _rootName;

        public List<IHasResponseTime> Responses { get; set; } = new List<IHasResponseTime>();
    }
}
