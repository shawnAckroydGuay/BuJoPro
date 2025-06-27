
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using PdfSharpCore.Drawing;
using System.Drawing.Printing;

using WM.LaTex;
using System.Diagnostics;
using System.Globalization;


namespace BuJoProApplicationLogic.BuJoCreator
{
    public class AlpagaAgendaCreator : IAlpagaAgendaCreator
    {
        public string TemplatePath = AppContext.BaseDirectory + "/AgendaCreator/Alpaga/";
        private readonly byte[] _dotedPaper;
        private readonly byte[] _splittedDotedPaper;
        private readonly byte[] _blankPaper;
        private readonly byte[] _categoriesMois;
        public string MonthTemplatePath = AppContext.BaseDirectory + "/AgendaCreator/Alpaga/AlpagaCalendrierHL.tex";
        public string AnnualPlanningPath = AppContext.BaseDirectory + "/AgendaCreator/Alpaga/PlanificationAnnuelleZigZag.tex";
        public string CouverturePath = AppContext.BaseDirectory + "/AgendaCreator/Alpaga/CouvertureHL.tex";
        private readonly LatexToPdf _pdfCreator;

        public AlpagaAgendaCreator()
        {
            _pdfCreator = new LatexToPdf();
            _dotedPaper = File.ReadAllBytes(TemplatePath + "DottedHL.pdf");
            _splittedDotedPaper = File.ReadAllBytes(TemplatePath + "SeparatedDottedHL.pdf");
            _blankPaper = File.ReadAllBytes(TemplatePath + "BlankHL.pdf");
            _categoriesMois = File.ReadAllBytes(TemplatePath + "SeparatedDottedSixCategories.pdf");
        }

        public byte[] CreerLePlanificateurEnPdf(int premierMois,
         string titre, 
         byte[]? imageCouverture,
         int nombreDeMoisVoulu = 6)
        {
            if(imageCouverture is not null)
                EnregistrerImageCouverture(imageCouverture);

            var listeDesMois = new List<DateTime>();

            for (int i = 0; i < nombreDeMoisVoulu; i++)
            {
                //Ici les calculs servent à gérer si l'utilisateur demande 6 mois et que le premier
                //mois est par exemple septembre(09): 09 10 11 12 01 02 03
                int mois = (premierMois - 1 + i) % 12 + 1;
                int anneeDuMois = DateTime.Now.Year + (premierMois - 1 + i) / 12;
                var moisEnFormatDateTime = new DateTime(anneeDuMois, mois, 1);
                listeDesMois.Add(moisEnFormatDateTime);
            }

            var pagesAImprimer = new List<byte[]>();

            var couverture = ObtenirCouverture(titre);
            pagesAImprimer.Add(couverture);
            pagesAImprimer.Add(_splittedDotedPaper);
            pagesAImprimer.Add(_splittedDotedPaper);

            var pagesPlannings = CreerPagesPlanningAnnuelle(premierMois);
            pagesAImprimer.Add(pagesPlannings.Item1);
            pagesAImprimer.Add(pagesPlannings.Item2);


            foreach (var month in listeDesMois)
            {
                var calendrierLatexFormatte = CreerLaPageCalendrierDuMois(month.Year, month.Month);
                File.WriteAllText(TemplatePath + "alpagaV2.tex", calendrierLatexFormatte);
                var alpagaBytes = _pdfCreator.Convert(TemplatePath, "alpagaV2");
                pagesAImprimer.Add(alpagaBytes);
                pagesAImprimer.Add(_categoriesMois);
                pagesAImprimer.Add(_splittedDotedPaper);
                pagesAImprimer.Add(_splittedDotedPaper);
                pagesAImprimer.Add(_splittedDotedPaper);
            }

            pagesAImprimer.Add(_blankPaper);
            return CreerAgendaPDFPourPlierFormatLettre(pagesAImprimer);

        }

        public void EnregistrerImageCouverture(byte[] imageCouverture){
            var extension = imageCouverture.GetImageExtension();
            File.WriteAllBytes(TemplatePath + "/imageCouverture" + extension, imageCouverture);
        }

        public byte[] ObtenirCouverture(string titre){
            var couverture = File.ReadAllText(CouverturePath);
            couverture = couverture
                .Replace("titreARemplacer", titre);
            File.WriteAllText(TemplatePath + "cover.tex", couverture);
            return _pdfCreator.Convert(TemplatePath, "cover");
        }

