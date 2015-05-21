using System;
using System.IO;
using System.Security.Cryptography;

namespace Rogolev.Nsudotnet.Enigma
{
    class Encryptor
    {
        private const string KeyFileExtesion = "key";
        public static void EncryptFile(SymmetricAlgorithm algo, string inFileName, string outFileName)
        {
            string keyFileExtension = string.Concat(KeyFileExtesion, Path.GetExtension(inFileName));
            string keyFileName = Path.ChangeExtension(inFileName, keyFileExtension);
            var sKey = Convert.ToBase64String(algo.Key);
            var iv = Convert.ToBase64String(algo.IV);
            using (FileStream inStream = new FileStream(inFileName, FileMode.Open, FileAccess.Read),
                outStream = new FileStream(outFileName, FileMode.Create, FileAccess.Write),
                keyStream = new FileStream(keyFileName, FileMode.Create, FileAccess.Write))
            {
                var keyWriter = new BinaryWriter(keyStream);
                keyWriter.Write(iv);
                keyWriter.Write(sKey);
                var encryptor = algo.CreateEncryptor();
                var cryptoStream = new CryptoStream(inStream, encryptor, CryptoStreamMode.Read);
                cryptoStream.CopyTo(outStream);
            }
        }
    }
}
