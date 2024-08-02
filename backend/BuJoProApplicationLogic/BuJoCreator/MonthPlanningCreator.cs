using PdfSharpCore;
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

        public byte[] CreateSixMonths(int firstMonth, int monthCount = 6)
        {
            /*
             Since the agenda will be folded, this process is a bit odd. 
             Let's consider that the user want a bullet journal for JANUARY until JUNE.
             The way it's made is a bit like this : 
                    - page 1 (front): White
                    - page 2 (back): year planning 1 / last page (TODO)
                    - page 3 (front): year planning 2 / last page (TODO)
                    - page 4 (back): backlog / blank page
                    - page 5 (front): backlog / blank page 
                    - page 6 (back): january / 3 tasks + planning
                    - page 7 (front): june / 3 tasks + planning
                    - page 8 (back): blank 
                    - page 9 (front) : blank 
                    - page 10 (back) : february / 3 tasks + planning
                    - page 11 (front) : May / 3 task + planning
                    - page 12 (back) : blank 
                    - page 13 (front) : blank 
                    - page 14 (back) : March / 3 task + planning 
                    - page 15 (front) : April / 3 tasks + planning 
                    - page 16 (back) : blank
                    - page 17 (front) :blank (useless page will not be used, could discard it)
             */
            var outputDocument = new PdfDocument();


            var months = new List<DateTime>();
            for (int i = 0; i < monthCount; i++)
            {
                int month = (firstMonth + i - 1) % 12 + 1;
                int year = DateTime.Now.Year + (firstMonth + i - 1) / 12;
                var monthDate = new DateTime(year, month, 1);
                months.Add(monthDate);
            }

            var secondPageStream = new MemoryStream(_dotedPaper);
            var secondPagePdf = PdfReader.Open(secondPageStream, PdfDocumentOpenMode.Import);
            var page = new PdfPage
            {
                Size = PageSize.Letter,
                Orientation = PageOrientation.Landscape
            };
            outputDocument.AddPage(page);
            outputDocument.AddPage(secondPagePdf.Pages[0]);
            outputDocument.AddPage(secondPagePdf.Pages[0]);

            for (int i = 0; i < monthCount / 2; i++)
            {
                string backPageMonth = CreateMonthPlanningPage(months.ElementAt(i).Year, months.ElementAt(i).Month);
                string frontPageMonth = CreateMonthPlanningPage(months.ElementAt(i).Year, months.ElementAt(monthCount - 1 - i).Month);
                File.WriteAllText(LatexPath + "backPageMonth.tex", backPageMonth);
                File.WriteAllText(LatexPath + "frontPageMonth.tex", frontPageMonth);
                byte[]? firstMonthBytes = _pdfCreator.Convert(LatexPath, "backPageMonth");
                byte[]? secondMonthBytes = _pdfCreator.Convert(LatexPath, "frontPageMonth");

                var streamFirstMonth = new MemoryStream(firstMonthBytes);
                var inputPdfDocumentFirstMonth = PdfReader.Open(streamFirstMonth, PdfDocumentOpenMode.Import);
                _ = outputDocument.AddPage(inputPdfDocumentFirstMonth.Pages[0]);

                var streamSecondMonth = new MemoryStream(secondMonthBytes);
                var inputPdfDocumentSecondMonth = PdfReader.Open(streamSecondMonth, PdfDocumentOpenMode.Import);
                _ = outputDocument.AddPage(inputPdfDocumentSecondMonth.Pages[0]);

                var dottedMonthStream = new MemoryStream(_dotedPaper);
                var inputDottedPaper = PdfReader.Open(dottedMonthStream, PdfDocumentOpenMode.Import);
                _ = outputDocument.AddPage(inputDottedPaper.Pages[0]);
                _ = outputDocument.AddPage(inputDottedPaper.Pages[0]);
            }

            using (MemoryStream stream = new MemoryStream())
            {
                outputDocument.Save(stream, false);
                return stream.ToArray();
            }

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
                var day = new DateTime(year, month, i);
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
            string monthPlanningWithDaysCount = monthPlanningWithDaysList
                    .Replace("daysCountToReplace", daysCount.ToString())
                    .Replace("daysCountPlusOneToReplace", (daysCount + 1).ToString());
            return monthPlanningWithDaysCount;
        }

    }
}
