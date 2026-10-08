# PO Mail Attachments: functional specification

Client: Larkfield Supply (a fictional distributor used for this lab).
Platform: Acumatica ERP 2026 R2 (26.200).

## Problem

When a buyer emails a purchase order to a vendor, Acumatica attaches only the printed order form.
The supporting files the buyer has already attached to the order (drawings, quotes, specifications)
have to be added to every email by hand, and they are often forgotten.

## Goal

Let a purchase order take its own files to the vendor. The buyer controls it per order,
and each vendor provides a sensible default.

## Screens

| Screen | ID | Change |
|---|---|---|
| Purchase Orders | PO301000 | New checkbox **Attach Files to Email** on the Other tab |
| Vendors | AP303000 | New checkbox **Attach PO Files to Email** on the Purchase Settings tab, on by default for new vendors |
| Print/Email Purchase Orders | PO503000 | New read-only column showing each order's own setting |

## Rules

| ID | When | Then |
|---|---|---|
| R1 | Email Purchase Order runs on PO301000 and the order has the option on | The email carries the printed form plus every file attached to the order, as they are, with no conversion or merging |
| R2 | The option is off, or the order has no files | Native behaviour, only the printed form |
| R3 | An order is created, or its vendor is changed | The option takes the vendor default |
| R4 | Orders are emailed from PO503000 | Each order follows its own option and gets the same email it would get from PO301000; this also holds when emails are grouped per vendor |
| R5 | The files of an order add up to more than 25 MB | Before any email is created, the user sees the size and answers Yes or No. No creates nothing. On PO503000 the question is asked once for the whole selection. Imports, API calls and scheduled runs are not asked |
| R6 | The order is closed or cancelled | The option stays editable |
| R7 | The user prints the order and sends it from the report viewer | Same result as R1 |

## Out of scope

Compressing files, merging them into one PDF, sending links instead of files.

## Acceptance checks

1. Vendor with the default on, new order for that vendor: the option is on. Switch to a vendor with the default off: the option turns off.
2. Order with the option on and two files: the email activity has the form and both files.
3. Same order with the option off: the email activity has the form only.
4. Order with the option on and no files: the email activity has the form only.
5. Files above 25 MB: the question appears; No creates no email, Yes creates it with all files.
6. PO503000 with three orders, one of them above 25 MB: one question before processing; each email matches its order's option.
