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
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mistware.Utils
{
    /// Simple XML Reader to read web.config and app.config files. 
    /// Currently only reads data from attributes.  
    public static class XmlFileRead
    { 
        /// Load the file into an XML tree
        /// <param name="filename">Fully qualified name of the file to load.</param>
        /// <returns>Root node pointing to a tree of XML data.</returns>
        public static XmlFileNode Load(string filename)
        {
            XmlFile file = ReadFile(filename);
            EatDeclaration(file);
            return GetNode(file);
        } 

        private static XmlFile ReadFile(string filename)
        {
            StringBuilder sb = new StringBuilder();

            using (StreamReader reader = File.OpenText(filename))
            {
                while (!reader.EndOfStream) sb.Append(reader.ReadLine());
            }
            XmlFile file = new XmlFile();
            file.File = sb.ToString().ToCharArray();
            file.Pos = 0;

            return file;
        } 

        private static XmlFileNode GetNode(XmlFile file)
        {
            return GetNode(file, null);
        }
        
        private static void GetNodes(XmlFile file, XmlFileNode parent)
        {
            while (file.Pos < file.MaxPos)            
            {
                XmlFileNode node = GetNode(file, parent.Name);
                if (node == null) break;
                parent.Nodes.Add(node);
            }
        }

        private static XmlFileNode GetNode(XmlFile file, string parentName)
        {
            EatComments(file);

            if (Test(file,"</"))
            {
                // End Tag
                string name = GetElementName(file);
                if (parentName == null) 
                    throw new Exception("XmlReader: Misplaced end tag - </" + name + ">. Parent was null.");   
                if (name.ToLower() == parentName.ToLower())
                {
                    if (Test(file, '>')) return null;
                    else throw new Exception("XmlReader: End Tag should end with a >.");
                }
                else throw new Exception("XmlReader: Misplaced end tag - </" + name + ">");
            }
            else if (!Test(file, '<')) 
                throw new Exception("XmlReader: Neither '<' nor '</' found when looking for tag start.");

            XmlFileNode node = new XmlFileNode();

            node.Name = GetElementName(file);
            node.Attributes = GetAttributes(file);

            if (Test(file, "/>")) { }
            else if (Test(file, '>'))
            {
                // Node has not been closed so we need to get subnodes
                GetNodes(file, node);
            }

            return node;
        }

        private static string GetElementName(XmlFile file)
        {
            int pos = file.Pos;
            int n = 0;
            char c;
            char[] name = new char[100]; 

            while (pos < file.MaxPos)
            {
                c = file.File[pos+n];
                if ( (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '.' )
                {
                    name[n] = c;
                    ++n;
                }
                else break;   
            }

            file.Pos = file.Pos + n;
            EatWhitespace(file);

            return new string(name, 0, n);
        }

        private static List<XmlFileAttribute> GetAttributes(XmlFile file)
        {
            List<XmlFileAttribute> attributes = new List<XmlFileAttribute>();

            while (file.Pos < file.MaxPos)
            {
                XmlFileAttribute a = GetAttribute(file);
                if (a == null) break;
                attributes.Add(a);
            }

            return attributes;
        }

        private static XmlFileAttribute GetAttribute(XmlFile file)
        {
            string name = GetName(file);
            if (name == null) return null;

            if (!Test(file,'=')) throw new Exception("XmlFileRead: No '=' between attribute name and value");
            if (!Test(file,'"')) throw new Exception("XmlFileRead: Attribute value does not begin with a double quote");
            string value = GetString(file);
            if (file.Pos >= file.MaxPos) 
                throw new Exception("XmlFileRead: Attribute value did not end with a double quote");

            EatWhitespace(file);
            
            return new XmlFileAttribute(name, value);             
        }

        private static string GetName(XmlFile file)
        {
            int pos = file.Pos;
            int n = 0;
            char c;

            while (pos+n < file.MaxPos)
            {
                c = file.File[pos+n];
                if ( (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') )
                    ++n;
                else
                    break;                    
            }

            if (n==0) return null;

            string s = new string(file.File, pos, n);
            file.Pos = pos + n;

            return s;
        }      

        private static string GetString(XmlFile file)
        {
            int pos = file.Pos;
            int n = 0;

            while (pos+n < file.MaxPos)
            {
                if (file.File[pos+n] == '"') break;
                ++n;
            }

            string s = n>0 ? new string(file.File, pos, n) : "";
            file.Pos = pos + n + 1;

            return s;
        }      

        private static void EatDeclaration(XmlFile file) 
        {
            if (Test(file, "<?")) Skip(file, "?>");
            EatWhitespace(file);
        }

        private static void EatComments(XmlFile file) 
        {
            EatWhitespace(file);
            while (Test(file, "<!--")) 
            {
                Skip(file, "-->");
                EatWhitespace(file);
            }
        }

        private static void EatWhitespace(XmlFile file)
        {
            int pos = file.Pos;

            while (pos < file.MaxPos)
            {
                switch (file.File[pos])
                {
                     case (char)0x9:
                     case (char)0x20:
                        pos++;
                        continue;
                     default:
                        file.Pos = pos;
                        return;
                }
            }

            file.Pos = file.MaxPos+1;
        }

        private static bool Test(XmlFile file, char test)
        {
            if (file.File[file.Pos] == test) 
            {
                file.Pos++;
                return true;
            }
            else return false;
        }

        private static bool Test(XmlFile file, string test)
        {
            char[] l = test.ToCharArray();
            int n = test.Length - 1;
            int i=0;
            int pos = file.Pos;

            while (pos+i < file.MaxPos)
            {
                if (file.File[pos+i] != l[i]) return false;
                else if (i < n) ++i;
                else break;
            }
            file.Pos = pos+n+1;

            return true;
        }

        private static void Skip(XmlFile file, string search)
        {
            char[] l = search.ToCharArray();
            int n = search.Length - 1;
            int i=0;
            int pos = file.Pos;

            while (pos < file.MaxPos)
            {
                if (file.File[pos] != l[i]) i=0;
                else if (i < n)             ++i;
                else
                {
                    ++pos;
                    break;
                }
                ++pos;
            }
            file.Pos = pos;
        }
    }

    /// XML file as a char array, pointer and size. 
    /// Transient intermediate entity. Only used while XML is parsed. 
    public class XmlFile
    {
        /// The XML file loaded into memory by XMLReader.Load()
        public char[] File { get; set; }

        /// Current pointer to next char in File
        public int    Pos  { get; set; }
        
        /// Maximum value of Pos (i.e. size of File) 
        public int    MaxPos 
        { 
            get
            {
                if (maxPos == 0) maxPos = File.Length;
                return maxPos;
            } 
        }
        private int maxPos = 0;

        /// Returns the remainder of File (from Pos onwards) as a string.
        public override string ToString()
        {
            if (Pos > MaxPos) throw new Exception("XmlFile: Error in XmlFile. Pos outside file.");
            return new string(File, Pos, MaxPos-Pos);
        }
    }

    /// An XMLFileNode, which consists of: Name, list of attributes and list of subnodes    
    public class XmlFileNode
    {
        /// Construct new XmlFileNode 
        public XmlFileNode()
        {
            Attributes = new List<XmlFileAttribute>();
            Nodes      = new List<XmlFileNode>(); 
        }

        /// The name of the node (the word immediately after the opening chevron)
        public string                 Name       { get; set; }
        
        /// List of attributes in the node opening tag
        public List<XmlFileAttribute> Attributes { get; set; }

        /// List of subnodes belonging to this node
        public List<XmlFileNode>      Nodes      { get; set; }
    }

    /// An XMLFileAttribute, i.e. a name/value pair  
    public class XmlFileAttribute
    {
        /// Construct new XmlAttribute, with name and value
        /// <param name="name">The attribute name.</param>
        /// <param name="value">The attribute value.</param>
        public XmlFileAttribute(string name, string value)
        {
            Name  = name;
            Value = value;
        }

        /// XML Attribute Name
        public string Name  { get; set; }

        /// XML Attribute Value
        public string Value { get; set; }

        /// Returns name=value 
        public override string ToString()
        {
            return Name + " = " + Value; 
        }
    }
}