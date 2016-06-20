using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace OrthoCite.Tests
{
    [TestClass]
    public class DataSaveTest
    {
        const string DATASAVES_PATH = @".\datasaves";


        DataSave _dataSave;

        [TestInitialize()]
        public void Initialize()
        {
            _dataSave = new DataSave(DATASAVES_PATH);
        }

        [TestCleanup()]
        public void Cleanup()
        {
            Directory.Delete(DATASAVES_PATH, true);
        }


        [TestMethod]
        public void DataSave_Ctor_should_create_subfolder()
        {
            Assert.IsTrue(Directory.Exists(DATASAVES_PATH));
        }

        [TestMethod]
        public void DataSave_Save_and_Load_should_save_and_restore_a_datasave()
        {
            _dataSave.Name = "Marvin";
            _dataSave.District = 4;
            _dataSave.NumberOfLives = 2;
            _dataSave.ValidateMiniGame(DataSaveMiniGame.DOORGAME);
            _dataSave.ValidateMiniGame(DataSaveMiniGame.PLATFORMER);
            _dataSave.TrapsNpcTalkTo(1);
            _dataSave.TrapsNpcTalkTo(5);

            _dataSave.Save(false);
            Assert.IsTrue(File.Exists(DATASAVES_PATH + @"\marvin.oct"));

            _dataSave.Load("marvin");
            Assert.AreEqual(_dataSave.Name, "Marvin");
            Assert.AreEqual(_dataSave.District, 4);
            Assert.AreEqual(_dataSave.NumberOfLives, 2);
            Assert.IsTrue(_dataSave.MiniGameIsValidated(DataSaveMiniGame.DOORGAME));
            Assert.IsTrue(_dataSave.MiniGameIsValidated(DataSaveMiniGame.PLATFORMER));
            Assert.IsFalse(_dataSave.MiniGameIsValidated(DataSaveMiniGame.REARRANGER));
            Assert.IsFalse(_dataSave.MiniGameIsValidated(DataSaveMiniGame.BOSS));
            Assert.IsTrue(_dataSave.TrapsNpcWereTalkedTo(1));
            Assert.IsFalse(_dataSave.TrapsNpcWereTalkedTo(2));
            Assert.IsFalse(_dataSave.TrapsNpcWereTalkedTo(3));
            Assert.IsFalse(_dataSave.TrapsNpcWereTalkedTo(4));
            Assert.IsTrue(_dataSave.TrapsNpcWereTalkedTo(5));
            Assert.IsFalse(_dataSave.TrapsNpcWereTalkedTo(6));
            Assert.IsFalse(_dataSave.TrapsNpcWereTalkedTo(7));
            Assert.IsFalse(_dataSave.TrapsNpcWereTalkedTo(8));
        }

        [TestMethod]
        public void DataSave_List_should_list_all_datasaves()
        {
            _dataSave.Name = "Bélican";
            _dataSave.Save(false);
            Assert.IsTrue(File.Exists(DATASAVES_PATH + @"\belican.oct"));
            _dataSave.Name = "Nom à jouer";
            _dataSave.Save(false);
            Assert.IsTrue(File.Exists(DATASAVES_PATH + @"\nom-a-jouer.oct"));

            var datasaves = _dataSave.List();
            Assert.AreEqual(datasaves.Count, 2);
            Assert.AreEqual(datasaves["belican"], "Bélican");
            Assert.AreEqual(datasaves["nom-a-jouer"], "Nom à jouer");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DataSave_District_should_fail_when_invalid_district()
        {
            _dataSave.District = 0;
        }
    }
}
