using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using KoobecaFeedController.DAL.Adapters;
using KoobecaFeedController.DAL.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KoobecaFeedController.DAL.Tests {
    [TestClass]
    public class UnitTest1 {
        [TestMethod]
        public void TestMethod1() {
            var rc = ActivityActions.Get(306, 20);
            var str = Encoding.UTF8.GetString(rc[0].body);
            var ser = new XmlSerializer(typeof(ActivityAction));
            var sw = new StringWriter();
            ser.Serialize(sw, rc[0]);
            var result = sw.ToString();
            Console.WriteLine("{0}", result);
            Assert.IsNotNull(rc);
        }
        [TestMethod]
        public void TestMethod2() {
            var rc = UserMemberships.Get(1);
            Assert.IsNotNull(rc);
        }
        [TestMethod]
        public void TestMethod3() {
            ////var rc = ActivityActions.Get(306, 20, getRelatedActions: true);
            //var rc = ActivityActions.GetRelatedActions(10148);
            //Assert.IsNotNull(rc);
        }
        [TestMethod]
        public void TestMethod4() {
            var rc = CoreSettings.GetAll();
            Assert.IsNotNull(rc);
        }
        [TestMethod]
        public void TestMethod5() {
            var rc = CoreSettings.Get("aaf.allowed.buysell.content.0");
            Assert.AreEqual(rc, "user");
        }
        [TestMethod]
        public void TestMethod6() {
            // Remove test setting before test start
            CoreSettings.Clear("zumzumzum");

            // Get test setting, expect null because it doesn't exist
            var rc = CoreSettings.Get("zumzumzum");
            Assert.IsNull(rc);

            // Get test setting with default value, expect that value
            rc = CoreSettings.Get("zumzumzum", "888");
            Assert.AreEqual(rc, "888");

            // Set test setting to value "777" without allowing add new,
            // Expect 0 affected rows
            var affectedRows = CoreSettings.Set("zumzumzum", "777");
            Assert.AreEqual(affectedRows, 0);

            // Get the test setting, expect null because we didn't actually add it
            rc = CoreSettings.Get("zumzumzum");
            Assert.IsNull(rc);

            // Set the test setting, this time allow adding a new record
            // Expect affected rows to be 1 (new row added)
            affectedRows = CoreSettings.Set("zumzumzum", "777", true);
            Assert.AreEqual(affectedRows, 1);

            // Get the test setting, expect value "777" because we did add the row
            rc = CoreSettings.Get("zumzumzum");
            Assert.AreEqual(rc, "777");

            // Remove the test setting, expect affected rows to be 1 because we know
            // it exists
            affectedRows = CoreSettings.Clear("zumzumzum");
            Assert.AreEqual(affectedRows, 1);
        }
    }
}
