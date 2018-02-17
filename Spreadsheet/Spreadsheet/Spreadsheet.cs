// Skeleton written by Joe Zachary for CS 3500, January 2017

// The rest written by Yuntong Lu (u1060544), February 16 2018
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Dependencies;
using Formulas;

namespace SS
{
    /// <summary>
    /// This is the calss with can be treat as the speardsheet with lots of cells
    /// since the calss is called, the cells, relations between cells are all 
    /// called.
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        // one dictionary which will hold all the cells called sheet
        private Dictionary<string, cell> sheet;
        // check if the name matches letter+number
        private Regex legalName = new Regex(@"^[a-zA-Z]+[1-9][0-9]*$");
        // see in the dependency graph
        private DependencyGraph dependency;

        /// <summary>
        /// the spreadsheet will takes no peremeter 
        /// </summary>
        public Spreadsheet ()
        {
            sheet = new Dictionary<string, cell>();
            dependency = new DependencyGraph();
        }

        /// <summary>
        ///  the cell class which contains the name of the cell and the
        ///  content of the cell
        /// </summary>
        private class cell
        { 
         
            public cell (string _name, object _content)
            {
                name = _name;
                content = _content;
            }

            /// <summary>
            /// thos two method can get the value and update the value
            /// </summary>
            public string name { get; }
            public object content { get; set; }
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// </summary>
        public override object GetCellContents(string name)
        {
            // check if the input is legal 
            if (name == null || !legalName.IsMatch(name))
                throw new InvalidNameException();

            // find the content and return them
            if (sheet.ContainsKey(name))
                return sheet[name].content;

            return "";           
        }


        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            // go through all the cells in the sheet,
            // once it contains is ""
            // ignore it
            HashSet<string> result = new HashSet<string>();
            foreach (string name in sheet.Keys)
            {
                if (!sheet[name].Equals(""))
                    result.Add(name);
            }

            return result;
        }


        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes number.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        public override ISet<string> SetCellContents(string name, double number)
        {
            // this hash set will be return later
            HashSet<string> relatedCells = new HashSet<string>();

            // check the name and number, it will throw some exception 
            if (number == null)           
                throw new ArgumentNullException();
            if (name == null || !legalName.IsMatch(name))
                throw new InvalidNameException();

            // if the name is already exist in sheet
            if (sheet.ContainsKey(name))
            {
                // delete it can then add new one
                sheet.Remove(name);
                cell mycell = new cell(name, number);
                sheet.Add(name, mycell);
            }

            // if it isn't in the sheet
            else
            {
                // add them
                cell mycell = new cell(name, number);
                sheet.Add(name, mycell);
            }

            // check any related cells and get the name
            foreach (string cellName in GetCellsToRecalculate(name))
                relatedCells.Add(cellName);

            return relatedCells;
        }


        /// <summary>
        /// If text is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes text.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        public override ISet<string> SetCellContents(string name, string text)
        {
            // this hash set will be return later
            HashSet<string> relatedCells = new HashSet<string>();

            // check the name and number, it will throw some exception 
            if (text == null)
                throw new ArgumentNullException();
            if (name == null || !legalName.IsMatch(name))
                throw new InvalidNameException();

            // if the name is already exist in sheet
            if (sheet.ContainsKey(name))
            {
                // delete the old one
                sheet.Remove(name);
                // check if the new one is legal
                if (text.Equals(""))
                    return relatedCells;
                // add the new one
                cell mycell = new cell(name, text);
                sheet.Add(name, mycell);
            }

            // if the name is not exsit in here
            else
            {
                // check and add
                if (text.Equals(""))
                    return relatedCells;

                cell mycell = new cell(name, text);
                sheet.Add(name, mycell);
            }

            // get the return value
            foreach (string cellName in GetCellsToRecalculate(name))            
                relatedCells.Add(cellName);


            return relatedCells;
        }


        /// <summary>
        /// Requires that all of the variables in formula are valid cell names.
        /// 
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException.
        /// 
        /// Otherwise, the contents of the named cell becomes formula.  The method returns a
        /// Set consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        public override ISet<string> SetCellContents(string name, Formula formula)
        {
            // checkif everything is legal
            if (formula.Equals(null))
                throw new ArgumentNullException();
            if (name == null || !legalName.IsMatch(name))
                throw new InvalidNameException();

            // if the sheet have the name already
            if (sheet.ContainsKey(name))
            {
                // delete it and add it
                sheet.Remove(name);
                cell mycell = new cell(name, formula);
                sheet.Add(name, mycell);

                // if in the formula contains some cell's name
                // get the related names
                foreach(string cellsName in formula.GetVariables())
                {
                    if (legalName.IsMatch(cellsName))
                        dependency.AddDependency(name, cellsName);
                }
            }

            // if the sheet doesn't exsit the cell
            else
            {
                // add the cell
                cell mycell = new cell(name, formula);
                sheet.Add(name, mycell);

                // get the dependency of the cells
                foreach (string cellsName in formula.GetVariables())
                {
                    if (legalName.IsMatch(cellsName))
                        dependency.AddDependency(name, cellsName);
                }
            }

            HashSet<string> relatedCells = new HashSet<string>();

            // save the dependency to the set and return it
            foreach (string cellName in GetCellsToRecalculate(name))
                relatedCells.Add(cellName);

            return relatedCells;
        }


        /// <summary>
        /// A convenience method for invoking the other version of GetCellsToRecalculate
        /// with a singleton set of names.  See the other version for details.
        /// </summary>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            return dependency.GetDependees(name);
        }

    }
}
