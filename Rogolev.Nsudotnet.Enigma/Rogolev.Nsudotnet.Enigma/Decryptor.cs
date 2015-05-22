using System.IO;
using System.Security.Cryptography;

namespace Rogolev.Nsudotnet.Enigma
{
    internal class Decryptor : FileConverter
    {
        public Decryptor(string inFileName, string outFileName, SymmetricAlgorithm algo) : base(inFileName, outFileName, algo)
        {
        }

        protected override void Transformation(Stream inStream, Stream outStream)
        {
            var cryptoStream = new CryptoStream(outStream, Algo.CreateDecryptor(), CryptoStreamMode.Write);
            inStream.CopyTo(cryptoStream);
        }
    }
}
