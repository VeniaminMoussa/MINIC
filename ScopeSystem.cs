using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MINIC
{
    public class ScopeSystem : MINICBaseVisitor<int>
    {
        public Scope m_Root;//variables namespace (tree)
        public Scope m_FunctionsNameSpace;//functionsName Namespace

        protected int m_nestingLevel = 0;
        private HashSet<string> m_globalVarSymbolTable = new HashSet<string>();
        private Dictionary<String, Dictionary<String, String>> m_localSymbolFunctionTable = new Dictionary<string, Dictionary<String, String>>();
        private HashSet<string> m_FunctionsSymbolTable = new HashSet<string>();

        protected int M_NestingLevel
        {
            get => m_nestingLevel;
            set => m_nestingLevel = value;
        }

        public void DeclareGlobalVariable(string varname)
        {
            CodeContainer rep;

            if (!m_globalVarSymbolTable.Contains(varname))
            {
                m_globalVarSymbolTable.Add(varname);
            }
        }

        public Boolean DeclareLocalFunvtionVariable(string function, string varname)
        {
            if (m_localSymbolFunctionTable.ContainsKey(varname))
            {
                if (m_localSymbolFunctionTable[varname].ContainsKey(function))
                {
                    return false;
                }
                else
                {
                    m_localSymbolFunctionTable[varname].Add(function, varname);
                    return true;
                }
            }
            else
            {
                m_localSymbolFunctionTable.Add(varname, new Dictionary<string, string>());
                m_localSymbolFunctionTable[varname].Add(function, varname);
                return true;
            }
        }

        public Boolean hasGlobalVariable(string varname)
        {
            if (m_globalVarSymbolTable.Contains(varname))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean hasLocalVariable(string function, string varname)
        {
            if (m_localSymbolFunctionTable.ContainsKey(varname))
            {
                if (m_localSymbolFunctionTable[varname].ContainsKey(function))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                m_localSymbolFunctionTable.Add(varname, new Dictionary<string, string>());
                return false;
            }
        }

        public void DeclareFunction(string funname)
        {
            if (!m_FunctionsSymbolTable.Contains(funname))
            {
                m_globalVarSymbolTable.Add(funname);
            }
        }

        public virtual void EnterScope()
        {
            m_nestingLevel++;
        }

        public virtual void LeaveScope()
        {
            if (m_nestingLevel > 0)
            {
                m_nestingLevel--;
            }
            else
            {
                throw new Exception("Non-matched nesting");
            }
        }


    }

    public class Scope : ASTElement
    {
        private Dictionary<string, MINICASTElement> m_SymbolTable;

        public Scope(int context) : base(context)
        {

        }


    }
}