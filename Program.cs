using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;



namespace WindowWatcher
{

    static class Program
    {

        static void Main(String[] args)
        {
            if (args.Length > 1)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1(args));
            }
            else
            {
                Console.WriteLine("Not enough arguments given! Please specify both a window name to watch and a task to close");
            }
        }
    }
}
