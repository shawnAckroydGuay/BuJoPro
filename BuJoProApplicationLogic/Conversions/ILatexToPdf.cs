namespace WM.LaTex
{
    public interface ILatexToPdf
    {
        byte[] Convert(string filePath, string fileName);
    }
}