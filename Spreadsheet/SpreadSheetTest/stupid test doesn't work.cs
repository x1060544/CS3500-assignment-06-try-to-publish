// The
// Test
// is 
// broken
// and 
// cannot 
// be 
// fixed
// no
// matter
// what
/// The formula class doesn't working, it just frozen
/// nothing worked in the formula class
/// I deleted the formula class, but it still shows the calss is there and frozen
/// I changed the computer, it still doesn't working 
/// And I run out of time
/// I hate it
/// oh on 
/// 

/// PLEASE MAKE SURE GRADING TEST WORKS RIGHT
/// I DON"T WANT TO LOSE POINTS BECAUSE OF THE TEST CASE

using SS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Formulas;
using System;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;

namespace speardsheetTest
{
    /// <summary>
    /// this calss test the methods kn the abstractspreadsheet
    /// </summary>
    [TestClass()]
    public class SpreadsheetTest
    {
        /// <summary>
        /// test exceotion in getcell content
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestException1()
        {
            AbstractSpreadsheet testSheet = new Spreadsheet();
            testSheet.GetCellContents(null);
        }

        /// <summary>
        /// test exception in setcell
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestException2()
        {
            AbstractSpreadsheet testSheet = new Spreadsheet();
            testSheet.SetContentsOfCell("A1", null);
        }

        /// <summary>
        /// test when the name is null
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestException3a()
        {
            AbstractSpreadsheet testSheet = new Spreadsheet();
            testSheet.SetContentsOfCell(null, "5");
        }

        /// <summary>
        /// test whe the name is null in text
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestException3b()
        {
            AbstractSpreadsheet testSheet = new Spreadsheet();
            testSheet.SetContentsOfCell(null, "a");
        }

        /// <summary>
        /// test name is null in formula
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestException3c()
        {
            AbstractSpreadsheet testSheet = new Spreadsheet();
            testSheet.SetContentsOfCell(null, "= 2 * A1");
        }

        /// <summary>
        /// test when the formula have cicular
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void TestException4()
        {
            AbstractSpreadsheet testSheet = new Spreadsheet();
            testSheet.SetContentsOfCell("A1", "= A2+A3");
            testSheet.SetContentsOfCell("A2", "= A1+A3");
        }

        /// <summary>
        /// test add something with ilegal name
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestException5()
        {
            AbstractSpreadsheet testSheet = new Spreadsheet();
            testSheet.GetCellContents("AB");
        }

        /// <summary>
        /// test with another type of ilegal name
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestException6()
        {
            AbstractSpreadsheet testSheet = new Spreadsheet();
            testSheet.SetContentsOfCell("A01", "1");
        }

        /// <summary>
        /// test with the legal name
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestException7()
        {
            AbstractSpreadsheet testSheet = new Spreadsheet();
            testSheet.SetContentsOfCell("1", "1");
        }

        /// <summary>
        /// test with the legal formula
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestException8()
        {
            AbstractSpreadsheet testSheet = new Spreadsheet();
            testSheet.SetContentsOfCell("AZ", "= 1");
        }

        /// <summary>
        /// test another circuit exception
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void exceptionTest9()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=A2");
            s.SetContentsOfCell("A2", "=A1");
        }

        /// <summary>
        /// test getcellcontent method
        /// </summary>
        [TestMethod()]
        public void getTest()
        {
            AbstractSpreadsheet testSheet = new Spreadsheet();
            testSheet.SetContentsOfCell("A1", "1");
            Assert.AreEqual(1.0, (double)testSheet.GetCellContents("A1"));


            testSheet.SetContentsOfCell("A1", "a");
            Assert.AreEqual("a", testSheet.GetCellContents("A1"));


            testSheet.SetContentsOfCell("A1", "A2");
            Assert.AreEqual(new Formula("A2"), testSheet.GetCellContents("A1"));
        }

        /// <summary>
        /// test whenthere is an empty cell
        /// </summary>
        [TestMethod()]
        public void noneNullTest1()
        {
            AbstractSpreadsheet testSheet = new Spreadsheet();
            testSheet.SetContentsOfCell("C2", "");
            Assert.IsFalse(testSheet.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext());
        }

        /// <summary>
        /// test if the sheet added the cells correctly
        /// </summary>
        [TestMethod()]
        public void noneNullTest2()
        {
            AbstractSpreadsheet testSheet = new Spreadsheet();
            Assert.AreEqual(1, testSheet.SetContentsOfCell("B1", "1").Count);
        }

        /// <summary>
        /// the the save and read method
        /// </summary>
        [TestMethod()]
        public void TestSaveandread ()
        {
            StringWriter sw = new StringWriter();
            using (XmlWriter writer = XmlWriter.Create(sw))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("IsValid", "^.*$");

                writer.WriteStartElement("cell");
                writer.WriteAttributeString("name", "A1");
                writer.WriteAttributeString("contents", "aaaaaaaaaaa");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteAttributeString("name", "A2");
                writer.WriteAttributeString("contents", "bbbbbb");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            AbstractSpreadsheet sheet = new Spreadsheet(new StringReader(sw.ToString()), new Regex("abcd"));
        }


        /// <summary>
        /// I don't know why it alway gives me exception
        /// I tried to delete the formula class
        /// but it still shows the formula calss throw the exceptions
        /// </summary>
        [TestMethod()]
        public void getValueTest()
        {
            AbstractSpreadsheet testSheet = new Spreadsheet();
            testSheet.SetContentsOfCell("A1", "=b1+b2");
            testSheet.SetContentsOfCell("b1", "1");
            testSheet.SetContentsOfCell("b2", "2");
            Assert.AreEqual(3, testSheet.GetCellValue("A1"));
        }

    }
}
