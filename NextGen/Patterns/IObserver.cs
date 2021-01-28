using System;
using System.Collections.Generic;
using System.Text;
using Generator.Exceptions;
using Generator.Statements;

namespace Generator.ObserverPattern
{
    public interface IObserver
    {
        void ProcessUpdate(Object o);
        void ProcessUpdate(Object o, string msg, NotificationType t, SourceCodeContext scc);
        void ProcessUpdate(Object o, string firstpart, string linkeditem, string lastpart, string filename, int linenr);
    }

    public enum NotificationType
    {
        Info,
        Erreur
    }
}
