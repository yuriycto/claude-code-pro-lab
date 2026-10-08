using System;

namespace Larkfield.POMailAttachments
{
	/// <summary>
	/// A file attached to a purchase order, reduced to what the mailer needs.
	/// </summary>
	public sealed class OrderFile
	{
		public OrderFile(Guid fileId, string name, long sizeBytes)
		{
			if (fileId == Guid.Empty)
				throw new ArgumentException("File ID must not be empty.", nameof(fileId));

			if (sizeBytes < 0)
				throw new ArgumentOutOfRangeException(nameof(sizeBytes), sizeBytes, "File size cannot be negative.");

			FileId = fileId;
			Name = name ?? string.Empty;
			SizeBytes = sizeBytes;
		}

		public Guid FileId { get; }

		public string Name { get; }

		/// <summary>
		/// Size of the current revision of the file.
		/// </summary>
		public long SizeBytes { get; }
	}
}
