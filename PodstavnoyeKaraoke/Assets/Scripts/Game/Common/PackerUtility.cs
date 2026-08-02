using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using CompressionLevel = System.IO.Compression.CompressionLevel;

namespace Game.Common
{
    public static class PackerUtility
    {
        private static readonly string Password = "MySecretPassword123";
        private const int BufferSize = 8 * 1024 * 1024; // 8 MB

        // Метаданные файла
        private class FileMetadata
        {
            public string RelativePath;
            public long Size;
            public long DataOffset;
        }

        // =========================================================================
        // 📦 ASYNC PACK
        // =========================================================================
        public static async Task PackAsync(string folderPath, string mpPath, IProgress<float> progress = null)
        {
            if (!Directory.Exists(folderPath))
                throw new DirectoryNotFoundException(folderPath);

            string[] files = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);
            List<FileMetadata> metadata = new List<FileMetadata>();

            long offset = 0;
            long totalSize = 0;

            // Считаем общий размер файлов
            foreach (var file in files)
            {
                long size = new FileInfo(file).Length;
                totalSize += size;

                string relative = Path.GetRelativePath(folderPath, file).Replace('\\', '/');
                metadata.Add(new FileMetadata
                {
                    RelativePath = relative,
                    Size = size,
                    DataOffset = offset
                });

                offset += size;
            }

            byte[] header = EncryptHeader(metadata);

            byte[] buffer = new byte[BufferSize];

            long processed = 0;

            await using (var outFs = new FileStream(mpPath, FileMode.Create, FileAccess.Write, FileShare.None, BufferSize, true))
            {
                // HEADER
                await outFs.WriteAsync(BitConverter.GetBytes(header.Length));
                await outFs.WriteAsync(header);

                // HMAC
                using var hmac = new HMACSHA256(GetKey());

                foreach (var file in files)
                {
                    await using var inFs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, BufferSize, true);

                    int read;
                    while ((read = await inFs.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await outFs.WriteAsync(buffer, 0, read);
                        hmac.TransformBlock(buffer, 0, read, null, 0);

                        processed += read;
                        progress?.Report((float)processed / totalSize);
                    }
                }

                // финальный блок hmac
                hmac.TransformFinalBlock(Array.Empty<byte>(), 0, 0);

                await outFs.WriteAsync(hmac.Hash);
            }

            progress?.Report(1f);
            Debug.Log("MP PACKED: " + mpPath);
        }

        // =========================================================================
        // 📂 ASYNC UNPACK
        // =========================================================================
        public static async Task UnpackAsync(string mpPath, string outputFolder, IProgress<float> progress = null)
        {
            if (!File.Exists(mpPath))
                throw new FileNotFoundException(mpPath);

            byte[] buffer = new byte[BufferSize];

            await using var fs = new FileStream(mpPath, FileMode.Open, FileAccess.Read, FileShare.Read, BufferSize, true);

            // HEADER
            byte[] lenBytes = new byte[4];
            await fs.ReadAsync(lenBytes, 0, 4);
            int headerLen = BitConverter.ToInt32(lenBytes, 0);

            byte[] header = new byte[headerLen];
            await fs.ReadAsync(header, 0, headerLen);
            var metadata = DecryptHeader(header);

            long dataOffset = 4 + headerLen;
            long dataLength = fs.Length - dataOffset - 32; // без хэша

            long processed = 0;

            using var hmac = new HMACSHA256(GetKey());

            foreach (var f in metadata)
            {
                string fullPath = Path.Combine(outputFolder, f.RelativePath);
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

                await using var outFs = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, BufferSize, true);

                fs.Position = dataOffset + f.DataOffset;

                long remaining = f.Size;

                while (remaining > 0)
                {
                    int toRead = (int)Math.Min(BufferSize, remaining);
                    int read = await fs.ReadAsync(buffer, 0, toRead);
                    if (read <= 0)
                        throw new EndOfStreamException();

                    await outFs.WriteAsync(buffer, 0, read);
                    hmac.TransformBlock(buffer, 0, read, null, 0);

                    remaining -= read;
                    processed += read;

                    progress?.Report((float)processed / dataLength);
                }
            }

            // Проверяем HMAC
            hmac.TransformFinalBlock(Array.Empty<byte>(), 0, 0);

            fs.Position = fs.Length - 32;
            byte[] hmacStored = new byte[32];
            await fs.ReadAsync(hmacStored, 0, 32);

            if (!ConstantTimeEquals(hmacStored, hmac.Hash))
                throw new CryptographicException("MP damaged or modified!");

            progress?.Report(1f);

