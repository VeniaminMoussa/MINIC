using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MINIC
{
    public class ScopeSystem
    {
        public Scope m_Root = null;//variables namespace (tree)

        protected Stack<Scope> m_parents = new Stack<Scope>();

        protected int m_nestingLevel = 0;

        public ScopeSystem()
        {
            m_parents.Push(m_Root);
        }

        public Scope GetScope()
        {
            return m_parents.Peek();
        }

        public virtual void EnterScope()
        {
            m_nestingLevel++;
            Scope newScope = new Scope(m_parents.Peek(),0);
            m_parents.Push(newScope);
        }

        public virtual void LeaveScope()
        {
            if (m_nestingLevel > 0)
            {
                m_nestingLevel--;
                m_parents.Pop();
            }
            else
            {
                throw new Exception("Non-matched nesting");
            }
        }


    }

    public class Scope : ASTElement
    {
        private Dictionary<string, MINICASTElement> m_SymbolTable = new Dictionary<string, MINICASTElement>();
        public Scope m_parent;
        public List<Scope>[] m_children = null;


        public Scope(Scope parent, int context) : base(context)
        {
            m_parent = parent;
        }

        public override IEnumerable<ASTElement> GetChildren()
        {
            return base.GetChildren();
        }

        public override void AddChild(ASTElement child, int contextIndex)
        {
            base.AddChild(child, contextIndex);
        }

        public override ASTElement GetChild(int context, int index)
        {
            return base.GetChild(context, index);
        }

        public MINICASTElement resolve(string varname)
        {
            if (hasVariable(varname))
            {
                return m_SymbolTable[varname];
            }
            else
            {
                if (getParentScope() != null)
                {
                    return getParentScope().resolve(varname);
                }
            }

            return null;
        }

        public Scope getParentScope()
        {
            return m_parent;
        }

        public void DeclareVariable(string varname)
        {
            if (!hasVariable(varname))
            {
                m_SymbolTable.Add(varname, new CIdentifier(varname));
            }
        }

        public Boolean hasVariable(string varname)
        {
            if (m_SymbolTable.ContainsKey(varname))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}