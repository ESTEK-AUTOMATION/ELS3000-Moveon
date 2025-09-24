using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace Project
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                Process currentProcess = Process.GetCurrentProcess();
                currentProcess.PriorityClass = ProcessPriorityClass.RealTime;

                if (currentProcess.ProcessName.Contains(".vshost"))
                {
                    if (Process.GetProcessesByName(currentProcess.ProcessName.Remove(currentProcess.ProcessName.Length - ".vshost".Length)).Length >= 1)
                    {
                        MessageBox.Show("Application is already running.");
                    }
                    else
                    {
                        Initialize();
                    }
                }
                else
                {
                    if (Process.GetProcessesByName(currentProcess.ProcessName).Length > 1)
                    {
                        MessageBox.Show("Application is already running.");
                    }
                    else
                    {
                        Initialize();
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        static void Initialize()
        {
            Customer.CustomerFormMainInterface formMainInterface = new Customer.CustomerFormMainInterface();
#if !DEBUGSEQ
#if RELEASE
            formMainInterface.CustomerRunExeSequence = true;
            formMainInterface.CustomerRunRtssSequence = false;
#endif
#if RTSSRELEASE
            formMainInterface.CustomerRunRtssSequence = true;
            formMainInterface.CustomerRunExeSequence = false;
#endif
#else
            formMainInterface.CustomerRunRtssSequence = false;
            formMainInterface.CustomerRunExeSequence = false;
#endif
            formMainInterface.Initialize();
            formMainInterface.CustomerAssemblyName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
            formMainInterface.CustomerAssemblyVersion = System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();
            formMainInterface.CustomerAssemblyVersion1 = System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();
            formMainInterface.Text = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;

            Application.Run(formMainInterface); 
        }

        
    }
}
