using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MINIC
{
    public abstract class ASTBaseVisitor<T>
    {
        protected ASTVisitableElement currentParent = null;

        public virtual T Visit(ASTVisitableElement node)
        {
            return node.Accept(visitor: this);
        }

        public virtual T VisitChildren(ASTVisitableElement node)
        {
            ASTVisitableElement oldParent = currentParent;
            currentParent = node;
            T result = default(T);
            foreach (ASTVisitableElement child in node.GetChildren())
            {
                result = AggregateResult(result, child.Accept(visitor: this));
            }
            currentParent = oldParent;
            return result;
        }

        public virtual T AggregateResult(T oldResult,T value )
        {
            return value;
        }
    }


}
