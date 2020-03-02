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

namespace Mistware.Utils
{
    /// Interface to be implemented by replacement loggers 
    public interface ILog
    {
        /// Write Debug level log entry 
        void   Debug(object msg);

        /// Write Info level log entry 
        void   Info(object msg);

        /// Write Warn level log entry 
        void   Warn(object msg);

        /// Write Error level log entry 
        void   Error(object msg);

        /// Write Fatal level log entry 
        void   Fatal(object msg);

        /// Set the name of the log file
        string LogFile { get; set; }
    }

    /// Log class with injectable loggers
    public class Log
    {
        private Log() {}

        private static Log _me = new Log();

        /// Returns singleton instance of Log class. 
        public static Log Me { get { return _me; } }

        /// Property for injecting a logger, such as a database logger (the default is to use the RollingLogFile logger)
        public  ILog Logger
        {
            set
            {
                logger = value;
            }
            private get
            {
                if (logger == null) logger = new RollingLogFile();
                return logger;
            }
        }
        private ILog logger = null;    

        /// Fully qualified name of the logfile (or "console").
        public string LogFile
        {
            set
            {
                this.Logger.LogFile = value;
            }
            get
            {
                return this.Logger.LogFile;
            }
        }

        /// Determines whether debug is logged (only logged if this is true). 
        public bool DebugOn 
        {   
            get { return debugOn; } 
            set { debugOn = value; } 
        }
        private bool debugOn = false;  

        /// The Action handler to be called in the event of a Fatal (error).
        /// This gives the app an opportunity to inject code that handles a Fatal (e.g. redirect to Error page and email to admin)         
        public  Action<string> FatalHandler
        {
            get { return fatalHandler; }
            set { fatalHandler = value; }
        }  
        private Action<string> fatalHandler = null;

        /// The Action handler to be called in the event of an Error.
        /// This gives the app an opportunity to inject code that handles an Error (e.g. email to admin) 
        
        public  Action<string> ErrorHandler
        {
            get { return errorHandler; }
            set { errorHandler = value; }
        }
        private Action<string> errorHandler = null;

        /// Write log entry of level 
        /// <param name="level">The level of the log message (Debug, Info, Warn, Error or Fatal)</param>
        /// <param name="msg">The message to log</param>
        public void CallLogger(LogLevel level, object msg)
        {
            switch (level)
            {
                case LogLevel.Debug: 
                    this.Debug(msg);
                    break;

                case LogLevel.Info:    
                    this.Info(msg);
                    break;

                case LogLevel.Warn:    
                    this.Warn(msg);
                    break;

                case LogLevel.Error:    
                    this.Error(msg);
                    break;

                case LogLevel.Fatal:    
                    this.Fatal(msg);
                    break;              

            }
        }      

        /// Write Debug level log entry 
        public void Debug(object msg)
        {
            if (DebugOn) this.Logger.Debug(msg);
        }

        /// Write Info level log entry 
        public void Info(object msg)
        {
            this.Logger.Info(msg);
        }
        
        /// Write Warn level log entry 
        public void Warn(object msg)
        {
            this.Logger.Warn(msg);
        }

        /// Write Error level log entry, and call the ErrorHandler (if set). 
        public void Error(object msg)
        {
            this.Logger.Error(msg);
            if (ErrorHandler != null) ErrorHandler((string)msg);
        }

        /// Write each Inner Error for exception 
        public void InnerError(Exception ex)
        {
            Exception x = ex;
            while (x != null)
            {
                Error(x.Message);
                x = x.InnerException;
            }
        }

        /// Write Fatal level log entry, and call the FatalHandler (if set). 
        public void Fatal(object msg)
        {
            this.Logger.Fatal(msg);
            if (FatalHandler != null) FatalHandler((string)msg);            
        }

    }  

    /// The default and simplest logfile is a rolling log.
    /// A flat text file that is written to until its size gets to 10Mb,
    /// At which point it is renamed and a fresh file started. 
    /// If LogFile name is not set (i.e. null) or "console" then the log is
    /// simply written to the console.    
    public class RollingLogFile : ILog
    {

        /// Write Debug level log entry 
        public void Debug(object msg)
        {
            Write(LogLevel.Debug, msg);
        }

