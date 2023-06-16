

using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.Text;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var latexPath = "C:\\Users\\shawn\\source\\repos\\LatexGenerator\\LatexGenerator\\LaTex";

var pdfCreator = new WM.LaTex.PdfCreator();

var monthPlanningPdfBytes = pdfCreator.Erzeuge(latexPath, "monthPlanning");

var outputDocument = new PdfDocument();
MemoryStream stream = new MemoryStream(monthPlanningPdfBytes);

var inputPdfDocument = PdfReader.Open(stream, PdfDocumentOpenMode.Import);
outputDocument.AddPage(inputPdfDocument.Pages[0]);
outputDocument.AddPage(inputPdfDocument.Pages[0]);

outputDocument.Save(latexPath + "\\monthPlanning.pdf");
File.Delete(latexPath + "\\monthPlanning.log");
File.Delete(latexPath + "\\monthPlanning.aux");

