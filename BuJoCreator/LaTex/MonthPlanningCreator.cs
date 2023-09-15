using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using WM.LaTex;

namespace BuJoCreator.LaTex
{
    public class MonthPlanningCreator
    {
        public string TemplatePath = "/Users/shawn/Code/productivityAgendaCreator/BuJoCreator/LaTex/monthPlanning.tex";
        private readonly byte[] _dotedPaper;
        public string latexPath = "/Users/shawn/Code/productivityAgendaCreator/BuJoCreator/LaTex";
        private readonly PdfCreator _pdfCreator;

        public MonthPlanningCreator()
        {
            _pdfCreator = new PdfCreator();
            _dotedPaper = File.ReadAllBytes(latexPath + "/squaredots.pdf");
        }

        public void CreateSixMonths()
        {
            var outputDocument = new PdfDocument();

            int monthCount = 6;
            int maxMonth = 12;
            int minMonth = 7;

            MemoryStream dottedMonthStream1 = new(_dotedPaper);
            PdfSharpCore.Pdf.PdfDocument inputDottedPaper1 = PdfReader.Open(dottedMonthStream1, PdfDocumentOpenMode.Import);
            outputDocument.AddPage(inputDottedPaper1.Pages[0]);


            for (int i = 0; i < monthCount / 2; i++)
            {
                string firstMonth = CreateMonthPlanningPage(2023, minMonth);
                string secondMonth = CreateMonthPlanningPage(2023, maxMonth);
                minMonth++;
                maxMonth--;
                File.WriteAllText("/Users/shawn/Code/productivityAgendaCreator/BuJoCreator/LaTex/firstMonth.tex", firstMonth);
                File.WriteAllText("/Users/shawn/Code/productivityAgendaCreator/BuJoCreator/LaTex/secondMonth.tex", secondMonth);
                byte[]? firstMonthBytes = _pdfCreator.Create(latexPath, "firstMonth");
                byte[]? secondMonthBytes = _pdfCreator.Create(latexPath, "secondMonth");

                MemoryStream streamFirstMonth = new(firstMonthBytes);
                PdfSharpCore.Pdf.PdfDocument inputPdfDocumentFirstMonth = PdfReader.Open(streamFirstMonth, PdfDocumentOpenMode.Import);
                outputDocument.AddPage(inputPdfDocumentFirstMonth.Pages[0]);

                MemoryStream streamSecondMonth = new(secondMonthBytes);
                PdfSharpCore.Pdf.PdfDocument inputPdfDocumentSecondMonth = PdfReader.Open(streamSecondMonth, PdfDocumentOpenMode.Import);
                outputDocument.AddPage(inputPdfDocumentSecondMonth.Pages[0]);

                MemoryStream dottedMonthStream = new(_dotedPaper);
                PdfSharpCore.Pdf.PdfDocument inputDottedPaper = PdfReader.Open(dottedMonthStream, PdfDocumentOpenMode.Import);
                outputDocument.AddPage(inputDottedPaper.Pages[0]);
                outputDocument.AddPage(inputDottedPaper.Pages[0]);
            }
            outputDocument.Save(latexPath + "/result/monthPlanning.pdf");

        }


        public string CreateMonthPlanningPage(int year, int month)
        {
            string dayList = CreateDaysList(year, month);
            int dayCount = DateTime.DaysInMonth(year, month);
            return GetMonthPageWithReplacedValues(dayList, dayCount);
        }

        public string CreateDaysList(int year, int month)
        {
            string daysOfTheMonth = "\\def\\bulletCount{";
            int daysOfTheMonths = DateTime.DaysInMonth(year, month);
            for (int i = 1; i <= daysOfTheMonths; i++)
            {
                DateTime day = new(year, month, i);
                string dayName = day.ToString("D", System.Globalization.CultureInfo.CreateSpecificCulture("fr-FR"));
                daysOfTheMonth += i.ToString("D2") + dayName[..1].ToUpper() + ",";
            }
            for (int i = daysOfTheMonths; i <= 40; i++)
            {
                daysOfTheMonth += "XX,";
            }

            return daysOfTheMonth.Remove(daysOfTheMonth.Length - 1) + "}";

        }

        public string GetMonthPageWithReplacedValues(string dayList, int daysCount)
        {
            string originalMonthPlanningTemplate = File.ReadAllText(TemplatePath);
            string monthPlanningWithDaysList = originalMonthPlanningTemplate.Replace("listOfDaysToReplace", dayList);
            string monthPlanningWithDaysCount = monthPlanningWithDaysList.Replace("daysCountToReplace", daysCount.ToString()).Replace("daysCountPlusOneToReplace", (daysCount + 1).ToString());
            return monthPlanningWithDaysCount;
        }

    }
}
