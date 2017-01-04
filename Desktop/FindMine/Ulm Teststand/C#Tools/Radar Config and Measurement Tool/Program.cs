using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Radar_Config_and_Measurement_Tool
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Console.WriteLine("Application is started");
            Application.Run(new Main());
        }
    }
}
