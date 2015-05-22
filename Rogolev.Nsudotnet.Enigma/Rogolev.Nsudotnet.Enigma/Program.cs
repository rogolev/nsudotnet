using System;
using System.IO;
using System.Security.Cryptography;

namespace Rogolev.Nsudotnet.Enigma
{
    class Program
    {
        private readonly static string Usage =
            string.Concat("Usage: <encrypt|decrypt> <algortihm> <input file> (<key file>) <output file> ", "\n",
            "Note: <key file> is only necessary if you need to decrypt an input file");
        static void Main(string[] args)
        {
            if (args.Length != 4 && args.Length != 5)
            {
                Console.WriteLine(Usage);
                return;
            }
            try
            {
                using (var algo = DefineAlgorithm(args[1]))
                {
                    switch (args[0].ToLower())
                    {
                        case "encrypt":
                            if (args.Length == 4)
                                EncryptFile(algo, args[2], args[3]);
                            else
                                Console.WriteLine(Usage);
                            break;
                        case "decrypt":
                            if (args.Length == 5)
                                DecryptFile(algo, args[2], args[3], args[4]);
                            else
                                Console.WriteLine(Usage);
                            break;
                        default:
                            Console.WriteLine(Usage);
                            return;
                    }
                }
            }
            catch (AlgorithmNotSupportedException e)
            {
                Console.WriteLine(string.Concat("Algorithm ", e.Message, " is not supported"));
            }
            
        }

        private static SymmetricAlgorithm DefineAlgorithm(string algoName)
        {
            switch (algoName.ToLower())
            {
                case "rijndael": return Rijndael.Create();
                case "aes": return Aes.Create();
                case "des": return DES.Create();
                case "rc2": return RC2.Create();
                default:
                    throw new AlgorithmNotSupportedException(algoName);
            }
        }
        internal class AlgorithmNotSupportedException : Exception
        {
            public AlgorithmNotSupportedException(string s)
                : base(s)
            {
            }
        }

        private static void EncryptFile(SymmetricAlgorithm algo, string inFileName, string outFileName)
        {
            algo.GenerateIV();
            algo.GenerateKey();
            const string keyFileAdditionalExtension = "key";
            string keyFileExtension = string.Concat(keyFileAdditionalExtension, Path.GetExtension(inFileName));
            string keyFileName = Path.ChangeExtension(inFileName, keyFileExtension);
            StoreKey(keyFileName, algo.IV, algo.Key);
            Encryptor encryptor = new Encryptor(inFileName, outFileName, algo);
            encryptor.PerformTransformation();
        }

        private static void StoreKey(string keyFileName, byte[] iv, byte[] key)
        {
            using (var keyFileStream = new FileStream(keyFileName, FileMode.Create, FileAccess.Write)) 
            {
                var keyWriter = new BinaryWriter(keyFileStream);
                keyWriter.Write(Convert.ToBase64String(iv));
                keyWriter.Write(Convert.ToBase64String(key));
            }
        }

        private static void DecryptFile(SymmetricAlgorithm algo, string inFileName, string keyFileName, string outFileName)
        {
            byte[] iv, key;
            RestoreKey(keyFileName, out iv, out key);
            algo.IV = iv;
            algo.Key = key;
            Decryptor decryptor = new Decryptor(inFileName, outFileName, algo);
            decryptor.PerformTransformation();
        }

        private static void RestoreKey(string keyFileName, out byte[] iv, out byte[] key)
        {
            using (var keyFileStream = new FileStream(keyFileName, FileMode.Open, FileAccess.Read))
            {
                var keyReader = new BinaryReader(keyFileStream);
                iv = Convert.FromBase64String(keyReader.ReadString());
                key = Convert.FromBase64String(keyReader.ReadString());
            }
        }
    }
}
