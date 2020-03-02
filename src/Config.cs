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
using System.Collections;
using System.Collections.Generic;

namespace Mistware.Utils
{
    /// Static class to handle application settings. Consolidated configuration from environment (variables) 
    /// and config file (which defaults to "web.config"). The Setup method must be called to read in environment 
    /// variables and config file settings. Thereafter settings can be accessed via Get and Set methods. 
    /// Or using a number of built in properties: ContentRoot, WebRoot, AppName, AppURL, Env, Debug, LogFile.
    ///
    /// The config file is read in by Setup(), which looks for Xpath of /configuration/appsettings/add (and uses 
    /// key and value attributes) or /configuration/connectionstrings/add (and uses name and connectionstring attributes).
    public static class Config
    {
        private static Dictionary<string,string> settings = null;

        private static IDictionary               envVars  = null;
      
        /// <summary>
        /// Load all settings from environment variables and config file.  
        /// </summary>
        /// <param name="configFile">Name of the file with application configuration (defaults to web.config).</param>                  
        /// <param name="contentRoot">Sets "ContentRoot".</param>       
        /// <param name="webRoot">Sets "WebRoot" (in .Net Core this is the wwwroot folder below ContentRoot).</param>
        /// <param name="appName">Sets "AppName".</param>   
        public static void Setup(string configFile, string contentRoot, string webRoot = null, string appName = null)
        {
            if (configFile  == null) configFile = "web.config";
            if (contentRoot == null) throw new Exception("Null contentRoot in Config.Setup");
            if (webRoot     == null) webRoot = StripDelimiter(contentRoot) + PathDelimiter + "wwwroot";
            if (appName     == null) appName = LastPathSegment(contentRoot);

            // Obtain environment variables
            settings = new Dictionary<string,string>();
            envVars = Environment.GetEnvironmentVariables();
     
            // Copy across some other settings
            Config.Set("NewLine",     Environment.NewLine);

            ContentRoot = StripDelimiter(contentRoot);
            WebRoot     = StripDelimiter(webRoot);
            AppName     = appName;

            ReadConfig(StripDelimiter(contentRoot) + PathDelimiter + configFile);

            // Ensure defaults are set
            _  = Env;
            if (Get("LogFile")==null) Set("LogFile", AppName+".log");
        }

        private static void ReadConfig(string configFile)
        {
            if (System.IO.File.Exists(configFile))
            {
                XmlFileNode root = XmlFileRead.Load(configFile); 
                if (root == null) return;
                if (root.Name == null || root.Name.ToLower() != "configuration") 
                    throw new Exception("Illegal Configuration file: " + configFile);
                    
                foreach (XmlFileNode node in root.Nodes)
                {
                    if (node.Name.ToLower() == "appsettings"      ) ReadAttributes(node.Nodes, "key",  "value");
                    if (node.Name.ToLower() == "connectionstrings") ReadAttributes(node.Nodes, "name", "connectionstring");
                }
            }
        } 
        private static void ReadAttributes(List<XmlFileNode> nodes, string keyName, string valueName)
        {
            string key;
            string value;

            foreach (XmlFileNode node in nodes)
            {
                if (node.Name.ToLower() == "add")
                {
                    key   = null;
                    value = null;
                    foreach (XmlFileAttribute attribute in node.Attributes) 
                    {
                        if (attribute.Name.ToLower() == keyName)   key   = attribute.Value;
                        if (attribute.Name.ToLower() == valueName) value = attribute.Value;
                    }
                    if (key != null && value != null) Config.Set(key, value);
                }
            }
        }  

        /// <summary>
        /// Add or update a setting in the settings dictionary.
        /// </summary>
        /// <param name="key">Name of the setting.</param>        
        /// <param name="value">Value of the setting.</param>
        public static void Set(string key, string value)
        {
            if (settings == null) throw new Exception("Config.Setup() must be called, before Config can be used.");

            if (key != null || value != null)
            {
                if (settings.ContainsKey(key)) settings[key] = value;
                else                           settings.Add(key, value);
            } 
        }

        /// <summary>
        /// Get a setting. If it is not in the settings dictionary, then the environment variables dictionary
        /// will also be searched.
        /// </summary>
        /// <param name="key">Name of the setting.</param>        
        /// <returns>Value of the setting, or null if not set.</returns>
        public static string Get(string key)
        {
            if (settings == null) throw new Exception("Config.Setup() must be called, before Config can be used.");

            if (key == null) return null;

            if (settings.ContainsKey(key)) return (string)settings[key];
            else if (envVars.Contains(key)) 
            {
                string value = (string)envVars[key];
                settings.Add(key, value);
                return value;
            }
            else                           return null;
        }

        /// <summary>
        /// The folder containing the application (no default). 
        /// </summary>
        public static string ContentRoot
        { 
            get { return Get("ContentRoot"); } 
            set { Set("ContentRoot", value); } 
        }      

        /// <summary>
        /// The folder containing static files to be served (css, js, etc). Typically the wwwroot 
        /// folder within ContentRoot (if null defaults to ContentRoot/wwwroot).
        /// </summary>
        public static string WebRoot 
        { 
            get { return Get("WebRoot"); } 
            set { Set("WebRoot", value); } 
        }      

        /// <summary>
        /// The name of the application (defaults to the last folder in the ContentRoot path). 
        /// </summary>
        public static string AppName 
        { 
            get { return Get("AppName"); } 
            set { Set("AppName", value); } 
        }      

        /// <summary>
        /// The URL to access the application. 
        /// </summary>
        public static string AppURL 
        { 
            get { return Get("AppURL"); } 
            set { Set("AppURL", value); } 
        }      

        /// <summary>
        /// The name of the environment (Development, Test, Staging or Production).
        /// If not set, then this is obtained from the ASPNETCORE_ENVIRONMENT environment variable.
        /// </summary>
        public static string Env
        {
            get
            {
                string env = Get("Env");     
                if (env == null)
                {
                    env = Get("ASPNETCORE_ENVIRONMENT");
                    if (env == null) env = "Development";
                    Set("Env", env);    
                }                 
                return env;
            }
            set
            {
              Set("Env", value);      
            }
        }

        /// <summary>
        /// Returns true if Debug is required. Debug is only required in the Development environment. 
        /// Defaults to true if Environment is Development
        /// </summary>
        public static bool Debug
        {
            get { return Env == "Development"; }
        }

        /// <summary>
        /// The full path and filename of the log file (assuming it is stored in the logs folder under ContentRoot).
        /// </summary>
        public static string LogFile
        {
            get { return ContentRoot + PathDelimiter + "Logs" + PathDelimiter + Get("LogFile"); }
        }

        private static string PathDelimiter
        {
            get { return System.IO.Path.DirectorySeparatorChar.ToString(); }
        }

        /// Ensure last character in path is not a delimiter
        private static string StripDelimiter(string path)
        {
            if (path == null) return null;

            if (path.Right(1) == PathDelimiter) path = path.Left(path.Length - 1);
            return path;
        }

        /// Get Last segment of path
        private static string LastPathSegment(string path)
        {
            if (path == null) return null;

            path = StripDelimiter(path);
            int i = path.LastIndexOf(PathDelimiter);
            if (i <= 0) return null;
            
            return path.Substring(i + 1);
        }

        /// <summary>
        /// Returns a string with each setting in JSON format ("key: value, "). 
        /// </summary> 
        public static string DebugConfig()
        {
            string result = null;
            foreach (KeyValuePair<string, string> kp in settings)
            {
                if (result != null) result += ", ";
                result += kp.Key + ": " + kp.Value;
            }
            return result;
        }
    }
}
