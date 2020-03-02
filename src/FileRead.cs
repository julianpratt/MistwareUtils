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
using System.Net;


namespace Mistware.Utils
{
    /// <summary>
    /// Load text file into string
    /// </summary>
    public static class FileLoad
    {

        /// <summary>
        /// Load text file into string
        /// </summary>
        public static string Load(string path, string filename)
        {
            try
            {
                FileRead reader = new FileRead(path, filename);
                string file = reader.ReadToEnd();
                reader.Dispose();
                return file; 
            }
            catch (Exception ex)
            {
                throw new Exception("Could not load " + path + filename + ". Error was: " +ex.Message);
            }
        }
    }

    /// <summary>
    /// StreamReader wrapper to read text file from filesystem or the internet.
    /// </summary>
    public class FileRead : IDisposable
    {
        /// Location of the file to read (the full name of the file is Path+Filename)
        public string           Path           { get; }

        /// Name of the file to read (the full name of the file is Path+Filename)
        public string           Filename       { get; }

        private Stream          ResponseStream { get; set; }

        private StreamReader    Reader         { get; set; }

        private HttpWebResponse WebResponse    { get; set; }

        /// Open file for reading. The full name of the file is Path+Filename.
        /// If the first 4 characters of Path are "http" then the file is read from the internet,
        /// otherwise an attempt is made to find it in the local filesystem.
        /// <param name="path">Location of the file to read</param>
        /// <param name="filename">Name of the file to read</param>
        public FileRead(string path, string filename)
        {
            path.ThrowOnNullOrEmpty("path", "path cannot be null or empty in FileRead");
            filename.ThrowOnNullOrEmpty("filename", "filename cannot be null or empty in FileRead");

            Path = path;
            Filename = filename;
            Reader = null;
            WebResponse = null;

            string fullname = path + filename;
            string type = (fullname.Right(4) == ".xml") ? "xml" : "txt";  

            try
            {
                 if (path.Left(4) == "http")
                {
                    // Read file from the internet
                    WebOpen(fullname, type);
                }
                else
                {
                    // Read file from local filesystem
                    if (!File.Exists(fullname)) throw new Exception("Could not read " + fullname + " - file not found.");
                    // Found file
                    Reader = File.OpenText(fullname);
                }
            }
            catch (Exception ex)
            {
                this.Dispose();
                throw new Exception("Could not open " + fullname + ". Error was: " +ex.Message);
            }
        }

        private void WebOpen(string url, string type)
        {
            // declare httpwebrequet wrt url defined above
            HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(url);
            // set method as get
            webrequest.Method = "GET";
            // set content type
            webrequest.ContentType = (type.ToLower() == "xml") ? "text/xml" : "text/plain";
            // No data is sent 
            webrequest.ContentLength = 0;
            // declare & read response from service
            WebResponse = (HttpWebResponse)webrequest.GetResponse();

            if (WebResponse.StatusCode == HttpStatusCode.OK)
            {
                ResponseStream = WebResponse.GetResponseStream();
                if ( WebResponse.ContentLength <= 0 ) // It may be valid to have no content, so avoid spurious error. 
                {
                    ResponseStream.Close();
                    ResponseStream = null;
                }
                else
                {
                    Reader = new StreamReader(ResponseStream);
                }
            }
        }

        /// Reads a line of characters from the current file and returns the data as a string.
        /// <returns>The next line from the input file, or null if the end of the input file is reached.</returns>
        /// <exception cref="OutOfMemoryException">There is insufficient memory to allocate a buffer for the returned string.</exception>
        /// <exception cref="IOException">An I/O error occurs.</exception>
        public string ReadLine()
        {
            return (Reader != null) ? Reader.ReadLine() : null;
        }

        /// Reads all characters from the current position to the end of the file. 
        /// <returns>The rest of the file as a string, from the current position to the end. If the current position is at the end of the file, returns an empty string ("").</returns>
        /// <exception cref="OutOfMemoryException">There is insufficient memory to allocate a buffer for the returned string.</exception>
        /// <exception cref="IOException">An I/O error occurs.</exception>
        public string ReadToEnd()
        {
            return (Reader != null) ? Reader.ReadToEnd() : null;
        }

        /// Close a stream. In fact it just does a Dispose().
        /// If either ResponseStream or WebResponse are open, then close these also.  
        public void Dispose()
        {
            if (Reader != null) 
            {
                Reader.Dispose();
                Reader = null;
            }
           
            if (ResponseStream != null)
            {
                ResponseStream.Close();
                ResponseStream.Dispose();
                ResponseStream = null;
            } 
            
            if (WebResponse != null) 
            {
                WebResponse.Close();
                WebResponse.Dispose();
                WebResponse    = null;
            }
        
        }

        /// Check whether a stream is still open (i.e. hasn't been closed).
        public bool IsOpen()
        {
            return (Reader != null); 
        }

        /// Tests whether stream has any more characters to read. False if more to read. True if at end.
        public bool EndOfStream { get { return Reader != null ? (Reader.Peek() == -1) : true; } }

    }
}