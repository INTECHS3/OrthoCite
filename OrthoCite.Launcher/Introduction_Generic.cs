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
using System.Reflection;
using System.IO;

namespace OrthoCite.Launcher
{
    public partial class Introduction_Generic : Form
    {

        const int limitMin = 2;
        const int limitSecond = 37;
        Main _saveMainForm;
        int Compteur = 0;

        public Introduction_Generic(Main main)
        {
            _saveMainForm = main;
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            playerBrowser.Navigate(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\orthocite_intro_final_final.swf");
            CountVideo.Start();

        }
            
        private void CountVideo_Tick(object sender, EventArgs e)
        {
            int totalSecond = limitMin * 60 + limitSecond;
            Compteur++;
            if (totalSecond == Compteur) Close();
        }
        
        private void Introduction_Generic_FormClosing(object sender, FormClosingEventArgs e)
        {
            int totalSecond = limitMin * 60 + limitSecond;
            if (Compteur < totalSecond) e.Cancel = true;
        }
    }
}