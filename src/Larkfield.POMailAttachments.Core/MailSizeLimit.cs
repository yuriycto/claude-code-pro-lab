using System;
using System.Collections.Generic;
using System.Linq;

namespace Larkfield.POMailAttachments
{
	/// <summary>
	/// The message size above which a mail server is likely to reject an email.
	/// </summary>
	public sealed class MailSizeLimit
	{
		/// <summary>
		/// 25 MB, a limit many public and corporate mail servers apply to a single message.
		/// </summary>
		public const long DefaultBytes = 25L * SizeUnits.BytesPerMegabyte;

		public static readonly MailSizeLimit Default = new MailSizeLimit(DefaultBytes);

		public MailSizeLimit(long bytes)
		{
			if (bytes <= 0)
				throw new ArgumentOutOfRangeException(nameof(bytes), bytes, "The limit must be positive.");

			Bytes = bytes;
		}

		public long Bytes { get; }

		/// <summary>
		/// A message exactly at the limit is still accepted.
		/// </summary>
		public bool IsExceededBy(long totalBytes) => totalBytes > Bytes;

		/// <summary>
		/// Returns the plans whose files alone already go over the limit, in their original order.
		/// </summary>
		public IReadOnlyList<OrderMailPlan> FindOversized(IEnumerable<OrderMailPlan> plans)
		{
			if (plans == null)
				return new List<OrderMailPlan>();

			return plans
				.Where(plan => plan != null && plan.HasFiles && IsExceededBy(plan.TotalBytes))
				.ToList();
		}
	}
}
