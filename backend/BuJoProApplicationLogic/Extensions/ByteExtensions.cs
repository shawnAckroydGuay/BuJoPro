
public static class ByteExtensions
{
    private static readonly Dictionary<byte[], string> _fileSignatures = new()
    {
        { new byte[] { 0xFF, 0xD8, 0xFF }, ".jpg" }, // JPEG
        { new byte[] { 0x89, 0x50, 0x4E, 0x47 }, ".png" }, // PNG
        { new byte[] { 0x47, 0x49, 0x46, 0x38 }, ".gif" }, // GIF
        { new byte[] { 0x42, 0x4D }, ".bmp" }, // BMP
        { new byte[] { 0x52, 0x49, 0x46, 0x46 }, ".webp" }, // WebP (RIFF)
    };

    public static string GetImageExtension(this byte[] fileData)
    {
        if (fileData == null || fileData.Length < 4)
            return null;

        foreach (var signature in _fileSignatures)
        {
            if (fileData.Take(signature.Key.Length).SequenceEqual(signature.Key))
                return signature.Value;
        }

        return null; // Format inconnu
    }
}