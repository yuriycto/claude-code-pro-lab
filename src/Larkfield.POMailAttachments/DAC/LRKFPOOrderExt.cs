using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.AP;
using PX.Objects.PO;

namespace Larkfield.POMailAttachments
{
	/// <summary>
	/// Per order switch (PO301000, Other tab) that adds the files attached to the order to the vendor email.
	/// Also shown, read only, on Print/Email Purchase Orders (PO503000) through POPrintOrderOwned.
	/// </summary>
	public sealed class LRKFPOOrderExt : PXCacheExtension<POOrder>
	{
		public static bool IsActive() => true;

		#region UsrLRKFAttachFiles
		/// <summary>
		/// Takes the vendor default when the order is created and again whenever the vendor changes.
		/// </summary>
		[PXDBBool]
		[PXDefault(typeof(SelectFrom<Vendor>
			.Where<Vendor.bAccountID.IsEqual<POOrder.vendorID.FromCurrent>>
			.SearchFor<LRKFVendorExt.usrLRKFAttachFiles>),
			PersistingCheck = PXPersistingCheck.Nothing)]
		[PXFormula(typeof(Default<POOrder.vendorID>))]
		[PXUIField(DisplayName = Messages.AttachFilesToEmail)]
		public bool? UsrLRKFAttachFiles { get; set; }
		public abstract class usrLRKFAttachFiles : PX.Data.BQL.BqlBool.Field<usrLRKFAttachFiles> { }
		#endregion
	}
}
