using System;
using System.Collections.Generic;
using System.Linq;

namespace Larkfield.POMailAttachments
{
	/// <summary>
	/// The files one purchase order email carries on top of the printed order form.
	/// </summary>
	public sealed class OrderMailPlan
	{
		private OrderMailPlan(string orderType, string orderNbr, IReadOnlyList<OrderFile> files)
		{
			OrderType = orderType;
			OrderNbr = orderNbr;
			Files = files;
			TotalBytes = files.Sum(file => file.SizeBytes);
		}

		/// <summary>
		/// Builds the plan for one order. When the order does not ask for its files, the plan is empty
		/// and the email keeps the native content. A file linked twice is sent once.
		/// </summary>
		public static OrderMailPlan Create(string orderType, string orderNbr, bool attachFiles, IEnumerable<OrderFile> files)
		{
			if (string.IsNullOrWhiteSpace(orderNbr))
				throw new ArgumentException("Order number is required.", nameof(orderNbr));

			List<OrderFile> selected = attachFiles && files != null
				? files.Where(file => file != null)
					.GroupBy(file => file.FileId)
					.Select(group => group.First())
					.ToList()
				: new List<OrderFile>();

			return new OrderMailPlan(orderType?.Trim() ?? string.Empty, orderNbr.Trim(), selected);
		}

		/// <summary>
		/// A key that identifies an order across graphs and caches.
		/// </summary>
		public static string KeyOf(string orderType, string orderNbr)
			=> (orderType?.Trim() ?? string.Empty) + "|" + (orderNbr?.Trim() ?? string.Empty);

		public string OrderType { get; }

		public string OrderNbr { get; }

		public string Key => KeyOf(OrderType, OrderNbr);

		/// <summary>
		/// The order as a user sees it, for example "RO 000123".
		/// </summary>
		public string DisplayName => OrderType.Length == 0 ? OrderNbr : OrderType + " " + OrderNbr;

		public IReadOnlyList<OrderFile> Files { get; }

		public IEnumerable<Guid> FileIds => Files.Select(file => file.FileId);

		public long TotalBytes { get; }

		public bool HasFiles => Files.Count > 0;
	}
}
