using System.Collections.Generic;
using System.Linq;
using PX.Common;
using PX.Data;
using PX.Objects.PO;
using PX.SM;

namespace Larkfield.POMailAttachments
{
	/// <summary>
	/// Print/Email Purchase Orders (PO503000). Each selected order follows its own Attach Files to Email
	/// setting, which LRKFPOOrderEntryExt applies when the mass process runs the email action.
	/// Here the user is asked once, before processing starts, if any selected order is too large to email.
	/// </summary>
	public class LRKFPOPrintOrderExt : PXGraphExtension<POPrintOrder>
	{
		public static bool IsActive() => true;

		public override void Initialize()
		{
			Base.Records.SetParametersDelegate(ConfirmLargeEmails);
		}

		protected virtual bool ConfirmLargeEmails(List<POPrintOrder.POPrintOrderOwned> orders)
		{
			if (Base.Filter.Current?.Action != POPrintOrder.WellKnownActions.POOrderScreen.EmailPurchaseOrder
				|| PXContext.GetSlot<AUSchedule>() != null)
				return true;

			IReadOnlyList<OrderMailPlan> oversized = MailSizeLimit.Default.FindOversized(
				orders.Select(order => LRKFOrderMailPlans.For(Base, order)));

			if (oversized.Count == 0)
				return true;

			string message = PXMessages.LocalizeFormatNoPrefix(Messages.LargeEmailBatch,
				OrderListText.Describe(oversized), SizeUnits.ToMegabytes(MailSizeLimit.Default.Bytes));

			return Base.Records.Ask(Messages.LargeEmailHeader, message, MessageButtons.YesNo) == WebDialogResult.Yes;
		}
	}
}
