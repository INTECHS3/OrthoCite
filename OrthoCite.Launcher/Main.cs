using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Threading;

namespace OrthoCite.Launcher
{
    public partial class Main : Form
    {
        DataSave _dataSave;
        Introduction_Generic formSecon;

        public Main()
        {
            InitializeComponent();
           
            _dataSave = new DataSave(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\datasaves");
            _dataSave.Clear();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            LoadDatasavesIntoList();

            ListDatasaves.SelectedIndexChanged += ListDatasaves_SelectedIndexChanged;
        }

        private void ListDatasaves_SelectedIndexChanged(object sender, EventArgs e)
        {
            BtnDelete.Enabled = true;
            BtnLoad.Enabled = true;
        }

        private void LoadDatasavesIntoList()
        {
            Dictionary<string, string> datasaves = _dataSave.List();
            if (datasaves.Count != 0)
            {
                ListDatasaves.DataSource = new BindingSource(datasaves, null);
                ListDatasaves.DisplayMember = "Value";
                ListDatasaves.ValueMember = "Key";
                ListDatasaves.Enabled = true;
            }
            else
            {
                ListDatasaves.DataSource = new List<string> { "Aucune sauvegarde." };
                ListDatasaves.Enabled = false;
            }
            ListDatasaves.ClearSelected();
            BtnDelete.Enabled = false;
            BtnLoad.Enabled = false;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TextName.Text)) return;
            _dataSave.Name = TextName.Text;
            _dataSave.Save(false);
            LoadDatasavesIntoList();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            File.Delete(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\datasaves\" + ListDatasaves.SelectedValue + ".oct");
            LoadDatasavesIntoList();
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            if ((string)ListDatasaves.SelectedValue != "datasave")
            {
                if (File.Exists(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\datasaves\datasave.oct"))
                {
                    _dataSave.Load("datasave");
                    _dataSave.Save(false);
                    File.Delete(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\datasaves\datasave.oct");
                }

                File.Move(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\datasaves\" + ListDatasaves.SelectedValue + ".oct", Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\datasaves\datasave.oct");
            }


            _dataSave.Load("datasave");

            if(_dataSave.District == 1 && !_dataSave.MiniGameIsValidated(DataSaveMiniGame.REARRANGER) && !_dataSave.MiniGameIsValidated(DataSaveMiniGame.PLATFORMER) && !_dataSave.MiniGameIsValidated(DataSaveMiniGame.DOORGAME))
            {
                try
                {
                    formSecon = new Introduction_Generic(this);
                    formSecon.ShowDialog();

                }
                catch { MessageBox.Show("We have a bug ..."); }
            }
            

            var p = new Process();
            p.StartInfo.FileName = "OrthoCité.exe";
            p.Start();
            Close();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
