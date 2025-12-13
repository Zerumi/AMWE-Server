// Encryption & Decryption code source here: https://stackoverflow.com/questions/10168240/encrypting-decrypting-a-string-in-c-sharp
#pragma warning disable // legacy encryption used for backward compatibility with old passwords

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AMWE_RealTime_Server.Models
{
    public class Encryption
    {
        private static string Loadcode()
        {
            return Environment.GetEnvironmentVariable("Security__Legacy_Crypt_Code") ?? throw new Exception("Crypt code not found in environment variables");
        }
        private static readonly string PassPhrase = Loadcode();

        // This constant is used to determine the keysize of the encryption algorithm in bits.
        // We divide this by 8 within the code below to get the equivalent number of bytes.
        private const int Keysize = 128;

        // This constant determines the number of iterations for the password bytes generation function.
        private const int DerivationIterations = 1000;

        public static string Encrypt(string plainText)
        {
            // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
            // so that the same Salt and IV values can be used when decrypting.  
            byte[] saltStringBytes = Generate128BitsOfRandomEntropy();
            byte[] ivStringBytes = Generate128BitsOfRandomEntropy();
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(PassPhrase, saltStringBytes, DerivationIterations);
            byte[] keyBytes = password.GetBytes(Keysize / 8);
            using RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.BlockSize = 128;
            symmetricKey.Mode = CipherMode.CBC;
            symmetricKey.Padding = PaddingMode.PKCS7;
            using ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes);
            using MemoryStream memoryStream = new MemoryStream();
            using CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
            // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
            byte[] cipherTextBytes = saltStringBytes;
            cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
            cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            return Convert.ToBase64String(cipherTextBytes);
        }

        public static string Decrypt(string cipherText)
        {
            try
            {   // Get the complete stream of bytes that represent:
                // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
                byte[] cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
                // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
                byte[] saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
                // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
                byte[] ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
                // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
                byte[] cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8 * 2).Take(cipherTextBytesWithSaltAndIv.Length - (Keysize / 8 * 2)).ToArray();

                using Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(PassPhrase, saltStringBytes, DerivationIterations);
                byte[] keyBytes = password.GetBytes(Keysize / 8);
                using RijndaelManaged symmetricKey = new RijndaelManaged();
                symmetricKey.BlockSize = 128;
                symmetricKey.Mode = CipherMode.CBC;
                symmetricKey.Padding = PaddingMode.PKCS7;
                using ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes);
                using MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
                using CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
                byte[] plainTextBytes = new byte[cipherTextBytes.Length];
                int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                memoryStream.Close();
                cryptoStream.Close();
                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private static byte[] Generate128BitsOfRandomEntropy()
        {
            byte[] randomBytes = new byte[16]; // 32 Bytes will give us 256 bits.
            using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
            {
                // Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }
    }
}