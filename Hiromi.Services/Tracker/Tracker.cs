using System;
using System.Collections.Generic;

namespace Hiromi.Services.Tracker
{
    public class Tracker
    {
        public IList<IList<Object>> Progress { get; }
        public IList<IList<Object>> Archive { get; }

        public Tracker(IList<IList<object>> progress, IList<IList<object>> archive)
        {
            Progress = progress;
            Archive = archive;
        }
    }
}