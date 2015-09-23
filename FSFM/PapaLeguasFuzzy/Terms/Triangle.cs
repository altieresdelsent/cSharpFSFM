using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using LinearAlgebra;
namespace PapaLeguasFuzzy.Terms
{
    public class Triangle : Term
    {
        public Triangle(double a, double c,string name, Variable Variable)
            : this(a,(a+c)/2.0d,c,name, Variable)
        {
        }

        public Triangle(double a, double b, double c,string name, Variable Variable)
            : base(name, Variable)
        {
            this.A = Math.Min(a,c);
            this.C = Math.Max(a, c);
            this.B = b;
            this.ALinearFunc = new LinearFunc(this.A, 0.0d, this.B, 1.0d);
            this.CLinearFunc = new LinearFunc(this.B, 1.0d, this.C, 0.0d);
        }
        public double A { get; private set; }
        public double B { get; private set; }
        public double C { get; private set; }
        public LinearFunc ALinearFunc { get; private set; }
        public LinearFunc CLinearFunc { get; private set; }

        public double Area
        {
            get
            {
                return ((A - C) / 2.0d);
            }
        }

        public double CenterOfGravity
        {
            get
            {
                return ((C - B) == (A + B)) ? B : (A + B + C) / 3.0d;
            }
        }

        public double AreaCenterOfGravity
        {
            get
            {
                return Area * CenterOfGravity;
            }
        }
        protected override Expression GenerateAssign(ParameterExpression nameVariableX, ParameterExpression nameVariableY)
        {
            var constant1 = Expression.Constant(1.0d, typeof(double));
            var constant0 = Expression.Constant(0.0d, typeof(double));
            var constantBminusA = Expression.Constant(B - A, typeof(double));
            var constantCminusB = ((B - A) == (C - B))? constantBminusA : Expression.Constant(C - B, typeof(double));
            var constantA = Expression.Constant(A, typeof(double));
            var constantB = Expression.Constant(B, typeof(double));
            var constantC = Expression.Constant(C, typeof(double));
            var vertexA = Expression.Divide(Expression.Subtract(nameVariableX, constantA), constantBminusA);
            var vertexC = Expression.Divide(Expression.Subtract(constantC, nameVariableX), constantCminusB);
            var condition1 = Expression.Or(Expression.LessThan(nameVariableX, constantA), Expression.GreaterThan(nameVariableX, constantC));
            var condition2 = Expression.Equal(nameVariableX, constantB);
            var condition3 = Expression.LessThan(nameVariableX, constantB);
            var formulaTriangle = Expression.Condition(condition1,
                    constant0,
                    Expression.Condition(condition2,
                         constant1,
                        Expression.Condition(condition3,
                            vertexA,
                            vertexC
                        )
                    )
                );
            return Expression.Assign(nameVariableY,formulaTriangle);
            //if (crispValue < tMember.vertexA) || (crispValue > tMember.vertexC)
            //    return tMember.height * 0.0;
            //elseif crispValue == tMember.vertexB
            //    return tMember.height * 1.0;
            //elseif crispValue < tMember.vertexB
            //    return tMember.height * (crispValue - tMember.vertexA) / (tMember.vertexB - tMember.vertexA);
            //else
            //    return tMember.height * (tMember.vertexC - crispValue) / (tMember.vertexC - tMember.vertexB);
            //end
        }
    }
}
