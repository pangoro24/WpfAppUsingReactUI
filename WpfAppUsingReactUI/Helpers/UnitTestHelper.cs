using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WpfAppUsingReactUI.Helpers
{
    public class UnitTestHelper
    {
        public async void TestCrypto()
        {
            var data = "Cryptographic example";
            var password = "MySecretKey";
            var salt = CriptoHelper.CreateSalt(16);
            // MessageBox.Show("Encrypting String " + data + ", with salt " + BitConverter.ToString(salt),"Alert", MessageBoxButton.OK);
            var bytes = CriptoHelper.EncryptAes(data, password, salt);
            //MessageBox.Show("Encrypted, Now Decrypting","Alert", MessageBoxButton.OK);
            var str = CriptoHelper.DecryptAes(bytes, password, salt);
            // MessageBox.Show("Decryted " + str,"Alert", MessageBoxButton.OK);

            string beforeFilePath = @"d:\temp\before.txt";
            string cipherFilePath = @"d:\temp\cipher.txt";
            string afterFilePath = @"d:\temp\after.txt";

            await CriptoHelper.EncryptThenDecryptAsync(beforeFilePath, cipherFilePath, afterFilePath);
        }
    }
}
