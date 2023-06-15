using Aspose.Pdf;

Console.WriteLine("Hello, World!");


TeXLoadOptions options = new TeXLoadOptions();

Aspose.Pdf.Document pdfDocument = new Aspose.Pdf.Document("LaTex/monthPlanning.tex", options);

pdfDocument.Save("LaTeX-to-PDF.pdf");
