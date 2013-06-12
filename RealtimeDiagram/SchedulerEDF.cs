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
        
        public void Schedule()
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

            Queue<double> queueTime = new Queue<double>();
            foreach (double t in listReleaseTime)
                queueTime.Enqueue(t);

            // 시간 진행
            while(queueTime.Count != 0)
            {
                double currentTime = queueTime.Dequeue();
                List<TaskEvent> nextEvents = GetSameTimeEvents(listReleaseEvent, currentTime);
                if (nextEvents.Count == 0)
                    continue;
                
                nextEvents.Sort(compareHardDeadline);

                double nextTime = _endTime;
                if (queueTime.Count != 0)
                    nextTime = queueTime.First();

                foreach(TaskEvent e in nextEvents)
                {
                    e.AbsStartTime = currentTime;
                    e.AbsCompleteTime = currentTime + e.RemainExecution;

                    // 다음 Job의 릴리즈에도 완료하지 못하였다면
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
                        if (false == queueTime.Contains(nextTime))
                        {
                            queueTime.Enqueue(nextTime);
                            List<double> temp = queueTime.ToList();
                            temp.Sort();
                            queueTime.Clear();

                            foreach (double d in temp)
                                queueTime.Enqueue(d);
                        }
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
