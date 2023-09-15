using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using WM.LaTex;

namespace BuJoProApplicationLogic.BuJoCreator
{
    public class MonthPlanningCreator : IMonthPlanningCreator
    {
        public string MonthTemplatePath = AppContext.BaseDirectory + "/Templates/month-standard-6-categories.tex";
        private readonly byte[] _dotedPaper;
        public string LatexPath = AppContext.BaseDirectory + "/Templates/";
        private readonly LatexToPdf _pdfCreator;

        public MonthPlanningCreator()
        {
            _pdfCreator = new LatexToPdf();
            _dotedPaper = File.ReadAllBytes(LatexPath+ "dotted-blank-page.pdf");
        }

        public void CreateSixMonths()
        {
            PdfDocument outputDocument = new();

            int monthCount = 6;
            int maxMonth = 12;
            int minMonth = 7;

            MemoryStream dottedMonthStream1 = new(_dotedPaper);
            PdfDocument inputDottedPaper1 = PdfReader.Open(dottedMonthStream1, PdfDocumentOpenMode.Import);
            _ = outputDocument.AddPage(inputDottedPaper1.Pages[0]);


            for (int i = 0; i < monthCount / 2; i++)
            {
                string firstMonth = CreateMonthPlanningPage(2023, minMonth);
                string secondMonth = CreateMonthPlanningPage(2023, maxMonth);
                minMonth++;
                maxMonth--;
                File.WriteAllText(LatexPath + "firstMonth.tex", firstMonth);
                File.WriteAllText(LatexPath + "secondMonth.tex", secondMonth);
                byte[]? firstMonthBytes = _pdfCreator.Convert(LatexPath, "firstMonth");
                byte[]? secondMonthBytes = _pdfCreator.Convert(LatexPath, "secondMonth");

                MemoryStream streamFirstMonth = new(firstMonthBytes);
                PdfDocument inputPdfDocumentFirstMonth = PdfReader.Open(streamFirstMonth, PdfDocumentOpenMode.Import);
                _ = outputDocument.AddPage(inputPdfDocumentFirstMonth.Pages[0]);

                MemoryStream streamSecondMonth = new(secondMonthBytes);
                PdfDocument inputPdfDocumentSecondMonth = PdfReader.Open(streamSecondMonth, PdfDocumentOpenMode.Import);
                _ = outputDocument.AddPage(inputPdfDocumentSecondMonth.Pages[0]);

                MemoryStream dottedMonthStream = new(_dotedPaper);
                PdfDocument inputDottedPaper = PdfReader.Open(dottedMonthStream, PdfDocumentOpenMode.Import);
                _ = outputDocument.AddPage(inputDottedPaper.Pages[0]);
                _ = outputDocument.AddPage(inputDottedPaper.Pages[0]);
            }
            outputDocument.Save(LatexPath + "/result/monthPlanning.pdf");

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
            string originalMonthPlanningTemplate = File.ReadAllText(MonthTemplatePath);
            string monthPlanningWithDaysList = originalMonthPlanningTemplate.Replace("listOfDaysToReplace", dayList);
            string monthPlanningWithDaysCount = monthPlanningWithDaysList.Replace("daysCountToReplace", daysCount.ToString()).Replace("daysCountPlusOneToReplace", (daysCount + 1).ToString());
            return monthPlanningWithDaysCount;
        }

    }
}
