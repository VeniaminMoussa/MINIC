using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MINIC
{

    class MINICTranslation : MINICASTBaseVisitor<int>
    {
        private CFile m_translatedFile;
        public CFile FileBuilder => m_translatedFile;

        private Stack<CEmmitableCodeContainer> m_parents = new Stack<CEmmitableCodeContainer>();
        private Stack<int> m_parentContexts = new Stack<int>();
        private Stack<string> m_functionNames = new Stack<string>();
        private Stack<string> m_functionCalls = new Stack<string>();
        //..........
        private Stack<int> m_for = new Stack<int>();
        //..........
        private Stack<CCompoundStatement> m_scopeCompounds = new Stack<CCompoundStatement>();

        public override int VisitCompileUnit(CCompileUnit node)
        {
            m_translatedFile = new CFile(CodeBlockType.CB_FILE, 3, null);

            m_translatedFile.AddCode("#include <stdio.h>\n", CFile.CB_PREPROCESSOR);
            m_translatedFile.AddCode("#include <stdlib.h>\n", CFile.CB_PREPROCESSOR);

            m_parents.Push(m_translatedFile.MMainFunctionDefinition);
            m_parentContexts.Push(CCFunctionDefinition.CB_FUNCTIONDEFINITION_BODY);
            // Visit Statements Context and emmit code to main functions
            foreach (ASTVisitableElement child in node.GetChildrenContext(CCompileUnit.CT_COMPILEUNIT_STATEMENTS))
            {
                Visit(child);
            }
            m_parents.Pop();    
            m_parentContexts.Pop();

            m_parents.Push(m_translatedFile);
            m_parentContexts.Push(CFile.CB_FUNDEFS);
            // Visit Function Definitions and emmit code to distinct functions
            foreach (ASTVisitableElement child in node.GetChildrenContext(CCompileUnit.CT_COMPILEUNIT_FUNDEFS))
            {
                Visit(child);
            }
            
            m_parents.Pop();
            m_parentContexts.Pop();

            return 0;
        }

        public override int VisitCompoundStatement(CCompoundstatement node)
        {
            CEmmitableCodeContainer parent = m_parents.Peek() as CEmmitableCodeContainer;
            CCompoundStatement rep = new CCompoundStatement(CodeBlockType.CB_COMPOUNDSTATEMENT, 2, parent);
            
            parent.AddCode(rep, m_parentContexts.Peek());
            
            m_parents.Push(rep);
            m_parentContexts.Push(CCompoundStatement.CC_COMPOUNDSTATEMENT_BODY);
            if (m_scopeCompounds.Count() < 1)
            {
                m_scopeCompounds.Push(rep);
            }

            foreach (ASTVisitableElement child in node.GetChildrenContext(CCompoundstatement.CT_BODY))
            {
                Visit(child);
            }
            if (m_scopeCompounds.Count() >= 1)
            {
                m_scopeCompounds.Pop();
            }
            
            m_parents.Pop();
            m_parentContexts.Pop();

            return 0;
        }

        public override int VisitFunction(CFunction node)
        {
            //1. Create Output File
            CFile parent = m_parents.Peek() as CFile;
            CodeContainer repDeclare = new CodeContainer(CodeBlockType.CB_FILE, parent);
            CCFunctionDefinition rep = new CCFunctionDefinition(CodeBlockType.CB_FUNCTIONDEFINITION, 2, parent);

            //2. Add Function Definition to the File in the appropriate context
            parent.AddCode(repDeclare, CFile.CB_GLOBALS);
            parent.AddCode(rep, CFile.CB_FUNDEFS);

            m_parents.Push(rep);
            m_parentContexts.Push(CCFunctionDefinition.CB_FUNCTIONDEFINITION_DECLARATIONS);
            
            //3. Assemble the function header
            CIdentifier id = node.GetChild(CFunction.CT_FNAME, 0) as CIdentifier;
            m_translatedFile.DeclareFunction(id.M_Name);

            rep.AddCode("float " + id.M_Name + "(", CCFunctionDefinition.CB_FUNCTIONDEFINITION_DECLARATIONS);
            repDeclare.AddCode("float " + id.M_Name + "(", CFile.CB_GLOBALS);

            string last = node.GetChildrenContext(CFunction.CT_FARGS).Last().M_GraphVizName;

            foreach (ASTElement s in node.GetChildrenContext(CFunction.CT_FARGS))
            {
                repDeclare.AddCode("float " + s.M_Name, CFile.CB_GLOBALS);
                rep.AddCode("float " + s.M_Name, CCFunctionDefinition.CB_FUNCTIONDEFINITION_DECLARATIONS);

                if (!s.M_GraphVizName.Equals(last))
                {
                    repDeclare.AddCode(", ", CFile.CB_GLOBALS);
                    rep.AddCode(", ", CCFunctionDefinition.CB_FUNCTIONDEFINITION_DECLARATIONS);
                }
                parent.DeclareLocalFunvtionVariable(id.M_Name, s.M_Name);
            }
            repDeclare.AddCode(");\n", CFile.CB_GLOBALS);
            rep.AddCode(")", CCFunctionDefinition.CB_FUNCTIONDEFINITION_DECLARATIONS);

            m_parents.Pop();
            m_parentContexts.Pop();

            m_parents.Push(rep);
            m_parentContexts.Push(CCFunctionDefinition.CB_FUNCTIONDEFINITION_BODY);
            m_functionNames.Push(id.M_Name);

            foreach (ASTVisitableElement child in node.GetChildrenContext(CFunction.CT_BODY))
            {
                Visit(child);
            }

            m_functionNames.Pop();
            m_parents.Pop();
            m_parentContexts.Pop();

            return 0;
        }

        public override int VisitFcall(CFCall node)
        {
            CIdentifier id = node.GetChild(CFCall.CT_FNAME, 0) as CIdentifier;
            m_translatedFile.DeclareFunction(id.M_Name);
            int context = m_parentContexts.Peek();
            CEmmitableCodeContainer rep = m_parents.Peek();
            
            rep.AddCode(id.M_Name, context);
            rep.AddCode("(", context);

            int i = 0;
            int last = node.GetChildrenContextNumber(CFCall.CT_ARGS);

            foreach (ASTVisitableElement child in node.GetChildrenContext(CFCall.CT_ARGS))
            {
                m_functionCalls.Push(id.M_Name);
                Visit(child);
                m_functionCalls.Pop();

                if (!((i + 1) == last))
                {
                    rep.AddCode(", ", context);
                }
                
                i++;
            }

            rep.AddCode(")", context);
            return 0;
        }

        public override int VisitIf(CIf node)
        {
            CEmmitableCodeContainer parent = m_parents.Peek() as CEmmitableCodeContainer;
            CIfStatement rep = new CIfStatement(CodeBlockType.CB_IFSTATEMENTS, 3, parent);

            parent.AddCode(rep, m_parentContexts.Peek());

            m_parents.Push(rep);
            m_parentContexts.Push(CIfStatement.CC_IFSTATEMENT_CONDITION);

            foreach (ASTVisitableElement child in
                node.GetChildrenContext(CIf.CT_CONDITION))
            {
                Visit(child);
            }

            m_parents.Pop();
            m_parentContexts.Pop();

            m_parents.Push(rep);
            m_parentContexts.Push(CIfStatement.CC_IFSTATEMENT_IFBODY);

            // Visit Function Definitions and emmit code to distinct functions
            foreach (ASTVisitableElement child in node.GetChildrenContext(CIf.CT_IFCLAUSE))
            {
                Visit(child);
            }

            m_parents.Pop();
            m_parentContexts.Pop();

            if (node.GetChildrenContext(CIf.CT_ELSECLAUSE).Count() == 0)
                return 0;

            m_parents.Push(rep);
            m_parentContexts.Push(CIfStatement.CC_IFSTATEMENT_ELSEBODY);

            // Visit Function Definitions and emmit code to distinct functions
            foreach (ASTVisitableElement child in node.GetChildrenContext(CIf.CT_ELSECLAUSE))
            {
                Visit(child);
            }

            m_parents.Pop();
            m_parentContexts.Pop();

            return 0;
        }

        public override int VisitWhile(CWhile node)
        {
            CEmmitableCodeContainer parent = m_parents.Peek() as CEmmitableCodeContainer;
            CWhileStatement rep = new CWhileStatement(CodeBlockType.CB_WHILESTATEMENT, 2, parent);

            parent.AddCode(rep, m_parentContexts.Peek());

            m_parents.Push(rep);
            m_parentContexts.Push(CWhileStatement.CC_WHILESTATEMENT_CONDITION);

            foreach (ASTVisitableElement child in
                node.GetChildrenContext(CWhile.CT_CONDITION))
            {
                Visit(child);
            }
            m_parents.Pop();
            m_parentContexts.Pop();

            m_parents.Push(rep);
            m_parentContexts.Push(CWhileStatement.CC_WHILESTATEMENT_BODY);

            // Visit Function Definitions and emmit code to distinct functions
            foreach (ASTVisitableElement child in node.GetChildrenContext(CWhile.CT_BODY))
            {
                Visit(child);
            }

            m_parents.Pop();
            m_parentContexts.Pop();

            return 0;
        }
        //..............
        public override int VisitFor(CFor node)
        {
            // CEmmitableCodeContainer rep = m_parents.Peek();

            CEmmitableCodeContainer parent = m_parents.Peek() as CEmmitableCodeContainer;
            CForStatement rep = new CForStatement(CodeBlockType.CB_FORSTATEMENT, 2, parent);

            parent.AddCode(rep, m_parentContexts.Peek());

            m_parents.Push(rep);
            m_parentContexts.Push(CForStatement.CC_FORARGS_STATEMENT);
            m_for.Push(1);

            int i = 0;
            int last = node.GetChildrenContextNumber(CFor.CT_FOR_ARGS);

            foreach (ASTVisitableElement child in node.GetChildrenContext(CFor.CT_FOR_ARGS))
            {
                if ((last != 3) && (i == 0))
                {
                    rep.AddCode(" ; ", CForStatement.CC_FORARGS_STATEMENT);
                }

                Visit(child);

                if (!((i + 1) == last))
                {
                    rep.AddCode(" ; ", CForStatement.CC_FORARGS_STATEMENT);
                }

                i++;
            }

            m_for.Pop();
            m_parents.Pop();
            m_parentContexts.Pop();

            

            m_parents.Push(rep);
            m_parentContexts.Push(CForStatement.CC_FORSTATEMENT_BODY);

            // Visit Function Definitions and emmit code to distinct functions
            foreach (ASTVisitableElement child in node.GetChildrenContext(CFor.CT_BODY))
            {
                Visit(child);
            }

            m_parents.Pop();
            m_parentContexts.Pop();

            return 0;
        }
        //..............
        public override int VisitAssignment(CAssignment node)
        {
            CEmmitableCodeContainer parent = m_parents.Peek() as CEmmitableCodeContainer;
            int context = m_parentContexts.Peek();

            CodeContainer rep = new CodeContainer(CodeBlockType.CB_CODEREPOSITORY, parent);
            parent.AddCode(rep, m_parentContexts.Peek());

            m_parents.Push(rep);
            m_parentContexts.Push(context);

            foreach (ASTVisitableElement child in
                node.GetChildrenContext(CAssignment.CT_LEFT))
            {
                Visit(child);
            }
            m_parents.Pop();
            m_parentContexts.Pop();

            rep.AddCode("=", context);

            m_parents.Push(rep);
            m_parentContexts.Push(context);
            // Visit Statements Context and emmit code to main functions
            foreach (ASTVisitableElement child in
                node.GetChildrenContext(CAssignment.CT_RIGHT))
            {
                Visit(child);
            }
            m_parents.Pop();
            m_parentContexts.Pop();

            //..........
            if (m_for.Count == 0)
            {
                rep.AddCode(";");
                rep.AddNewLine();
            }
            //..........

            // rep.AddCode(";");
            // rep.AddNewLine();

            return 0;
        }
        //..............
        public override int VisitAddition(CAddition node)
        {
            int context = m_parentContexts.Peek();
            CEmmitableCodeContainer rep = m_parents.Peek();

            foreach (ASTVisitableElement child in
                node.GetChildrenContext(CAddition.CT_LEFT))
            {
                Visit(child);
            }

            rep.AddCode("+", context);
            
            foreach (ASTVisitableElement child in node.GetChildrenContext(CAddition.CT_RIGHT))
            {
                Visit(child);
            }

            return 0;
        }

        public override int VisitSubtraction(CSubtraction node)
        {
            int context = m_parentContexts.Peek();
            CEmmitableCodeContainer rep = m_parents.Peek();

            foreach (ASTVisitableElement child in
                node.GetChildrenContext(CSubtraction.CT_LEFT))
            {
                Visit(child);
            }

            rep.AddCode("-", context);

            foreach (ASTVisitableElement child in node.GetChildrenContext(CSubtraction.CT_RIGHT))
            {
                Visit(child);
            }

            return 0;
        }

        public override int VisitMultiplication(CMultiplication node)
        {
            int context = m_parentContexts.Peek();
            CEmmitableCodeContainer rep = m_parents.Peek();

            foreach (ASTVisitableElement child in
                node.GetChildrenContext(CMultiplication.CT_LEFT))
            {
                Visit(child);
            }

            rep.AddCode("*", context);

            foreach (ASTVisitableElement child in node.GetChildrenContext(CMultiplication.CT_RIGHT))
            {
                Visit(child);
            }

            return 0;
        }

        public override int VisitDivision(CDivision node)
        {
            int context = m_parentContexts.Peek();
            CEmmitableCodeContainer rep = m_parents.Peek();

            foreach (ASTVisitableElement child in
                node.GetChildrenContext(CDivision.CT_LEFT))
            {
                Visit(child);
            }

            rep.AddCode("/", context);

            foreach (ASTVisitableElement child in node.GetChildrenContext(CDivision.CT_RIGHT))
            {
                Visit(child);
            }

            return 0;
        }

        public override int VisitPlus(CPlus node)
        {
            int context = m_parentContexts.Peek();
            CEmmitableCodeContainer rep = m_parents.Peek() as CEmmitableCodeContainer;

            rep.AddCode("(+", context);

            foreach (ASTVisitableElement child in
                node.GetChildrenContext(CPlus.CT_EXPRESSION))
            {
                Visit(child);
            }

            rep.AddCode(")", context);

            return 0;
        }

        public override int VisitMinus(CMinus node)
        {
            int context = m_parentContexts.Peek();
            CEmmitableCodeContainer rep = m_parents.Peek() as CEmmitableCodeContainer;

            rep.AddCode("(-", context);

            foreach (ASTVisitableElement child in
                node.GetChildrenContext(CMinus.CT_EXPRESSION))
            {
                Visit(child);
            }

            rep.AddCode(")", context);

            return 0;
        }

        public override int VisitOr(COr node)
        {
            int context = m_parentContexts.Peek();
            CEmmitableCodeContainer rep = m_parents.Peek() as CEmmitableCodeContainer;

            foreach (ASTVisitableElement child in
                node.GetChildrenContext(COr.CT_LEFT))
            {
                Visit(child);
            }

            rep.AddCode(" || ", context);

            foreach (ASTVisitableElement child in node.GetChildrenContext(COr.CT_RIGHT))
            {
                Visit(child);
            }

            return 0;
        }

        public override int VisitAnd(CAnd node)
        {
            int context = m_parentContexts.Peek();
            CEmmitableCodeContainer rep = m_parents.Peek() as CEmmitableCodeContainer;

            foreach (ASTVisitableElement child in
                node.GetChildrenContext(CAnd.CT_LEFT))
            {
                Visit(child);
            }

            rep.AddCode(" && ", context);

            foreach (ASTVisitableElement child in node.GetChildrenContext(CAnd.CT_RIGHT))
            {
                Visit(child);
            }

            return 0;
        }

        public override int VisitNot(CNot node)
        {
            int context = m_parentContexts.Peek();
            CEmmitableCodeContainer rep = m_parents.Peek() as CEmmitableCodeContainer;

            rep.AddCode("!", context);
            rep.AddCode("(", context);

            foreach (ASTVisitableElement child in
                node.GetChildrenContext(CNot.CT_BODY))
            {
                Visit(child);
            }

            rep.AddCode(")", context);

            return 0;
        }

        public override int VisitEqual(CEqual node)
        {
            int context = m_parentContexts.Peek();
            CEmmitableCodeContainer rep = m_parents.Peek() as CEmmitableCodeContainer;

            foreach (ASTVisitableElement child in
                node.GetChildrenContext(CEqual.CT_LEFT))
            {
                Visit(child);
            }
            rep.AddCode(" == ", context);

            foreach (ASTVisitableElement child in
                node.GetChildrenContext(CEqual.CT_RIGHT))
            {
                Visit(child);
            }

            return 0;
        }

        public override int VisitNequal(CNequal node)
        {
            int context = m_parentContexts.Peek();
            CEmmitableCodeContainer rep = m_parents.Peek() as CEmmitableCodeContainer;

            foreach (ASTVisitableElement child in
                node.GetChildrenContext(CNequal.CT_LEFT))
            {
                Visit(child);
            }
            rep.AddCode(" != ", context);

            foreach (ASTVisitableElement child in
                node.GetChildrenContext(CNequal.CT_RIGHT))
            {
                Visit(child);
            }

            return 0;
        }

        public override int VisitGt(CGt node)
        {
            int context = m_parentContexts.Peek();
            CEmmitableCodeContainer rep = m_parents.Peek() as CEmmitableCodeContainer;

            foreach (ASTVisitableElement child in
                node.GetChildrenContext(CGt.CT_LEFT))
            {
                Visit(child);
            }
            rep.AddCode(" > ", context);

            foreach (ASTVisitableElement child in
                node.GetChildrenContext(CGt.CT_RIGHT))
            {
                Visit(child);
            }

            return 0;
        }

        public override int VisitGte(CGte node)
        {
            int context = m_parentContexts.Peek();
            CEmmitableCodeContainer rep = m_parents.Peek() as CEmmitableCodeContainer;

            foreach (ASTVisitableElement child in
                node.GetChildrenContext(CGte.CT_LEFT))
            {
                Visit(child);
            }
            rep.AddCode(" >= ", context);

            foreach (ASTVisitableElement child in
                node.GetChildrenContext(CGte.CT_RIGHT))
            {
                Visit(child);
            }

            return 0;
        }

        public override int VisitLt(CLt node)
        {
            int context = m_parentContexts.Peek();
            CEmmitableCodeContainer rep = m_parents.Peek() as CEmmitableCodeContainer;

            foreach (ASTVisitableElement child in
                node.GetChildrenContext(CLt.CT_LEFT))
            {
                Visit(child);
            }
            rep.AddCode(" < ",context);

            foreach (ASTVisitableElement child in
                node.GetChildrenContext(CLt.CT_RIGHT))
            {
                Visit(child);
            }

            return 0;
        }

        public override int VisitLte(CLte node)
        {
            int context = m_parentContexts.Peek();
            CEmmitableCodeContainer rep = m_parents.Peek() as CEmmitableCodeContainer;

            foreach (ASTVisitableElement child in
                node.GetChildrenContext(CLte.CT_LEFT))
            {
                Visit(child);
            }
            rep.AddCode(" <= ", context);

            foreach (ASTVisitableElement child in
                node.GetChildrenContext(CLte.CT_RIGHT))
            {
                Visit(child);
            }

            return 0;
        }

        public override int VisitReturn(CReturn node)
        {
            int context = m_parentContexts.Peek();
            CEmmitableCodeContainer rep = m_parents.Peek() as CEmmitableCodeContainer;

            rep.AddCode("return ", context);

            foreach (ASTVisitableElement child in node.GetChildrenContext(CReturn.CT_EXPRESSION))
            {
                Visit(child);
            }

            rep.AddCode(";", context);

            return 0;
        }

        public override int VisitBREAK(CBreak node)
        {
            int context = m_parentContexts.Peek();
            CEmmitableCodeContainer rep = m_parents.Peek() as CEmmitableCodeContainer;
            rep.AddNewLine(context);
            rep.AddCode("break;", context);

            return 0;
        }

        public override int VisitIDENTIFIER(CIdentifier node)
        {
            int context = m_parentContexts.Peek();
            CEmmitableCodeContainer rep = m_parents.Peek();

            if (m_functionNames.Count != 0)
            {
                string parent_Name = m_functionNames.Peek();

                if (FileBuilder.DeclareLocalFunvtionVariable(parent_Name, node.M_Name))
                {
                    if (m_scopeCompounds.Count != 0)
                    {
                        CCompoundStatement repFunction = m_scopeCompounds.Peek() as CCompoundStatement;
                        repFunction.AddCode("float " + node.M_Name + ";\n", CCompoundStatement.CC_COMPOUNDSTATEMENT_DECLARATIONS);
                    }
                }
                rep.AddCode(node.M_Name, context);
            }
            else
            {
                if (m_functionCalls.Count != 0)
                {
                    rep.AddCode(node.M_Name, context);
                }
                else
                {
                    FileBuilder.DeclareGlobalVariable(node.M_Name);
                    rep.AddCode(node.M_Name, context);
                }
            }

            return 0;
        }

        public override int VisitNUMBER(CNUMBER node)
        {
            int context = m_parentContexts.Peek();

            CEmmitableCodeContainer parent = m_parents.Peek();
            parent.AddCode(node.Number, context);

            return 0;
        }
    }
}
