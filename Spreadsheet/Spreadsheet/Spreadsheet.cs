// Skeleton written by Joe Zachary for CS 3500, January 2017

// The rest written by Yuntong Lu (u1060544), February 23 2018
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
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

        private Regex isvalName;
        // see in the dependency graph
        private DependencyGraph dependency;

        // when the first constructor used
        // ignore the new regex
        private int constructorUse;

        public override bool Changed { get; protected set; }


        /// <summary>
        /// the spreadsheet will takes no peremeter 
        /// </summary>
        public Spreadsheet()
        {
            sheet = new Dictionary<string, cell>();
            dependency = new DependencyGraph();
            constructorUse = 1;
        }


        /// <summary>
        /// it will take a new isVaild check if match the requirement
        /// </summary>
        /// <param name="isValid"></param>
        public Spreadsheet(Regex isValid)
        {
            isvalName = isValid;
            sheet = new Dictionary<string, cell>();
            dependency = new DependencyGraph();
        }


        /// <summary>
        /// read the source field and fill the spread sheet in the field
        /// </summary>
        /// <param name="source"></param>
        /// <param name="newIsValid"></param>
        public Spreadsheet(TextReader source, Regex newIsValid)
        {
            isvalName = newIsValid;
            Regex oldIsvail = null;
            sheet = new Dictionary<string, cell>();
            dependency = new DependencyGraph();


            // Create an XmlSchemaSet object.
            XmlSchemaSet sc = new XmlSchemaSet();

            // NOTE: To read states3.xsd this way, it must be stored in the same folder with the
            // executable.  To arrange this, I set the "Copy to Output Directory" propery of states3.xsd to
            // "Copy If Newer", which will copy states3.xsd as part of each build (if it has changed
            // since the last build).
            sc.Add(null, "Spreadsheet.xsd");

            // Configure validation.
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.Schemas = sc;
            settings.ValidationEventHandler += ValidationCallback;

            // try to read the field
            try
            {
                // begin to read
                using (XmlReader reader = XmlReader.Create(source, settings))
                {
                    // in the each token, 
                    // first check if it is vaild
                    // then save them into the sheet once it is vaild
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            switch (reader.Name)
                            {
                                case "spreadsheet":
                                    try
                                    {
                                        oldIsvail = new Regex(reader["IsValid"]);
                                    }
                                    catch
                                    {
                                        throw new SpreadsheetReadException("wrong regular expression");
                                    }
                                    break;
                                // checkall the requirements
                                case "cell":
                                    try
                                    {
                                        string name = reader["name"];

                                        if (!oldIsvail.IsMatch(name))
                                            throw new SpreadsheetReadException("the name is ilegal");
                                        if (!isvalName.IsMatch(name))
                                            throw new SpreadsheetVersionException("the name is ilegal");
                                        if (sheet.ContainsKey(name))
                                            throw new SpreadsheetReadException("the name is duplicate");

                                        SetContentsOfCell(name, reader["contents"]);

                                    }
                                    catch
                                    {
                                        throw new SpreadsheetReadException("can not read name or contents");
                                    }
                                    break;
                            }
                        }
                    }
                }
            }

            // throw exception once it doen't read
            catch
            {
                throw new IOException();
            }


        }

        /// <summary>
        ///  the cell class which contains the name of the cell and the
        ///  content of the cell
        /// </summary>
        private class cell
        {

            public cell(string _name, object _content, object _value)
            {
                name = _name;
                content = _content;
                value = _value;
            }

            /// <summary>
            /// thos two method can get the value and update the value
            /// </summary>
            public string name { get; }
            public object content { get; set; }
            public object value { get; set; }
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
            if (name == null || !legalName.IsMatch(name) || (constructorUse != 1 && !isvalName.IsMatch(name)))
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
        protected override ISet<string> SetCellContents(string name, double number)
        {
            // this hash set will be return later
            HashSet<string> relatedCells = new HashSet<string>();

            // check the name and number, it will throw some exception 
            if (number == null)
                throw new ArgumentNullException();
            if (name == null || !legalName.IsMatch(name) || (constructorUse != 1 && !isvalName.IsMatch(name)))
                throw new InvalidNameException();

            // if the name is already exist in sheet
            if (sheet.ContainsKey(name))
            {
                // delete it can then add new one
                sheet.Remove(name);
                cell mycell = new cell(name, number, number);
                sheet.Add(name, mycell);
            }

            // if it isn't in the sheet
            else
            {
                // add them
                cell mycell = new cell(name, number, number);
                sheet.Add(name, mycell);
            }

            // check any related cells and get the name
            foreach (string cellName in GetCellsToRecalculate(name))
                relatedCells.Add(cellName);

            Changed = true;
            RecalculteCells();
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
        protected override ISet<string> SetCellContents(string name, string text)
        {
            // this hash set will be return later
            HashSet<string> relatedCells = new HashSet<string>();

            // check the name and number, it will throw some exception 
            if (text == null)
                throw new ArgumentNullException();
            if (name == null || !legalName.IsMatch(name) || (constructorUse != 1 && !isvalName.IsMatch(name)))
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
                cell mycell = new cell(name, text, text);
                sheet.Add(name, mycell);
            }

            // if the name is not exsit in here
            else
            {
                // check and add
                if (text.Equals(""))
                    return relatedCells;

                cell mycell = new cell(name, text, text);
                sheet.Add(name, mycell);
            }

            // get the return value
            foreach (string cellName in GetCellsToRecalculate(name))
                relatedCells.Add(cellName);

            Changed = true;
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
        protected override ISet<string> SetCellContents(string name, Formula formula)
        {
            // checkif everything is legal
            if (formula.Equals(null))
                throw new ArgumentNullException();
            if (name == null || !legalName.IsMatch(name) || (constructorUse != 1 && !isvalName.IsMatch(name)))
                throw new InvalidNameException();

            // if the sheet have the name already
            if (sheet.ContainsKey(name))
            {
                // delete it and add it
                sheet.Remove(name);
                cell mycell = new cell(name, formula, new FormulaError());
                RecalculteCells();
                sheet.Add(name, mycell);

                // if in the formula contains some cell's name
                // get the related names
                foreach (string cellsName in formula.GetVariables())
                {
                    if (legalName.IsMatch(cellsName))
                        dependency.AddDependency(name, cellsName);
                }
            }

            // if the sheet doesn't exsit the cell
            else
            {
                // add the cell
                cell mycell = new cell(name, formula, new FormulaError());
                RecalculteCells();
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

            Changed = true;
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


        /// <summary>
        /// save the sheet in a field with some squences
        /// </summary>
        /// <param name="dest"></param>
        public override void Save(TextWriter dest)
        {
            // create a save field
            using (XmlWriter writer = XmlWriter.Create(dest))
            {
                // save the title part
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                // save the isvaild
                if (isvalName != null)
                    writer.WriteAttributeString("IsValid", isvalName.ToString());
                else
                    writer.WriteAttributeString("IsValid", legalName.ToString());

                // save the body part
                foreach (string cellName in GetNamesOfAllNonemptyCells())
                {
                    // save the name
                    writer.WriteStartElement("cell");
                    writer.WriteAttributeString("name", cellName);

                    // save the content depend on the formula
                    if (sheet[cellName].content is Formula)
                        writer.WriteAttributeString("contents", "=" + sheet[cellName].content.ToString());
                    else
                        writer.WriteAttributeString("contents", sheet[cellName].content.ToString());
                    writer.WriteEndElement();
                }

                // end the save field
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }



        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        ///
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a FormulaError.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override object GetCellValue(string name)
        {
            // check if the name is legal
            if (name == null || !legalName.IsMatch(name) || (constructorUse != 1 && !isvalName.IsMatch(name)))
                throw new InvalidNameException();

            // if no name in it, just return nothing
            if (!sheet.ContainsKey(name))
                return "";

            // give the value
            return sheet[name].value;

        }


        /// <summary>
        /// give each element in formula a value
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private double lookup(String name)
        {
            // find the element
            if (sheet.ContainsKey(name))
            {
                // give the value if it can
                double cellValue;
                bool isDouble = double.TryParse(sheet[name].content.ToString(), out cellValue);
                if (isDouble)
                    return cellValue;
                // if the value is string, throw exception
                else
                    throw new UndefinedVariableException("undefined value");
            }

            // no value
            else
                throw new UndefinedVariableException("undefined value");
        }



        /// <summary>
        /// once the new cell is determained, the new value should be used
        /// </summary>
        private void RecalculteCells()
        {
            // find all the cells need to recalculate
            foreach (string cell in sheet.Keys)
            {
                if (sheet[cell].content is Formula)
                {
                    // recalculte them, if can
                    // give the value
                    // if cannot, give error
                    Formula f = new Formula(sheet[cell].content.ToString());
                     try
                     {                    
                        sheet[cell].value = f.Evaluate(lookup);
                     }
                     catch
                     {   
                         sheet[cell].value = new FormulaError();
                     }
                }

            }
        }



        /// <summary>
        /// If content is null, throws an ArgumentNullException.
        ///
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        ///
        /// Otherwise, if content parses as a double, the contents of the named
        /// cell becomes that double.
        ///
        /// Otherwise, if content begins with the character '=', an attempt is made
        /// to parse the remainder of content into a Formula f using the Formula
        /// constructor with s => s.ToUpper() as the normalizer and a validator that
        /// checks that s is a valid cell name as defined in the AbstractSpreadsheet
        /// class comment.  There are then three possibilities:
        ///
        ///   (1) If the remainder of content cannot be parsed into a Formula, a
        ///       Formulas.FormulaFormatException is thrown.
        ///
        ///   (2) Otherwise, if changing the contents of the named cell to be f
        ///       would cause a circular dependency, a CircularException is thrown.
        ///
        ///   (3) Otherwise, the contents of the named cell becomes f.
        ///
        /// Otherwise, the contents of the named cell becomes content.
        ///
        /// If an exception is not thrown, the method returns a set consisting of
        /// name plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell.
        ///
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// <summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public override ISet<string> SetContentsOfCell(string name, string content)
        {
            // check if all the thing is legal
            if (content == null)
                throw new ArgumentNullException();

            if (name == null || !legalName.IsMatch(name) || (constructorUse != 1 && !isvalName.IsMatch(name)))
                throw new InvalidNameException();

            // if teh content is double
            // set the double
            double doubleContent;
            bool isDouble = double.TryParse(content, out doubleContent);
            if (isDouble)
            {
                return SetCellContents(name, doubleContent);
            }

            // if the content is formula
            else if (content.StartsWith("="))
            {
                string formulaContent = content.Substring(1, content.Length - 1);

                // try to save it with "="
                try
                {
                    Formula formula = new Formula(formulaContent, s => s.ToUpper(), s => true);
                    return SetCellContents(name, formula);
                }
                catch
                {
                    GetCellsToRecalculate(name);
                    throw new Formulas.FormulaFormatException("can not convert to formula");
                }

            }

            // if the content is string, go here
            else
            {
                return SetCellContents(name, content);
            }

        }



        // Display any validation errors.
        private static void ValidationCallback(object sender, ValidationEventArgs e)
        {
            throw new SpreadsheetReadException("can not can not not not read this file!!!");
        }
    }
}
