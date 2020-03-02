/*
License

Some of this may have been acquired from other sources, whose copyright has 
been lost. So no copyright is claimed and it is unreasonable to grant 
permission to use, copy, modify, etc (as in the normal MIT License). 

If any copyright holders identify their material herein, then the
appropriate copyright notice will be added. 

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Mistware.Utils
{
    /// AES Encryption and Decryption wrapper. Encryption.Encrypt(string) returns AES encrypted 
    /// string encoded to base64. Encryption.Decrypt(string) reverses this by undoing the base64 
    /// encoding and then the AES encryption. The encrypted values are only valid for that day. 
    public static class Encryption
    {
        private static string key = null;

        /// <summary>
        /// Facilitates replacement of the AES key.
        /// Set overrides the default key. The key string should not have spaces. Has private getter.
        /// </summary>
        public static string Key 
        { 
            private get
            {
                return key;
            }
            set 
            { 
                if (value != null) key = value;
            } 
        }
    
        /// <summary>
        /// AES Encrypt <paramref name="input"/> and Base64 encode it.
        /// </summary>
        /// <param name="input">The string to encrypt</param>
        /// <returns><paramref name="input"/> encrypted</returns>
        public static string Encrypt(string input)
        {
            try
            {
                UTF8Encoding encoder = new UTF8Encoding();
                return ToBase64(Transform(encoder.GetBytes(input), true));
            }
            catch (Exception ex)
            {
                throw new Exception("Encryption.Encrypt failed. Message: " + ex.Message);
            }
        }

        /// <summary>
        /// Base64 decode and AES Decrypt <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The string to decrypt</param>
        /// <returns><paramref name="input"/> decrypted</returns>
        public static string Decrypt(string input)
        {
            try
            {
                UTF8Encoding encoder = new UTF8Encoding();
                return encoder.GetString(Transform(FromBase64(input), false));
            }
            catch (Exception ex)
            {
                throw new Exception("Encryption.Decrypt failed. Message: " + ex.Message);
            }
        }

        private static byte[] Transform(byte[] buffer, bool encrypt)
        {
            Key.ThrowOnNullOrEmpty("Key","Cannot Encrypt or Decrypt without a key!");

            byte[] vector = { 146, 64, 191, 11, 23, 32, 114, 119, 231, 21, 212, 112, 79, 3, 113, 156 };
            byte[] key    = FromBase64(Key);

            Aes aes = Aes.Create();    
            ICryptoTransform transform;
            if (encrypt) 
                transform = aes.CreateEncryptor(key, vector);
            else    
                transform = aes.CreateDecryptor(key, vector);

            MemoryStream stream = new MemoryStream();
            using (CryptoStream cs = new CryptoStream(stream, transform, CryptoStreamMode.Write))
            {
                cs.Write(buffer, 0, buffer.Length);
            }
            return stream.ToArray();
        }

        // Base64 encoding takes groups of 3 bytes (8 bits) and encodes them into 4 characters 
        // using the following 64 characters: ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/
        // the encoded value is padded with '=' so the total number of characters is divisible by 4.
        //  
        // However the last 2 characters ('+' and '/') can cause problems in various situations 
        // (e.g. when used in a URL). 
        // So these can be replaced: '-' in place of '+'' and '_' in place of '/'. 
        // As for the padding: just remove it (the =) - one can infer the amount of padding needed. 
        // At the other end: just reverse the process
        //  

        private static byte[] FromBase64(string s)
        {
            return System.Convert.FromBase64String(s.Replace('_', '/').Replace('-', '+'));
        } 
        
        private static string ToBase64(byte[] buffer)
        {
            return System.Convert.ToBase64String(buffer).Replace('+', '-').Replace('/', '_');
        } 

        private static readonly char[] padding = { '=' };

    }    

}
