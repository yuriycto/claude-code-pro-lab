# claude-code-pro-lab

Lab repository for the **Claude Code Pro** course. It holds one small but real project, an Acumatica ERP
customization, and every homework after this one arrives as a separate pull request.

## The project: PO Mail Attachments

When a purchase order is emailed to a vendor, Acumatica sends only the printed order form.
This customization adds the files attached to the order (drawings, quotes, specifications)
to the same email, per order, with a default on each vendor and a warning before an email gets too large
for mail servers.

- Requirements: [docs/spec.md](docs/spec.md)
- How it hooks into Acumatica 2026 R2: [docs/design.md](docs/design.md)

Larkfield Supply, the client named in the spec, is fictional.

## Layout

```
src/
  Larkfield.POMailAttachments.Core/   plain .NET logic: size limit, file selection, dialog text
  Larkfield.POMailAttachments/        Acumatica 2026 R2 customization (DAC and graph extensions)
tests/
  Larkfield.POMailAttachments.Core.Tests/   xUnit tests for the Core logic
docs/                                 spec and design notes
```

The Acumatica project compiles the Core sources into its own assembly, so a site receives a single DLL.

## Build and test

Unit tests need only the .NET 10 SDK:

```bash
dotnet test tests/Larkfield.POMailAttachments.Core.Tests
```

The customization needs the `Bin` folder of an Acumatica 2026 R2 site or installation.
By default the build looks in `C:\Program Files\Acumatica ERP\Files\Bin`:

```bash
dotnet build src/Larkfield.POMailAttachments -c Release -p:AcumaticaBinPath="D:\Sites\MySite\Bin"
```

## Status

Done:

- [x] Vendor default and per order option (DAC extensions, defaulting from the vendor)
- [x] Email Purchase Order sends each order's own files (PO301000 and PO503000, separate and grouped emails)
- [x] Size check with a Yes/No question on PO301000 and once per run on PO503000
- [x] Core logic covered by unit tests

Next:

- [ ] Show the fields on PO301000, AP303000 and PO503000 (classic and modern UI)
- [ ] Customization package with the database columns, published to a local 2026 R2 site
- [ ] Verify the PO503000 question on a live site
- [ ] Keep the option editable on closed and cancelled orders (workflow field state)
- [ ] Email from the report viewer after Print Purchase Order
- [ ] Acuminator analyzers in the build, CI on pull requests

## Контекст курсу

Задача 1.3 «Навчальний репозиторій»: цей репозиторій і є здавальним артефактом курсу.
Усередині невеликий реальний проєкт, на який агент може дивитися, а кожна наступна домашка
приходить окремим PR.
