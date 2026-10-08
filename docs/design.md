# Design notes

## Where the email is created in 2026 R2

Both entry points end up in the same action:

- PO301000, **Email Purchase Order**: `POOrderEntry.EmailPurchaseOrder` calls the protected
  `POOrderEntry.Notification`, which starts a long operation and calls
  `POOrderEntry_ActivityDetailsExt.SendNotifications`.
- PO503000, action **Email Purchase Order**: the workflow mass process runs
  `PO301000$emailPurchaseOrder` for the selected orders, with `adapter.MassProcess = true`.

`SendNotifications` accepts one `attachments` list for all documents. There is no public,
non-generic hook per document, so the customization overrides `EmailPurchaseOrder`
(`LRKFPOOrderEntryExt`) and repeats the short native `Notification` flow, adding the files.

## One attachment list, many orders

For every document the native code calls `documentParametersGetter` and then reads `attachments`
for that same document. This holds in the single, separate and aggregated (one email per vendor) paths.
`SendWithOrderFiles` therefore refills the list inside `documentParametersGetter`,
so each email gets its own order's files. If a future Acumatica version changes that order,
this is the place to revisit.

## Size check

- Sizes come from `UploadFileRevision.Size` (kilobytes) of the current revision of each file linked
  to the order through `NoteDoc`. A `PXFieldScope` keeps the file content out of the query.
- The printed form is rendered later, so it is not counted. It is usually far below the limit.
- The rule itself (limit, rounding, which orders are oversized, how they are listed) lives in the
  Core project and is unit tested without Acumatica.
- PO301000 asks through `Document.Ask`. PO503000 asks once in `SetParametersDelegate`, before
  processing starts, because a dialog is not possible inside the long operation.

## Defaulting

`LRKFPOOrderExt.UsrLRKFAttachFiles` defaults from `LRKFVendorExt.UsrLRKFAttachFiles` through
`PXDefault` with a `SearchFor` query and `PXFormula(Default<POOrder.vendorID>)`, so no event code is needed.
Vendors created before the customization have an empty value, which is treated as off.

## Naming

AcuPower convention: a four letter prefix (`LRKF`) on custom fields and classes, no underscores in class names.
