using PX.Common;

namespace Larkfield.POMailAttachments
{
	[PXLocalizable]
	public static class Messages
	{
		public const string AttachFilesToEmail = "Attach Files to Email";
		public const string AttachPOFilesToEmail = "Attach PO Files to Email";

		public const string LargeEmailHeader = "Large Email";

		/// <summary>{0} order, {1} size of its files in MB, {2} limit in MB.</summary>
		public const string LargeEmailSingle = "The files attached to purchase order {0} add up to {1} MB, which is more than the {2} MB many mail servers accept. Create the email anyway?";

		/// <summary>{0} list of orders with sizes, {1} limit in MB.</summary>
		public const string LargeEmailBatch = "The files attached to these purchase orders are larger than the {1} MB many mail servers accept: {0}. Create the emails anyway?";
	}
}
