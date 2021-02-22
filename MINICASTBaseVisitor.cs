using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MINIC
{
    public abstract class  MINICASTBaseVisitor<T> : ASTBaseVisitor<T>
    {
        public virtual T VisitCompileUnit(CCompileUnit node)
        {
            return VisitChildren(node);
        }

        public virtual T VisitCompoundStatement(CCompoundstatement node)
        {
            return VisitChildren(node);
        }

        public virtual T VisitFunction(CFunction node)
        {
            return VisitChildren(node);
        }
        
        public virtual T VisitFcall(CFCall node)
        {
            return VisitChildren(node);
        }

        public virtual T VisitPlus(CPlus node)
        {
            return VisitChildren(node);
        }

        public virtual T VisitMinus(CMinus node)
        {
            return VisitChildren(node);
        }
        
        public virtual T VisitIf(CIf node)
        {
            return VisitChildren(node);
        }

        public virtual T VisitWhile(CWhile node)
        {
            return VisitChildren(node);
        }
        //............
        public virtual T VisitFor(CFor node)
        {
            return VisitChildren(node);
        }

        //............
        public virtual T VisitAssignment(CAssignment node)
        {
            return VisitChildren(node);
        }

        public virtual T VisitAddition(CAddition node)
        {
            return VisitChildren(node);
        }

        public virtual T VisitSubtraction(CSubtraction node)
        {
            return VisitChildren(node);
        }

        public virtual T VisitMultiplication(CMultiplication node)
        {
            return VisitChildren(node);
        }

        public virtual T VisitDivision(CDivision node)
        {
            return VisitChildren(node);
        }

        public virtual T VisitOr(COr node)
        {
            return VisitChildren(node);
        }

        public virtual T VisitAnd(CAnd node)
        {
            return VisitChildren(node);
        }

        public virtual T VisitNot(CNot node)
        {
            return VisitChildren(node);
        }

        public virtual T VisitEqual(CEqual node)
        {
            return VisitChildren(node);
        }

        public virtual T VisitNequal(CNequal node)
        {
            return VisitChildren(node);
        }

        public virtual T VisitGt(CGt node)
        {
            return VisitChildren(node);
        }

        public virtual T VisitGte(CGte node)
        {
            return VisitChildren(node);
        }

        public virtual T VisitLt(CLt node)
        {
            return VisitChildren(node);
        }

        public virtual T VisitLte(CLte node)
        {
            return VisitChildren(node);
        }

        public virtual T VisitReturn(CReturn node)
        {
            return VisitChildren(node);
        }

        public virtual T VisitBREAK(CBreak node)
        {
            return default(T);
        }

        public virtual T VisitIDENTIFIER(CIdentifier node)
        {
            return default(T);
        }

        public virtual T VisitNUMBER(CNUMBER node)
        {
            return default(T);
        }
    }
}
