using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Linq.Expressions
{
  public static class ExpressionEx
  {
    public static Expression For(Type iteratorType, Expression operand, Func<Expression, Expression> op, Expression initialValue = null)
    {
      var breakLabel = Expression.Label();
      var iterator = Expression.Variable(iteratorType);
      var stop = Expression.Variable(iteratorType);

      return Expression.Block(
        new[] { iterator, stop },
        Expression.Assign(stop, Expression.Convert(operand, iteratorType)),
        Expression.Assign(iterator, initialValue ?? Expression.Constant(Convert.ChangeType(0, iteratorType), iteratorType)),
        Expression.Loop(
            Expression.IfThenElse(
                Expression.LessThan(iterator, stop),
                Expression.Block(
                    op(iterator),
                    Expression.AddAssign(iterator, Expression.Constant(Convert.ChangeType(1, iteratorType), iteratorType))
                ),
                Expression.Break(breakLabel)
            ),
        breakLabel)
      );
    }

    public static T Access<T>(T[] array, long index) => array[index];
    public static Expression ArrayAccess(Expression array, Expression index) => Expression.Call(typeof(ExpressionEx).GetMethod("Access").MakeGenericMethod(array.Type.GetElementType()), array, index);
  }
}