            Debug.Log("MP UNPACKED: " + outputFolder);
        }

        // =========================================================================
        // HEADER encryption
        // =========================================================================
    
        // =========================================================================
        // 📦 ASYNC SIMPLE PACK (ZIP)
        // =========================================================================
        public static async Task PackSimpleAsync(string folderPath, string zipPath, IProgress<float> progress = null)
        {
            if (!Directory.Exists(folderPath))
                throw new DirectoryNotFoundException(folderPath);

            var files = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);
            long totalSize = files.Sum(f => new FileInfo(f).Length);
            long processed = 0;
            byte[] buffer = new byte[8 * 1024 * 1024]; // 8 MB

            using var zipFs = new FileStream(zipPath, FileMode.Create, FileAccess.Write, FileShare.None, buffer.Length, true);
            using var archive = new ZipArchive(zipFs, ZipArchiveMode.Create, leaveOpen: true);

            foreach (var file in files)
            {
                string relativePath = Path.GetRelativePath(folderPath, file).Replace('\\', '/');
                var entry = archive.CreateEntry(relativePath, CompressionLevel.NoCompression);

                await using var entryStream = entry.Open();
                await using var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, buffer.Length, true);

                int read;
                while ((read = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await entryStream.WriteAsync(buffer, 0, read);
                    processed += read;
                    progress?.Report((float)processed / totalSize);
                }
            }

            progress?.Report(1f);
            Debug.Log("ZIP PACKED: " + zipPath);
        }

        // =========================================================================
        // 📂 ASYNC SIMPLE UNPACK (ZIP)
        // =========================================================================
        public static async Task UnpackSimpleAsync(string zipPath, string outputFolder, IProgress<float> progress = null)
        {
            if (!File.Exists(zipPath))
                throw new FileNotFoundException(zipPath);

            byte[] buffer = new byte[8 * 1024 * 1024]; // 8 MB

            using var zipFs = new FileStream(zipPath, FileMode.Open, FileAccess.Read, FileShare.Read, buffer.Length, true);
            using var archive = new ZipArchive(zipFs, ZipArchiveMode.Read, leaveOpen: true);

            long totalSize = archive.Entries.Sum(e => e.Length);
            long processed = 0;

            foreach (var entry in archive.Entries)
            {
                string fullPath = Path.Combine(outputFolder, entry.FullName);
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

                await using var entryStream = entry.Open();
                await using var outFs = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, buffer.Length, true);

                int read;
                while ((read = await entryStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await outFs.WriteAsync(buffer, 0, read);
                    processed += read;
                    progress?.Report((float)processed / totalSize);
                }
            }

            progress?.Report(1f);
            Debug.Log("ZIP UNPACKED: " + outputFolder);
        }
    
        /// <summary>
        /// Возвращает контрольную сумму набора файлов.
        /// Гарантирует одинаковый результат при одинаковых файлах.
        /// </summary>
    
        public static string GetChecksum(IEnumerable<string> filePaths)
        {
            using var sha256 = SHA256.Create();
            var builder = new StringBuilder();

            foreach (var path in filePaths.OrderBy(p => p))
            {
                var fileInfo = new FileInfo(path);

                builder.Append(fileInfo.Name);   // только имя файла
                builder.Append(fileInfo.Length); // только размер
            }

            var bytes = Encoding.UTF8.GetBytes(builder.ToString());
            var hash = sha256.ComputeHash(bytes);

            return BitConverter.ToString(hash).Replace("-", "");
        }

        private static byte[] EncryptHeader(List<FileMetadata> files)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var f in files)
                sb.Append($"{f.RelativePath}|{f.Size}|{f.DataOffset};");

            byte[] plain = Encoding.UTF8.GetBytes(sb.ToString());

            using Aes aes = Aes.Create();
            aes.Key = GetKey();
            aes.IV = GetIV();

            var encryptor = aes.CreateEncryptor();
            return encryptor.TransformFinalBlock(plain, 0, plain.Length);
        }

        private static List<FileMetadata> DecryptHeader(byte[] encrypted)
        {
            using Aes aes = Aes.Create();
            aes.Key = GetKey();
            aes.IV = GetIV();

            var decryptor = aes.CreateDecryptor();
            byte[] plain = decryptor.TransformFinalBlock(encrypted, 0, encrypted.Length);

            string text = Encoding.UTF8.GetString(plain);
            List<FileMetadata> files = new();

            foreach (var part in text.Split(';', StringSplitOptions.RemoveEmptyEntries))
            {
                string[] p = part.Split('|');
                files.Add(new FileMetadata
                {
                    RelativePath = p[0],
                    Size = long.Parse(p[1]),
                    DataOffset = long.Parse(p[2])
                });
            }

            return files;
        }

        private static byte[] GetKey() => SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(Password));
        private static byte[] GetIV() => MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(Password));

        private static bool ConstantTimeEquals(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) return false;
            int diff = 0;
            for (int i = 0; i < a.Length; i++) diff |= a[i] ^ b[i];
            return diff == 0;
        }
    }
}
