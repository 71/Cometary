using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Cometary.Expressions
{
    /// <summary>
    /// Class that transforms a compiled LINQ query
    /// <see cref="Expression{TDelegate}"/> to a <see cref="QueryExpression"/>.
    /// </summary>
    /// <example>
    /// Expression body = Expressionist.Express(() => from item in items
    ///                                               where item.ID != 0
    ///                                               let user = item.GetUser()
    ///                                               select $"{user.FirstName} {user.LastName}");
    /// 
    /// QueryExpression query = Expressionist.Query(body);
    /// </example>
    internal sealed class QueryExpressionTransformer : ExpressionVisitor
    {
        #region Other members
        private QueryExpression _query;
        private readonly IList<QueryClause> _clauses = new LightList<QueryClause>();

        /// <summary>
        /// 
        /// </summary>
        public Expression Body { get; }

        /// <summary>
        /// 
        /// </summary>
        public QueryExpression Query => _query ?? (_query = Transform());

        /// <summary>
        /// 
        /// </summary>
        internal QueryExpressionTransformer(Expression body)
        {
            Body = body;
        }

        private QueryExpression Transform()
        {
            MethodCallExpression call = Body as MethodCallExpression;

            if (call == null || !call.Type.IsAssignableTo<IEnumerable>())
                throw new ArgumentException("The given body must be a LINQ expression.");

            Expression @new = Visit(Body);

            return Expressive.Query(_clauses.ToArray());
        }
        #endregion

        #region Visiting
        private readonly Dictionary<PropertyInfo, ParameterExpression> declaredVariables =
            new Dictionary<PropertyInfo, ParameterExpression>();

        private readonly IList<ParameterExpression> variables = new LightList<ParameterExpression>();

        public override Expression Visit(Expression node)
        {
            if (node == null)
                return null;

            return base.Visit(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            PropertyInfo prop = node.Member as PropertyInfo;

            if (prop != null && prop.DeclaringType.IsCompilerGenerated())
                return (Expression)GetVariable(prop.PropertyType, prop.Name) ?? node;

            return base.VisitMember(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node == null)
                return null;

            Expression result = base.VisitMethodCall(node);

            node = result as MethodCallExpression;

            if (node == null)
                return result;

            LambdaExpression GetLambda()
                => node.Arguments.Last() as LambdaExpression ?? throw InvalidBody();

            switch (node.Method.Name)
            {
                case nameof(Enumerable.Select):
                    _clauses.Add(VisitSelect(node, GetLambda()));
                    break;
                case nameof(Enumerable.SelectMany):
                    _clauses.Add(VisitSelectMany(node, (LambdaExpression)node.Arguments[1], (LambdaExpression)node.Arguments.Last()));
                    break;
                case nameof(Enumerable.Where):
                    _clauses.Add(VisitWhere(node, GetLambda()));
                    break;
                case nameof(Enumerable.GroupJoin):
                    _clauses.Add(VisitGroupJoin(node));
                    break;
                case nameof(Enumerable.GroupBy):
                    _clauses.Add(VisitGroupBy(node));
                    break;
                case nameof(Enumerable.Join):
                    _clauses.Add(VisitJoin(node));
                    break;
                case nameof(Enumerable.OrderBy):
                    _clauses.Add(VisitOrderBy(node, GetLambda(), false));
                    break;
                case nameof(Enumerable.OrderByDescending):
                    _clauses.Add(VisitOrderBy(node, GetLambda(), true));
                    break;
            }

            return result;
        }

        private static Exception InvalidBody()
        {
            throw new ArgumentException("Invalid body given.");
        }

        private void InitializeCollection(MethodCallExpression node)
        {
            return;
            //if (collection != null)
            //    return;

            //collection = node.Arguments[0];

            //LambdaExpression lambda = node.Arguments[1] as LambdaExpression;

            //if (lambda == null)
            //    throw InvalidBody();

            //item = Expression.Variable(lambda.Parameters[0].Type, lambda.Parameters[0].Name);

            //_clauses.Add(Expressionist.From(item, collection));
        }

        private ParameterExpression GetVariable(Type type, string name)
        {
            foreach (ParameterExpression variable in variables)
            {
                if (variable.Name == name && variable.Type == type)
                    return variable;
            }

            ParameterExpression result = Expression.Variable(type, name);

            variables.Add(result);

            return result;
        }

        private ParameterExpression GetVariable(ParameterExpression variable)
        {
            return GetVariable(variable.Type, variable.Name);
        }

        private QueryClause VisitSelect(MethodCallExpression node, LambdaExpression selector)
        {
            if (selector.Body is NewExpression @new)
            {
                // let {variable} = {expression}
                PropertyInfo prop = (PropertyInfo)@new.Members[1];
                ParameterExpression variable = GetVariable(prop.PropertyType, prop.Name);

                return Expressive.Let(variable, @new.Arguments[1]);
            }

            if (true)
            {
                // select {expression}
                return Expressive.Select(selector.Body);
            }
            else
            {
                
            }

            return null;
        }

        private QueryClause VisitSelectMany(MethodCallExpression node, LambdaExpression selector, LambdaExpression projector)
        {
            if (projector != selector)
            {
                Type transparentType = projector.ReturnType;

                if (transparentType.IsCompilerGenerated())
                {
                    // from {item1} in {collection1}
                    // from {item2} in {collection2}
                    ParameterExpression firstVar = GetVariable(projector.Parameters[0]);
                    ParameterExpression secondVar = GetVariable(projector.Parameters[1]);

                    _clauses.Add(Expressive.From(firstVar, node.Arguments[0]));

                    return Expressive.From(secondVar, selector.Body);
                }
                else
                {
                    // from {item} in {collection}
                    // select {item}
                    ParameterExpression newItem = GetVariable(projector.Parameters[1]);

                    _clauses.Add(Expressive.From(newItem, selector.Body));

                    return Expressive.Select(projector.Body);
                }
            }
            return null;
        }

        private QueryClause VisitWhere(MethodCallExpression node, LambdaExpression predicate)
        {
            if (_clauses.Count == 0)
                _clauses.Add(Expressive.From(GetVariable(predicate.Parameters[0]), node.Arguments[0]));

            return Expressive.Where(predicate.Body);
        }

        private QueryClause VisitGroupJoin(MethodCallExpression node)
        {
            return null;
        }

        private QueryClause VisitGroupBy(MethodCallExpression node)
        {
            return null;
        }

        private QueryClause VisitJoin(MethodCallExpression node)
        {
            return null;
        }

        private QueryClause VisitOrderBy(MethodCallExpression node, LambdaExpression lambda, bool descending)
        {
            if (_clauses.Count == 0)
                _clauses.Add(Expressive.From(GetVariable(lambda.Parameters[1]), node.Arguments[0]));

            return Expressive.OrderBy(lambda.Body, !descending);
        } 
        #endregion
    }
}
