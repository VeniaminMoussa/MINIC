using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MINIC
{
    public class ScopeSystem
    {
        public Scope m_Root;//variables namespace (tree)
        public Scope m_FunctionsNameSpace;//functionsName Namespace

        //leaveScope method
        //enterScope method
        //declearVariable method

    }

    public class Scope : ASTElement
    {
        private Dictionary<string, MINICASTElement> m_SymbloTable;

        public Scope(int context) : base(context)
        {

        }


    }
}
