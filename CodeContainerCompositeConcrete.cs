using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MINIC
{ 
    public class CFile : CodeContainerComposite
    {
        public const int CB_PREPROCESSOR = 0, CB_GLOBALS = 1, CB_FUNDEFS = 2;
        public static readonly string[] ContextNames = {
            "PREPROCESSORCONTEXT" ,"GLOBALSCONTEXT", "FUNDEFSCONTEXT"
        };

        private HashSet<string> m_globalVarSymbolTable = new HashSet<string>();
        private Dictionary<String, Dictionary<String, String>> m_localSymbolFunctionTable = new Dictionary<string, Dictionary<String, String>>();
        private HashSet<string> m_FunctionsSymbolTable = new HashSet<string>();

        private CMainFunctionDefinition m_mainFunctionDefinition;
        public CMainFunctionDefinition MMainFunctionDefinition => m_mainFunctionDefinition;

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

        public void DeclareGlobalVariable(string varname)
        {
            CodeContainer rep;
            if (!m_globalVarSymbolTable.Contains(varname))
            {
                m_globalVarSymbolTable.Add(varname);
                rep = new CodeContainer(CodeBlockType.CB_CODEREPOSITORY, this);
                rep.AddCode("float " + varname + ";\n", CFile.CB_GLOBALS);
                AddCode(rep, CFile.CB_GLOBALS);
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

        public CFile(CodeBlockType nodeType, int contexts, CEmmitableCodeContainer parent) : base(nodeType, contexts,parent)
        {
            m_mainFunctionDefinition = new CMainFunctionDefinition(this);
            AddCode(m_mainFunctionDefinition,CFile.CB_FUNDEFS);
        }

        public override CodeContainer AssemblyCodeContainer()
        {
            CodeContainer rep = new CodeContainer(CodeBlockType.CB_NA, null);

            rep.AddCode(AssemblyContext(CFile.CB_PREPROCESSOR));
            rep.AddCode(AssemblyContext(CFile.CB_GLOBALS));
            rep.AddCode(AssemblyContext(CFile.CB_FUNDEFS));
            return rep;
        }

        public override void PrintStructure(StreamWriter m_ostream)
        {

            m_ostream.WriteLine("digraph {");

            ExtractSubgraphs(m_ostream, CFile.CB_GLOBALS, ContextNames);
            ExtractSubgraphs(m_ostream, CFile.CB_PREPROCESSOR, ContextNames);
            ExtractSubgraphs(m_ostream, CFile.CB_FUNDEFS, ContextNames);


            foreach (List<CEmmitableCodeContainer> cEmmitableCodeContainers in m_repository)
            {
                foreach (CEmmitableCodeContainer codeContainer in cEmmitableCodeContainers)
                {
                    codeContainer.PrintStructure(m_ostream);
                }
            }

            m_ostream.WriteLine("}");
            m_ostream.Close();

            // Prepare the process to run
            ProcessStartInfo start = new ProcessStartInfo();
            // Enter in the command line arguments, everything you would enter after the executable name itself
            start.Arguments = "-Tgif CodeStructure.dot " + " -o" + " CodeStructure.gif";
            // Enter the executable to run, including the complete path
            start.FileName = "dot";
            // Do you want to show a console window?
            start.WindowStyle = ProcessWindowStyle.Hidden;
            start.CreateNoWindow = true;
            int exitCode;

            // Run the external process & wait for it to finish
            using (Process proc = Process.Start(start))
            {
                proc.WaitForExit();

                // Retrieve the app's exit code
                exitCode = proc.ExitCode;
            }
        }
    }

    public class CIfStatement : CodeContainerComposite
    {
        public const int CC_IFSTATEMENT_CONDITION = 0, CC_IFSTATEMENT_IFBODY = 1, CC_IFSTATEMENT_ELSEBODY = 2;
        public static readonly string[] ContextNames = {
            "IFSTATEMENT_CONDITIONCONTEXT" ,"IFSTATEMENT_IFBODYCONTEXT", "IFSTATEMENT_ELSEBODYCONTEXT"
        };

        public CIfStatement(CodeBlockType nodeType, int contexts, CEmmitableCodeContainer parent) : base(nodeType, contexts, parent) { }

        public override CodeContainer AssemblyCodeContainer()
        {
            CodeContainer rep = new CodeContainer(CodeBlockType.CB_IFSTATEMENTS, M_Parent);
            rep.AddCode("if ( ");
            rep.AddCode(AssemblyContext(CIfStatement.CC_IFSTATEMENT_CONDITION));
            rep.AddCode(" )");
            rep.AddCode(AssemblyContext(CIfStatement.CC_IFSTATEMENT_IFBODY));


            if (this.GetContextChildren(CIfStatement.CC_IFSTATEMENT_ELSEBODY).Length != 0)
            {
                rep.AddCode("else");
                rep.AddCode(AssemblyContext(CIfStatement.CC_IFSTATEMENT_ELSEBODY));
            }

            return rep;
        }

        public override void PrintStructure(StreamWriter m_ostream)
        {
            ExtractSubgraphs(m_ostream, CIfStatement.CC_IFSTATEMENT_CONDITION, ContextNames);
            ExtractSubgraphs(m_ostream, CIfStatement.CC_IFSTATEMENT_IFBODY, ContextNames);
            if (this.GetContextChildren(CIfStatement.CC_IFSTATEMENT_ELSEBODY).Length != 0)
            {
                ExtractSubgraphs(m_ostream, CIfStatement.CC_IFSTATEMENT_ELSEBODY, ContextNames);
            }

            foreach (List<CEmmitableCodeContainer> cEmmitableCodeContainers in m_repository)
            {
                foreach (CEmmitableCodeContainer codeContainer in cEmmitableCodeContainers)
                {
                    codeContainer.PrintStructure(m_ostream);
                }
            }
            m_ostream.WriteLine("\"{0}\"->\"{1}\"", M_Parent.M_NodeName, M_NodeName);
        }
    }

    public class CResultStatement : CodeContainerComposite
    {
        public const int CC_EXPRESSIONSTATEMENT_BODY = 0;
        public static readonly string[] ContextNames = {
            "EXPRESSIONSTATEMENT_BODYCONTEXT"
        };


        public CResultStatement(CodeBlockType nodeType, int contexts, CEmmitableCodeContainer parent) : base(nodeType, contexts, parent) { }

        public override CodeContainer AssemblyCodeContainer()
        {
            CodeContainer rep = new CodeContainer(CodeBlockType.CB_CODEREPOSITORY, M_Parent);
            rep.AddCode("printf(\"res=%f\\n\",");
            rep.AddCode(AssemblyContext(CResultStatement.CC_EXPRESSIONSTATEMENT_BODY));
            rep.AddCode(");");
            rep.AddNewLine();
            return rep;
        }

        public override void PrintStructure(StreamWriter m_ostream)
        {
            ExtractSubgraphs(m_ostream, CResultStatement.CC_EXPRESSIONSTATEMENT_BODY, ContextNames);

            foreach (List<CEmmitableCodeContainer> cEmmitableCodeContainers in m_repository)
            {
                foreach (CEmmitableCodeContainer codeContainer in cEmmitableCodeContainers)
                {
                    codeContainer.PrintStructure(m_ostream);
                }
            }
            m_ostream.WriteLine("\"{0}\"->\"{1}\"", M_Parent.M_NodeName, M_NodeName);
        }
    }

    public class CCompoundStatement : CodeContainerComposite
    {
        public const int CC_COMPOUNDSTATEMENT_DECLARATIONS = 0, CC_COMPOUNDSTATEMENT_BODY = 1;
        public static readonly string[] ContextNames = {
            "COMPOUNDSTATEMENT_DECLARATIONSCONTEXT" ,"COMPOUNDSTATEMENT_BODYCONTEXT"
        };

        public CCompoundStatement(CodeBlockType nodeType, int contexts, CEmmitableCodeContainer parent) : base(nodeType, contexts, parent) { }

        public override CodeContainer AssemblyCodeContainer()
        {
            CodeContainer rep = new CodeContainer(CodeBlockType.CB_COMPOUNDSTATEMENT, M_Parent);
            rep.AddCode("{");
            rep.EnterScope();
            rep.AddCode("//  ***** Local declarations *****");
            rep.AddNewLine();
            rep.AddCode(AssemblyContext(CCompoundStatement.CC_COMPOUNDSTATEMENT_DECLARATIONS));
            rep.AddCode("//  ***** Code Body *****");
            rep.AddNewLine();
            rep.AddCode(AssemblyContext(CCompoundStatement.CC_COMPOUNDSTATEMENT_BODY));
            rep.LeaveScope();
            rep.AddCode("}");
            return rep;
        }

        public override void PrintStructure(StreamWriter m_ostream)
        {
            ExtractSubgraphs(m_ostream, CCompoundStatement.CC_COMPOUNDSTATEMENT_BODY, ContextNames);
            ExtractSubgraphs(m_ostream, CCompoundStatement.CC_COMPOUNDSTATEMENT_DECLARATIONS, ContextNames);
            foreach (List<CEmmitableCodeContainer> cEmmitableCodeContainers in m_repository)
            {
                foreach (CEmmitableCodeContainer codeContainer in cEmmitableCodeContainers)
                {
                    codeContainer.PrintStructure(m_ostream);
                }
            }
            m_ostream.WriteLine("\"{0}\"->\"{1}\"", M_Parent.M_NodeName, M_NodeName);
        }
    }

    public class CWhileStatement : CodeContainerComposite
    {
        public const int CC_WHILESTATEMENT_CONDITION = 0, CC_WHILESTATEMENT_BODY = 1;
        public static readonly string[] ContextNames = {
            "WHILESTATEMENT_CONDITIONCONTEXT" ,"WHILESTATEMENT_BODYCONTEXT"
        };

        public CWhileStatement(CodeBlockType nodeType, int contexts, CEmmitableCodeContainer parent) : base(nodeType, contexts, parent) { }

        public override CodeContainer AssemblyCodeContainer()
        {
            CodeContainer rep = new CodeContainer(CodeBlockType.CB_WHILESTATEMENT, M_Parent);
            rep.AddCode("while ( ");
            rep.AddCode(AssemblyContext(CWhileStatement.CC_WHILESTATEMENT_CONDITION));
            rep.AddCode(" )");
            rep.AddCode(AssemblyContext(CWhileStatement.CC_WHILESTATEMENT_BODY));
            return rep;
        }

        public override void PrintStructure(StreamWriter m_ostream)
        {
            ExtractSubgraphs(m_ostream, CWhileStatement.CC_WHILESTATEMENT_CONDITION, ContextNames);
            ExtractSubgraphs(m_ostream, CWhileStatement.CC_WHILESTATEMENT_BODY, ContextNames);

            foreach (List<CEmmitableCodeContainer> cEmmitableCodeContainers in m_repository)
            {
                foreach (CEmmitableCodeContainer codeContainer in cEmmitableCodeContainers)
                {
                    codeContainer.PrintStructure(m_ostream);
                }
            }
            m_ostream.WriteLine("\"{0}\"->\"{1}\"", M_Parent.M_NodeName, M_NodeName);
        }
    }
    //..............
    public class CForStatement : CodeContainerComposite
    {
        public const int CC_FORARGS_STATEMENT = 0, CC_FORSTATEMENT_BODY = 1;
        public static readonly string[] ContextNames = {
            "FORARGS_STATEMENT_CONDITIONCONTEXT" ,"FOR_STATEMENT_BODYCONTEXT"
        };

        public CForStatement(CodeBlockType nodeType, int contexts, CEmmitableCodeContainer parent) : base(nodeType, contexts, parent) { }

        public override CodeContainer AssemblyCodeContainer()
        {
            CodeContainer rep = new CodeContainer(CodeBlockType.CB_FORSTATEMENT, M_Parent);
            rep.AddCode("for ( ");
            rep.AddCode(AssemblyContext(CForStatement.CC_FORARGS_STATEMENT));
            rep.AddCode(" )");
            rep.AddCode(AssemblyContext(CForStatement.CC_FORSTATEMENT_BODY));
            return rep;
        }

        public override void PrintStructure(StreamWriter m_ostream)
        {
            ExtractSubgraphs(m_ostream, CForStatement.CC_FORARGS_STATEMENT, ContextNames);
            ExtractSubgraphs(m_ostream, CForStatement.CC_FORSTATEMENT_BODY, ContextNames);

            foreach (List<CEmmitableCodeContainer> cEmmitableCodeContainers in m_repository)
            {
                foreach (CEmmitableCodeContainer codeContainer in cEmmitableCodeContainers)
                {
                    codeContainer.PrintStructure(m_ostream);
                }
            }
            m_ostream.WriteLine("\"{0}\"->\"{1}\"", M_Parent.M_NodeName, M_NodeName);
        }
    }
    //..............
    public class CCFunctionDefinition : CodeContainerComposite
    {
        public const int CB_FUNCTIONDEFINITION_DECLARATIONS = 0, CB_FUNCTIONDEFINITION_BODY = 1;
        public static readonly string[] ContextNames = {
            "FUNCTIONDEFINITION_DECLARATIONSCONTEXT" ,"FUNCTIONDEFINITION_BODYCONTEXT"
        };

        public CCFunctionDefinition(CodeBlockType nodeType, int contexts, CEmmitableCodeContainer parent) : base(nodeType, contexts, parent) { }

        public override CodeContainer AssemblyCodeContainer()
        {
            // throw new NotImplementedException();

            CodeContainer rep = new CodeContainer(CodeBlockType.CB_NA, M_Parent);
            // 1. Emmit Header
            rep.AddCode(AssemblyContext(CCFunctionDefinition.CB_FUNCTIONDEFINITION_DECLARATIONS));
            
            // 4. Emmit Code Body
            rep.AddCode(AssemblyContext(CCFunctionDefinition.CB_FUNCTIONDEFINITION_BODY));
            rep.AddNewLine();
            
            return rep;
        }

        public override void PrintStructure(StreamWriter m_ostream)
        {
            ExtractSubgraphs(m_ostream, CCFunctionDefinition.CB_FUNCTIONDEFINITION_BODY, ContextNames);
            ExtractSubgraphs(m_ostream, CCFunctionDefinition.CB_FUNCTIONDEFINITION_DECLARATIONS, ContextNames);

            foreach (List<CEmmitableCodeContainer> cEmmitableCodeContainers in m_repository)
            {
                foreach (CEmmitableCodeContainer codeContainer in cEmmitableCodeContainers)
                {
                    codeContainer.PrintStructure(m_ostream);
                }
            }
            m_ostream.WriteLine("\"{0}\"->\"{1}\"", M_Parent.M_NodeName, M_NodeName);
        }
    }

    public class CMainFunctionDefinition : CCFunctionDefinition
    {
        public CMainFunctionDefinition(CEmmitableCodeContainer parent) : base(CodeBlockType.CB_FUNCTIONDEFINITION, 2,
            parent)
        {
        }

        public override CodeContainer AssemblyCodeContainer()
        {
            CodeContainer rep = new CodeContainer(CodeBlockType.CB_NA, null);
            string mainheader = "int main(int argc, char* argv[]){";
            rep.AddCode(mainheader);
            rep.EnterScope();
            rep.AddCode(AssemblyContext(CCFunctionDefinition.CB_FUNCTIONDEFINITION_DECLARATIONS));
            rep.AddCode(AssemblyContext(CCFunctionDefinition.CB_FUNCTIONDEFINITION_BODY));
            rep.AddCode("return 0;");
            rep.LeaveScope();
            rep.AddCode("}");
        
            return rep;  
        }
    }

}
