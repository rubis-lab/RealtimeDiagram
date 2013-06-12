﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RealtimeDiagram
{
    public partial class TimeDiagram : UserControl
    {
        PeriodicTask _task;
        double _startTime;
        double _endTime;

        List<double> _listReleaseTime;
        List<double> _listSoftDeadline;
        List<double> _listHardDeadline;

        public TimeDiagram()
        {
            InitializeComponent();

            _startTime = 0;
            _endTime = 100;
            _task = new PeriodicTask("task", 5, 3, 3, 0.8, 5, 0);

            _listReleaseTime = _task.GetReleaseTime(_startTime, _endTime);
            _listSoftDeadline = _task.GetSoftDeadline(_startTime, _endTime);
            _listHardDeadline = _task.GetHardDeadline(_startTime, _endTime);
        }

        List<TaskEvent> _listTaskEvent;

        public void SetTask(PeriodicTask task, double startTime, double endTime)
        {
            _task = task;
            _startTime = startTime;
            _endTime = endTime;

            _listReleaseTime = _task.GetReleaseTime(_startTime, _endTime);
            _listSoftDeadline = _task.GetSoftDeadline(_startTime, _endTime);
            _listHardDeadline = _task.GetHardDeadline(_startTime, _endTime);
        }

        internal void SetTask(List<TaskEvent> list, int startTime, long endTime)
        {
            _startTime = startTime;
            _endTime = endTime;

            _listTaskEvent = list;
            _listReleaseTime = list[0].PeriodicTask.GetReleaseTime(startTime, endTime);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            this.Invalidate();
        }

        protected void DrawSingleTask(PaintEventArgs e)
        {
            Pen penBlack = new Pen(Brushes.Black);
            Pen penRed = new Pen(Brushes.Red);
            Pen penBlue = new Pen(Brushes.Blue);

            // 베이스 라인 길이
            int timelineWidth = (int)(this.Width * 0.8);

            // 단위 유닛
            float unit = (float)(timelineWidth / (_endTime - _startTime));


            // 베이스 라인 그리기
            Point timeline1 = new Point((int)(this.Width * 0.1), (int)(this.Height * 0.6));
            Point timeline2 = new Point(timeline1.X + timelineWidth, timeline1.Y);
            e.Graphics.DrawLine(penBlack, timeline1, timeline2);



            // 눈금 그리기
            float gridUnit = (float)(_endTime - _startTime) / 10;
            for (int i = 0; i <= 10; i++)
            {
                Point p1 = new Point((int)(timeline1.X + unit * gridUnit * i), timeline1.Y);
                Point p2 = new Point((int)(timeline1.X + unit * gridUnit * i), timeline1.Y + 3);

                e.Graphics.DrawLine(penBlack, p1, p2);

                // 단위 숫자 그리기
                String number = String.Format("{0:F0}", (_startTime + gridUnit * i));
                SizeF sizeNumber = e.Graphics.MeasureString(number, this.Font);
                e.Graphics.DrawString(number, this.Font, Brushes.Black,
                    new PointF((timeline1.X + unit * gridUnit * i) - sizeNumber.Width / 2, timeline1.Y + sizeNumber.Height / 2));
            }

            // Task 박스 그리기
            foreach (double r in _listReleaseTime)
            {
                float startX = (float)(timeline1.X + (r * unit));
                float executionWidth = (float)(_task.ExecutionTime * unit);
                int softDeadlineWidth = (int)(_task.SoftDeadline * unit);

                e.Graphics.DrawRectangle(penBlack, startX, timeline1.Y - 20, executionWidth, 20);

                // Release time 표시
                e.Graphics.DrawLine(penBlue,
                    new Point((int)startX, (int)(timeline1.Y - 30)),
                    new Point((int)startX, timeline1.Y));

                e.Graphics.DrawLine(penBlue,
                    new Point((int)startX - 3, (int)timeline1.Y - 30 + 3),
                    new Point((int)startX, (int)(timeline1.Y - 30)));

                e.Graphics.DrawLine(penBlue,
                    new Point((int)startX + 3, (int)timeline1.Y - 30 + 3),
                    new Point((int)startX, (int)(timeline1.Y - 30)));

                // Soft Deadline 표시
                e.Graphics.DrawLine(penRed,
                    new Point((int)startX + softDeadlineWidth, (int)(timeline1.Y - 30)),
                    new Point((int)startX + softDeadlineWidth, timeline1.Y));

                e.Graphics.DrawLine(penRed,
                    new Point((int)startX + softDeadlineWidth - 3, (int)timeline1.Y - 3),
                    new Point((int)startX + softDeadlineWidth, timeline1.Y));

                e.Graphics.DrawLine(penRed,
                    new Point((int)startX + softDeadlineWidth + 3, (int)timeline1.Y - 3),
                    new Point((int)startX + softDeadlineWidth, timeline1.Y));
            }

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);

            if (_listTaskEvent == null)
                DrawSingleTask(e);
            else
                DrawTaskEvents(e);
        }

        private void DrawTaskEvents(PaintEventArgs e)
        {
            Pen penBlack = new Pen(Brushes.Black);
            Pen penRed = new Pen(Brushes.Red);
            Pen penBlue = new Pen(Brushes.Blue);

            // 베이스 라인 길이
            int timelineWidth = (int)(this.Width * 0.8);
            int boxHeight = (int)(this.Height * 0.3);
            int arrowHeight = (int)(this.Height * 0.4);

            // 단위 유닛
            float unit = (float)(timelineWidth / (_endTime - _startTime));


            // 베이스 라인 그리기
            Point timeline1 = new Point((int)(this.Width * 0.1), (int)(this.Height * 0.6));
            Point timeline2 = new Point(timeline1.X + timelineWidth, timeline1.Y);
            e.Graphics.DrawLine(penBlack, timeline1, timeline2);



            // 눈금 그리기
            float gridUnit = (float)(_endTime - _startTime) / 10;
            for (int i = 0; i <= 10; i++)
            {
                Point p1 = new Point((int)(timeline1.X + unit * gridUnit * i), timeline1.Y);
                Point p2 = new Point((int)(timeline1.X + unit * gridUnit * i), timeline1.Y + 3);

                e.Graphics.DrawLine(penBlack, p1, p2);

                // 단위 숫자 그리기
                String number = String.Format("{0:F0}", (_startTime + gridUnit * i));
                SizeF sizeNumber = e.Graphics.MeasureString(number, this.Font);
                e.Graphics.DrawString(number, this.Font, Brushes.Black,
                    new PointF((timeline1.X + unit * gridUnit * i) - sizeNumber.Width / 2, timeline1.Y + sizeNumber.Height / 2));
            }

            // Task 박스 그리기
            foreach (TaskEvent evnt in _listTaskEvent)
            {
                float startX = (float)(timeline1.X + (evnt.AbsStartTime * unit));
                float executionWidth = (float)((evnt.AbsCompleteTime - evnt.AbsStartTime) * unit);
                int softDeadlineWidth = (int)(evnt.AbsSoftDeadline * unit);

                e.Graphics.FillRectangle(Brushes.SteelBlue, startX, timeline1.Y - boxHeight, executionWidth, boxHeight);
            }

            
            foreach (double time in _listReleaseTime)
            {
                float startX = (float)(timeline1.X + (time * unit)); 
                int softDeadlineWidth = (int)(_listTaskEvent[0].PeriodicTask.SoftDeadline * unit);

                // Release time 표시
                e.Graphics.DrawLine(penBlue,
                    new Point((int)startX, (int)(timeline1.Y - arrowHeight)),
                    new Point((int)startX, timeline1.Y));

                e.Graphics.DrawLine(penBlue,
                    new Point((int)startX - 3, (int)timeline1.Y - arrowHeight + 3),
                    new Point((int)startX, (int)(timeline1.Y - arrowHeight)));

                e.Graphics.DrawLine(penBlue,
                    new Point((int)startX + 3, (int)timeline1.Y - arrowHeight + 3),
                    new Point((int)startX, (int)(timeline1.Y - arrowHeight)));

                // Soft Deadline 표시
                e.Graphics.DrawLine(penRed,
                    new Point((int)startX + softDeadlineWidth, (int)(timeline1.Y - arrowHeight)),
                    new Point((int)startX + softDeadlineWidth, timeline1.Y));

                e.Graphics.DrawLine(penRed,
                    new Point((int)startX + softDeadlineWidth - 3, (int)timeline1.Y - 3),
                    new Point((int)startX + softDeadlineWidth, timeline1.Y));

                e.Graphics.DrawLine(penRed,
                    new Point((int)startX + softDeadlineWidth + 3, (int)timeline1.Y - 3),
                    new Point((int)startX + softDeadlineWidth, timeline1.Y));
            }

        }

    }
}
