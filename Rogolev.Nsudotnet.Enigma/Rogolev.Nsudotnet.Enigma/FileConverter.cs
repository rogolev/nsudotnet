using System.IO;
using System.Security.Cryptography;

namespace Rogolev.Nsudotnet.Enigma
{
    internal abstract class FileConverter
    {
        protected SymmetricAlgorithm Algo;
        private readonly FileInfo _inFile;
        private readonly FileInfo _outFile;

        protected FileConverter(string inFileName, string outFileName, SymmetricAlgorithm algo)
        {
            Algo = algo;
            _inFile = new FileInfo(inFileName);
            _outFile = new FileInfo(outFileName);
        }

        public void PerformTransformation()
        {
            using (FileStream inFileStream = _inFile.OpenRead(),
                outFileStream = _outFile.OpenWrite())
            {
                Transformation(inFileStream, outFileStream);
            }
        }

        protected abstract void Transformation(Stream inStream, Stream outStream);
    }
}
