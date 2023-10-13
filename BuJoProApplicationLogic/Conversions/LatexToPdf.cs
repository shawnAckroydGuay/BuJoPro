using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace WM.LaTex
{
    public class LatexToPdf : ILatexToPdf
    {
        public byte[] Convert(string filePath, string fileName)
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
                interpolatedStringHandler.AppendLiteral("/" + fileName + ".tex");
                string stringAndClear = interpolatedStringHandler.ToStringAndClear();
                startInfo.Arguments = stringAndClear;
                for (int index = 0; index < 2; ++index)
                {
                    process.Start();
                    process.WaitForExit();
                }
            }
            pdfByte = File.ReadAllBytes(filePath + "/" + fileName + ".pdf");
            var filesToDelete = Directory.GetFiles(filePath, fileName + ".*").ToList();
            filesToDelete.ForEach(File.Delete);
            return pdfByte;
        }

    }
}
