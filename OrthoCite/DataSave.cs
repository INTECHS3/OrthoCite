using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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
        internal byte NumberOfCredits;
        internal byte TrapsNpcTalkedTo;
#pragma warning restore CS0649
    }

    public class DataSave
    {
        DataSaveStruct _dataSave;
        readonly string _path;

        public string Name
        {
            get { return _dataSave.Name; }
            set { _dataSave.Name = value; }
        }

        public byte District
        {
            get { return _dataSave.District; }
            set {
                if (value <= 0 || value > 4) throw new ArgumentException("District must be between 1 and 4", nameof(value));
                _dataSave.District = value;
            }
        }

        public byte NumberOfLives
        {
            get { return _dataSave.NumberOfLives; }
            set {
                if (value < 0 || value > 10) throw new ArgumentException("Lives must be between 0 and 10", nameof(value));
                _dataSave.NumberOfLives = value;
            }
        }

        public byte NumberOfCredits
        {
            get { return _dataSave.NumberOfCredits; }
            set
            {
                if (value < 0) throw new ArgumentException("Credits must be above 0", nameof(value));
                _dataSave.NumberOfCredits = value;
            }
        }

        public bool MiniGameIsValidated(byte miniGameId)
        {
     
            if (miniGameId <= 0 || miniGameId > 8) throw new ArgumentException("Mini-game ID must be between 1 and 8", nameof(miniGameId));
            int mask = 1 << 8 - miniGameId;
            return (_dataSave.ValidatedMiniGames & mask) != 0;
        }

        public void ValidateMiniGame(byte miniGameId)
        {
            if (miniGameId <= 0 || miniGameId > 8) throw new ArgumentException("Mini-game ID must be between 1 and 8", nameof(miniGameId));
            int mask = 1 << 8 - miniGameId;
            _dataSave.ValidatedMiniGames |= (byte)mask;
        }

        public bool TrapsNpcWereTalkedTo(byte NpcId)
        {
            if (NpcId <= 0 || NpcId > 8) throw new ArgumentException("NPC ID must be between 1 and 8", nameof(NpcId));
            int mask = 1 << 8 - NpcId;
            return (_dataSave.TrapsNpcTalkedTo & mask) != 0;
        }

        public void TrapsNpcTalkTo(byte NpcId)
        {
            if (NpcId <= 0 || NpcId > 8) throw new ArgumentException("NPC ID must be between 1 and 8", nameof(NpcId));
            int mask = 1 << 8 - NpcId;
            _dataSave.TrapsNpcTalkedTo |= (byte)mask;
        }

        public DataSave(string path)
        {
            Clear();
            _path = path;

            Directory.CreateDirectory(_path);

            if (File.Exists(_path + @"\datasave.oct"))
            {
                Load("datasave");
            }
        }

        public void Clear()
        {
            _dataSave = new DataSaveStruct { Name = "Joueur", District = 1, ValidatedMiniGames = 0, NumberOfLives = 3, NumberOfCredits = 0, TrapsNpcTalkedTo = 0 };
        }

        public void Load(string datasaveSlug)
        {
            using (var file = File.OpenRead(_path + @"\" + datasaveSlug + ".oct"))
            {
                var reader = new BinaryFormatter();
                _dataSave = (DataSaveStruct)reader.Deserialize(file);
            }
        }

        public void Save(bool current = true)
        {
            string slug = _GenerateSlug(_dataSave.Name);
            string path;
            if (current)
            {
                path = _path + @"\datasave.oct";
            }
            else
            {
                path = _path + @"\" + slug + ".oct";
            }

            using (var file = File.OpenWrite(path))
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
                slug = slug.Substring(0, slug.Length - 4);
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
