using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OrthoCite.Launcher
{
    public partial class Main : Form
    {
        DataSave _dataSave;

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

            var p = new Process();
            p.StartInfo.FileName = "OrthoCité.exe";
            p.Start();
            Close();
        }
    }
}
