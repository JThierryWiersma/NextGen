using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using Generator.Exceptions;
using Generator.Expressions;
using Generator.Statements;

namespace Generator.CodeProcessors
{
    class Parser
    {
        public Parser()
        {
        }

        public List<CodeLine> Load(String filename)
        {
            TextReader                  r           = new StreamReader(filename);
            List<CodeLine>              result      = LoadSourceFromReader(filename, r);
            return result;
        }
        /// <summary>
        /// Laad het bestand uit de reader in. Gewoon de tekst lezen en omzetten in CodeLines.
        /// </summary>
        /// <param name="filename">Naam van de file die we nu doen</param>
        /// <param name="reader">De textlezer</param>
        /// <returns>De programmacode</returns>
        private List<CodeLine> LoadSourceFromReader(String filename, TextReader reader)
        {
            List<CodeLine> codelines = new List<CodeLine>();
            int linenr = 1;  //linenr update
            string s = reader.ReadLine();
            while (s != null)
            {
                codelines.Add(CodeLine.BuildCodeLine(filename, linenr, s));
                s = reader.ReadLine();
                linenr++;
            }
            reader.Close();
            return codelines;
        }

        

    }
}
