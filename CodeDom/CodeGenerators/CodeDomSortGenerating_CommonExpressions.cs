using System.CodeDom;

namespace CodeDomSort
{
    public partial class CodeDomSortGenerating
    {
        public static class CommonExpressions
        {
            public static readonly CodeArgumentReferenceExpression LowIndexArgumentReference  =
                new CodeArgumentReferenceExpression(Constants.LowIndexParameterName);

            public static readonly CodeArgumentReferenceExpression TopIndexArgumentReference =
                new CodeArgumentReferenceExpression(Constants.TopIndexParameterName);

            public static readonly CodeArgumentReferenceExpression ArrayArgumentReference =
                new CodeArgumentReferenceExpression(Constants.ArrayParameterName);

            public static readonly CodeArgumentReferenceExpression PivotElementStrategyArgumentReference =
                new CodeArgumentReferenceExpression(Constants.PivotElementStrategyParameterName);


            /// <summary>
            /// Returns IComparable<T> declaration
            /// </summary>
            /// <returns></returns>
            public static readonly CodeTypeParameter GenericComparableType = new CodeTypeParameter()
            {

                Name = Constants.GenericTypeName,
                Constraints =
                {
                    new CodeTypeReference("IComparable",
                        new CodeTypeReference(CodeDomSortGenerating.Constants.GenericTypeName))
                }
            };
        }

    }
}