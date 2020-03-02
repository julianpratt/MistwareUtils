using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

using Mistware.Utils;

namespace MistwareUtilsTest
{
	class Program
	{
		static void Main(string[] args)
		{
			string sFail;
			bool bFail;
			string sTest, sTest2, s;
			DateTime dBadDate = new DateTime(1,1,1);
			DateTime dTest;

			sFail="";
			sTest="ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			sTest2="ABC\nDEFGHI\tJKLMNOPQRSTUVWXYZ";

            // Cache Tests
			Cache<string>.GetCache().Set("one", () => "First Value" );
            Cache<string>.GetCache().Set("two", () => "Second Value");
            List<string> l = Cache<string>.GetCache().ListKeys();
            foreach (string s1 in l) if (s1 != "one" && s1 != "two" ) sFail += "CacheTest1 ";
			if (Cache<string>.GetCache().Get("two", () => "Second Value") != "Second Value") sFail += "CacheTest2 ";
			
            // Config Tests
			try
			{
				Config.Setup("test.xml",".", "web", "app");
			}
			catch (Exception ex)
			{
				Console.WriteLine("Config.Setup failed with: " + ex.Message);
				sFail += "ConfigTest1 ";
			}
			if (Config.ContentRoot    != ".")        sFail += "ConfigTest2 "; 
			if (Config.WebRoot        != "web")      sFail += "ConfigTest3 "; 
			if (Config.AppName        != "app")      sFail += "ConfigTest4 "; 
			if (Config.AppURL.Left(8) != "http://r") sFail += "ConfigTest5 ";
			if (Config.Env.Left(4)    != "Prod")     sFail += "ConfigTest6 ";
			if (Config.Debug)                        sFail += "ConfigTest7 ";
			string delim = System.IO.Path.DirectorySeparatorChar.ToString();
			if (Config.LogFile != "."+delim+"Logs"+delim+"app.log") sFail += "ConfigTest8 ";


			// DateTime tests

			// Time Tests

			// Test Midnight, Noon and SetTime
			if (DateTime.Now.Midnight().Hour      != 0)  sFail+="Midnight ";
			if (DateTime.Now.Noon().Hour          != 12) sFail+="Noon ";
			if (DateTime.Now.SetTime(5,59).Minute != 59) sFail+="SetTime1 ";
			if (DateTime.Now.SetTime(5,59).Second != 0)  sFail+="SetTime2 ";

            // Date Tests

            // Test DOW
            if ("24/12/2009".ToDateTime().DOW() != 4) sFail+="DOW1 ";
            if ("27/12/2009".ToDateTime().DOW() != 7) sFail+="DOW2 ";
            if ("29/02/2008".ToDateTime().DOW() != 5) sFail+="DOW3 ";
            if ("01/01/2010".ToDateTime().DOW() != 5) sFail+="DOW4 ";

			// Test Jan1WeekDay
			// Answers as per: http://en.wikipedia.org/wiki/File:Permanent_calendar.png
            if ("1/1/1600".ToDateTime().Jan1WeekDay() != 6 )  sFail+="Jan1WeekDay1 ";
            if ("1/1/1601".ToDateTime().Jan1WeekDay() != 1 )  sFail+="Jan1WeekDay2 ";
            if ("1/1/1602".ToDateTime().Jan1WeekDay() != 2 )  sFail+="Jan1WeekDay3 ";
            if ("1/1/1603".ToDateTime().Jan1WeekDay() != 3 )  sFail+="Jan1WeekDay4 ";
            if ("1/1/1604".ToDateTime().Jan1WeekDay() != 4 )  sFail+="Jan1WeekDay5 ";
            if ("1/1/1605".ToDateTime().Jan1WeekDay() != 6 )  sFail+="Jan1WeekDay6 ";
            if ("1/1/1606".ToDateTime().Jan1WeekDay() != 7 )  sFail+="Jan1WeekDay7 ";
            if ("1/1/1607".ToDateTime().Jan1WeekDay() != 1 )  sFail+="Jan1WeekDay8 ";
            if ("1/1/1608".ToDateTime().Jan1WeekDay() != 2 )  sFail+="Jan1WeekDay9 ";
            if ("1/1/1609".ToDateTime().Jan1WeekDay() != 4 )  sFail+="Jan1WeekDay10 ";
            if ("1/1/1610".ToDateTime().Jan1WeekDay() != 5 )  sFail+="Jan1WeekDay11 ";
            if ("1/1/1611".ToDateTime().Jan1WeekDay() != 6 )  sFail+="Jan1WeekDay12 ";
            if ("1/1/1612".ToDateTime().Jan1WeekDay() != 7 )  sFail+="Jan1WeekDay13 ";
            if ("1/1/1613".ToDateTime().Jan1WeekDay() != 2 )  sFail+="Jan1WeekDay14 ";
            if ("1/1/1614".ToDateTime().Jan1WeekDay() != 3 )  sFail+="Jan1WeekDay15 ";
            if ("1/1/1615".ToDateTime().Jan1WeekDay() != 4 )  sFail+="Jan1WeekDay16 ";
            if ("1/1/1616".ToDateTime().Jan1WeekDay() != 5 )  sFail+="Jan1WeekDay17 ";
            if ("1/1/1617".ToDateTime().Jan1WeekDay() != 7 )  sFail+="Jan1WeekDay18 ";
            if ("1/1/1618".ToDateTime().Jan1WeekDay() != 1 )  sFail+="Jan1WeekDay19 ";

            // Test DOY
            dTest = "1/1/2009".ToDateTime();
            bFail = false;
            for (int i=0; i<365; i++)
            {
              if ( dTest.DOY() != i+1 ) bFail=true;
              dTest = dTest.AddDays(1);
            }
            if (bFail) sFail+="DOY1 ";

            dTest = "1/1/2008".ToDateTime();
            bFail = false;
            for (int i=0; i<366; i++)
            {
              if ( dTest.DOY() != i+1 ) bFail=true;
              dTest = dTest.AddDays(1);
            }
            if (bFail) sFail+="DOY2 ";
            
            // Test IsLeapYear
            if ("1/1/2009".ToDateTime().IsLeapYear() == true  )  sFail+="IsLeapYear1 ";
            if ("1/1/1600".ToDateTime().IsLeapYear() == false )  sFail+="IsLeapYear2 ";
            if ("1/1/1700".ToDateTime().IsLeapYear() == true  )  sFail+="IsLeapYear4 ";
            if ("1/1/1800".ToDateTime().IsLeapYear() == true  )  sFail+="IsLeapYear5 ";
            if ("1/1/1900".ToDateTime().IsLeapYear() == true  )  sFail+="IsLeapYear6 ";
            if ("1/1/2000".ToDateTime().IsLeapYear() == false )  sFail+="IsLeapYear3 ";
            if ("1/1/2100".ToDateTime().IsLeapYear() == true  )  sFail+="IsLeapYear7 ";
            if ("1/1/2200".ToDateTime().IsLeapYear() == true  )  sFail+="IsLeapYear8 ";
            if ("1/1/2300".ToDateTime().IsLeapYear() == true  )  sFail+="IsLeapYear9 ";

            // Test ISOWeekNum
			if ("1/3/2020".ToDateTime().ISOWeekNum() != 9  )     						sFail+="ISOWeekNum1 ";
			if ("3/1/2010".ToDateTime().ISOWeekNum() != 53  )     						sFail+="ISOWeekNum2 ";

            // Test ISOWeekOne
			if ("1/3/2009".ToDateTime().ISOWeekOne() != "29/12/2008".ToDateTime())      sFail+="ISOWeekOne ";

            // Test ISOYear
			if ("3/1/2010".ToDateTime().ISOYear() != 2009)          	                sFail+="ISOWeekOne ";

            // Test ToDateStamp
			if ("25/12/2019".ToDateTime().Noon().ToDateStamp()    != "201912251200"   ) sFail+="ToDateStamp ";

			// Test ToLogStamp
			if ("25/12/2019".ToDateTime().Midnight().ToLogStamp() != "25/12 00:00:00" ) sFail+="ToLogStamp ";

            // Test ToDateString
			if ("25/12/2019".ToDateTime().ToDateString()          != "25/12/2019"     ) sFail+="ToDateString ";

            // Test ToISODateString
			if ("1/3/2020".ToDateTime().ToISODateString()         != "2020-9-7"       ) sFail+="ToISODateString ";
    
			// Test Encryption
			Encryption.Key = string.Format("TheLORDismyShepherdIshallnot{0:dd}inwantHemakes=", DateTime.Now);
			if (Encryption.Decrypt(Encryption.Encrypt(sTest)) != sTest) sFail+="Encryption ";
			
			// Test Log and FileRead
			Log.Me.LogFile="Test.log";
			Log.Me.Info("Hello, World");
			using (FileRead f = new FileRead("."+delim, "Test.log")) s = f.ReadLine();
			if (s.Left(9)   != DateTime.Now.ToLogStamp().Left(9)) sFail+="Log&FileRead1 ";
			if (s.Right(21) != "] INFO : Hello, World")           sFail+="Log&FileRead2 ";
			File.Delete("Test.Log");

			// Test MIME
			if (MIME.GetMimeType("mp3") != "audio/mpeg") sFail+="MIME ";

            // String Tests

			// Test Truncate
 			if (sTest.Truncate(12) != "ABCDEFGHIJKL"  ) sFail+="Truncate ";
           
            // Test Left
            if (sTest.Left(10)   != "ABCDEFGHIJ"    ) sFail+="Left1 ";
            if (sTest.Left(50)   != sTest           ) sFail+="Left2 ";

            // Test Mid
            if (sTest.Mid(25,1)  != "Z"            ) sFail+="Mid1 ";
            if (sTest.Mid(26,1)  != ""             ) sFail+="Mid2 ";
            if (sTest.Mid(27,50) != ""             ) sFail+="Mid3 ";
            if (sTest.Mid(10,3)  != "KLM"          ) sFail+="Mid4 ";
            if (sTest.Mid(16,20) != "QRSTUVWXYZ"   ) sFail+="Mid5 ";

            // Test Right
            if (sTest.Right(10) != "QRSTUVWXYZ"    ) sFail+="Right1 ";
            if (sTest.Right(50) != sTest           ) sFail+="Right2 ";

			// Test ThrowOnNullOrEmpty
			try
			{
				s = null;
				s.ThrowOnNullOrEmpty("s","message");
				sFail+="ThrowOnNullOrEmpty ";
			}
			catch {}

            //Test IsInteger
            if ("1.0".IsInteger()                  ) sFail+="IsInteger1 ";
            if ("?z£".IsInteger()                 ) sFail+="IsInteger2 ";
            if ("a".IsInteger()                    ) sFail+="IsInteger3 ";
            if (!("123".IsInteger())               ) sFail+="IsInteger4 ";

            //Test ToInteger
            if ("10".ToInteger() != 10             ) sFail+="ToInteger1 ";
            if ("-10".ToInteger() != -10           ) sFail+="ToInteger2 ";
            try
            {
              int i = "fred".ToInteger(); 
              sFail+="ToInteger3 ";
            }
            catch {}

			// Test ToDouble
			if ("5.4".ToDouble() != 5.4             ) sFail+="ToDouble ";

			// Test ToBool
			if ("false".ToBool()                    ) sFail+="ToBool ";

			// Test ToEnum<T>
			if ("THURSDAY".ToEnum<DayOfWeek>(true) != DayOfWeek.Thursday) sFail+="ToEnum ";

            // Test CleanString
            if (sTest2.CleanString() != sTest                  ) sFail+="CleanString ";

            // Test EscapeSingleQuote
            if ("abc\'xyz".EscapeSingleQuote() != "abc\'\'xyz" ) sFail+="EscapeSingleQuote ";

			// Test StripQuotes
			if ("\"abc\"".StripQuotes() != "abc"               ) sFail+="StripQuotes ";

            // Test RemoveNonNumeric
            if ("a1b2c3.4xyz".RemoveNonNumeric() != "1234"     ) sFail+="RemoveNonNumeric ";

            // Test ToDateTime
            if ("".ToDateTime()     != dBadDate    ) sFail+="ToDateTime1 ";
            if ("fred".ToDateTime() != dBadDate    ) sFail+="ToDateTime2 ";

			// Test AsciiCtlStrip
			if ("abcde\vfgh\rijk".AsciiCtlStrip() != "abcdefghijk") sFail+="AsciiCtlStrip ";

			// Test MailMerge and ToDict
			if ("Hi Mr {Fred}".MailMerge("Fred=Bloggs".ToDict(),"{}") != "Hi Mr Bloggs") sFail+="MailMerge&ToDict ";

			// Test NormalPath
			if ("abc\\def".NormalPath() != "abc/def") sFail+="NormalPath ";

			// Test Padding
			if ("abcdef".Padding(4,'=') != "abcdef==") sFail+="Padding ";

			// Test Wordise
            s="one two   three  \"Fred Bloggs\"   ";
			if (s.Wordise().ListToString(":") != "one:two:three:\"Fred Bloggs\"") sFail+="Wordise ";

			// Test Match
            if (!("abcde".Match("abcd"))) sFail+="Match ";

			// Test ToStream
            StreamReader reader = new StreamReader(sTest.ToStream());
			if (reader.ReadToEnd() != sTest) sFail+="ToStream ";
            reader.Dispose();

			// Test ListTrim
            s = "a: b :c :d";
			if (s.StringToList(':').ListTrim().ListToString(":") != "a:b:c:d") sFail+="ListTrim ";

			// Test ListToString and StringToList 
			s = "a:b:c:d";
			if (s.StringToList(':').ListToString(":") != s) sFail+="ListToString&StringToList ";

			// Test ListToUpper
			if (s.StringToList(':').ListToUpper().ListToString(";") != "A;B;C;D") sFail+="ListToUpper ";

			// Test ToInlist
			if (s.StringToList(':').ToInList() != "\'a\',\'b\',\'c\',\'d\'") sFail+="ToInList ";

			// Test Validate
			if (!"b".Validate(s.StringToList(':'))) sFail+="Validate ";

            // Test MultiPart
            if (s.MultiPart(':').ToList<string>().ListToString(":") != s) sFail+="Multipart ";
           
            
			// Test XML

            // Test LoadList and ToDictionary
            Dictionary<string,string> dict = XML.LoadList("Types.xml", "Types/SearchTypes", "value", "name").ToDictionary();
            if (dict.DictToString() != "R=Report,M=Memo,WP=Welding Procedure") sFail+="XML LoadList&ToDictionary ";
        
            // Test XMLTransform and AsString
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load("Types.xml");
            string xml = xdoc.AsString();
            xml = XML.XmlNodePoke(xml, "Types/SearchTypes[1]", "text", "Rubbish");
            xml = XML.XMLTransform(xml , "Types.xslt");
       
            // Test ReadXMLNode
            if (XML.ReadXmlNode(xml,"root/Types/Type[@Key='WP']") != "Welding Procedure (WP)") sFail+="ReadXMLNode "; 

            // Test ReadXMLAttribute
            if (XML.ReadXmlAttribute(xml,"root/Types/Type[@Key='M']/@Name") != "Memo") sFail+="ReadXMLAttribute ";

            // Test XMLNodePoke
            if (XML.ReadXmlNode(xml,"root/Types/Type[@Key='R']") != "Rubbish") sFail+="XMLNodePoke ";
        
            if (sFail.Length == 0) 
            {
                  Console.WriteLine("**********************");
                  Console.WriteLine("** All Tests passed **");
                  Console.WriteLine("**********************");      
            }            
            else 
            {
                  Console.WriteLine("!!!!!!!!!!!!!!!!!!!");                  
                  Console.WriteLine("!! Failed tests: " + sFail);                  
                  Console.WriteLine("!!!!!!!!!!!!!!!!!!!");                                    
            }


        }
    }
}