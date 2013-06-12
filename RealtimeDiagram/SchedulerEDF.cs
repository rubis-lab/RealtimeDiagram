using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeDiagram
{
    class SchedulerEDF
    {
        double _startTime;
        double _endTime;

        List<PeriodicTask> _listTaskSet;

        public List<PeriodicTask> ListTaskSet
        {
            get { return _listTaskSet; }
            set { _listTaskSet = value; }
        }

        List<List<PeriodicTask>> _listListPriodicTask = new List<List<PeriodicTask>>();
        List<List<TaskEvent>> _listListEventOutput = new List<List<TaskEvent>>();

        public List<List<TaskEvent>> ListListEventOutput
        {
            get { return _listListEventOutput; }
            set { _listListEventOutput = value; }
        }

        public SchedulerEDF(List<PeriodicTask> taskSet, double startTime, double endTime)
        {
            _listTaskSet = taskSet;
            _startTime = startTime;
            _endTime = endTime;

            for (int i=0; i<taskSet.Count; i++)
                _listListEventOutput.Add(new List<TaskEvent>());
        }

        Comparison<TaskEvent> compareStartTime = delegate(TaskEvent e1, TaskEvent e2) { return e1.AbsStartTime.CompareTo(e2.AbsStartTime); };
        private int compareHardDeadline(TaskEvent x, TaskEvent y)
        {
            return x.AbsHardDeadline.CompareTo(y.AbsHardDeadline);
        }

        TaskEvent _currentEvent = null;
        public void Schedule()
        {
            List<TaskEvent> listReleaseEvent = new List<TaskEvent>();
            foreach (PeriodicTask task in _listTaskSet)
            {
                listReleaseEvent.AddRange(task.GetEventList(_startTime, _endTime));
            }
            listReleaseEvent.Sort(compareStartTime);

            for (int t = 0; t < _endTime; t++)
            {
                if (_currentEvent != null)
                {
                    _currentEvent.RemainExecution--;

                    if (_currentEvent.RemainExecution <= 0)
                    {
                        TaskEvent newEvent = new TaskEvent(_currentEvent.PeriodicTask);
                        newEvent.AbsHardDeadline = _currentEvent.AbsHardDeadline;
                        newEvent.AbsSoftDeadline = _currentEvent.AbsSoftDeadline;
                        newEvent.AbsReleaseTime = _currentEvent.AbsReleaseTime;
                        newEvent.AbsCompleteTime = t;
                        newEvent.RemainExecution = t - _currentEvent.AbsStartTime;

                        _listListEventOutput[_currentEvent.PeriodicTask.TaskNumber].Add(newEvent);
                    }
                }

                List<TaskEvent> events = GetSameTimeEvents(listReleaseEvent, t);
                if (events.Count == 0)
                    continue;

                if (_currentEvent != null)
                    events.Add(_currentEvent);

                events.Sort(compareHardDeadline);
                
                _currentEvent = events[0];
                _currentEvent.AbsStartTime = t;               

            }
        }

        public void ScheduleOld()
        {
            List<double> listReleaseTime = new List<double>();
            List<TaskEvent> listReleaseEvent = new List<TaskEvent>();
            foreach (PeriodicTask task in _listTaskSet)
            {
                listReleaseEvent.AddRange(task.GetEventList(_startTime, _endTime));
                List<double> listTime = task.GetReleaseTime(_startTime, _endTime);
                foreach (double t in listTime)
                {
                    if (false == listReleaseTime.Contains(t))
                        listReleaseTime.Add(t);
                }
            }
            listReleaseEvent.Sort(compareStartTime);
            listReleaseTime.Sort();

            // 시간 진행
            for(int i = 0; i < listReleaseTime.Count; i++)
            {
                double currentTime = listReleaseTime[i];
                List<TaskEvent> nextEvents = GetSameTimeEvents(listReleaseEvent, currentTime);
                if (nextEvents == null)
                    continue;
                
                nextEvents.Sort(compareHardDeadline);

                double nextTime = _endTime;
                int nextIndex = i + nextEvents.Count;
                if (listReleaseTime.Count > nextIndex)
                    nextTime = listReleaseTime[nextIndex];

                foreach(TaskEvent e in nextEvents)
                {
                    e.AbsStartTime = currentTime;
                    e.AbsCompleteTime = currentTime + e.RemainExecution;

                    // 다음 릴리즈에도 완료하지 못하였다면
                    if (nextTime < e.AbsCompleteTime)
                    {
                        e.AbsCompleteTime = nextTime;
                        
                        // 남은 수행시간을 가진 새로운 이벤트를 생성
                        TaskEvent newEvent = new TaskEvent(e.PeriodicTask);
                        newEvent.AbsStartTime = nextTime;
                        newEvent.AbsSoftDeadline = e.AbsSoftDeadline;
                        newEvent.AbsHardDeadline = e.AbsHardDeadline;
                        newEvent.RemainExecution = e.RemainExecution - (nextTime - currentTime);
                        newEvent.AbsCompleteTime = nextTime + newEvent.RemainExecution;

                        listReleaseEvent.Add(newEvent);
                    }

                    _listListEventOutput[e.PeriodicTask.TaskNumber].Add(e);

                    currentTime = e.AbsCompleteTime;
                }
            }
        }


        public List<TaskEvent> GetSameTimeEvents(List<TaskEvent> list, double time)
        {  
            return list.FindAll(delegate(TaskEvent e) { return e.AbsStartTime == time; });
        }

     
    }
}
