using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrthoCite.Entities;

namespace OrthoCite.Tests
{
    [TestClass]
    public class DialogBoxTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DialogBox_AddDialog_should_fail_when_delay_0()
        {
            RuntimeData runtimeData = new RuntimeData();
            DialogBox dialogbox = new DialogBox(runtimeData);

            dialogbox.AddDialog("delay 0", 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DialogBox_SetText_should_fail_when_text_is_empty()
        {
            RuntimeData runtimeData = new RuntimeData();
            DialogBox dialogbox = new DialogBox(runtimeData);

            dialogbox.SetText(string.Empty);
        }
    }
}
