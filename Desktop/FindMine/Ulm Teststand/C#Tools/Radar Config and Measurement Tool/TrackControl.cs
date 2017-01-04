using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EH.RadarControl;
using System.IO.Ports;

namespace Radar_Config_and_Measurement_Tool
{
    public partial class Main : Form
    {
        private void btn_Mill_Motor_Goto_Click(object sender, EventArgs e)
        {
            MillTrack_PositionControl control = new MillTrack_PositionControl(enableAutomationDebugOutputToolStripMenuItem.Checked);
            if (!control.openCOM(cB_Mill_Motor_Ports.SelectedItem.ToString()))
            {
                MessageBox.Show("Can't open COM Port", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!control.setPosition((UInt32)num_Mill_Motor_Pos.Value))
            {
                MessageBox.Show("Error occured during set position", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            control.closeCOM();
        }

        private void btn_Mill_Motor_refDrive_Click(object sender, EventArgs e)
        {
            MillTrack_PositionControl control = new MillTrack_PositionControl(enableAutomationDebugOutputToolStripMenuItem.Checked);
            if (!control.openCOM(cB_Mill_Motor_Ports.SelectedItem.ToString()))
            {
                MessageBox.Show("Can't open COM Port", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!control.referenceDrive())
            {
                MessageBox.Show("Error occured during reference drive", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            control.closeCOM();
        }

        private void cB_Mill_Motor_Ports_DropDown(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();

            Array.Sort(ports, StringComparer.InvariantCulture);

            cB_Mill_Motor_Ports.Items.Clear();

            foreach (string p in ports)
            {
                cB_Mill_Motor_Ports.Items.Add(p);
            }
        }

        private void cB_Mill_Tracker_COM_DropDown(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();

            Array.Sort(ports, StringComparer.InvariantCulture);

            cB_Mill_Tracker_COM.Items.Clear();

            foreach (string p in ports)
            {
                cB_Mill_Tracker_COM.Items.Add(p);
            }
        }

        private void btn_Mill_Tracker_get_Click(object sender, EventArgs e)
        {
            MillTrack_PositionTracker tracker = new MillTrack_PositionTracker(enableAutomationDebugOutputToolStripMenuItem.Checked);
            if (!tracker.openCOM(cB_Mill_Tracker_COM.SelectedItem.ToString()))
            {
                MessageBox.Show("Can't open COM Port", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            double pos = tracker.getPosition();
            tracker.closeCOM();

            MessageBox.Show("Position = " + pos + "mm", "Position", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void cB_FEW_Motor_COM_DropDown(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();

            Array.Sort(ports, StringComparer.InvariantCulture);

            cB_FEW_Motor_COM.Items.Clear();

            foreach (string p in ports)
            {
                cB_FEW_Motor_COM.Items.Add(p);
            }
        }

        private void cB_FEW_Tracker_COM_DropDown(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();

            Array.Sort(ports, StringComparer.InvariantCulture);

            cB_FEW_Tracker_COM.Items.Clear();

            foreach (string p in ports)
            {
                cB_FEW_Tracker_COM.Items.Add(p);
            }
        }

        private void btn_FEW_Motor_refDrive_Click(object sender, EventArgs e)
        {
            PositionControl control;
            if (gB_Track_FEW_MotorSettings.Visible)
            {
                control = new UniUlm_PositionControl((double)num_TrackControl_Lab_Steps.Value, (UInt32)num_TrackControl_Lab_MaxDist.Value, enableAutomationDebugOutputToolStripMenuItem.Checked);
            }
            else
            {
                control = new Lab_PositionControl(enableAutomationDebugOutputToolStripMenuItem.Checked);
            }
            if (!control.openCOM(cB_FEW_Motor_COM.SelectedItem.ToString()))
            {
                MessageBox.Show("Can't open COM Port", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!control.referenceDrive())
            {
                MessageBox.Show("Error occured during reference drive", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            control.closeCOM();
        }

        private void btn_FEW_Motor_Goto_Click(object sender, EventArgs e)
        {
            PositionControl control;
            if (gB_Track_FEW_MotorSettings.Visible)
            {
                control = new UniUlm_PositionControl((double)num_TrackControl_Lab_Steps.Value, (UInt32)num_TrackControl_Lab_MaxDist.Value, enableAutomationDebugOutputToolStripMenuItem.Checked);
            }
            else
            {
                control = new Lab_PositionControl(enableAutomationDebugOutputToolStripMenuItem.Checked);
            }
            if (!control.openCOM(cB_FEW_Motor_COM.SelectedItem.ToString()))
            {
                MessageBox.Show("Can't open COM Port", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!control.setPosition((UInt32)num_FEW_Motor_Pos.Value))
            {
                MessageBox.Show("Error occured during set position", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            control.closeCOM();
        }

        private void btn_FEW_Motor_Get_Click(object sender, EventArgs e)
        {
            PositionControl control;
            if (gB_Track_FEW_MotorSettings.Visible)
            {
                control = new UniUlm_PositionControl((double)num_TrackControl_Lab_Steps.Value, (UInt32)num_TrackControl_Lab_MaxDist.Value, enableAutomationDebugOutputToolStripMenuItem.Checked);
            }
            else
            {
                control = new Lab_PositionControl(enableAutomationDebugOutputToolStripMenuItem.Checked);
            }
            if (!control.openCOM(cB_FEW_Motor_COM.SelectedItem.ToString()))
            {
                MessageBox.Show("Can't open COM Port", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            UInt32 pos = 0;
            if (gB_Track_FEW_MotorSettings.Visible)
            {
                pos = (control as UniUlm_PositionControl).getPosition();
            }
            else
            {
                pos = (control as Lab_PositionControl).getPosition();
            }

            control.closeCOM();

            MessageBox.Show("Position = " + pos + "mm", "Position", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btn_FEW_Tracker_get_Click(object sender, EventArgs e)
        {
            Lab_PositionTracker tracker = new Lab_PositionTracker(enableAutomationDebugOutputToolStripMenuItem.Checked);
            if (!tracker.openCOM(cB_FEW_Tracker_COM.SelectedItem.ToString()))
            {
                MessageBox.Show("Can't open COM Port", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            double pos = tracker.getPosition();
            tracker.closeCOM();

            MessageBox.Show("Position = " + pos + "mm", "Position", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void num_TrackControl_Lab_MaxDist_ValueChanged(object sender, EventArgs e)
        {
            num_FEW_Motor_Pos.Maximum = num_TrackControl_Lab_MaxDist.Value;
            lbl_FEW_Pos.Text = "Position: 0 ... " + num_TrackControl_Lab_MaxDist.Value.ToString();
        }
    }
}
