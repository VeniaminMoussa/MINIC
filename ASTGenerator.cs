using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace MINIC
{
    public class ASTGenerator : MINICBaseVisitor<int>
    {
        private CCompileUnit m_root;
        Stack<ValueTuple<MINICASTElement, int>> m_parents = new Stack<(MINICASTElement, int)>();
        Dictionary<String, MINICASTElement> varGlobalSymbolTable = new Dictionary<string, MINICASTElement>();
        Dictionary<String, MINICASTElement> functionSymbolTable = new Dictionary<string, MINICASTElement>();
        Dictionary<String, Dictionary<String, MINICASTElement>> varLocalSymbolTable = new Dictionary<string, Dictionary<String, MINICASTElement>>();
        
        public CCompileUnit M_Root => m_root;

        public override int VisitCompileUnit([NotNull] MINICParser.CompileUnitContext context)
        {
            CCompileUnit newNode = new CCompileUnit();
            m_root = newNode;

            m_parents.Push((newNode, CCompileUnit.CT_COMPILEUNIT_STATEMENTS));
            foreach (MINICParser.StatementContext statementContext in context.statement())
            {
                base.Visit(statementContext);
            }
            m_parents.Pop();

            m_parents.Push((newNode, CCompileUnit.CT_COMPILEUNIT_FUNDEFS));
            foreach (MINICParser.FunctionDefinitionContext functionDefinitionContext in context.functionDefinition())
            {
                base.Visit(functionDefinitionContext);
            }
            m_parents.Pop();

            return 0;
        }

        public override int VisitExpr_ASSIGN([NotNull] MINICParser.Expr_ASSIGNContext context)
        {
            CAssignment newNode = new CAssignment();
            ValueTuple<MINICASTElement, int> parent = m_parents.Peek();
            parent.Item1.AddChild(newNode, parent.Item2);

            m_parents.Push((newNode,CAssignment.CT_LEFT));
            Visit(tree: context.IDENTIFIER());
            m_parents.Pop();

            m_parents.Push((newNode, CAssignment.CT_RIGHT));
            Visit(tree: context.expression());
            m_parents.Pop();

            return 0;
        }

        public override int VisitExpr_PLUSMINUS([NotNull] MINICParser.Expr_PLUSMINUSContext context)
        {
            switch (context.op.Type)
            {
                case MINICLexer.PLUS:
                    CAddition newNode1 = new CAddition();
                    ValueTuple<MINICASTElement, int> parent1 = m_parents.Peek();
                    parent1.Item1.AddChild(newNode1, parent1.Item2);

                    m_parents.Push((newNode1, CAddition.CT_LEFT));
                    Visit(tree: context.expression(i: 0));
                    m_parents.Pop();

                    m_parents.Push((newNode1, CAddition.CT_RIGHT));
                    Visit(tree: context.expression(i: 1));
                    m_parents.Pop();
                    break;
                case MINICLexer.MINUS:
                    CSubtraction newNode2 = new CSubtraction();
                    ValueTuple<MINICASTElement, int> parent2 = m_parents.Peek();
                    parent2.Item1.AddChild(newNode2, parent2.Item2);
                    
                    m_parents.Push((newNode2, CSubtraction.CT_LEFT));
                    Visit(tree: context.expression(i: 0));
                    m_parents.Pop();

                    m_parents.Push((newNode2, CSubtraction.CT_RIGHT));
                    Visit(tree: context.expression(i: 1));
                    m_parents.Pop();
                    break;
                default:
                    break;
            }

            return 0;
        }

        public override int VisitExpr_DIVMULT([NotNull] MINICParser.Expr_DIVMULTContext context)
        {
            switch (context.op.Type)
            {
                case MINICLexer.MULT:
                    CMultiplication newNode1 = new CMultiplication();
                    ValueTuple<MINICASTElement, int> parent1 = m_parents.Peek();
                    parent1.Item1.AddChild(newNode1, parent1.Item2);

                    m_parents.Push((newNode1, CMultiplication.CT_LEFT));
                    Visit(tree: context.expression(i: 0));
                    m_parents.Pop();

                    m_parents.Push((newNode1, CMultiplication.CT_RIGHT));
                    Visit(tree: context.expression(i: 1));
                    m_parents.Pop();
                    break;
                case MINICLexer.DIV:
                    CDivision newNode2 = new CDivision();
                    ValueTuple<MINICASTElement, int> parent2 = m_parents.Peek();
                    parent2.Item1.AddChild(newNode2, parent2.Item2);

                    m_parents.Push((newNode2, CDivision.CT_LEFT));
                    Visit(tree: context.expression(i: 0));
                    m_parents.Pop();

                    m_parents.Push((newNode2, CDivision.CT_RIGHT));
                    Visit(tree: context.expression(i: 1));
                    m_parents.Pop();
                    break;
                default:
                    break;
            }

            return 0;
        }

        public override int VisitExpr_PLUS(MINICParser.Expr_PLUSContext context)
        {
            CPlus newNode = new CPlus();
            ValueTuple<MINICASTElement, int> parent = m_parents.Peek();
            parent.Item1.AddChild(newNode, parent.Item2);

            m_parents.Push((newNode, CPlus.CT_EXPRESSION));
            Visit(tree: context.expression());
            m_parents.Pop();

            return 0;
        }

        public override int VisitExpr_MINUS(MINICParser.Expr_MINUSContext context)
        {
            CMinus newNode = new CMinus();
            ValueTuple<MINICASTElement, int> parent = m_parents.Peek();
            parent.Item1.AddChild(newNode, parent.Item2);

            m_parents.Push((newNode, CMinus.CT_EXPRESSION));
            Visit(tree: context.expression());
            m_parents.Pop();

            return 0;
        }

        public override int VisitExpr_COMPARISON([NotNull] MINICParser.Expr_COMPARISONContext context)
        {
            switch (context.op.Type)
            {
                case MINICLexer.GT:
                    CGt newNode1 = new CGt();
                    ValueTuple<MINICASTElement, int> parent1 = m_parents.Peek();
                    parent1.Item1.AddChild(newNode1, parent1.Item2);

                    m_parents.Push((newNode1, CGt.CT_LEFT));
                    Visit(tree: context.expression(i: 0));
                    m_parents.Pop();

                    m_parents.Push((newNode1, CGt.CT_RIGHT));
                    Visit(tree: context.expression(i: 1));
                    m_parents.Pop();
                    break;
                case MINICLexer.GTE:
                    CGte newNode2 = new CGte();
                    ValueTuple<MINICASTElement, int> parent2 = m_parents.Peek();
                    parent2.Item1.AddChild(newNode2, parent2.Item2);

                    m_parents.Push((newNode2, CGte.CT_LEFT));
                    Visit(tree: context.expression(i: 0));
                    m_parents.Pop();

                    m_parents.Push((newNode2, CGte.CT_RIGHT));
                    Visit(tree: context.expression(i: 1));
                    m_parents.Pop();
                    break;
                case MINICLexer.LT:
                    CLt newNode3 = new CLt();
                    ValueTuple<MINICASTElement, int> parent3 = m_parents.Peek();
                    parent3.Item1.AddChild(newNode3, parent3.Item2);

                    m_parents.Push((newNode3, CLt.CT_LEFT));
                    Visit(tree: context.expression(i: 0));
                    m_parents.Pop();

                    m_parents.Push((newNode3, CLt.CT_RIGHT));
                    Visit(tree: context.expression(i: 1));
                    m_parents.Pop();
                    break;
                case MINICLexer.LTE:
                    CLte newNode4 = new CLte();
                    ValueTuple<MINICASTElement, int> parent4 = m_parents.Peek();
                    parent4.Item1.AddChild(newNode4, parent4.Item2);

                    m_parents.Push((newNode4, CLte.CT_LEFT));
                    Visit(tree: context.expression(i: 0));
                    m_parents.Pop();

                    m_parents.Push((newNode4, CLte.CT_RIGHT));
                    Visit(tree: context.expression(i: 1));
                    m_parents.Pop();
                    break;
                case MINICLexer.EQUAL:
                    CEqual newNode5 = new CEqual();
                    ValueTuple<MINICASTElement, int> parent5 = m_parents.Peek();
                    parent5.Item1.AddChild(newNode5, parent5.Item2);

                    m_parents.Push((newNode5, CEqual.CT_LEFT));
                    Visit(tree: context.expression(i: 0));
                    m_parents.Pop();

                    m_parents.Push((newNode5, CEqual.CT_RIGHT));
                    Visit(tree: context.expression(i: 1));
                    m_parents.Pop();
                    break;
                case MINICLexer.NEQUAL:
                    CNequal newNode6 = new CNequal();
                    ValueTuple<MINICASTElement, int> parent6 = m_parents.Peek();
                    parent6.Item1.AddChild(newNode6, parent6.Item2);

                    m_parents.Push((newNode6, CNequal.CT_LEFT));
                    Visit(tree: context.expression(i: 0));
                    m_parents.Pop();

                    m_parents.Push((newNode6, CNequal.CT_RIGHT));
                    Visit(tree: context.expression(i: 1));
                    m_parents.Pop();
                    break;
                default:
                    break;
            }

            return 0;
        }

        public override int VisitExpr_AND([NotNull] MINICParser.Expr_ANDContext context)
        {
            CAnd newNode = new CAnd();
            ValueTuple<MINICASTElement, int> parent = m_parents.Peek();
            parent.Item1.AddChild(newNode, parent.Item2);

            m_parents.Push((newNode, CAnd.CT_LEFT));
            Visit(tree: context.expression(i: 0));
            m_parents.Pop();

            m_parents.Push((newNode, CAnd.CT_RIGHT));
            Visit(tree: context.expression(i: 1));
            m_parents.Pop();

            return 0;
        }

        public override int VisitExpr_OR([NotNull] MINICParser.Expr_ORContext context)
        {
            COr newNode = new COr();
            ValueTuple<MINICASTElement, int> parent = m_parents.Peek();
            parent.Item1.AddChild(newNode, parent.Item2);

            m_parents.Push((newNode, COr.CT_LEFT));
            Visit(tree: context.expression(i: 0));
            m_parents.Pop();

            m_parents.Push((newNode, COr.CT_RIGHT));
            Visit(tree: context.expression(i: 1));
            m_parents.Pop();

            return 0;
        }

        public override int VisitExpr_NOT([NotNull] MINICParser.Expr_NOTContext context)
        {
            CNot newNode = new CNot();
            ValueTuple<MINICASTElement, int> parent = m_parents.Peek();
            parent.Item1.AddChild(newNode, parent.Item2);

            m_parents.Push((newNode, CNot.CT_BODY));
            Visit(tree: context.expression());
            m_parents.Pop();

            return 0;
        }

        public override int VisitFunctionDefinition([NotNull] MINICParser.FunctionDefinitionContext context)
        {
            CFunction newNode = new CFunction();
            ValueTuple<MINICASTElement, int> parent = m_parents.Peek();
            parent.Item1.AddChild(newNode, parent.Item2);

            m_parents.Push((newNode, CFunction.CT_FNAME));
            Visit(tree: context.IDENTIFIER());
            m_parents.Pop();

            if (context.fargs() != null)
            {
                m_parents.Push((newNode, CFunction.CT_FARGS));
                Visit(tree: context.fargs());
                m_parents.Pop();
            }

            m_parents.Push((newNode, CFunction.CT_BODY));
            Visit(tree: context.compoundStatement());
            m_parents.Pop();

            return 0;
        }

        public override int VisitExpr_FCALL([NotNull] MINICParser.Expr_FCALLContext context)
        {
            CFCall newNode = new CFCall();
            ValueTuple<MINICASTElement, int> parent = m_parents.Peek();
            parent.Item1.AddChild(newNode, parent.Item2);

            m_parents.Push((newNode, CFCall.CT_FNAME));
            Visit(tree: context.IDENTIFIER());
            m_parents.Pop();

            if (context.args() != null)
            {
                m_parents.Push((newNode, CFCall.CT_ARGS));
                Visit(tree: context.args());
                m_parents.Pop();
            }

            return 0;
        }

        public override int VisitIfstatement(MINICParser.IfstatementContext context)
        {
            CIf newNode = new CIf();
            ValueTuple<MINICASTElement, int> parent = m_parents.Peek();
            parent.Item1.AddChild(newNode, parent.Item2);

            m_parents.Push((newNode, CIf.CT_CONDITION));
            Visit(tree: context.expression());
            m_parents.Pop();
            
            m_parents.Push((newNode, CIf.CT_IFCLAUSE));
            Visit(tree: context.statement(i: 0));
            m_parents.Pop();

            if (context.statement().Count() == 2)
            {
                m_parents.Push((newNode, CIf.CT_ELSECLAUSE));
                Visit(tree: context.statement(i: 1));
                m_parents.Pop();
            }

            return 0;
        }

        public override int VisitStatement_ReturnStatement([NotNull] MINICParser.Statement_ReturnStatementContext context)
        {
            CReturn newNode = new CReturn();
            ValueTuple<MINICASTElement, int> parent = m_parents.Peek();
            parent.Item1.AddChild(newNode, parent.Item2);

            m_parents.Push((newNode, CReturn.CT_EXPRESSION));
            Visit(tree: context.expression());
            m_parents.Pop();

            return 0;
        }

        public override int VisitWhilestatement([NotNull] MINICParser.WhilestatementContext context)
        {
            CWhile newNode = new CWhile();
            ValueTuple<MINICASTElement, int> parent = m_parents.Peek();
            parent.Item1.AddChild(newNode, parent.Item2);

            m_parents.Push((newNode, CWhile.CT_CONDITION));
            Visit(tree: context.expression());
            m_parents.Pop();

            m_parents.Push((newNode, CWhile.CT_BODY));
            Visit(tree: context.statement());
            m_parents.Pop();

            return 0;
        }
        //............
        public override int VisitForstatement(MINICParser.ForstatementContext context)
        {
            CFor newNode = new CFor();
            ValueTuple<MINICASTElement, int> parent = m_parents.Peek();
            parent.Item1.AddChild(newNode, parent.Item2);

            m_parents.Push((newNode, CFor.CT_FOR_ARGS));
            Visit(tree: context.forargs());
            m_parents.Pop();

            m_parents.Push((newNode, CFor.CT_BODY));
            Visit(tree: context.statement());
            m_parents.Pop();

            return 0;
        }
        //............
        public override int VisitCompoundStatement(MINICParser.CompoundStatementContext context)
        {
            CCompoundstatement newNode = new CCompoundstatement();
            ValueTuple<MINICASTElement, int> parent = m_parents.Peek();
            parent.Item1.AddChild(newNode, parent.Item2);

            m_parents.Push((newNode, CReturn.CT_EXPRESSION));
            Visit(tree: context.statementList());
            m_parents.Pop();

            return 0;
        }

        public override int VisitTerminal([NotNull] ITerminalNode node)
        {
            switch (node.Symbol.Type)
            {
                case MINICLexer.IDENTIFIER:
                    ValueTuple<MINICASTElement, int> parent1 = m_parents.Peek();
                    ValueTuple<MINICASTElement, int> rootParent;
                    MINICASTElement newIdentifier = null;

                    if (m_parents.Count() >= 2)
                    {
                        rootParent = m_parents.ElementAt(m_parents.Count() - 2);
                    }
                    else
                    {
                        rootParent = m_parents.ElementAt(m_parents.Count() - 1);
                    }

                    if (rootParent.Item1.GetType().FullName.Contains("CFunction"))//if the root is a function definition
                    {
                        if (node.Parent.RuleContext.RuleIndex == MINICParser.RULE_functionDefinition)//name identifier of function
                        {
                            if (functionSymbolTable.ContainsKey(node.Symbol.Text))
                            {
                                newIdentifier = functionSymbolTable[node.Symbol.Text];
                            }
                            else
                            {
                                newIdentifier = new CIdentifier(node.Symbol.Text);
                                functionSymbolTable.Add(node.Symbol.Text, newIdentifier);
                            }

                        }
                        else if (node.Parent.RuleContext.RuleIndex == MINICParser.RULE_fargs)//arguments variables in function
                        {
                            if (varLocalSymbolTable.ContainsKey(node.Symbol.Text))
                            {
                                if (varLocalSymbolTable[node.Symbol.Text].ContainsKey(rootParent.Item1.M_GraphVizName))
                                {
                                    newIdentifier = varLocalSymbolTable[node.Symbol.Text][rootParent.Item1.M_GraphVizName];
                                }
                                else
                                {
                                    newIdentifier = new CIdentifier(node.Symbol.Text);
                                    varLocalSymbolTable[node.Symbol.Text].Add(rootParent.Item1.M_GraphVizName, newIdentifier);
                                }
                            }
                            else
                            {
                                newIdentifier = new CIdentifier(node.Symbol.Text);
                                varLocalSymbolTable.Add(node.Symbol.Text, new Dictionary<string, MINICASTElement>());
                                varLocalSymbolTable[node.Symbol.Text].Add(rootParent.Item1.M_GraphVizName, newIdentifier);
                            }
                        }
                        else if (parent1.Item1.GetType().FullName.Contains("CFCall"))//Fcall identifiers in this function
                        {
                            if (MINICParser.RULE_args == node.Parent.Parent.RuleContext.RuleIndex)//identifiers of agrs in fcall
                            {
                                if (varLocalSymbolTable.ContainsKey(node.Symbol.Text))
                                {
                                    if (varLocalSymbolTable[node.Symbol.Text].ContainsKey(rootParent.Item1.M_GraphVizName))
                                    {
                                        newIdentifier = varLocalSymbolTable[node.Symbol.Text][rootParent.Item1.M_GraphVizName];
                                    }
                                }
                                else if (varGlobalSymbolTable.ContainsKey(node.Symbol.Text))
                                {
                                    newIdentifier = varGlobalSymbolTable[node.Symbol.Text];
                                }
                            }
                            else if (MINICParser.RULE_expression == node.Parent.RuleContext.RuleIndex)//Fcall name identifier
                            {
                                if (functionSymbolTable.ContainsKey(node.Symbol.Text))
                                {
                                    newIdentifier = functionSymbolTable[node.Symbol.Text];
                                }
                                else//........
                                {
                                    newIdentifier = new CIdentifier(node.Symbol.Text);
                                    functionSymbolTable.Add(node.Symbol.Text, newIdentifier);
                                }//........
                            }
                             
                        }
                        else//every other variable in this function
                        {
                            if (varLocalSymbolTable.ContainsKey(node.Symbol.Text))
                            {
                                if (varLocalSymbolTable[node.Symbol.Text].ContainsKey(rootParent.Item1.M_GraphVizName))
                                {
                                    newIdentifier = varLocalSymbolTable[node.Symbol.Text][rootParent.Item1.M_GraphVizName];
                                }
                            }
                            else
                            {
                                if (varGlobalSymbolTable.ContainsKey(node.Symbol.Text))
                                {
                                    newIdentifier = varGlobalSymbolTable[node.Symbol.Text];
                                }
                                else
                                {
                                    newIdentifier = new CIdentifier(node.Symbol.Text);
                                    varLocalSymbolTable.Add(node.Symbol.Text, new Dictionary<string, MINICASTElement>());
                                    varLocalSymbolTable[node.Symbol.Text].Add(rootParent.Item1.M_GraphVizName, newIdentifier);
                                }
                            }
                        }
                    }
                    else if (parent1.Item1.GetType().FullName.Contains("CFCall"))//Fcall name identifier
                    {
                        if (MINICParser.RULE_args == node.Parent.Parent.RuleContext.RuleIndex)//identifier of agrs in fcall 
                        {
                            if (varGlobalSymbolTable.ContainsKey(node.Symbol.Text))
                            {
                                newIdentifier = varGlobalSymbolTable[node.Symbol.Text];
                            }
                        }
                        else if (MINICParser.RULE_expression == node.Parent.RuleContext.RuleIndex)//Fcall name identifier
                        {
                            if (functionSymbolTable.ContainsKey(node.Symbol.Text))
                            {
                                newIdentifier = functionSymbolTable[node.Symbol.Text];
                            }
                            else//........
                            {
                                newIdentifier = new CIdentifier(node.Symbol.Text);
                                functionSymbolTable.Add(node.Symbol.Text, newIdentifier);
                            }//........
                        }
                    }
                    else//identifiers except function names, params and arguments
                    {
                        if (varGlobalSymbolTable.ContainsKey(node.Symbol.Text))
                        {
                            newIdentifier = varGlobalSymbolTable[node.Symbol.Text];
                        }
                        else
                        {
                            newIdentifier = new CIdentifier(node.Symbol.Text);
                            varGlobalSymbolTable.Add(node.Symbol.Text, newIdentifier);
                        }
                    }
                    parent1.Item1.AddChild(newIdentifier, parent1.Item2);
                    break;
                case MINICLexer.NUMBER:
                    CNUMBER newNumber = new CNUMBER(node.Symbol.Text);
                    ValueTuple<MINICASTElement, int> parent2 = m_parents.Peek();
                    parent2.Item1.AddChild(newNumber, parent2.Item2);
                    break;
                case MINICLexer.BREAK:
                    CBreak newBrake = new CBreak();
                    ValueTuple<MINICASTElement, int> parent3 = m_parents.Peek();
                    parent3.Item1.AddChild(newBrake, parent3.Item2);
                    break;
                default:
                    break;
            }

            return 0;
        }

    }
}
