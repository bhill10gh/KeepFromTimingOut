using System;
using System.Windows.Forms;
using System.Windows;
using System.Runtime.InteropServices;
using KeepFromTimingOut;

namespace KeepFromTimingOut
{
    public partial class KeepFromLocking : Form
    {
        bool KeepGoing = false;
        int pointPos = -1;
        private const uint MOUSEEVENTF_MOVE = 1;

        public KeepFromLocking()
        {
            InitializeComponent();


            GlobalHotKey.RegisterHotKey("Alt + Shift + Q", () => Stop_KeyPressed());
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, uint dwExtraInfo);

        public void StartTimer()
        {
            Every4Minutes.Interval = 1000;
            Every4Minutes.Tick += new EventHandler(MoveNow);

            pointPos = pointPos * -1;

            Cursor.Position = new System.Drawing.Point(Cursor.Position.X + pointPos, Cursor.Position.Y + pointPos);
            lblRunStatus.Text = "Running";
            Every4Minutes.Start();
        }

        void MoveNow(object sender, EventArgs e)
        {
            POINT point = KeepFromLocking.GetCursorPosition();
            uint x = (uint)point.X, y = (uint)point.Y;
            if(x > 1500 && y > 800)
            {
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new System.Drawing.Point(100, 100);
            }
            else
            {
                mouse_event(MOUSEEVENTF_MOVE, 1, 1, 0, 0);
            }
            //Cursor.Position = new Point(Cursor.Position.X + pointPos, Cursor.Position.Y + pointPos);
            
            //System.Threading.Thread.Sleep(60000);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            KeepGoing = true;
            StartTimer();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            KeepGoing = false;
        }

        private void Every4Minutes_Tick(object sender, EventArgs e)
        {
            if (!KeepGoing)
            {
                Every4Minutes.Stop();
                lblRunStatus.Text = "Not Running";
                lblRunStatus.Visible = true;
                return;
            }

            lblRunStatus.Visible = !lblRunStatus.Visible;
        }


        /// <summary>
        /// Retrieves the cursor's position, in screen coordinates.
        /// </summary>
        /// <see>See MSDN documentation for further information.</see>
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        public static POINT GetCursorPosition()
        {
            POINT lpPoint;
            GetCursorPos(out lpPoint);
            // NOTE: If you need error handling
            // bool success = GetCursorPos(out lpPoint);
            // if (!success)

            return lpPoint;
        }

        private void lblRunStatus_TextChanged(object sender, EventArgs e)
        {
            if(lblRunStatus.Text == "Not Running")
            {
                btnStart.Enabled = true;
                btnStop.Enabled = false;
                btnStart.Visible = true;
                btnStop.Visible = false;
                lblRunStatus.ForeColor = System.Drawing.Color.Black;
            }
            else
            {
                btnStart.Enabled = false;
                btnStop.Enabled = true;
                btnStart.Visible = false;
                btnStop.Visible = true;
                lblRunStatus.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void Stop_KeyPressed()
        {
            KeepGoing = false;
        }
    }

    /// <summary>
    /// Struct representing a point.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;

        public static implicit operator System.Windows.Point(POINT point)
        {
            return new System.Windows.Point(point.X, point.Y);
        }
    }

}

