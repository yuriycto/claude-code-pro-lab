using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PX.Common;
using PX.Data;
using PX.Objects.CR;
using PX.Objects.PO;
using PX.Objects.PO.GraphExtensions.POOrderEntryExt;
using PX.SM;

namespace Larkfield.POMailAttachments
{
	/// <summary>
	/// Adds each order's own files to the vendor email created by Email Purchase Order.
	/// In 2026 R2 both the PO301000 action and the PO503000 mass process run this action,
	/// so this is the single place where the behaviour lives.
	/// </summary>
	public class LRKFPOOrderEntryExt : PXGraphExtension<POOrderEntry>
	{
		/// <summary>Native mailing ID used when the action is called without one.</summary>
		private const string PurchaseOrderMailing = "PURCHASE ORDER";

		public static bool IsActive() => true;

		public delegate IEnumerable EmailPurchaseOrderDelegate(PXAdapter adapter, string notificationCD);

		[PXOverride]
		public virtual IEnumerable EmailPurchaseOrder(PXAdapter adapter, string notificationCD, EmailPurchaseOrderDelegate baseMethod)
		{
			List<POOrder> orders = adapter.Get<POOrder>().ToList();
			List<OrderMailPlan> plans = orders.Select(order => LRKFOrderMailPlans.For(Base, order)).ToList();

			// Nothing to add: the native action runs exactly as before.
			if (!plans.Any(plan => plan.HasFiles))
				return baseMethod(adapter, notificationCD);

			// PO503000 asks once for the whole selection before processing starts, see LRKFPOPrintOrderExt.
			if (!adapter.MassProcess && !ConfirmLargeEmail(plans))
				return orders;

			SendWithOrderFiles(orders, plans, notificationCD ?? PurchaseOrderMailing, new MassEmailingActionParameters(adapter));
			return orders;
		}

		/// <summary>
		/// Asks the user before creating an email whose files go over the mail size limit.
		/// Imports, API calls and scheduled runs cannot answer a dialog and go ahead as the native action would.
		/// </summary>
		protected virtual bool ConfirmLargeEmail(IReadOnlyList<OrderMailPlan> plans)
		{
			if (Base.IsImport || Base.IsContractBasedAPI || PXContext.GetSlot<AUSchedule>() != null)
				return true;

			IReadOnlyList<OrderMailPlan> oversized = MailSizeLimit.Default.FindOversized(plans);
			if (oversized.Count == 0)
				return true;

			string limit = SizeUnits.ToMegabytes(MailSizeLimit.Default.Bytes);
			string message = oversized.Count == 1
				? PXMessages.LocalizeFormatNoPrefix(Messages.LargeEmailSingle, oversized[0].DisplayName, SizeUnits.ToMegabytes(oversized[0].TotalBytes), limit)
				: PXMessages.LocalizeFormatNoPrefix(Messages.LargeEmailBatch, OrderListText.Describe(oversized), limit);

			return Base.Document.Ask(Messages.LargeEmailHeader, message, MessageButtons.YesNo) == WebDialogResult.Yes;
		}

		/// <summary>
		/// Same flow as the native POOrderEntry.Notification, plus the files of every order.
		/// </summary>
		protected virtual void SendWithOrderFiles(IList<POOrder> orders, IReadOnlyList<OrderMailPlan> plans, string notificationCD, MassEmailingActionParameters emailingParameters)
		{
			POOrder[] documents = orders.ToArray();
			Dictionary<string, Guid?[]> filesByOrder = plans.ToDictionary(
				plan => plan.Key,
				plan => plan.FileIds.Select(id => (Guid?)id).ToArray());

			PXLongOperation.StartOperation(Base, () =>
			{
				var orderEntry = PXGraph.CreateInstance<POOrderEntry>();
				var activities = orderEntry.GetExtension<POOrderEntry_ActivityDetailsExt>();

				// SendNotifications takes one attachment list for all documents. For every document it calls
				// documentParametersGetter first and reads the list right after, in the single, separate and
				// aggregated paths alike, so refilling the list there gives each email its own order's files.
				var attachments = new List<Guid?>();

				activities.SendNotifications(
					sourceTypeGetter: doc => PONotificationSource.Vendor,
					notifications: notificationCD,
					documents: documents,
					branchIDGetter: doc => doc.BranchID,
					documentParametersGetter: doc =>
					{
						attachments.Clear();
						if (filesByOrder.TryGetValue(OrderMailPlan.KeyOf(doc.OrderType, doc.OrderNbr), out Guid?[] files))
							attachments.AddRange(files);

						return new Dictionary<string, string>
						{
							[nameof(POOrder) + "." + nameof(POOrder.OrderType)] = doc.OrderType,
							[nameof(POOrder) + "." + nameof(POOrder.OrderNbr)] = doc.OrderNbr,
						};
					},
					emailingParameters: emailingParameters,
					groupingQualifier: doc => doc.VendorID,
					attachments: attachments);
			});
		}
	}
}
