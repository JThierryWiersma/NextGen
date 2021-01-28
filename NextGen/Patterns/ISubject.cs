using System;
using System.Collections.Generic;
using System.Text;
using Generator.Exceptions;
using Generator.Statements;

namespace Generator.ObserverPattern
{
    public interface ISubject
    {
        void AddObserver(IObserver o);
        void RemoveObserver(IObserver o);
        void Notify();
        void Notify(string msg, NotificationType t, SourceCodeContext scc);
        void Notify(string firstpart, string linkeditem, string lastpart, string filename, int linenr);
    }
}
