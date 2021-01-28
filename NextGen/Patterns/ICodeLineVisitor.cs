using System;
using System.Collections.Generic;
using System.Text;
using Generator.Statements;

namespace Generator.VisitorPattern
{
    public interface ICodeLineVisitor
    {
        void Visit(CodeLine l);

        void Visit(Comment l);
        
        void Visit(BreakStatement l);
        void Visit(ReturnStatement l);
        void Visit(ExitStatement l);
        void Visit(VarDeclaration l);
        void Visit(GlobalVarDeclaration l);
        void Visit(InfoMessage l);
        void Visit(ErrorMessage l);

        void Visit(IfStatement l);
        void Visit(ElseIfStatement l);
        void Visit(ElseStatement l);
        void Visit(EndIfStatement l);

        void Visit(DoStatement l);
        void Visit(WhileStatement l);
        void Visit(LoopStatement l);

        void Visit(Include l);
        void Visit(FunctionDeclaration l);
        void Visit(EndFunctionDeclaration l);
        void Visit(Assignment l);
        void Visit(VoidFunctionCall l);

    }
}
