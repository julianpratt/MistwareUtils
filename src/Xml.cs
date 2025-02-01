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
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Xml.Serialization;

namespace Mistware.Utils
{
    /// A collection of static utility functions for extracting data from XML files.
	public static class XML
	{
        /// Reads a list of key/value pairs from an XML file.
        /// <param name="sFile">The name of the XML file to read.</param>
        /// <param name="sRoot">The root node of the list.</param>
        /// <param name="sKeyNode">The name of the key node. Can be null in which case no key node is read.</param>
        /// <param name="sValueNode">The name of the value node. Can be null in which case no value node is read.</param>
        public static List<KeyValuePair<string,string>> LoadList(string sFile, string sRoot, string sKeyNode, string sValueNode)
        {
            //XmlReader rdr = XmlReader.Create(sFile);
            XPathDocument doc = new XPathDocument(sFile);
            //create the XPath navigator
            XPathNavigator nav = ((IXPathNavigable)doc).CreateNavigator();
            //create the XPathNodeIterator of nodes at Root
            XPathNodeIterator iter = nav.Select(sRoot);

            // Create a new hash table.
            List<KeyValuePair<string,string>> list = new List<KeyValuePair<string,string>>();

            string sKey   = "";
            string sValue = "";

            while (iter.MoveNext())
            {
                XPathNodeIterator newIter = iter.Current.SelectDescendants(XPathNodeType.Element, false);
                while (newIter.MoveNext())
                {
                    if (sKeyNode   != null && newIter.Current.Name == sKeyNode)   sKey   = newIter.Current.Value;
                    if (sValueNode != null && newIter.Current.Name == sValueNode) sValue = newIter.Current.Value;
                }
                list.Add(new KeyValuePair<string,string>(sKey, sValue));
                sKey   = "";
                sValue = "";
            }
            return list;
        }

        /// <summary>
        /// Convert a list of Key/Value pairs to a string Dictionary, n.b. this will fail if there are duplicate keys.
        /// </summary>
        /// <param name="list">Output from LoadList()</param>
        /// <returns>Dictionary of strings</returns>
        public static Dictionary<string,string> ToDictionary(this List<KeyValuePair<string,string>> list)
        {  
            try 
            {
                return list.ToDictionary(pair => pair.Key, pair => pair.Value);
            }
            catch (Exception ex)
            {
                string msg = "Failed to convert list of Key/Values to dictionary. Probably due to duplicate keys. Error was: ";
                throw new Exception(msg + ex.Message);
            }
            
        }

        /// <summary>
        /// In memory XML transform.
        /// </summary>
        /// <param name="xml">String with the input xml file</param>
        /// <param name="xsltFile">Name of the xslt transform file</param>
        /// <returns>String with the output xml file.</returns>
        public static string XMLTransform(string xml, string xsltFile)
        {

            string output = String.Empty;
            string state = "start";
            try
            {
                using (StringReader sri = new StringReader(xml))
                {
                    using (XmlReader xri = XmlReader.Create(sri))
                    {
                        XslCompiledTransform xslt = new XslCompiledTransform();
                        xslt.Load(xsltFile);
                        state = "xslt loaded";

                        XmlWriterSettings xws = xslt.OutputSettings.Clone();
                        xws.Encoding = Encoding.UTF8;
                        state = "encoding set to UTF-8";

                        using (MemoryStream ms = new MemoryStream())
                        using (XmlWriter xwo = XmlWriter.Create(ms, xws ))
                        {
                            state = "about to transform";
                            xslt.Transform(xri, xwo);
                            ms.Position = 0;
                            StreamReader reader = new StreamReader(ms);
                            output = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("In TransformXML(" + state + ") was: " + ex.Message);
            }

            return output;
        }
              
        /// <summary>
        /// Extract an XML fragment/node (aka InnerXml) from an XML file.
        /// </summary>
        /// <param name="xml">String with the input xml file</param>
        /// <param name="xpath">XPath specifying node</param>
        /// <returns>String with the output node (xml fragment) value</returns>
        public static string ReadXmlNode(string xml, string xpath)
        {
            XmlNode node = GetNode(ImportXml(xml), xpath);
            if (node == null) return ""; 

            return node.InnerXml;
        }

        /// <summary>
        /// Extract an attribute value from an XML file.
        /// </summary>
        /// <param name="xml">String with the input xml file</param>
        /// <param name="xpath">XPath specifying attribute</param>
        /// <returns>String with the attribute value</returns>
        public static string ReadXmlAttribute(string xml, string xpath)
        {
            XmlNode node = GetNode(ImportXml(xml), xpath);
            if (node == null) return ""; 
        
            return node.Value;
        }

        private static XmlDocument ImportXml(string xml) 
        {
            XmlDocument xmlDoc = new XmlDocument();

            try
            {
                xmlDoc.LoadXml(xml);    
            }
            catch (Exception ex)
            {
                throw new Exception("Could not ImportXml. Error was " + ex.Message);
            }
            return xmlDoc;
        }

        private static XmlNode GetNode(XmlDocument xmlDoc, string xpath)
        {
            if (xmlDoc == null || xpath == null) return null;

            XmlNode node = null;

            try
            {
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                //nsmgr.AddNamespace("d", "http://schemas.microsoft.com/ado/2007/08/dataservices");
        
                node = xmlDoc.SelectSingleNode(xpath, nsmgr);
            }
            catch (Exception ex)
            {
                throw new Exception("Error: Could not GetNode. Error was " + ex.Message);
            }
            return node;    
        }

        /// <summary>
        /// Replace the contents of node in an xml file.
        /// </summary>
        /// <param name="xml">String with the input xml file</param>
        /// <param name="xpath">XPath specifying the parent of the node to be replaced</param>
        /// <param name="nodename">the name of the node</param>
        /// <param name="value">the replacement value </param>
        public static string XmlNodePoke(string xml, string xpath, string nodename, string value) 
        {
            XmlDocument xmlDoc = ImportXml(xml);
            if (xmlDoc == null) return "";

            // xpath specifies the parent of the node to be replaced
            XmlNode node = GetNode(xmlDoc, xpath); 

            //Create new element node
            XmlNode elem = xmlDoc.CreateNode(XmlNodeType.Element, nodename, null);
            elem.InnerText = value;

            //Replace the node.
            node.ReplaceChild(elem, xmlDoc.SelectSingleNode(xpath + "/" + nodename));

            return xmlDoc.AsString();
        }
    }

    /// Extend XmlDocument class
    public static class XmlDocumentExtensions
    {
        /// <summary>
        /// Return XmlDocument as a string
        /// </summary>  
        public static string AsString(this XmlDocument xmlDoc)
        {
            string s = null;

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = new UnicodeEncoding(false, false); // no BOM in a .NET string
            settings.Indent = true;
            settings.OmitXmlDeclaration = false;

            using (StringWriter sw = new StringWriter())
            {
                using (XmlWriter xw = XmlWriter.Create(sw, settings))
                {
                    xmlDoc.WriteTo(xw);
                    xw.Flush();
                    s = sw.ToString();
                }
            }
            return s;
        }
    }   
}