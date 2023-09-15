using BuJoCreator.LaTex;
using System.Text;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

// var latexPath = "C:\\Users\\shawn\\source\\repos\\productivityAgendaCreator\\BuJoCreator\\LaTex";
//var latexPath = "/Users/shawn/Code/productivityAgendaCreator/BuJoCreator/LaTex";

//var pdfCreator = new WM.LaTex.PdfCreator();
 
//var monthPlanningPdfBytes = pdfCreator.Create(latexPath, "monthPlanningTest");

var montPlanningCreator = new MonthPlanningCreator();
montPlanningCreator.CreateSixMonths();

//var outputDocument = new PdfDocument();
//MemoryStream stream = new MemoryStream(monthPlanningPdfBytes);

//var inputPdfDocument = PdfReader.Open(stream, PdfDocumentOpenMode.Import);
//outputDocument.AddPage(inputPdfDocument.Pages[0]);
//outputDocument.AddPage(inputPdfDocument.Pages[0]);

//outputDocument.Save(latexPath + "/result/monthPlanning.pdf");


