//ArmanDoesStuff 2021
//https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.aes?view=net-5.0

using System.IO;
using System.Security.Cryptography;

namespace Armony.Utilities
{
    public static class EncrypterAES
    {
        // Change this key: https://www.random.org/cgi-bin/randbyte?nbytes=32&format=d
        private static byte[] Key = { 113, 217, 19, 14, 23, 16, 25, 45, 114, 184, 27, 162, 11, 222, 222, 209, 241, 24, 175, 144, 143, 53, 196, 44, 24, 46, 17, 218, 111, 236, 53, 249 };

        // a hardcoded IV should not be used for production AES-CBC code
        // IVs should be unpredictable per ciphertext
        private static byte[] IV = { 106, 64, 191, 111, 23, 123, 113, 109, 231, 121, 152, 114, 79, 32, 114, 110 };

        public static byte[] EncryptStringToBytesAes(string plainText)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
            {
                UnityEngine.Debug.LogError("plainText invalid");
                return null;
            }
            byte[] encrypted;

            // Create an Aes object with the specified key and IV
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        public static string DecryptStringFromBytesAes(byte[] cipherText)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
            {
                UnityEngine.Debug.LogError("cipherText invalid");
                return null;
            }

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object with the specified key and IV
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
    }
}
