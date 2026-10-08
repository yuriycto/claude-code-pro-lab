using System;
using System.Collections.Generic;
using System.Linq;

namespace Larkfield.POMailAttachments
{
	public static class OrderListText
	{
		/// <summary>
		/// Lists orders for a confirmation dialog without letting it grow unbounded,
		/// for example "RO 000101 (26.3 MB), RO 000102 (31 MB) (+4)".
		/// The output holds no words, so the surrounding localized message stays translatable.
		/// </summary>
		public static string Describe(IReadOnlyList<OrderMailPlan> plans, int maxListed = 3)
		{
			if (maxListed < 1)
				throw new ArgumentOutOfRangeException(nameof(maxListed), maxListed, "At least one order must be listed.");

			if (plans == null || plans.Count == 0)
				return string.Empty;

			string listed = string.Join(", ", plans
				.Take(maxListed)
				.Select(plan => plan.DisplayName + " (" + SizeUnits.ToMegabytes(plan.TotalBytes) + " MB)"));

			int rest = plans.Count - maxListed;
			return rest > 0 ? listed + " (+" + rest + ")" : listed;
		}
	}
}
