using System;
using System.Net;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using System.Text;
using static System.Environment;

namespace OrthoCite.Launcher
{
    public partial class Main : Form
    {
        DataSave _dataSave;
        Introduction_Generic formSecon;

        string baseUrl = @"https://team-developpement.fr/orthocite/index.php?page=generateXML&name=";
        Dictionary<string, string> differentXmlWithUrl = new Dictionary<string, string>()
        {
            { "boss", "superboss" },
            { "DoorGame", "doorgame_1" },
            { "DoorGame2", "doorgame_2" },
            { "GuessGame", "guessgame" },
            { "platformer", "platformer" },
            { "Rearranger", "rearranger" },
            { "StopGame", "stopgame" },
            { "ThrowGame", "throwgame" }
        };

        public Main()
        {
            InitializeComponent();

            _dataSave = new DataSave(Environment.GetFolderPath(SpecialFolder.ApplicationData) + @"\OrthoCite\datasaves");
            _dataSave.Clear();
            xmlLoad.Maximum = 8;
            xmlLoad.Minimum = 0;
            xmlLoad.Step = 1;
            messageUpdate.Text = "Jeu à jour !";
            if (!UpdateXML()) MessageBox.Show("Erreur lors de la mise à jour");
            
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
            File.Delete(Environment.GetFolderPath(SpecialFolder.ApplicationData) + @"\OrthoCite\datasaves" + ListDatasaves.SelectedValue + ".oct");
            LoadDatasavesIntoList();
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            if ((string)ListDatasaves.SelectedValue != "datasave")
            {
                if (File.Exists((Environment.GetFolderPath(SpecialFolder.ApplicationData) + @"\OrthoCite\datasaves\datasave.oct")))
                {
                    _dataSave.Load("datasave");
                    _dataSave.Save(false);
                    File.Delete(Environment.GetFolderPath(SpecialFolder.ApplicationData) + @"\OrthoCite\datasaves\datasave.oct");
                }

                File.Move(Environment.GetFolderPath(SpecialFolder.ApplicationData) + @"\OrthoCite\datasaves\" + ListDatasaves.SelectedValue + ".oct", Environment.GetFolderPath(SpecialFolder.ApplicationData) + @"\OrthoCite\datasaves\datasave.oct");
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
            p.StartInfo.FileName = "OrthoCite.exe";
            p.Start();
            Close();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool UpdateXML()
        {
            
            messageUpdate.Visible = false; 
            xmlLoad.Value = 0;

            BtnAdd.Enabled = false;
            BtnLoad.Enabled = false;
            BtnDelete.Enabled = false;
            pictureBox1.Enabled = false;

            try
            {
                foreach (KeyValuePair<string, string> i in differentXmlWithUrl)
                {
                    XmlDocument xml = new XmlDocument();
                    WebClient wc = new WebClient();
                    wc.Encoding = Encoding.UTF8;

                    xml.LoadXml(wc.DownloadString(baseUrl + i.Value));
                    xml.Save(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\Content\dictionaries\" + i.Key + ".xml");
                    xmlLoad.PerformStep();
                }
            }
            catch { return false; }
            
            messageUpdate.Visible = true;
            BtnAdd.Enabled = true;
            BtnLoad.Enabled = true;
            BtnDelete.Enabled = true;
            pictureBox1.Enabled = true;
            return true;
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            UpdateXML();
        }

        
    }

}
