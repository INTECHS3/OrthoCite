using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrthoCite.Entities;

namespace OrthoCite.Tests
{
    [TestClass]
    public class DataSaveTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DataSave_should_fail()
        {
            throw new ArgumentException();
        }
    }
}
