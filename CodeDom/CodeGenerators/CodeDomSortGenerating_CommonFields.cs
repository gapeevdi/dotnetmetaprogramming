using System.CodeDom;

namespace CodeDomSort
{
    public partial class CodeDomSortGenerating
    {
        private readonly CodeArgumentReferenceExpression _lowIndexArgumentReference =
            new CodeArgumentReferenceExpression(Constants.LowIndexParameterName);

        private readonly CodeArgumentReferenceExpression _topIndexArgumentReference =
            new CodeArgumentReferenceExpression(Constants.TopIndexParameterName);

        private readonly CodeArgumentReferenceExpression _arrayArgumentReference =
            new CodeArgumentReferenceExpression(Constants.ArrayParameterName);

        private readonly CodeArgumentReferenceExpression _pivotElementStrategyArgumentReference =
            new CodeArgumentReferenceExpression(Constants.PivotElementStrategyParameterName);

    }
}