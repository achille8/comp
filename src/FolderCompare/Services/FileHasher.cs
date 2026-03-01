namespace FolderCompare.Services;

using System.IO;
using System.Security.Cryptography;

public static class FileHasher
{
    public static async Task<byte[]> ComputeHashAsync(string filePath, CancellationToken ct)
    {
        using var sha256 = SHA256.Create();
        await using var stream = new FileStream(
            filePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 81920,
            useAsync: true);

        return await sha256.ComputeHashAsync(stream, ct).ConfigureAwait(false);
    }
}
