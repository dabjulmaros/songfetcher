using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
 
namespace SongIdentify_Winform
{
    public partial class Form1 : Form
    {
        Thread t_watcher;
        public Form1()
        {
            InitializeComponent();

            t_watcher = new Thread(new ThreadStart(Monitor));
            t_watcher.IsBackground = true; 
            t_watcher.Start();
        }
        public String returnName() {
            int sessionCnt = 0;
            string name = "No Playback";
            foreach (AudioSession item in AudioUtilities.GetAllSessions())
            {
                if (item.Process == null)
                {
                    item.Dispose();
                    continue;
                }
                //if audio playback active.
                if (item.State == AudioSessionState.Active)
                {
                    if (item.Process.ProcessName.Equals("spotify", StringComparison.InvariantCultureIgnoreCase))
                    {
                        name=item.Process.MainWindowTitle;
                    }
                    else if (item.Process.ProcessName.Equals("foobar2000", StringComparison.InvariantCultureIgnoreCase))
                    {
                        //UpdateLabel_T(item.Process.MainWindowTitle.Replace("[foobar2000]", ""));
                        name=item.Process.MainWindowTitle.Substring(0, item.Process.MainWindowTitle.Length - 12);
                    }
                    else if (item.Process.ProcessName.Equals("vlc", StringComparison.InvariantCultureIgnoreCase))
                    {
                        name=item.Process.MainWindowTitle.Substring(0, item.Process.MainWindowTitle.Length - 19);
                    }
                    else if (item.Process.ProcessName.Equals("nvcontainer", StringComparison.InvariantCultureIgnoreCase)) // nvidia gfe screen recording.
                    {
                        sessionCnt -= 1;
                    }
                    else
                    {
                        name=item.Process.MainWindowTitle;
                    }
                }
                item.Dispose();
            }
            return name;
        }

        /// <summary>
        /// Background thread that monitors running audio session & update labeltext.
        /// </summary>
        void Monitor()
        {
            int sessionCnt = 0;
            while (true)
            {
                Thread.Sleep(1000);
                foreach (AudioSession item in AudioUtilities.GetAllSessions())
                {
                    if (item.Process == null)
                    {
                        item.Dispose();
                        continue;
                    }
                    //if audio playback active.
                    if (item.State == AudioSessionState.Active)
                    {
                        System.Diagnostics.Debug.WriteLine(item.Process.ProcessName + "\t" + item.Process.MainWindowTitle);
                        sessionCnt += 1;
                        if (item.Process.ProcessName.Equals("spotify", StringComparison.InvariantCultureIgnoreCase))
                        {
                            UpdateLabel_T(item.Process.MainWindowTitle);
                        }
                        else if (item.Process.ProcessName.Equals("foobar2000", StringComparison.InvariantCultureIgnoreCase))
                        {
                            //UpdateLabel_T(item.Process.MainWindowTitle.Replace("[foobar2000]", ""));
                            UpdateLabel_T(item.Process.MainWindowTitle.Substring(0, item.Process.MainWindowTitle.Length - 12));
                        }
                        else if (item.Process.ProcessName.Equals("vlc", StringComparison.InvariantCultureIgnoreCase))
                        {
                            UpdateLabel_T(item.Process.MainWindowTitle.Substring(0, item.Process.MainWindowTitle.Length - 19));
                        }
                        else if (item.Process.ProcessName.Equals("nvcontainer", StringComparison.InvariantCultureIgnoreCase)) // nvidia gfe screen recording.
                        {
                            sessionCnt -= 1;
                        }
                        else
                        {
                            UpdateLabel_T(item.Process.MainWindowTitle);
                        }
                    }
                    item.Dispose();
                }
                if(sessionCnt <= 0)
                {
                     UpdateLabel_T("No Playback");
                }

                sessionCnt = 0;
            }
        }

        public void UpdateLabel_T(string title)
        {
            try
            {
                if (!InvokeRequired)
                {
                    songTitleLabel.Text = title;
                }
                else
                {
                    Invoke(new Action<string>(UpdateLabel_T), title);
                }
            }
            catch(Exception)
            {
                
            }
        }
               
    }

}
