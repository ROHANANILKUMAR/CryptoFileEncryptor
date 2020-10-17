using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FileEncryptionServices
{
    class Encryption
    {
        

            private static SymmetricAlgorithm mobjCryptoService = new DESCryptoServiceProvider();

            private static byte[] GetLegalKey(string Key)
            {
                string sTemp;
                if (mobjCryptoService.LegalKeySizes.Length > 0)
                {
                    int lessSize = 0;
                    int moreSize = mobjCryptoService.LegalKeySizes[0].MinSize;

                    // key sizes are in bits
                    while (Key.Length * 8 > moreSize)
                    {
                        lessSize = moreSize;
                        moreSize += mobjCryptoService.LegalKeySizes[0].SkipSize;
                    }
                    sTemp = Key.PadRight(moreSize / 8, ' ');
                }
                else
                    sTemp = Key;



                // convert the secret key to byte array
                //return ASCIIEncoding.ASCII.GetBytes(sTemp);

                return UTF8Encoding.UTF8.GetBytes(sTemp);
            }

            public static string Encrypt(string contend, string key = "C4Y?T0")
            {
                byte[] contendBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(contend);
                System.IO.MemoryStream ms = new MemoryStream();

                byte[] bytKey = GetLegalKey(key);

                mobjCryptoService.Key = bytKey;
                mobjCryptoService.IV = bytKey;

                ICryptoTransform crypto = mobjCryptoService.CreateEncryptor();

                CryptoStream cs = new CryptoStream(ms, crypto, CryptoStreamMode.Write);

                cs.Write(contendBytes, 0, contendBytes.Length);
                cs.FlushFinalBlock();

                byte[] EncByte = ms.ToArray();
                return Convert.ToBase64String(EncByte);



            }

        public static string Decrypt(string EncryptedData, string Key = "C4Y?T0")
        {
            byte[] EncryptdedDataBytes = Convert.FromBase64String(EncryptedData);

            MemoryStream ms = new MemoryStream(EncryptdedDataBytes, 0, EncryptdedDataBytes.Length);

            byte[] keyBytes = GetLegalKey(Key);

            mobjCryptoService.Key = keyBytes;
            mobjCryptoService.IV = keyBytes;

            ICryptoTransform encrypto = mobjCryptoService.CreateDecryptor();

            CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Read);

            StreamReader sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }




    }
        }
    

