using System;
using System.Globalization;

namespace Larkfield.POMailAttachments
{
	public static class SizeUnits
	{
		public const long BytesPerKilobyte = 1024;

		public const long BytesPerMegabyte = 1024 * 1024;

		/// <summary>
		/// Acumatica keeps the size of a file revision in kilobytes. Missing or negative sizes count as zero.
		/// </summary>
		public static long FromKilobytes(int? kilobytes)
			=> kilobytes > 0 ? kilobytes.Value * BytesPerKilobyte : 0;

		/// <summary>
		/// Megabytes with at most one decimal, rounded up so a size above the limit never prints as equal to it.
		/// </summary>
		public static string ToMegabytes(long bytes)
		{
			if (bytes <= 0)
				return "0";

			decimal tenths = Math.Ceiling(bytes * 10m / BytesPerMegabyte);
			return (tenths / 10m).ToString("0.#", CultureInfo.InvariantCulture);
		}
	}
}
