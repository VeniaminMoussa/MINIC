using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MINIC
{
    class Program
    {
        static void Main(string[] args)
        {
            StreamReader AStream = new StreamReader(args[0]);
            AntlrInputStream antlrInputStream = new AntlrInputStream(AStream);
            MINICLexer lexer = new MINICLexer(antlrInputStream);
            CommonTokenStream tokens = new CommonTokenStream(lexer);
            MINICParser parser = new MINICParser(tokens);
            IParseTree tree = parser.compileUnit();
            Console.WriteLine(tree.ToStringTree());

            STPrinterVisitor stPrinter = new STPrinterVisitor();
            stPrinter.Visit(tree);

            ASTGenerator astgen = new ASTGenerator();
            astgen.Visit(tree);
            
            ASTPrinterVisitor astPrinterVisitor = new ASTPrinterVisitor("test.ast.dot");
            astPrinterVisitor.Visit(astgen.M_Root);

            MINICTranslation cGenerator = new MINICTranslation();
            cGenerator.Visit(astgen.M_Root);
            CodeContainer file = cGenerator.FileBuilder.AssemblyCodeContainer();
            Console.WriteLine(file.ToString());

            StreamWriter m_streamWriter = new StreamWriter("CodeStructure.dot");
            cGenerator.FileBuilder.PrintStructure(m_streamWriter);

            MINICPrinter m_minicPrinter = new MINICPrinter("CodeStructure.c", file);
            m_minicPrinter.printer();
        }
    }
}
