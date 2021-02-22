using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MINIC
{
    public class ASTPrinterVisitor : MINICASTBaseVisitor<int>
    {
        private static int m_clusterSerial = 0;
        private StreamWriter m_ostream;
        private string m_dotName;

        // Constructor
        public ASTPrinterVisitor(string dotFileName)
        {
            m_ostream = new StreamWriter(dotFileName);
            m_dotName = dotFileName;
        }

        private void ExtractSubgraphs(ASTElement node, int context, string[] contextNames)
        {
            if(node.GetChildrenContextNumber(context)!=0)
            {
                m_ostream.WriteLine("\tsubgraph cluster" + m_clusterSerial++ + "{");
                m_ostream.WriteLine("\t\tnode [style=filled,color=white];");
                m_ostream.WriteLine("\t\tstyle=filled;");
                m_ostream.WriteLine("\t\tcolor=lightgrey;");
                m_ostream.Write("\t\t");

                foreach (ASTElement ln in node.GetChildrenContext(context))
                {
                    m_ostream.Write(ln.M_GraphVizName + ";");
                }

                m_ostream.WriteLine("\n\t\tlabel=" + contextNames[context] + ";");
                m_ostream.WriteLine("\t}");
            }
        }


        public override int VisitAddition(CAddition node)
        {
            ExtractSubgraphs(node, CAddition.CT_LEFT, CAddition.ContextNames);
            ExtractSubgraphs(node, CAddition.CT_RIGHT, CAddition.ContextNames);

            base.VisitAddition(node);

            m_ostream.WriteLine("{0}->{1}", currentParent.M_GraphVizName, node.M_GraphVizName);

            return 0;
        }

        public override int VisitAnd(CAnd node)
        {
            ExtractSubgraphs(node, CAnd.CT_LEFT, CAnd.ContextNames);
            ExtractSubgraphs(node, CAnd.CT_RIGHT, CAnd.ContextNames);

            base.VisitAnd(node);

            m_ostream.WriteLine("{0}->{1}", currentParent.M_GraphVizName, node.M_GraphVizName);

            return 0;
        }

        public override int VisitAssignment(CAssignment node)
        {
            ExtractSubgraphs(node, CAssignment.CT_LEFT, CAssignment.ContextNames);
            ExtractSubgraphs(node, CAssignment.CT_RIGHT, CAssignment.ContextNames);

            base.VisitAssignment(node);

            m_ostream.WriteLine("{0}->{1}", currentParent.M_GraphVizName, node.M_GraphVizName);

            return 0;
        }

        public override int VisitBREAK(CBreak node)
        {
            base.VisitBREAK(node);

            m_ostream.WriteLine("{0}->{1}", currentParent.M_GraphVizName, node.M_GraphVizName);

            return 0;
        }

        public override int VisitCompileUnit(CCompileUnit node)
        {
            m_ostream.WriteLine("digraph {");

            ExtractSubgraphs(node, CCompileUnit.CT_COMPILEUNIT_STATEMENTS, CCompileUnit.ContextNames);
            ExtractSubgraphs(node, CCompileUnit.CT_COMPILEUNIT_FUNDEFS, CCompileUnit.ContextNames);

            base.VisitCompileUnit(node);

            m_ostream.WriteLine("}");
            m_ostream.Close();

            // Prepare the process to run
            ProcessStartInfo start = new ProcessStartInfo();
            // Enter in the command line arguments, everything you would enter after the executable name itself
            start.Arguments = "-Tgif " + m_dotName + " -o" + m_dotName + ".gif";
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

        public override int VisitCompoundStatement(CCompoundstatement node)
        {
            ExtractSubgraphs(node, CCompoundstatement.CT_BODY, CCompoundstatement.ContextNames);

            base.VisitCompoundStatement(node);

            m_ostream.WriteLine("{0}->{1}", currentParent.M_GraphVizName, node.M_GraphVizName);

            return 0;
        }

        public override int VisitDivision(CDivision node)
        {
            ExtractSubgraphs(node, CDivision.CT_LEFT, CDivision.ContextNames);
            ExtractSubgraphs(node, CDivision.CT_RIGHT, CDivision.ContextNames);

            base.VisitDivision(node);

            m_ostream.WriteLine("{0}->{1}", currentParent.M_GraphVizName, node.M_GraphVizName);

            return 0;
        }

        public override int VisitEqual(CEqual node)
        {
            ExtractSubgraphs(node, CEqual.CT_LEFT, CEqual.ContextNames);
            ExtractSubgraphs(node, CEqual.CT_RIGHT, CEqual.ContextNames);

            base.VisitEqual(node);

            m_ostream.WriteLine("{0}->{1}", currentParent.M_GraphVizName, node.M_GraphVizName);

            return 0;
        }

        public override int VisitFunction(CFunction node)
        {
            ExtractSubgraphs(node, CFunction.CT_FNAME, CFunction.ContextNames);
            ExtractSubgraphs(node, CFunction.CT_FARGS, CFunction.ContextNames);
            ExtractSubgraphs(node, CFunction.CT_BODY, CFunction.ContextNames);

            base.VisitFunction(node);

            m_ostream.WriteLine("{0}->{1}", currentParent.M_GraphVizName, node.M_GraphVizName);

            return 0;
        }

        public override int VisitFcall(CFCall node)
        {
            ExtractSubgraphs(node, CFCall.CT_FNAME, CFCall.ContextNames);
            ExtractSubgraphs(node, CFCall.CT_ARGS, CFCall.ContextNames);

            base.VisitFcall(node);

            m_ostream.WriteLine("{0}->{1}", currentParent.M_GraphVizName, node.M_GraphVizName);

            return 0;
        }

        public override int VisitGt(CGt node)
        {
            ExtractSubgraphs(node, CGt.CT_LEFT, CGt.ContextNames);
            ExtractSubgraphs(node, CGt.CT_RIGHT, CGt.ContextNames);

            base.VisitGt(node);

            m_ostream.WriteLine("{0}->{1}", currentParent.M_GraphVizName, node.M_GraphVizName);

            return 0;
        }

        public override int VisitGte(CGte node)
        {
            ExtractSubgraphs(node, CGte.CT_LEFT, CGte.ContextNames);
            ExtractSubgraphs(node, CGte.CT_RIGHT, CGte.ContextNames);

            base.VisitGte(node);

            m_ostream.WriteLine("{0}->{1}", currentParent.M_GraphVizName, node.M_GraphVizName);

            return 0;
        }

        public override int VisitPlus(CPlus node)
        {
            ExtractSubgraphs(node, CPlus.CT_EXPRESSION, CPlus.ContextNames);

            base.VisitPlus(node);

            m_ostream.WriteLine("{0}->{1}", currentParent.M_GraphVizName, node.M_GraphVizName);

            return 0;
        }

        public override int VisitMinus(CMinus node)
        {
            ExtractSubgraphs(node, CMinus.CT_EXPRESSION, CMinus.ContextNames);

            base.VisitMinus(node);

            m_ostream.WriteLine("{0}->{1}", currentParent.M_GraphVizName, node.M_GraphVizName);

            return 0;
        }

        public override int VisitIDENTIFIER(CIdentifier node)
        {
            base.VisitIDENTIFIER(node);

            m_ostream.WriteLine("{0}->{1}", currentParent.M_GraphVizName, node.M_GraphVizName);

            return 0;
        }

        public override int VisitIf(CIf node)
        {
            ExtractSubgraphs(node, CIf.CT_CONDITION, CIf.ContextNames);
            ExtractSubgraphs(node, CIf.CT_IFCLAUSE, CIf.ContextNames);
            ExtractSubgraphs(node, CIf.CT_ELSECLAUSE, CIf.ContextNames);

            base.VisitIf(node);

            m_ostream.WriteLine("{0}->{1}", currentParent.M_GraphVizName, node.M_GraphVizName);

            return 0;
        }

        public override int VisitLt(CLt node)
        {
            ExtractSubgraphs(node, CLt.CT_LEFT, CLt.ContextNames);
            ExtractSubgraphs(node, CLt.CT_RIGHT, CLt.ContextNames);

            base.VisitLt(node);

            m_ostream.WriteLine("{0}->{1}", currentParent.M_GraphVizName, node.M_GraphVizName);

            return 0;
        }

        public override int VisitLte(CLte node)
        {
            ExtractSubgraphs(node, CLte.CT_LEFT, CLte.ContextNames);
            ExtractSubgraphs(node, CLte.CT_RIGHT, CLte.ContextNames);

            base.VisitLte(node);

            m_ostream.WriteLine("{0}->{1}", currentParent.M_GraphVizName, node.M_GraphVizName);

            return 0;
        }

        public override int VisitMultiplication(CMultiplication node)
        {
            ExtractSubgraphs(node, CMultiplication.CT_LEFT, CMultiplication.ContextNames);
            ExtractSubgraphs(node, CMultiplication.CT_RIGHT, CMultiplication.ContextNames);

            base.VisitMultiplication(node);

            m_ostream.WriteLine("{0}->{1}", currentParent.M_GraphVizName, node.M_GraphVizName);

            return 0;
        }

        public override int VisitNequal(CNequal node)
        {
            ExtractSubgraphs(node, CNequal.CT_LEFT, CNequal.ContextNames);
            ExtractSubgraphs(node, CNequal.CT_RIGHT, CNequal.ContextNames);

            base.VisitNequal(node);

            m_ostream.WriteLine("{0}->{1}", currentParent.M_GraphVizName, node.M_GraphVizName);

            return 0;
        }

        public override int VisitNot(CNot node)
        {
            ExtractSubgraphs(node, CNot.CT_BODY, CNot.ContextNames);

            base.VisitNot(node);

            m_ostream.WriteLine("{0}->{1}", currentParent.M_GraphVizName, node.M_GraphVizName);

            return 0;
        }

        public override int VisitNUMBER(CNUMBER node)
        {
            base.VisitNUMBER(node);

            m_ostream.WriteLine("{0}->{1}", currentParent.M_GraphVizName, node.M_GraphVizName);

            return 0;
        }

        public override int VisitOr(COr node)
        {
            ExtractSubgraphs(node, COr.CT_LEFT, COr.ContextNames);
            ExtractSubgraphs(node, COr.CT_RIGHT, COr.ContextNames);

            base.VisitOr(node);

            m_ostream.WriteLine("{0}->{1}", currentParent.M_GraphVizName, node.M_GraphVizName);

            return 0;
        }

        public override int VisitReturn(CReturn node)
        {
            ExtractSubgraphs(node, CReturn.CT_EXPRESSION, CReturn.ContextNames);

            base.VisitReturn(node);

            m_ostream.WriteLine("{0}->{1}", currentParent.M_GraphVizName, node.M_GraphVizName);

            return 0;
        }

        public override int VisitSubtraction(CSubtraction node)
        {
            ExtractSubgraphs(node, CSubtraction.CT_LEFT, CSubtraction.ContextNames);
            ExtractSubgraphs(node, CSubtraction.CT_RIGHT, CSubtraction.ContextNames);

            base.VisitSubtraction(node);

            m_ostream.WriteLine("{0}->{1}", currentParent.M_GraphVizName, node.M_GraphVizName);

            return 0;
        }

        public override int VisitWhile(CWhile node)
        {
            ExtractSubgraphs(node, CWhile.CT_CONDITION, CWhile.ContextNames);
            ExtractSubgraphs(node, CWhile.CT_BODY, CWhile.ContextNames);

            base.VisitWhile(node);

            m_ostream.WriteLine("{0}->{1}", currentParent.M_GraphVizName, node.M_GraphVizName);

            return 0;
        }
        //............
        public override int VisitFor(CFor node)
        {
            ExtractSubgraphs(node, CFor.CT_FOR_ARGS, CFor.ContextNames);
            ExtractSubgraphs(node, CFor.CT_BODY, CFor.ContextNames);

            base.VisitFor(node);

            m_ostream.WriteLine("{0}->{1}", currentParent.M_GraphVizName, node.M_GraphVizName);

            return 0;
        }
        //............
    }
}
