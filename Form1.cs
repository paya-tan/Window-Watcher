using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WindowWatcher
{
    public partial class Form1 : Form
    {
        private string WINDOWNAME = "";
        private string TASKNAME = "";

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)] //loads dll for checking window titles
        static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName); //imports function for checking windows by caption

        int wait_loop = 50000; //wait time in miliseconds
        Timer timer = new System.Windows.Forms.Timer(); //the timeout timer (for automatic resets using the variable from wait_loop)
        public Form1(String[] args)
        {
            WINDOWNAME = args[1];
            TASKNAME = args[2];
            InitializeComponent();
            this.TopMost = true; //forces the window to the top
            this.Hide(); //hides the window
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing); //runs our function to prevent user closing the window
            this.label1.Text = "Hmm, " + WINDOWNAME + " doesn\'t seem to be working right now.";
            this.label2.Text = "Would you like me to close " + TASKNAME + " for you?";
            if (args.Length > 0) //sets the wait time to the number of seconds specified at program start
            {
                int x = 0;
                bool parse_chk = int.TryParse(args[1], out x);
                if (parse_chk)
                {
                    wait_loop = x * 1000;
                }
            }
            checkWindow();
        }

        void Form1_FormClosing(Object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }

        void checkWindow() //our main window loops
        {
            if (timer.Enabled) //ensures the timeout timer isn't running
            {
                timer.Enabled = false;
                timer.Stop();
            }
            bool error_exist = false;
            while (!error_exist) //checks open windows for window with caption at set intervals
            {
                System.Threading.Thread.Sleep(5000); //Wait timer
                IntPtr windowPtr = FindWindowByCaption(IntPtr.Zero, WINDOWNAME);
                if (windowPtr != IntPtr.Zero) //if the window exists...
                {
                    error_exist = true;
                }
            }
            this.Show(); //shows the window after we leave the previous check loop, IE when it appears

            timer.Interval = wait_loop; //use our command line variable (or the default) for the timer's interval
            timer.Enabled = true; //enable the timer
            timer.Start(); //start the timer
            timer.Tick += (s, e) => //when the timer is finished...
            {
                onYesClicked(null, null); //run our kill function
                return;
            };
            while (timer.Enabled) //when the timer is running...
            {
                IntPtr windowPtr = FindWindowByCaption(IntPtr.Zero, WINDOWNAME); //check if the window exists
                if (windowPtr == IntPtr.Zero) //if it doesn't...
                {
                    onNoClicked(null, null);
                    return;
                }
                this.TopMost = false; //toggle window on top off
                this.TopMost = true; //toggle window on top back on (to reset it to top)
                //this.Activate(); //force window focus
                Application.DoEvents(); //process window events still so you can interact with your desktop
            }
        }

        private void onYesClicked(object sender, EventArgs e) //kills our window, hides the prompt window, then restarts our window check loop
        {
            foreach (var process in Process.GetProcessesByName(TASKNAME)) {
                process.Kill();
            }
            this.Hide();
            checkWindow();
        }

        private void onNoClicked(object sender, EventArgs e) //hides the window and restarts the window check loop
        {
            this.Hide();
            checkWindow();
        }


    }
}
