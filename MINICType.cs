using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MINIC
{
    public class MINICType : CNodeType<MINICType.NodeType>
    {
        public MINICType(NodeType m_nodeType) : base(m_nodeType) { }

        public enum NodeType
        {
            NT_NA,
            NT_COMPILEUNIT,

            NT_COMPOUNDSTATEMENT,

            NT_FUNCTION,
            NT_FCALL,
            NT_IFSTATEMENT,
            NT_WHILESTATEMENT,
            //............
            NT_FORSTATEMENT,
            //............

            NT_RETURN,
            NT_BREAK,

            NT_ASSIGNMENT,
            NT_ADDTION,
            NT_SUBTRACTION,
            NT_DIVISION,
            NT_MULTIPLICATION,

            NT_PLUS,
            NT_MINUS,

            NT_OR,
            NT_AND,
            NT_NOT,

            NT_EQUAL,
            NT_NEQUAL,
            NT_GT,
            NT_LT,
            NT_GTE,
            NT_LTE,

            NT_IDENTIFIER,
            NT_NUMBER,

            NT_FARGS,

        }

        public override NodeType Default()
        {
            return NodeType.NT_NA;
        }

        public override NodeType Map(int type)
        {
            return (NodeType)type;
        }

        public override int Map(NodeType type)
        {
            return (int)type;
        }

        public override NodeType NA()
        {
            return NodeType.NT_NA;
        }
    }

    public abstract class MINICASTElement : ASTVisitableElement
    {
        private MINICType m_nodeType;

        protected MINICASTElement(int context, MINICType.NodeType Type) : base(context)
        {
            this.m_nodeType = new MINICType(Type);
        }
    }

    //Depends on grammar...

    public class CCompileUnit : MINICASTElement
    {
        public const int CT_COMPILEUNIT_STATEMENTS = 0, CT_COMPILEUNIT_FUNDEFS = 1;

        public static readonly string[] ContextNames = {
            "COMPILEUNIT_STATEMENTSCONTEXT", "COMPILEUNIT_FUNDEFSCONTEXT"
        };

        public CCompileUnit() : base(context: 2, MINICType.NodeType.NT_COMPILEUNIT) {
            M_GraphVizName = "CCompileUnit" + M_GraphVizName;
        }

        public override T Accept<T>(ASTBaseVisitor<T> visitor)
        {
            MINICASTBaseVisitor<T> visitor_ = visitor as MINICASTBaseVisitor<T>;

            if(visitor_!= null)
            {
                visitor_.VisitCompileUnit(node: this);
            }

            return default(T);

        }
    }

    public class CFunction : MINICASTElement
    {
        public const int CT_FNAME = 0;
        public const int CT_FARGS = 1;
        public const int CT_BODY = 2;

        public static readonly string[] ContextNames = {
            "FUNCTION_NAMECONTEXT","FUNCTION_ARGSCONTEXT", "FUNCTION_BODYCONTEXT"
        };

        public CFunction() : base(context: 3, MINICType.NodeType.NT_FUNCTION) {
            M_GraphVizName = "CFunction" + M_GraphVizName;
        }

        public override T Accept<T>(ASTBaseVisitor<T> visitor)
        {
            MINICASTBaseVisitor<T> visitor_ = visitor as MINICASTBaseVisitor<T>;

            if (visitor_ != null)
            {
                visitor_.VisitFunction(node: this);
            }

            return default(T);

        }
    }

    public class CFCall : MINICASTElement
    {
        public const int CT_FNAME = 0;
        public const int CT_ARGS = 1;

        public static readonly string[] ContextNames = {
            "FCALL_NAMECONTEXT","FCALL_ARGSCONTEXT"
        };

        public CFCall() : base(context: 2, MINICType.NodeType.NT_FCALL)
        {
            M_GraphVizName = "CFCall" + M_GraphVizName;
        }

        public override T Accept<T>(ASTBaseVisitor<T> visitor)
        {
            MINICASTBaseVisitor<T> visitor_ = visitor as MINICASTBaseVisitor<T>;

            if (visitor_ != null)
            {
                visitor_.VisitFcall(node: this);
            }

            return default(T);

        }
    }

    public class CCompoundstatement : MINICASTElement
    {
        public const int CT_BODY = 0;

        public static readonly string[] ContextNames = {
            "COMPOUNDSTATEMENT_BODYCONTEXT"
        };

        public CCompoundstatement() : base(context: 1, MINICType.NodeType.NT_COMPOUNDSTATEMENT) {
            M_GraphVizName = "CCompoundstatement" + M_GraphVizName;
        }

        public override T Accept<T>(ASTBaseVisitor<T> visitor)
        {
            MINICASTBaseVisitor<T> visitor_ = visitor as MINICASTBaseVisitor<T>;

            if (visitor_ != null)
            {
                visitor_.VisitCompoundStatement(node: this);
            }

            return default(T);

        }
    }

    public class CIf : MINICASTElement
    {
        public const int CT_CONDITION = 0;
        public const int CT_IFCLAUSE = 1;
        public const int CT_ELSECLAUSE = 2;

        public static readonly string[] ContextNames = {
            "IF_CONDITIONCONTEXT","IF_IFCLAUSECONTEXT", "IF_ELSECLAUSECONTEXT"
        };

        public CIf() : base(context: 3, MINICType.NodeType.NT_IFSTATEMENT) {
            M_GraphVizName = "CIf" + M_GraphVizName;
        }

        public override T Accept<T>(ASTBaseVisitor<T> visitor)
        {
            MINICASTBaseVisitor<T> visitor_ = visitor as MINICASTBaseVisitor<T>;

            if (visitor_ != null)
            {
                visitor_.VisitIf(node: this);
            }

            return default(T);

        }
    }

    public class CWhile : MINICASTElement
    {
        public const int CT_CONDITION = 0;
        public const int CT_BODY = 1;

        public static readonly string[] ContextNames = {
            "WHILE_CONDITIONCONTEXT","WHILE_BODYCONTEXT"
        };

        public CWhile() : base(context: 2, MINICType.NodeType.NT_WHILESTATEMENT) {
            M_GraphVizName = "CWhile" + M_GraphVizName;
        }

        public override T Accept<T>(ASTBaseVisitor<T> visitor)
        {
            MINICASTBaseVisitor<T> visitor_ = visitor as MINICASTBaseVisitor<T>;

            if (visitor_ != null)
            {
                visitor_.VisitWhile(node: this);
            }

            return default(T);

        }
    }
    //............
    public class CFor : MINICASTElement
    {
        public const int CT_FOR_ARGS = 0;
        public const int CT_BODY = 1;

        public static readonly string[] ContextNames = {
            "FOR_FOR_ARGSCONTEXT","FOR_BODYCONTEXT"
        };

        public CFor() : base(context: 2, MINICType.NodeType.NT_FORSTATEMENT)
        {
            M_GraphVizName = "CFor" + M_GraphVizName;
        }

        public override T Accept<T>(ASTBaseVisitor<T> visitor)
        {
            MINICASTBaseVisitor<T> visitor_ = visitor as MINICASTBaseVisitor<T>;

            if (visitor_ != null)
            {
                visitor_.VisitFor(node: this);
            }

            return default(T);

        }
    }
    //............
    public class CReturn : MINICASTElement
    {
        public const int CT_EXPRESSION = 0;

        public static readonly string[] ContextNames = {
            "RETURN_EXPRESSIONCONTEXT"
        };

        public CReturn() : base(context: 1, MINICType.NodeType.NT_RETURN) {
            M_GraphVizName = "CReturn" + M_GraphVizName;
        }

        public override T Accept<T>(ASTBaseVisitor<T> visitor)
        {
            MINICASTBaseVisitor<T> visitor_ = visitor as MINICASTBaseVisitor<T>;

            if (visitor_ != null)
            {
                visitor_.VisitReturn(node: this);
            }

            return default(T);

        }
    }

    public class CBreak : MINICASTElement
    {
        public CBreak() : base(context: 0, MINICType.NodeType.NT_BREAK) {
            M_GraphVizName = "CBreak" + M_GraphVizName;
        }

        public override T Accept<T>(ASTBaseVisitor<T> visitor)
        {
            MINICASTBaseVisitor<T> visitor_ = visitor as MINICASTBaseVisitor<T>;

            if (visitor_ != null)
            {
                visitor_.VisitBREAK(node: this);
            }

            return default(T);

        }
    }

    public class CAssignment : MINICASTElement
    {
        public const int CT_LEFT = 0;
        public const int CT_RIGHT = 1;

        public static readonly string[] ContextNames = {
            "ASSIGNMENT_LEFTCONTEXT","ASSIGNMENT_RIGHTCONTEXT"
        };

        public CAssignment() : base(context: 2, MINICType.NodeType.NT_ASSIGNMENT) {
            M_GraphVizName = "CAssignment" + M_GraphVizName;
        }


        public override T Accept<T>(ASTBaseVisitor<T> visitor)
        {
            MINICASTBaseVisitor<T> visitor_ = visitor as MINICASTBaseVisitor<T>;

            if (visitor_ != null)
            {
                visitor_.VisitAssignment(node: this);
            }

            return default(T);

        }
    }

    public class CAddition : MINICASTElement
    {
        public const int CT_LEFT = 0;
        public const int CT_RIGHT = 1;

        public static readonly string[] ContextNames = {
            "ADDITION_LEFTCONTEXT","ADDITION_RIGHTCONTEXT"
        };

        public CAddition() : base(context: 2, MINICType.NodeType.NT_ADDTION) {
            M_GraphVizName = "CAddition" + M_GraphVizName;
        }

        public override T Accept<T>(ASTBaseVisitor<T> visitor)
        {
            MINICASTBaseVisitor<T> visitor_ = visitor as MINICASTBaseVisitor<T>;

            if (visitor_ != null)
            {
                visitor_.VisitAddition(node: this);
            }

            return default(T);
        }
    }

    public class CSubtraction : MINICASTElement
    {
        public const int CT_LEFT = 0;
        public const int CT_RIGHT = 1;

        public static readonly string[] ContextNames = {
            "SUBTRACTION_LEFTCONTEXT","SUBTRACTION_RIGHTCONTEXT"
        };

        public CSubtraction() : base(context: 2, MINICType.NodeType.NT_SUBTRACTION) {
            M_GraphVizName = "CSubtraction" + M_GraphVizName;
        }

        public override T Accept<T>(ASTBaseVisitor<T> visitor)
        {
            MINICASTBaseVisitor<T> visitor_ = visitor as MINICASTBaseVisitor<T>;

            if (visitor_ != null)
            {
                visitor_.VisitSubtraction(node: this);
            }

            return default(T);

        }
    }

    public class CMultiplication : MINICASTElement
    {
        public const int CT_LEFT = 0;
        public const int CT_RIGHT = 1;

        public static readonly string[] ContextNames = {
            "MULTIPLICATION_LEFTCONTEXT","MULTIPLICATION_RIGHTCONTEXT"
        };

        public CMultiplication() : base(context: 2, MINICType.NodeType.NT_MULTIPLICATION) {
            M_GraphVizName = "CMultiplication" + M_GraphVizName;
        }

        public override T Accept<T>(ASTBaseVisitor<T> visitor)
        {
            MINICASTBaseVisitor<T> visitor_ = visitor as MINICASTBaseVisitor<T>;

            if (visitor_ != null)
            {
                visitor_.VisitMultiplication(node: this);
            }

            return default(T);

        }
    }

    public class CDivision : MINICASTElement
    {
        public const int CT_LEFT = 0;
        public const int CT_RIGHT = 1;

        public static readonly string[] ContextNames = {
            "DIVISION_LEFTCONTEXT","DIVISION_RIGHTCONTEXT"
        };

        public CDivision() : base(context: 2, MINICType.NodeType.NT_DIVISION) {
            M_GraphVizName = "CDivision" + M_GraphVizName;
        }

        public override T Accept<T>(ASTBaseVisitor<T> visitor)
        {
            MINICASTBaseVisitor<T> visitor_ = visitor as MINICASTBaseVisitor<T>;

            if (visitor_ != null)
            {
                visitor_.VisitDivision(node: this);
            }

            return default(T);

        }
    }
    
    public class CPlus : MINICASTElement
    {
        public const int CT_EXPRESSION = 0;

        public static readonly string[] ContextNames = {
            "PLUS_EXPRESSIONCONTEXT"
        };

        public CPlus() : base(context: 1, MINICType.NodeType.NT_PLUS)
        {
            M_GraphVizName = "CPlus" + M_GraphVizName;
        }

        public override T Accept<T>(ASTBaseVisitor<T> visitor)
        {
            MINICASTBaseVisitor<T> visitor_ = visitor as MINICASTBaseVisitor<T>;

            if (visitor_ != null)
            {
                visitor_.VisitPlus(node: this);
            }

            return default(T);

        }
    }

    public class CMinus : MINICASTElement
    {
        public const int CT_EXPRESSION = 0;

        public static readonly string[] ContextNames = {
            "MINUS_EXPRESSIONCONTEXT"
        };

        public CMinus() : base(context: 1, MINICType.NodeType.NT_MINUS)
        {
            M_GraphVizName = "CMinus" + M_GraphVizName;
        }

        public override T Accept<T>(ASTBaseVisitor<T> visitor)
        {
            MINICASTBaseVisitor<T> visitor_ = visitor as MINICASTBaseVisitor<T>;

            if (visitor_ != null)
            {
                visitor_.VisitMinus(node: this);
            }

            return default(T);

        }
    }
    
    public class CAnd : MINICASTElement
    {
        public const int CT_LEFT = 0;
        public const int CT_RIGHT = 1;

        public static readonly string[] ContextNames = {
            "AND_LEFTCONTEXT","AND_RIGHTCONTEXT"
        };

        public CAnd() : base(context: 2, MINICType.NodeType.NT_AND) {
            M_GraphVizName = "CAnd" + M_GraphVizName;
        }

        public override T Accept<T>(ASTBaseVisitor<T> visitor)
        {
            MINICASTBaseVisitor<T> visitor_ = visitor as MINICASTBaseVisitor<T>;

            if (visitor_ != null)
            {
                visitor_.VisitAnd(node: this);
            }

            return default(T);

        }
    }

    public class COr : MINICASTElement
    {
        public const int CT_LEFT = 0;
        public const int CT_RIGHT = 1;

        public static readonly string[] ContextNames = {
            "OR_LEFTCONTEXT","OR_RIGHTCONTEXT"
        };

        public COr() : base(context: 2, MINICType.NodeType.NT_OR)
        {
            M_GraphVizName = "COr" + M_GraphVizName;
        }

        public override T Accept<T>(ASTBaseVisitor<T> visitor)
        {
            MINICASTBaseVisitor<T> visitor_ = visitor as MINICASTBaseVisitor<T>;

            if (visitor_ != null)
            {
                visitor_.VisitOr(node: this);
            }

            return default(T);

        }
    }

    public class CNot : MINICASTElement
    {
        public const int CT_BODY = 0;

        public static readonly string[] ContextNames = {
            "NOT_BODYCONTEXT"
        };

        public CNot() : base(context: 1, MINICType.NodeType.NT_NOT) {
            M_GraphVizName = "CNot" + M_GraphVizName;
        }

        public override T Accept<T>(ASTBaseVisitor<T> visitor)
        {
            MINICASTBaseVisitor<T> visitor_ = visitor as MINICASTBaseVisitor<T>;

            if (visitor_ != null)
            {
                visitor_.VisitNot(node: this);
            }

            return default(T);

        }
    }

    public class CNequal : MINICASTElement
    {
        public const int CT_LEFT = 0;
        public const int CT_RIGHT = 1;

        public static readonly string[] ContextNames = {
            "NEQUAL_LEFTCONTEXT","NEQUAL_RIGHTCONTEXT"
        };

        public CNequal() : base(context: 2, MINICType.NodeType.NT_NEQUAL) {
            M_GraphVizName = "CNequal" + M_GraphVizName;
        }

        public override T Accept<T>(ASTBaseVisitor<T> visitor)
        {
            MINICASTBaseVisitor<T> visitor_ = visitor as MINICASTBaseVisitor<T>;

            if (visitor_ != null)
            {
                visitor_.VisitNequal(node: this);
            }

            return default(T);

        }
    }

    public class CEqual : MINICASTElement
    {
        public const int CT_LEFT = 0;
        public const int CT_RIGHT = 1;

        public static readonly string[] ContextNames = {
            "EQUAL_LEFTCONTEXT","EQUAL_RIGHTCONTEXT"
        };

        public CEqual() : base(context: 2, MINICType.NodeType.NT_EQUAL) {
            M_GraphVizName = "CEqual" + M_GraphVizName;
        }

        public override T Accept<T>(ASTBaseVisitor<T> visitor)
        {
            MINICASTBaseVisitor<T> visitor_ = visitor as MINICASTBaseVisitor<T>;

            if (visitor_ != null)
            {
                visitor_.VisitEqual(node: this);
            }

            return default(T);

        }
    }

    public class CGt : MINICASTElement
    {
        public const int CT_LEFT = 0;
        public const int CT_RIGHT = 1;

        public static readonly string[] ContextNames = {
            "GT_LEFTCONTEXT","GT_RIGHTCONTEXT"
        };

        public CGt() : base(context: 2, MINICType.NodeType.NT_GT) {
            M_GraphVizName = "CGt" + M_GraphVizName;
        }

        public override T Accept<T>(ASTBaseVisitor<T> visitor)
        {
            MINICASTBaseVisitor<T> visitor_ = visitor as MINICASTBaseVisitor<T>;

            if (visitor_ != null)
            {
                visitor_.VisitGt(node: this);
            }

            return default(T);

        }
    }

    public class CLt : MINICASTElement
    {
        public const int CT_LEFT = 0;
        public const int CT_RIGHT = 1;

        public static readonly string[] ContextNames = {
            "LT_LEFTCONTEXT","LT_RIGHTCONTEXT"
        };

        public CLt() : base(context: 2, MINICType.NodeType.NT_LT) {
            M_GraphVizName = "CLt" + M_GraphVizName;
        }

        public override T Accept<T>(ASTBaseVisitor<T> visitor)
        {
            MINICASTBaseVisitor<T> visitor_ = visitor as MINICASTBaseVisitor<T>;

            if (visitor_ != null)
            {
                visitor_.VisitLt(node: this);
            }

            return default(T);

        }
    }

    public class CGte : MINICASTElement
    {
        public const int CT_LEFT = 0;
        public const int CT_RIGHT = 1;

        public static readonly string[] ContextNames = {
            "GTE_LEFTCONTEXT","GTE_RIGHTCONTEXT"
        };

        public CGte() : base(context: 2, MINICType.NodeType.NT_GTE) {
            M_GraphVizName = "CGte" + M_GraphVizName;
        }

        public override T Accept<T>(ASTBaseVisitor<T> visitor)
        {
            MINICASTBaseVisitor<T> visitor_ = visitor as MINICASTBaseVisitor<T>;

            if (visitor_ != null)
            {
                visitor_.VisitGte(node: this);
            }

            return default(T);

        }
    }

    public class CLte : MINICASTElement
    {
        public const int CT_LEFT = 0;
        public const int CT_RIGHT = 1;

        public static readonly string[] ContextNames = {
            "LTE_LEFTCONTEXT","LTE_RIGHTCONTEXT"
        };

        public CLte() : base(context: 2, MINICType.NodeType.NT_LTE) {
            M_GraphVizName = "CLte" + M_GraphVizName;
        }

        public override T Accept<T>(ASTBaseVisitor<T> visitor)
        {
            MINICASTBaseVisitor<T> visitor_ = visitor as MINICASTBaseVisitor<T>;

            if (visitor_ != null)
            {
                visitor_.VisitLte(node: this);
            }

            return default(T);

        }
    }

    public class CNUMBER : MINICASTElement
    {
        private string number;
        public string Number => number;

        public CNUMBER(string Number) : base(context: 0, MINICType.NodeType.NT_NUMBER)
        {
            this.number = Number;
            M_GraphVizName = "CNUMBER" + M_GraphVizName;
        }

        public override T Accept<T>(ASTBaseVisitor<T> visitor)
        {
            MINICASTBaseVisitor<T> visitor_ = visitor as MINICASTBaseVisitor<T>;

            if (visitor_ != null)
            {
                visitor_.VisitNUMBER(node: this);
            }

            return default(T);

        }
    }

    public class CIdentifier : MINICASTElement
    {
        public CIdentifier(string label) : base(context: 0, MINICType.NodeType.NT_IDENTIFIER)
        {
            M_Name = label;
            M_GraphVizName = "CIdentifier" + M_GraphVizName + "_" + label;
        }

        public override T Accept<T>(ASTBaseVisitor<T> visitor)
        {
            MINICASTBaseVisitor<T> visitor_ = visitor as MINICASTBaseVisitor<T>;

            if (visitor_ != null)
            {
                visitor_.VisitIDENTIFIER(node: this);
            }

            return default(T);

        }
    }

}
