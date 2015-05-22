using System.IO;
using System.Security.Cryptography;

namespace Rogolev.Nsudotnet.Enigma
{
    internal class FileEncryptor : FileConverter
    {
        public FileEncryptor(string inFileName, string outFileName, SymmetricAlgorithm algo) : base(inFileName, outFileName, algo)
        {
        }

        protected override void Transformation(Stream inStream, Stream outStream)
        {
            using (var cryptoStream = new CryptoStream(inStream, Algo.CreateEncryptor(), CryptoStreamMode.Read))
                cryptoStream.CopyTo(outStream);
        }
    }
}
