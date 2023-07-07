using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using WM.LaTex;

namespace BuJoCreator.LaTex
{
    public class MonthPlanningCreator
    {
        public string TemplatePath = "/Users/shawn/Code/productivityAgendaCreator/BuJoCreator/LaTex/monthPlanning.tex";
        private byte[] _dotedPaper;
        public string latexPath = "/Users/shawn/Code/productivityAgendaCreator/BuJoCreator/LaTex";
        private PdfCreator _pdfCreator;

        public MonthPlanningCreator()
        {
            _pdfCreator = new PdfCreator();
            _dotedPaper = File.ReadAllBytes(latexPath + "/squaredots.pdf");
        }

        public void CreateSixMonths()
        {
            var outputDocument = new PdfDocument();

            var monthCount = 6;
            var maxMonth = 12;
            var minMonth = 7;

            MemoryStream dottedMonthStream1 = new MemoryStream(_dotedPaper);
            var inputDottedPaper1 = PdfReader.Open(dottedMonthStream1, PdfDocumentOpenMode.Import);
            outputDocument.AddPage(inputDottedPaper1.Pages[0]);


            for (int i = 0; i < monthCount / 2; i++)
            {
                var firstMonth = CreateMonthPlanningPage(2023, minMonth);
                var secondMonth = CreateMonthPlanningPage(2023, maxMonth);
                minMonth++;
                maxMonth--;
                File.WriteAllText("/Users/shawn/Code/productivityAgendaCreator/BuJoCreator/LaTex/firstMonth.tex", firstMonth);
                File.WriteAllText("/Users/shawn/Code/productivityAgendaCreator/BuJoCreator/LaTex/secondMonth.tex", secondMonth);
                var firstMonthBytes = _pdfCreator.Create(latexPath, "firstMonth");
                var secondMonthBytes = _pdfCreator.Create(latexPath, "secondMonth");

                MemoryStream streamFirstMonth = new MemoryStream(firstMonthBytes);
                var inputPdfDocumentFirstMonth = PdfReader.Open(streamFirstMonth, PdfDocumentOpenMode.Import);
                outputDocument.AddPage(inputPdfDocumentFirstMonth.Pages[0]);

                MemoryStream streamSecondMonth= new MemoryStream(secondMonthBytes);
                var inputPdfDocumentSecondMonth = PdfReader.Open(streamSecondMonth, PdfDocumentOpenMode.Import);
                outputDocument.AddPage(inputPdfDocumentSecondMonth.Pages[0]);

                MemoryStream dottedMonthStream = new MemoryStream(_dotedPaper);
                var inputDottedPaper = PdfReader.Open(dottedMonthStream, PdfDocumentOpenMode.Import);
                outputDocument.AddPage(inputDottedPaper.Pages[0]);
                outputDocument.AddPage(inputDottedPaper.Pages[0]);
            }
            outputDocument.Save(latexPath + "/result/monthPlanning.pdf");

        }


        public string CreateMonthPlanningPage(int year, int month)
        {
            var dayList = CreateDaysList(year, month);
            var dayCount = DateTime.DaysInMonth(year, month);
            return GetMonthPageWithReplacedValues(dayList, dayCount);
        }

        public string CreateDaysList(int year, int month)
        {
            var daysOfTheMonth = "\\def\\bulletCount{";
            var daysOfTheMonths = DateTime.DaysInMonth(year, month);
            for (int i = 1; i <= daysOfTheMonths; i++)
            {
                var day = new DateTime(year, month, i);
                var dayName = day.ToString("D", System.Globalization.CultureInfo.CreateSpecificCulture("fr-FR"));
                daysOfTheMonth += i.ToString("D2") + dayName.Substring(0, 1).ToUpper() + ",";
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
            var monthPlanningWithDaysList = originalMonthPlanningTemplate.Replace("listOfDaysToReplace", dayList);
            var monthPlanningWithDaysCount = monthPlanningWithDaysList.Replace("daysCountToReplace", daysCount.ToString()).Replace("daysCountPlusOneToReplace", (daysCount + 1).ToString());
            return monthPlanningWithDaysCount;
        }

    }
}
