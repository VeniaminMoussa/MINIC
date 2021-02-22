using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace MINIC
{
    class STPrinterVisitor : MINICBaseVisitor<int>
    {
        private Stack<string> m_labels = new Stack<string>();
        private StreamWriter outFile = new StreamWriter(path:"test.dot");
        private static int ms_serialCounter = 0;

        public override int VisitCompileUnit([NotNull] MINICParser.CompileUnitContext context)
        {
            int serial = ms_serialCounter++;
            string s = "CompileUnit_" + serial;


            outFile.WriteLine("digraph G{");
            m_labels.Push(s);
            base.VisitChildren(context);
            m_labels.Pop();
            outFile.WriteLine("}");
            outFile.Close();

            // Prepare the process dot to run
            ProcessStartInfo start = new ProcessStartInfo();
            // Enter in the command line arguments, everything you would enter after the executable name itself
            start.Arguments = " -Tgif " +
                              Path.GetFileName(path: "test.dot") + " -o " +
                              Path.GetFileNameWithoutExtension("test") + ".gif";
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
            return 0;
        }

        public override int VisitFunctionDefinition(MINICParser.FunctionDefinitionContext context)
        {
            int serial = ms_serialCounter++;
            string s = "FunctionDefinition_" + serial;
            outFile.WriteLine("\"{0}\"->\"{1}\";", m_labels.Peek(), s);

            m_labels.Push(s);

            base.VisitChildren(context);

            m_labels.Pop();
            return 0;
        }

        public override int VisitExpr_FCALL([NotNull] MINICParser.Expr_FCALLContext context)
        {
            int serial = ms_serialCounter++;
            string s = "FCall_" + serial;
            outFile.WriteLine("\"{0}\"->\"{1}\";", m_labels.Peek(), s);

            m_labels.Push(s);

            base.VisitChildren(context);

            m_labels.Pop();
            return 0;
        }

        public override int VisitStatement(MINICParser.StatementContext context)
        {
            int serial = ms_serialCounter++;
            string s = "Statement_" + serial;
            outFile.WriteLine("\"{0}\"->\"{1}\";", m_labels.Peek(), s);

            m_labels.Push(s);

            base.VisitChildren(context);

            m_labels.Pop();
            return 0;
        }

        public override int VisitIfstatement(MINICParser.IfstatementContext context)
        {

            int serial = ms_serialCounter++;
            string s = "IfStatement_" + serial;
            outFile.WriteLine("\"{0}\"->\"{1}\";", m_labels.Peek(), s);

            m_labels.Push(s);

            base.VisitChildren(context);

            m_labels.Pop();
            return 0;
        }

        public override int VisitWhilestatement(MINICParser.WhilestatementContext context)
        {
            int serial = ms_serialCounter++;
            string s = "WhileStatement_" + serial;
            outFile.WriteLine("\"{0}\"->\"{1}\";", m_labels.Peek(), s);

            m_labels.Push(s);

            base.VisitChildren(context);

            m_labels.Pop();
            return 0;
        }
        //............
        public override int VisitForstatement(MINICParser.ForstatementContext context)
        {
            int serial = ms_serialCounter++;
            string s = "ForStatement_" + serial;
            outFile.WriteLine("\"{0}\"->\"{1}\";", m_labels.Peek(), s);

            m_labels.Push(s);

            base.VisitChildren(context);

            m_labels.Pop();
            return 0;
        }

        public override int VisitForargs(MINICParser.ForargsContext context)
        {
            int serial = ms_serialCounter++;
            string s = "ForArgs_" + serial;
            outFile.WriteLine("\"{0}\"->\"{1}\";", m_labels.Peek(), s);

            m_labels.Push(s);

            base.VisitChildren(context);

            m_labels.Pop();
            return 0;
        }
        //............
        public override int VisitCompoundStatement(MINICParser.CompoundStatementContext context)
        {
            int serial = ms_serialCounter++;
            string s = "CompoundStatement_" + serial;
            outFile.WriteLine("\"{0}\"->\"{1}\";", m_labels.Peek(), s);

            m_labels.Push(s);

            base.VisitChildren(context);

            m_labels.Pop();
            return 0;
        }

        public override int VisitStatementList(MINICParser.StatementListContext context)
        {
            int serial = ms_serialCounter++;
            string s = "StatementList" + serial;
            outFile.WriteLine("\"{0}\"->\"{1}\";", m_labels.Peek(), s);

            m_labels.Push(s);

            base.VisitChildren(context);

            m_labels.Pop();
            return 0;
        }


        public override int VisitArgs(MINICParser.ArgsContext context)
        {
            int serial = ms_serialCounter++;
            string s = "Args_" + serial;
            outFile.WriteLine("\"{0}\"->\"{1}\";", m_labels.Peek(), s);

            m_labels.Push(s);

            base.VisitChildren(context);

            m_labels.Pop();
            return 0;
        }

        public override int VisitFargs(MINICParser.FargsContext context)
        {
            int serial = ms_serialCounter++;
            string s = "Fargs_" + serial;
            outFile.WriteLine("\"{0}\"->\"{1}\";", m_labels.Peek(), s);

            m_labels.Push(s);

            base.VisitChildren(context);

            m_labels.Pop();
            return 0;
        }
        public override int VisitTerminal(ITerminalNode node)
        {
            int serial = ms_serialCounter++;
            string s = "";
            switch (node.Symbol.Type)
            {
                case MINICParser.NUMBER:
                    s = "Expr_NUMBER_" + serial;
                    // Preorder action
                    outFile.WriteLine("\"{0}\"->\"{1}\";", m_labels.Peek(), s);
                    break;
                case MINICParser.IDENTIFIER:
                    s = "Expr_IDENTIFIER_" + serial;
                    // Preorder action
                    outFile.WriteLine("\"{0}\"->\"{1}\";", m_labels.Peek(), s);
                    break;
                default:
                    break;
            }

            return base.VisitTerminal(node);
        }

        public override int VisitExpr_DIVMULT(MINICParser.Expr_DIVMULTContext context)
        {
            int serial = ms_serialCounter++;
            string s = "";
            switch (context.op.Type)
            {
                case MINICParser.MULT:
                    s = "Expr_MULT_" + serial;
                    // Preorder action
                    outFile.WriteLine("\"{0}\"->\"{1}\";", m_labels.Peek(), s);
                    break;
                case MINICParser.DIV:
                    s = "Expr_DIV_" + serial;
                    // Preorder action
                    outFile.WriteLine("\"{0}\"->\"{1}\";", m_labels.Peek(), s);
                    break;
                default:
                    break;
            }

            m_labels.Push(s);

            base.VisitChildren(context);

            m_labels.Pop();
            return 0;
        }

        public override int VisitExpr_PLUSMINUS(MINICParser.Expr_PLUSMINUSContext context)
        {
            int serial = ms_serialCounter++;
            string s = "";
            switch (context.op.Type)
            {
                case MINICParser.PLUS:
                    s = "Expr_PLUS_" + serial;
                    // Preorder action
                    outFile.WriteLine("\"{0}\"->\"{1}\";", m_labels.Peek(), s);
                    break;
                case MINICParser.MINUS:
                    s = "Expr_MINUS_" + serial;
                    // Preorder action
                    outFile.WriteLine("\"{0}\"->\"{1}\";", m_labels.Peek(), s);
                    break;
                default:
                    break;
            }
            m_labels.Push(s);

            base.VisitChildren(context);

            m_labels.Pop();
            return 0;
        }

        public override int VisitExpr_PLUS(MINICParser.Expr_PLUSContext context)
        {
            int serial = ms_serialCounter++;
            string s = "Expr_PLUS_" + serial;
            outFile.WriteLine("\"{0}\"->\"{1}\";", m_labels.Peek(), s);

            m_labels.Push(s);

            base.VisitChildren(context);

            m_labels.Pop();
            return 0;
        }

        public override int VisitExpr_MINUS(MINICParser.Expr_MINUSContext context)
        {
            int serial = ms_serialCounter++;
            string s = "Expr_MINUS_" + serial;
            outFile.WriteLine("\"{0}\"->\"{1}\";", m_labels.Peek(), s);

            m_labels.Push(s);

            base.VisitChildren(context);

            m_labels.Pop();
            return 0;
        }

        public override int VisitExpr_PARENTHESIS(MINICParser.Expr_PARENTHESISContext context)
        {
            int serial = ms_serialCounter++;
            string s = "Expr_PARENTHESIS_" + serial;
            outFile.WriteLine("\"{0}\"->\"{1}\";", m_labels.Peek(), s);

            m_labels.Push(s);

            base.VisitChildren(context);

            m_labels.Pop();
            return 0;
        }

        public override int VisitExpr_ASSIGN(MINICParser.Expr_ASSIGNContext context)
        {
            int serial = ms_serialCounter++;
            string s = "Expr_ASSIGN_" + serial;
            outFile.WriteLine("\"{0}\"->\"{1}\";", m_labels.Peek(), s);

            m_labels.Push(s);

            base.VisitChildren(context);

            m_labels.Pop();
            return 0;
        }

        public override int VisitExpr_NOT(MINICParser.Expr_NOTContext context)
        {
            int serial = ms_serialCounter++;
            string s = "Expr_NOT_" + serial;
            outFile.WriteLine("\"{0}\"->\"{1}\";", m_labels.Peek(), s);

            m_labels.Push(s);

            base.VisitChildren(context);

            m_labels.Pop();
            return 0;
        }

        public override int VisitExpr_AND(MINICParser.Expr_ANDContext context)
        {
            int serial = ms_serialCounter++;
            string s = "Expr_AND_" + serial;
            outFile.WriteLine("\"{0}\"->\"{1}\";", m_labels.Peek(), s);

            m_labels.Push(s);

            base.VisitChildren(context);

            m_labels.Pop();
            return 0;
        }

        public override int VisitExpr_OR(MINICParser.Expr_ORContext context)
        {
            int serial = ms_serialCounter++;
            string s = "Expr_OR_" + serial;
            outFile.WriteLine("\"{0}\"->\"{1}\";", m_labels.Peek(), s);

            m_labels.Push(s);

            base.VisitChildren(context);

            m_labels.Pop();
            return 0;
        }

        public override int VisitExpr_COMPARISON(MINICParser.Expr_COMPARISONContext context)
        {
            int serial = ms_serialCounter++;
            string s = "";
            switch (context.op.Type)
            {
                case MINICParser.GT:
                    s = "Expr_GT_" + serial;
                    // Preorder action
                    outFile.WriteLine("\"{0}\"->\"{1}\";", m_labels.Peek(), s);
                    break;
                case MINICParser.GTE:
                    s = "Expr_GTE_" + serial;
                    // Preorder action
                    outFile.WriteLine("\"{0}\"->\"{1}\";", m_labels.Peek(), s);
                    break;
                case MINICParser.LT:
                    s = "Expr_LT_" + serial;
                    // Preorder action
                    outFile.WriteLine("\"{0}\"->\"{1}\";", m_labels.Peek(), s);
                    break;
                case MINICParser.LTE:
                    s = "Expr_LTE_" + serial;
                    // Preorder action
                    outFile.WriteLine("\"{0}\"->\"{1}\";", m_labels.Peek(), s);
                    break;
                case MINICParser.EQUAL:
                    s = "Expr_EQUAL_" + serial;
                    // Preorder action
                    outFile.WriteLine("\"{0}\"->\"{1}\";", m_labels.Peek(), s);
                    break;
                case MINICParser.NEQUAL:
                    s = "Expr_NEQUAL_" + serial;
                    // Preorder action
                    outFile.WriteLine("\"{0}\"->\"{1}\";", m_labels.Peek(), s);
                    break;
                default:
                    break;
            }

            m_labels.Push(s);

            base.VisitChildren(context);

            m_labels.Pop();
            return 0;
        }    
    }
}