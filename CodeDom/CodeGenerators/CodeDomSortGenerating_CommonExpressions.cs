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
        }

    }
}