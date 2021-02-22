using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MINIC
{
    public enum CodeBlockType
    {
        CB_NA = -1,
        CB_FILE,
        CB_FUNCTIONDEFINITION,
        CB_WHILESTATEMENT,
        //..............
        CB_FORSTATEMENT,
        //..............
        CB_IFSTATEMENTS,
        CB_COMPOUNDSTATEMENT,
        CB_CODEREPOSITORY
    };


    public abstract class CEmmitableCodeContainer
    {
        private CodeBlockType m_nodeType;
        private int m_serialNumber;
        private static int m_serialNumberCounter = 0;
        private string m_nodeName;
        private CEmmitableCodeContainer m_parent;
        protected int m_nestingLevel = 0;

        protected CEmmitableCodeContainer M_Parent
        {
            get => m_parent;
        }

        public int M_SerialNumber
        {
            get => m_serialNumber;
        }

        public CodeBlockType MNodeType
        {
            get => m_nodeType;
        }

        public string M_NodeName
        {
            get => m_nodeName;
        }

        protected int M_NestingLevel
        {
            get => m_nestingLevel;
            set => m_nestingLevel = value;
        }

        protected CEmmitableCodeContainer(CodeBlockType nodeType, CEmmitableCodeContainer parent)
        {
            m_nodeType = nodeType;
            m_serialNumber = m_serialNumberCounter++;
            m_nodeName = m_nodeType + "_" + m_serialNumber;
            m_parent = parent;
            m_nestingLevel = parent?.M_NestingLevel ?? 0;
        }

        /// <summary>
        /// This method is specialized in concrete nodes and converts the composite
        /// structure of this node to a CodeContainer object having the text for this
        /// Code object
        /// </summary>
        /// <returns></returns>
        public abstract CodeContainer AssemblyCodeContainer();

        public abstract void AddCode(String code, int context = -1);
        public abstract void AddCode(CEmmitableCodeContainer code, int context = -1);
        public abstract void PrintStructure(StreamWriter m_ostream);
        public abstract string EmmitStdout();
        public abstract void EmmitToFile(StreamWriter f);

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

        public abstract void AddNewLine(int context = -1);
    }

    public abstract class CodeContainerComposite : CEmmitableCodeContainer
    {

        protected List<CEmmitableCodeContainer>[] m_repository;
        private static int m_clusterSerial = 0;

        protected CodeContainerComposite(CodeBlockType nodeType, int contexts, CEmmitableCodeContainer parent) : base(nodeType, parent)
        {
            m_repository = new List<CEmmitableCodeContainer>[contexts];
            for (int i = 0; i < contexts; i++)
            {
                m_repository[i] = new List<CEmmitableCodeContainer>();
            }

        }

        protected virtual CodeContainer AssemblyContext(int ct)
        {
            CodeContainer rep = new CodeContainer(CodeBlockType.CB_CODEREPOSITORY, this);
            for (int i = 0; i < m_repository[ct].Count; i++)
            {
                rep.AddCode(m_repository[ct][i].AssemblyCodeContainer());
            }

            return rep;
        }

        public override void AddCode(string code, int context)
        {
            CodeContainer container = new CodeContainer(CodeBlockType.CB_NA, this);
            container.AddCode(code, -1);
            m_repository[context].Add(container);
        }

        public override void AddCode(CEmmitableCodeContainer code, int context)
        {
            m_repository[context].Add(code);
        }

        public override void AddNewLine(int context)
        {
            CodeContainer container = new CodeContainer(CodeBlockType.CB_NA, this);
            container.AddNewLine();
            m_repository[context].Add(container);
        }

        public override string EmmitStdout()
        {
            string s = AssemblyCodeContainer().ToString();
            Console.WriteLine(s);
            return s;
        }

        public override string ToString()
        {
            string s = AssemblyCodeContainer().ToString();
            return s;
        }

        public override void EmmitToFile(StreamWriter f)
        {
            string s = AssemblyCodeContainer().ToString();
            f.WriteLine(s);
        }

        protected void ExtractSubgraphs(StreamWriter m_ostream, int context, string[] contextnames)
        {
            if (m_repository[context].Count != 0)
            {
                m_ostream.WriteLine("\tsubgraph cluster" + m_clusterSerial++ + "{");
                m_ostream.WriteLine("\t\tnode [style=filled,color=white];");
                m_ostream.WriteLine("\t\tstyle=filled;");
                m_ostream.WriteLine("\t\tcolor=lightgrey;");
                m_ostream.Write("\t\t");
                for (int i = 0; i < m_repository[context].Count; i++)
                {
                    m_ostream.Write(m_repository[context][i].M_NodeName + ";");
                }

                m_ostream.WriteLine("\n\t\tlabel=" + contextnames[context] + ";");
                m_ostream.WriteLine("\t}");
            }
        }

        internal CEmmitableCodeContainer GetChild(int ct, int index = 0)
        {
            return m_repository[ct][index];
        }

        internal CEmmitableCodeContainer[] GetContextChildren(int ct)
        {
            return m_repository[ct].ToArray();
        }

    }

    public class CodeContainer : CEmmitableCodeContainer
    {
        StringBuilder m_repository = new StringBuilder();

        public CodeContainer(CodeBlockType nodeType, CEmmitableCodeContainer parent) : base(nodeType, parent)
        {
        }

        public override void AddCode(string code, int context = -1)
        {
            string[] lines = code.Split(new[] {'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                m_repository.Append(line);
                if (code.Contains('\n'))
                {
                    m_repository.Append("\r\n");
                    m_repository.Append(new string('\t', m_nestingLevel));
                }
            }
        }

        public override void AddCode(CEmmitableCodeContainer code, int context = -1)
        {
            string str = code.ToString();
            AddCode(str, context);
        }

        public override void AddNewLine(int context = -1)
        {
            m_repository.Append("\r\n");
            m_repository.Append(new string('\t', m_nestingLevel));
        }

        public override void EnterScope()
        {
            base.EnterScope();
            AddNewLine();
        }

        public override void LeaveScope()
        {
            base.LeaveScope();
            AddNewLine();
        }

        public override string EmmitStdout()
        {
            System.Console.WriteLine(m_repository.ToString());
            return m_repository.ToString();
        }

        public override void EmmitToFile(StreamWriter f)
        {
            f.WriteLine(m_repository.ToString());
        }

        public override string ToString()
        {
            return m_repository.ToString();
        }

        public override CodeContainer AssemblyCodeContainer()
        {
            return this;
        }

        public override void PrintStructure(StreamWriter m_ostream)
        {
            m_ostream.WriteLine("\"{0}\"->\"{1}\"", M_Parent.M_NodeName, M_NodeName);
        }

    }

}