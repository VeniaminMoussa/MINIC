using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MINIC
{
    public abstract class ASTVisitableElement : ASTElement
    {
        protected ASTVisitableElement(int context) : base(context) { }

        public abstract T Accept<T>(ASTBaseVisitor<T> visitor);
    }
    public abstract class ASTElement
    {
        protected int m_serial;
        protected static int ms_serialCounter = 0;
        protected List<ASTElement> m_parent = new List<ASTElement>();
        protected string m_GraphVizGraphVizName;
        protected string m_name;
        protected List<ASTElement>[] m_children = null; 

        protected ASTElement(int context)
        {
            m_serial = ms_serialCounter++;
            m_GraphVizGraphVizName = GenerateNodeName();
            if (context != 0)
            {
                m_children = new List<ASTElement>[context];
                for (int i = 0; i < context; i++)
                {
                    m_children[i] = new List<ASTElement>();
                }
            }
        }

        public List<ASTElement> M_Parent
        {
            get => m_parent;
            set => m_parent = value;
        }

        public string M_GraphVizName
        {
            get => m_GraphVizGraphVizName;
            set => m_GraphVizGraphVizName = value;
        }

        public string M_Name
        {
            get => m_name;
            set => m_name = value;
        }

        public int GetChildrenContextNumber(int context)
        {
            return m_children[context].Count;
        }

        public IEnumerable<ASTElement> GetChildrenContext(int context)
        {
            foreach (ASTElement element in m_children[context])
            {
                yield return element;
            }
        }

        public IEnumerable<ASTElement> GetChildren()
        {
            for (int i=0 ; i < m_children.Length ; i++)
            {
                foreach (ASTElement element in m_children[i])
                {
                    yield return element;
                }
            }
        }

        public void AddChild(ASTElement child, int contextIndex)
        {
            child.m_parent.Add(this);

            m_children[contextIndex].Add(child);
        }

        public ASTElement GetChild(int context, int index)
        {
            return m_children[context][index];
        }

        public virtual string GenerateNodeName()
        {
            return "_" + m_serial;
        }
    }
}
