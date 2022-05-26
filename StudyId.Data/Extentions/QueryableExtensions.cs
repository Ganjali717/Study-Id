using System.Linq.Expressions;

namespace StudyId.Data.Extentions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, IEnumerable<EntitySorting> sortModels)
        {
            var expression = source.Expression;
            int count = 0;
            foreach (var item in sortModels)
            {
                var parameter = Expression.Parameter(typeof(T), "x");
                var method = !item.SortAsc ?
                    (count == 0 ? "OrderByDescending" : "ThenByDescending") :
                    (count == 0 ? "OrderBy" : "ThenBy");
                if (item.Column.Contains('.'))
                {
                    var leftOperand = item.Column.Split('.')
                        .Aggregate((Expression)parameter, Expression.PropertyOrField);
                    expression = Expression.Call(typeof(Queryable), method,
                        new Type[] { source.ElementType, leftOperand.Type },
                        expression, Expression.Quote(Expression.Lambda(leftOperand, parameter)));
                }
                else
                {
                    var selector = Expression.PropertyOrField(parameter, item.Column);
 
                    expression = Expression.Call(typeof(Queryable), method,
                        new Type[] { source.ElementType, selector.Type },
                        expression, Expression.Quote(Expression.Lambda(selector, parameter)));
                }
                count++;
            }
            return count > 0 ? source.Provider.CreateQuery<T>(expression) : source;
        }
    }

    public class EntitySorting
    {
        public string Column { get; set; }
        public bool SortAsc { get; set; }
    }
}
