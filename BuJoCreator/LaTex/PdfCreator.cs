using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace WM.LaTex
{
    public class PdfCreator
    {
        private byte[] _pdf;

        public PdfCreator(string tmpPath = "/tmp") => this.TmpPath = tmpPath;

        public string TmpPath { get; }

        public byte[] Create(string filePath, string fileName)
        {
            if (this._pdf == null)
            {
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
                try
                {
                    // this._pdf = File.ReadAllBytes(filePath + "\\" + fileName + ".pdf");
                    this._pdf = File.ReadAllBytes(filePath + "/" + fileName + ".pdf");
                }
                catch
                {
                }
                // File.Delete(filePath + "\\monthPlanning.log");
                // File.Delete(filePath + "\\monthPlanning.aux"); 
                File.Delete(filePath + "/monthPlanningTest.log");
                File.Delete(filePath + "/monthPlanningTest.aux");
            }
            return this._pdf;
        }
    }
}