        public (byte[], byte[]) CreerPagesPlanningAnnuelle(int premierMois)
        {

            var listeDesMois = new List<DateTime>();
            for (int i = 0; i < 12; i++)
            {
                //Ici les calculs servent à gérer si l'utilisateur demande 6 mois et que le premier
                //mois est par exemple septembre(09): 09 10 11 12 01 02 03
                int mois = (premierMois - 1 + i) % 12 + 1;
                int anneeDuMois = DateTime.Now.Year + (premierMois - 1 + i) / 12;
                var moisEnFormatDateTime = new DateTime(anneeDuMois, mois, 1);
                listeDesMois.Add(moisEnFormatDateTime);
            }

            var dic = new Dictionary<int, string>{
                {0, "firstMonth"},
                {1, "secondMonth"},
                {2, "thirdMonth"},
                {3, "fourthMonth"},
                {4, "fifthMonth"},
                {5, "sixthMonth"},
            };


            var definitionVariablesLatexPage1 = string.Empty;
            var definitionVariablesLatexPage2 = string.Empty;
            for (int i = 0; i < listeDesMois.Count; i++)
            {
                if (i < 6)
                    definitionVariablesLatexPage1 += CreerLaListeDesJoursDuMoisPlanning(
                        listeDesMois[i].Year,
                        listeDesMois[i].Month, dic[i]);
                else
                    definitionVariablesLatexPage2 += CreerLaListeDesJoursDuMoisPlanning(
                        listeDesMois[i].Year,
                        listeDesMois[i].Month, dic[i - 6]);
            }

            string planningAnnuelleLatex = File.ReadAllText(AnnualPlanningPath);
            var planningAnnuelleLatexPage1 = planningAnnuelleLatex
                .Replace("monthsDefinitionToReplace", definitionVariablesLatexPage1)
                .Replace("\\def\\isFirstPage{}", "\\newbool{isFirstPage}");
            var planningAnnuelleLatexPage2 = planningAnnuelleLatex
                .Replace("monthsDefinitionToReplace", definitionVariablesLatexPage2);

            File.WriteAllText(TemplatePath + "annualPlanningPage1.tex", planningAnnuelleLatexPage1);
            File.WriteAllText(TemplatePath + "annualPlanningPage2.tex", planningAnnuelleLatexPage2);
            var planningAnnuelPage1 = _pdfCreator.Convert(TemplatePath, "annualPlanningPage1");
            var planningAnnuelPage2 = _pdfCreator.Convert(TemplatePath, "annualPlanningPage2");
            return (planningAnnuelPage1, planningAnnuelPage2);
        }
        /// <summary>
        /// Cette fonction va simplement prendre une liste ordonnéee de fichiers pdf au format HL (Half-Letter)
        /// et va les imprimer de manière à ce qu'on puisse faire un agenda en pliant les feuilles de format
        /// lettre en deux. 
        /// Ex : Si j'ai 8 pdfs HL, cette fonction va les imprimer sur 2 pages format lettre recto-verso.
        /// Ces 2 pages seront pliées sur le côté court et assemblées ensemble afin de faire un 
        /// carnet. Ces deux pages format lettre vont donc créer un carnet de 8 pages au format HL.
        ///  feuille 1 : recto: [8|1] verso: [2|7]
        ///  feuille 2: recto: [6|3] verso: [4|5]
        ///  les chiffres représentent l'ordre des PDF HL
        /// </summary>
        public byte[] CreerAgendaPDFPourPlierFormatLettre(List<byte[]> pdfList)
        {
            // var outputPath = "output.pdf";
            PdfDocument outputDocument = new PdfDocument();

            var debut = 0;
            var fin = pdfList.Count() - 1;
            var pageflip = false;
            while (debut <= fin)
            {
                // Format lettre en points (1 pouce = 72 points)
                const double LETTER_WIDTH = 11 * 72;   // 11 pouces (largeur en mode paysage)
                const double LETTER_HEIGHT = 8.5 * 72; // 8.5 pouces (hauteur en mode paysage)

                // Chaque PDF devrait occuper exactement la moitié de la largeur
                const double PDF_WIDTH = LETTER_WIDTH / 2;   // 5.5 pouces
                const double PDF_HEIGHT = LETTER_HEIGHT;     // 8.5 pouces

                // Créer une page au format lettre en mode paysage
                var page = outputDocument.AddPage();
                page.Size = PdfSharpCore.PageSize.Letter;
                page.Orientation = PdfSharpCore.PageOrientation.Landscape;

                XGraphics gfx = XGraphics.FromPdfPage(page);

                var page1Stream = pageflip ? new MemoryStream(pdfList[debut])
                    : new MemoryStream(pdfList[fin]);

                var page2Stream = pageflip ? new MemoryStream(pdfList[fin])
                    : new MemoryStream(pdfList[debut]);

                // Positionner le premier PDF sur la moitié gauche
                XPdfForm form1 = XPdfForm.FromStream(page1Stream);
                gfx.DrawImage(form1, 0, 0, PDF_WIDTH, PDF_HEIGHT);

                // Positionner le deuxième PDF sur la moitié droite
                XPdfForm form2 = XPdfForm.FromStream(page2Stream);
                gfx.DrawImage(form2, PDF_WIDTH, 0, PDF_WIDTH, PDF_HEIGHT);

                fin--;
                debut++;
                pageflip = !pageflip;
            }
            // outputDocument.Save(outputPath);
            outputDocument.Close();

            using (MemoryStream stream = new MemoryStream())
            {
                outputDocument.Save(stream, false);
                return stream.ToArray();
            }
        }

