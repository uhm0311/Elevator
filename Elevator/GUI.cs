using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ElevatorArrivedListener = org.uhm.toy.elevator.Elevator.ElevatorArrivedListener;
using ElevatorPausedListener = org.uhm.toy.elevator.Elevator.ElevatorPausedListener;

namespace org.uhm.toy.elevator
{
    public partial class GUI : Form
    {
        private Elevator elevator;

        private Dictionary<int, CheckBox> requests = new Dictionary<int, CheckBox>();
        private Dictionary<int, RadioButton> currents = new Dictionary<int, RadioButton>();

        public GUI()
        {
            InitializeComponent();
        }

        private void OnLoad(object sender, EventArgs e)
        {
            foreach (Control control in checkboxes.Controls)
            {
                if (control is CheckBox request)
                {
                    requests.Add(GetFloor(request), request);
                }
            }

            foreach (Control control in radiobuttons.Controls)
            {
                if (control is RadioButton current)
                {
                    currents.Add(GetFloor(current), current);
                }
            }

            elevator = new Elevator(1000, new ElevatorArrivedListener(Arrived), new ElevatorPausedListener(Paused));
        }

        private void OnButtonClick(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                int floor = GetFloor(button);

                if (requests[floor].Checked = !requests[floor].Checked)
                {
                    elevator.Request(floor);
                }
                else
                {
                    elevator.Abort(floor);
                }
            }
        }

        private void Arrived(int floor)
        {
            Invoke((MethodInvoker)delegate { currents[floor].Checked = true; });
        }

        private void Paused(int floor)
        {
            Invoke((MethodInvoker)delegate { requests[floor].Checked = false; });
        }

        private int GetFloor(Control control)
        {
            return int.Parse(control.Text.Replace("F", ""));
        }
    }
}
