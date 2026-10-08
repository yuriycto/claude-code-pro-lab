using PX.Data;
using PX.Objects.AP;

namespace Larkfield.POMailAttachments
{
	/// <summary>
	/// Vendor default for sending purchase order files to the vendor (AP303000, Purchase Settings tab).
	/// New vendors start with the option on. Vendors created before the customization keep an empty value,
	/// which is treated as off.
	/// </summary>
	public sealed class LRKFVendorExt : PXCacheExtension<Vendor>
	{
		public static bool IsActive() => true;

		#region UsrLRKFAttachFiles
		[PXDBBool]
		[PXDefault(true, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = Messages.AttachPOFilesToEmail)]
		public bool? UsrLRKFAttachFiles { get; set; }
		public abstract class usrLRKFAttachFiles : PX.Data.BQL.BqlBool.Field<usrLRKFAttachFiles> { }
		#endregion
	}
}
