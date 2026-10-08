using System.Collections.Generic;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.PO;
using PX.SM;

namespace Larkfield.POMailAttachments
{
	/// <summary>
	/// Reads the files attached to a purchase order with the size of their current revision.
	/// </summary>
	public static class LRKFOrderMailPlans
	{
		public static OrderMailPlan For(PXGraph graph, POOrder order)
		{
			bool attachFiles = PXCache<POOrder>.GetExtension<LRKFPOOrderExt>(order)?.UsrLRKFAttachFiles == true;

			if (!attachFiles || order.NoteID == null)
				return OrderMailPlan.Create(order.OrderType, order.OrderNbr, attachFiles: false, files: null);

			return OrderMailPlan.Create(order.OrderType, order.OrderNbr, attachFiles: true, ReadFiles(graph, order));
		}

		private static List<OrderFile> ReadFiles(PXGraph graph, POOrder order)
		{
			var query = new SelectFrom<NoteDoc>
				.InnerJoin<UploadFile>
					.On<UploadFile.fileID.IsEqual<NoteDoc.fileID>>
				.InnerJoin<UploadFileRevision>
					.On<UploadFileRevision.fileID.IsEqual<UploadFile.fileID>
					.And<UploadFileRevision.fileRevisionID.IsEqual<UploadFile.lastRevisionID>>>
				.Where<NoteDoc.noteID.IsEqual<@P.AsGuid>>
				.View.ReadOnly(graph);

			var files = new List<OrderFile>();

			// Only the size is needed, so the file content itself is never loaded.
			using (new PXFieldScope(query.View,
				typeof(NoteDoc.noteID), typeof(NoteDoc.fileID),
				typeof(UploadFile.fileID), typeof(UploadFile.name), typeof(UploadFile.lastRevisionID),
				typeof(UploadFileRevision.fileID), typeof(UploadFileRevision.fileRevisionID), typeof(UploadFileRevision.size)))
			{
				foreach (PXResult<NoteDoc, UploadFile, UploadFileRevision> row in query.Select(order.NoteID))
				{
					UploadFile file = row;
					UploadFileRevision revision = row;

					if (file.FileID != null)
						files.Add(new OrderFile(file.FileID.Value, file.Name, SizeUnits.FromKilobytes(revision.Size)));
				}
			}

			return files;
		}
	}
}