        public string CreerLaPageCalendrierDuMois(int annee, int mois)
        {
            string listeDesJoursFormattes = CreerLaListeDesJoursDuMois(annee, mois);
            int nombreDeJourDuMois = DateTime.DaysInMonth(annee, mois);
            return ObtenirCalendrierLaTexFormatte(listeDesJoursFormattes, nombreDeJourDuMois);
        }

        /// <summary>
        /// Cette fonction permet de créer la liste des jours du mois selon le format désiré.
        /// Ici le format désiré est Jou. 00 Mois, ex : Ven. 01 Septembre
        /// Ces jours formattés seront mis en format texte dans les parenthèse de la variable 
        /// suivante : \def\bulletCount{}
        /// Les jours seront séparés par des virgules.
        /// Ex de sortie: \def\bulletCount{Ven. 01 Décembre, Sam. 02 Décembre, etc.}
        /// Le fichier LaTex va éventuellement se servir de ces jours afin de créer son calendrier.
        /// </summary>
        public string CreerLaListeDesJoursDuMois(int year, int month)
        {
            string joursDuMois = "\\def\\bulletCount{";
            int nombreDeJourDansLeMois = DateTime.DaysInMonth(year, month);
            for (int i = 1; i <= nombreDeJourDansLeMois; i++)
            {
                var jour = new DateTime(year, month, i);
                TextInfo textInfo = new CultureInfo("fr-FR", false).TextInfo;
                string jourFormatte = jour.ToString("ddd dd MMMM", new CultureInfo("fr-FR"));
                jourFormatte = textInfo.ToTitleCase(jourFormatte); //Ajouter une maj. au jour/mois.

                // Ici on échappe le caractère du point pour que l'interpréteur LaTex 
                //ne fasse pas un espace supplémentaire à la fin. 
                jourFormatte = jourFormatte.Replace(".", ".\\");
                joursDuMois += jourFormatte + ",";
            }
            for (int i = nombreDeJourDansLeMois; i <= 40; i++)
            {
                joursDuMois += "XX,";
            }
            return joursDuMois.Remove(joursDuMois.Length - 1) + "}";

        }

        public string CreerLaListeDesJoursDuMoisPlanning(int year, int month, string titre)
        {
            var frenchCulture = new CultureInfo("fr-FR");
            var textInfo = new CultureInfo("fr-FR", false).TextInfo;
            var nomDuMois = frenchCulture.DateTimeFormat.GetMonthName(month);
            nomDuMois = textInfo.ToTitleCase(nomDuMois);
            var joursDuMois = $"\\def\\{titre}Name{{{nomDuMois} {year}}}";
            joursDuMois += $"\\def\\{titre}DayList{{L,M,M,J,V,S,D,";
            var premierJour = new DateTime(year, month, 1);
            //Lundi=1, Mardi=2, ..., Dimanche=7
            var numeroJourDeSemaine = (int)premierJour.DayOfWeek;
            //Logique pour mettre des X pour les jours vide dans le calendrier
            //Ex: si mon mois commence un mercredi, alors lundi et mardi n'auront pas 
            //de jour dans l'affichage du calendrier
            for (int i = 1; i < numeroJourDeSemaine; i++)
            {
                joursDuMois += "XX,";
            }

            int nombreDeJourDansLeMois = DateTime.DaysInMonth(year, month);

            for (int i = 1; i <= nombreDeJourDansLeMois; i++)
            {
                var jour = new DateTime(year, month, i);
                string jourFormatte = jour.ToString("dd");
                jourFormatte = textInfo.ToTitleCase(jourFormatte);
                joursDuMois += $"{jourFormatte},";
            }

            //Enlever la dernière virgule
            return joursDuMois.Remove(joursDuMois.Length - 1) + "}";

        }

        /// <summary>
        /// Cette fonction a pour but de remplacer les variable du fichier LaTex 
        /// du calendrier du mois. 
        /// listOfDaysToReplace devra être remplacé par la liste des jours
        /// daysCountToReplace devra être remplacé par le nombre de jour dans le mois
        /// daysCountPlusOneToReplace devra être remplacé par le nombre de jour dans le mois + 1
        /// </summary>
        /// <param name="name">The name to greet.</param>
        public string ObtenirCalendrierLaTexFormatte(string dayList, int daysCount)
        {
            string calendrierLatexChoisi = File.ReadAllText(MonthTemplatePath);
            calendrierLatexChoisi = calendrierLatexChoisi
                .Replace("listOfDaysToReplace", dayList)
                .Replace("daysCountToReplace", daysCount.ToString())
                .Replace("daysCountPlusOneToReplace", (daysCount + 1).ToString());
            return calendrierLatexChoisi;
        }
    }
}
