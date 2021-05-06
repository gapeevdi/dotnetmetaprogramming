using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeDomSort
{
    public static class ArrayExtension
    {
        public static string ToFlatText<T>(this IReadOnlyCollection<T> @this)
        {
            if (@this == null)
            {
                return null;
            }

            return string.Join(", ", @this);
        }

        public static string ToItemCounts<T>(this IReadOnlyCollection<T> @this)
        {
            if (@this == null)
            {
                return null;
            }

            var itemCountReport = new StringBuilder();
            
            @this.GroupBy(item => item, (key, items) => (key:key, count:items.Count()))
                .Aggregate(itemCountReport,
                    (report, itemCountTuple) =>
                        itemCountReport.AppendLine($"item = {itemCountTuple.key},  count = {itemCountTuple.count}"));

            return itemCountReport.ToString();
        }
    }
}