using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeDiagram
{
    public class TaskEvent
    {
        PeriodicTask _periodicTask;

        public PeriodicTask PeriodicTask
        {
            get { return _periodicTask; }
            set { _periodicTask = value; }
        }

        double _absReleaseTime;

        public double AbsReleaseTime
        {
            get { return _absReleaseTime; }
            set { _absReleaseTime = value; }
        }

        double _absStartTime;

        public double AbsStartTime
        {
            get { return _absStartTime; }
            set { _absStartTime = value; }
        }
        double _absSoftDeadline;

        public double AbsSoftDeadline
        {
            get { return _absSoftDeadline; }
            set { _absSoftDeadline = value; }
        }
        double _absHardDeadline;

        public double AbsHardDeadline
        {
            get { return _absHardDeadline; }
            set { _absHardDeadline = value; }
        }
        double _absCompleteTime;

        public double AbsCompleteTime
        {
            get { return _absCompleteTime; }
            set { _absCompleteTime = value; }
        }

        double _remainExecution;

        public double RemainExecution
        {
            get { return _remainExecution; }
            set { _remainExecution = value; }
        }

        public TaskEvent(PeriodicTask task)
        {
            _periodicTask = task;
        }
    }
}
