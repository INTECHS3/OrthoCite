using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;

namespace OrthoCite
{
    //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
    [Serializable]
    struct DataSaveStruct
    {
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
#pragma warning disable CS0649
        internal string Name;
        internal byte District;
        internal byte ValidatedMiniGames;
        internal byte NumberOfLives;
        internal byte TrapsNpcTalkedTo;
#pragma warning restore CS0649
    }

    public class DataSave
    {
        DataSaveStruct _dataSave;
        string _path;

        public string Name
        {
            get { return _dataSave.Name; }
            set { _dataSave.Name = value; }
        }

        public byte District
        {
            get { return _dataSave.District; }
            set { _dataSave.District = value; }
        }

        public byte NumberOfLives
        {
            get { return _dataSave.NumberOfLives; }
            set { _dataSave.NumberOfLives = value; }
        }

        public bool MiniGameIsValidated(byte miniGameId)
        {
            return ((_dataSave.ValidatedMiniGames << (miniGameId - 1)) >> 7) == 1;
        }

        public void ValidateMiniGame(byte miniGameId)
        {
            int mask = 1 << 8 - miniGameId;
            _dataSave.ValidatedMiniGames |= (byte)mask;
        }

        public bool TrapsNpcWereTalkedTo(byte NpcId)
        {
            return ((_dataSave.TrapsNpcTalkedTo << (NpcId - 1)) >> 7) == 1;
        }

        public void TrapsNpcTalkTo(byte NpcId)
        {
            int mask = 1 << 8 - NpcId;
            _dataSave.TrapsNpcTalkedTo |= (byte)mask;
        }

        public DataSave()
        {
            _dataSave = new DataSaveStruct { Name = "", District = 1, ValidatedMiniGames = 0, NumberOfLives = 2, TrapsNpcTalkedTo = 0 };
            _path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\datasaves";

            Directory.CreateDirectory(_path);
        }

        public void Load(string datasaveSlug)
        {
            using (var file = File.OpenRead(_path + @"\" + datasaveSlug + ".oct"))
            {
                var reader = new BinaryFormatter();
                _dataSave = (DataSaveStruct)reader.Deserialize(file);
            }
        }

        public void Save()
        {
            string slug = _GenerateSlug(_dataSave.Name);
            using (var file = File.OpenWrite(_path + @"\" + slug + ".oct"))
            {
                var writer = new BinaryFormatter();
                writer.Serialize(file, _dataSave);
            }
        }

        public Dictionary<string, string> List()
        {
            DataSaveStruct previouslyLoadedSave = _dataSave;
            string[] filePaths = Directory.GetFiles(_path);
            Dictionary<string, string> datasaves = new Dictionary<string, string>();

            foreach (string filePath in filePaths)
            {
                if (!filePath.EndsWith(".oct")) continue;

                string slug = filePath.Split(new char[] { '\\' }).Last();
                Load(slug);
                datasaves.Add(slug, _dataSave.Name);
            }

            _dataSave = previouslyLoadedSave;

            return datasaves;
        }

        static string _GenerateSlug(string phrase)
        {
            string str = _RemoveAccent(phrase).ToLower();
            // invalid chars           
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            // convert multiple spaces into one space   
            str = Regex.Replace(str, @"\s+", " ").Trim();
            // cut and trim 
            str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();
            str = Regex.Replace(str, @"\s", "-"); // hyphens   
            return str;
        }

        static string _RemoveAccent(string txt)
        {
            var normalizedString = txt.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
