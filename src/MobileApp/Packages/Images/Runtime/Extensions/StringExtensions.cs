using System;
using System.Security.Cryptography;
using System.Text;

namespace Sotbifay.Images
{
    public static class StringExtensions
    {
        public static string ComputeSha256Hash(this string input)
        {
            if(input is null)
                throw new ArgumentNullException(nameof(input));
            
            const string hex = "0123456789ABCDEF";

            var byteCount = Encoding.UTF8.GetByteCount(input);
            var utf8Bytes = byteCount <= 256 ? stackalloc byte[byteCount] : new byte[byteCount];
            Encoding.UTF8.GetBytes(input.AsSpan(), utf8Bytes);

            using var sha256 = SHA256.Create();
            Span<byte> hashBytes = stackalloc byte[32];
            sha256.TryComputeHash(utf8Bytes, hashBytes, out var hashBytesCount);

            Span<char> charBuffer = stackalloc char[64];
            for (var i = 0; i < hashBytesCount; i++)
            {
                var hashByte = hashBytes[i];
                charBuffer[i * 2] = hex[hashByte >> 4];
                charBuffer[i * 2 + 1] = hex[hashByte & 0xF];
            }

            return new string(charBuffer);
        }
    }
}
