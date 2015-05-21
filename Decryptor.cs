using System;
using System.IO;
using System.Security.Cryptography;

namespace Rogolev.Nsudotnet.Enigma
{
    class Decryptor
    {
        public static void DecryptFile(SymmetricAlgorithm algo, string inFileName, string keyFileName, string outFileName)
        {
            using (FileStream inStream = new FileStream(inFileName, FileMode.Open, FileAccess.Read),
                outStream = new FileStream(outFileName, FileMode.Create, FileAccess.Write),
                keyStream = new FileStream(keyFileName, FileMode.Open, FileAccess.Read))
            {
                var keyReader = new BinaryReader(keyStream);
                var iv = Convert.FromBase64String(keyReader.ReadString());
                var sKey = Convert.FromBase64String(keyReader.ReadString());
                algo.Key = sKey;
                algo.IV = iv;
                var decryptor = algo.CreateDecryptor();
                var cryptoStream = new CryptoStream(outStream, decryptor, CryptoStreamMode.Write);
                inStream.CopyTo(cryptoStream);
            }
        }
    }
}
