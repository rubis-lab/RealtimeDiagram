using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RealtimeDiagram
{
    public partial class FormTimeDiagram : Form
    {
        public FormTimeDiagram()
        {
            InitializeComponent();            
        }


        internal void SetTask(List<List<TaskEvent>> listList, int startTime, long endTime)
        {
            foreach (List<TaskEvent> list in listList)
            {
                TimeDiagram diagram = new TimeDiagram();
                diagram.BackColor = System.Drawing.Color.White;
                diagram.Location = new System.Drawing.Point(3, 3);
                diagram.MinimumSize = new System.Drawing.Size(100, 100);
                diagram.Name = "timeDiagram1";
                diagram.Size = new System.Drawing.Size(582, 100);

                diagram.SetTask(list, startTime, endTime);

                flowLayoutPanel1.Controls.Add(diagram);
            }
        }

        internal void SetTask(PeriodicTask periodicTask, int startTime, long endTime)
        {
            TimeDiagram diagram = new TimeDiagram();
            diagram.BackColor = System.Drawing.Color.White;
            diagram.Location = new System.Drawing.Point(3, 3);
            diagram.MinimumSize = new System.Drawing.Size(100, 100);
            diagram.Name = "timeDiagram1";
            diagram.Size = new System.Drawing.Size(582, 100);

            diagram.SetTask(periodicTask, startTime, endTime);

            flowLayoutPanel1.Controls.Add(diagram);            
        }
    }
}
