using PCLCrypto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WpfAppUsingReactUI.Helpers
{
    class CriptoHelper
    {
        /// <summary>    
        /// Creates Salt with given length in bytes.    
        /// </summary>    
        /// <param name="lengthInBytes">No. of bytes</param>    
        /// <returns></returns>    
        public static byte[] CreateSalt(int lengthInBytes)
        {
            return WinRTCrypto.CryptographicBuffer.GenerateRandom(lengthInBytes);
        }

        /// <summary>    
        /// Creates a derived key from a comnination     
        /// </summary>    
        /// <param name="password"></param>    
        /// <param name="salt"></param>    
        /// <param name="keyLengthInBytes"></param>    
        /// <param name="iterations"></param>    
        /// <returns></returns>    
        public static byte[] CreateDerivedKey(string password, byte[] salt, int keyLengthInBytes = 32, int iterations = 1000)
        {
            byte[] key = NetFxCrypto.DeriveBytes.GetBytes(password, salt, iterations, keyLengthInBytes);
            return key;
        }

        /// <summary>    
        /// Encrypts given data using symmetric algorithm AES    
        /// </summary>    
        /// <param name="data">Data to encrypt</param>    
        /// <param name="password">Password</param>    
        /// <param name="salt">Salt</param>    
        /// <returns>Encrypted bytes</returns>    
        public static byte[] EncryptAes(string data, string password, byte[] salt)
        {
            byte[] key = CreateDerivedKey(password, salt);

            ISymmetricKeyAlgorithmProvider aes = WinRTCrypto.SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithm.AesCbcPkcs7);
            ICryptographicKey symetricKey = aes.CreateSymmetricKey(key);
            var bytes = WinRTCrypto.CryptographicEngine.Encrypt(symetricKey, Encoding.UTF8.GetBytes(data));
            return bytes;
        }
        /// <summary>    
        /// Decrypts given bytes using symmetric alogrithm AES    
        /// </summary>    
        /// <param name="data">data to decrypt</param>    
        /// <param name="password">Password used for encryption</param>    
        /// <param name="salt">Salt used for encryption</param>    
        /// <returns></returns>    
        public static string DecryptAes(byte[] data, string password, byte[] salt)
        {
            byte[] key = CreateDerivedKey(password, salt);

            ISymmetricKeyAlgorithmProvider aes = WinRTCrypto.SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithm.AesCbcPkcs7);
            ICryptographicKey symetricKey = aes.CreateSymmetricKey(key);
            var bytes = WinRTCrypto.CryptographicEngine.Decrypt(symetricKey, data);
            return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }

        static async Task CryptoTransformFileAsync(string sourcePath, string destinationPath, ICryptoTransform[] transforms, CancellationToken cancellationToken)
        {
            const int BufferSize = 4096;
            using (var sourceStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read, FileShare.Read, BufferSize, useAsync: true))
            {
                using (var destinationStream = new FileStream(destinationPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, BufferSize, useAsync: true))
                {
                    using (var cryptoStream = CryptoStream.WriteTo(destinationStream, transforms))
                    {
                        await sourceStream.CopyToAsync(cryptoStream, BufferSize, cancellationToken);
                        await cryptoStream.FlushAsync(cancellationToken);
                        cryptoStream.FlushFinalBlock();
                    }
                }
            }
        }

        public static async Task EncryptThenDecryptAsync(string beforeFilePath, string cipherFilePath, string afterFilePath)
        {
            ISymmetricKeyAlgorithmProvider aesGcm = WinRTCrypto.SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithm.AesCbcPkcs7);
            byte[] keyMaterial = WinRTCrypto.CryptographicBuffer.GenerateRandom(32);
            var cryptoKey = aesGcm.CreateSymmetricKey(keyMaterial);
            string key = "";
            foreach (var i in keyMaterial)
            {
                key = key + i + ",";
            }
            var encryptor = new ICryptoTransform[] { WinRTCrypto.CryptographicEngine.CreateEncryptor(cryptoKey) };
            var decryptor = new ICryptoTransform[] { WinRTCrypto.CryptographicEngine.CreateDecryptor(cryptoKey) };
            await CryptoTransformFileAsync(beforeFilePath, cipherFilePath, encryptor, CancellationToken.None);
            await CryptoTransformFileAsync(cipherFilePath, afterFilePath, decryptor, CancellationToken.None);
        }

        public static async Task decryptFile(string inFile, byte[] keyArray, string outFile)
        {
            ISymmetricKeyAlgorithmProvider aesGcm = WinRTCrypto.SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithm.AesCbcPkcs7);
            var cryptoKey = aesGcm.CreateSymmetricKey(keyArray);
            var decryptor = new ICryptoTransform[] { WinRTCrypto.CryptographicEngine.CreateDecryptor(cryptoKey) };
            await CryptoTransformFileAsync(inFile, outFile, decryptor, CancellationToken.None);
        }


    }
}
