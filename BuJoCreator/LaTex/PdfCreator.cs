using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace WM.LaTex
{
    public class PdfCreator
    {
        public string TmpPath { get; }

        public PdfCreator() { }


        public byte[]? Create(string filePath, string fileName)
        {
            Byte[] pdfByte;
            using (Process process = new Process())
            {
                process.EnableRaisingEvents = false;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.FileName = "pdflatex";
                ProcessStartInfo startInfo = process.StartInfo;
                DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(61, 2);
                interpolatedStringHandler.AppendLiteral(" -output-directory=");
                interpolatedStringHandler.AppendFormatted(filePath);
                interpolatedStringHandler.AppendLiteral(" -interaction=batchmode -shell-escape ");
                interpolatedStringHandler.AppendFormatted(filePath);
                // interpolatedStringHandler.AppendLiteral("\\" + fileName + ".tex");
                interpolatedStringHandler.AppendLiteral("/" + fileName + ".tex");
                string stringAndClear = interpolatedStringHandler.ToStringAndClear();
                startInfo.Arguments = stringAndClear;
                for (int index = 0; index < 2; ++index)
                {
                    process.Start();
                    process.WaitForExit();
                }
            }
            // this._pdf = File.ReadAllBytes(filePath + "\\" + fileName + ".pdf");
            pdfByte = File.ReadAllBytes(filePath + "/" + fileName + ".pdf");
            // File.Delete(filePath + "\\monthPlanning.log");
            // File.Delete(filePath + "\\monthPlanning.aux");
            File.Delete(filePath + "/" + fileName + ".pdf");
            File.Delete(filePath + "/" + fileName + ".tex");
            File.Delete(filePath + "/monthPlanningTest.log");
            File.Delete(filePath + "/monthPlanningTest.aux");
            return pdfByte;
        }

    }
}