        /// Write Info level log entry 
        public void Info(object msg)
        {
            Write(LogLevel.Info, msg);
        }
        
        /// Write Warn level log entry 
        public void Warn(object msg)
        {
            Write(LogLevel.Warn, msg);
        }

        /// Write Error level log entry 
        public void Error(object msg)
        {
            Write(LogLevel.Error, msg);
        }

        /// Write Fatal level log entry 
        public void Fatal(object msg)
        {
            Write(LogLevel.Fatal, msg);
        }

        /// Reads the logfile 
        public string ReadLog(string option)
        {
            if (logFile != "console") return File.ReadAllText(LogFile);
            else                      return null;    
        }

        
        /// Fully qualified name of the logfile (or "console").
        public  string LogFile     
        { 
            get
            {
                if (logFile == null)
                {
                    logFile = "console";
                }
                return logFile;         
            } 
            set
            {
                logFile = value;
            } 
        }
        private string logFile = null; 

        private void Write(LogLevel level, object msg)
        {
            LogRecord lr = new LogRecord(level, msg);

            if (LogFile == "console")
            {
                Console.WriteLine(lr.ToString());
            }
            else 
            {
                if (File.Exists(LogFile))
                {
                    FileInfo fi = new FileInfo(LogFile);
                    if (fi.Length > 10000000) RollLogFile();
                }
                WriteLogLine(lr.ToString());
            }
        }

        private Object thisLock = new Object();

        private void WriteLogLine(string line)
        {
            lock (thisLock)
            {              
                StreamWriter sw = File.AppendText(LogFile);
                sw.WriteLine(line);
                sw.Dispose();
            }    
        }

        private void RollLogFile()
        {
            try
            {
                if (File.Exists(LogFile + ".10")) File.Delete(LogFile + ".10");
                for (int i=9; i>0 ; i--)
                {
                    string si  = i.ToString();
                    string si1 = (i+1).ToString();
                    if (File.Exists(LogFile + "." + si)) 
                        File.Move(LogFile + "." + si, LogFile + "." + si1);
                }
                File.Move(LogFile, LogFile + ".1");
            }
            catch (Exception ex)
            {
                WriteLogLine("Exception in Mistware.Utils.Log.RollLogFile(): " + ex.Message);       
            }
        }      
    }

    /// Formats log record entries 
    public class LogRecord
    {
        /// Default ctor (no message and severity is debug) 
        public LogRecord()
        {
            this.Tag      = null;
            this.Occurred = new DateTime();
            this.Message  = null;
            this.Level    = LogLevel.Debug;
        }

        /// Normal ctor, using enum for severity and object for message
        public LogRecord(LogLevel level, object message)
        {
            int thread = System.Threading.Thread.CurrentThread.ManagedThreadId;

            this.Tag      = thread.ToString("d5");
            this.Occurred = DateTime.Now;
            this.Message  = message.ToString();
            this.Level    = level;
        }

        /// Alternative ctor with integer severity and string message
        public LogRecord(int severity, string message)
        {
            int thread = System.Threading.Thread.CurrentThread.ManagedThreadId;

            this.Tag      = thread.ToString("d5");
            this.Occurred = DateTime.Now;
            this.Message  = message;            
            this.Level    = (LogLevel)severity;
        }

        /// ThreadId (to distinguish between threads) 
        public string   Tag      { get; set; }

        /// Date and Time of log entry
        public DateTime Occurred { get; set; }

        /// Message Text
        public string   Message  { get; set; }

        /// Log Level (Debug to Fatal)
        public LogLevel Level    { get; set; }

        /// Integer number corresponding to Level
        public int      Severity { get { return (int)Level; } }

        /// Returns the formatted log entry
        public override string ToString()
        {
            return Occurred.ToLogStamp() + " - [" +  Tag + "] " + ( Level.ToString().ToUpper() + " " ).Left(5) + ": " + Message;
        }
    }

    /// Severity Level
    public enum LogLevel 
    { 
        /// Debug (can be switch off)
        Debug = 1, 

        /// Information only 
        Info = 2, 

        /// Warn - Minor Issue
        Warn = 3, 
        
        /// Error - Recoverable Error
        Error = 4, 
        
        /// Fatal - UnRecoverable Error (application should stop working)
        Fatal = 5 
    }
}