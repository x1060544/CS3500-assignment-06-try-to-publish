using SS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Formulas;
using System;
using System.Collections.Generic;

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
            testSheet.SetCellContents("A1", null);
        }

        /// <summary>
        /// test when the name is null
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestException3a()
        {
            AbstractSpreadsheet testSheet = new Spreadsheet();
            testSheet.SetCellContents(null, 5);
        }

        /// <summary>
        /// test whe the name is null in text
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestException3b()
        {
            AbstractSpreadsheet testSheet = new Spreadsheet();
            testSheet.SetCellContents(null, "a");
        }

        /// <summary>
        /// test name is null in formula
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestException3c()
        {
            AbstractSpreadsheet testSheet = new Spreadsheet();
            testSheet.SetCellContents(null, new Formula("2 * A1"));
        }

        /// <summary>
        /// test when the formula have cicular
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void TestException4()
        {
            AbstractSpreadsheet testSheet = new Spreadsheet();
            testSheet.SetCellContents("A1", new Formula("A2+A3"));
            testSheet.SetCellContents("A2", new Formula("A1+A3"));
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
            testSheet.SetCellContents("A01", 1);
        }

        /// <summary>
        /// test with the legal name
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestException7()
        {
            AbstractSpreadsheet testSheet = new Spreadsheet();
            testSheet.SetCellContents("1", "1");
        }

        /// <summary>
        /// test with the legal formula
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestException8()
        {
            AbstractSpreadsheet testSheet = new Spreadsheet();
            testSheet.SetCellContents("AZ", new Formula("1"));
        }

        /// <summary>
        /// test getcellcontent method
        /// </summary>
        [TestMethod()]
        public void getTest()
        {
            AbstractSpreadsheet testSheet = new Spreadsheet();
            testSheet.SetCellContents("A1", 1);
            Assert.AreEqual(1.0, (double)testSheet.GetCellContents("A1"));
            

            testSheet.SetCellContents("A1", "a");
            Assert.AreEqual("a", testSheet.GetCellContents("A1"));
    

            testSheet.SetCellContents("A1", new Formula("A2"));
            Assert.AreEqual(new Formula("A2"), testSheet.GetCellContents("A1"));
        }

        /// <summary>
        /// test whenthere is an empty cell
        /// </summary>
        [TestMethod()]
        public void noneNullTest1()
        {
            AbstractSpreadsheet testSheet = new Spreadsheet();
            testSheet.SetCellContents("C2", "");
            Assert.IsFalse(testSheet.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext());
        }

        /// <summary>
        /// test if the sheet added the cells correctly
        /// </summary>
        [TestMethod()]
        public void noneNullTest2()
        {
            AbstractSpreadsheet testSheet = new Spreadsheet();
            Assert.AreEqual(1, testSheet.SetCellContents("B1", 1).Count);
        }


    }
}