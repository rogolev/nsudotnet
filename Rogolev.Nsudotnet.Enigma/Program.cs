using System;
using System.Security.Cryptography;

namespace Rogolev.Nsudotnet.Enigma
{
    class Program
    {
        private const string Usage =
            @"Usage: <encrypt|decrypt> <cryptoalgortihm> <input file> (<key file>) <output file> " + "\n" +
            @"Note: <key file> is only necessary if you need to decrypt an input file";
        static void Main(string[] args)
        {
            if (args.Length != 4 && args.Length != 5)
                Console.WriteLine(Usage);
            try
            {
                using (var algo = DefineAlgorithm(args[1]))
                {
                    switch (args[0])
                    {
                        case "encrypt":
                            if (args.Length == 4)
                            {
                                algo.GenerateKey();
                                algo.GenerateIV();
                                Encryptor.EncryptFile(algo, args[2], args[3]);
                            }
                            else
                                Console.WriteLine(Usage);
                            break;
                        case "decrypt":
                            if (args.Length == 5)
                                Decryptor.DecryptFile(algo, args[2], args[3], args[4]);
                            else
                                Console.WriteLine(Usage);
                            break;
                        default:
                            Console.WriteLine(Usage);
                            return;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static SymmetricAlgorithm DefineAlgorithm(string algoName)
        {
            switch (algoName)
            {
                case "rijndael":
                case "aes": return Aes.Create();
                case "des": return DES.Create();
                case "rc2": return RC2.Create();
                default:
                    throw new AlgorithmNotSupportedException("Algorithm " + algoName + " is not supported");
            }
        }
        internal class AlgorithmNotSupportedException : Exception
        {
            public AlgorithmNotSupportedException(string s)
                : base(s)
            {
            }
        }
    }
}
