Mistware.Utils - miscellaneous C# utilities
========================================

Mistware is an identity chosen to evoke the concept of [Vapourware](https://en.wikipedia.org/wiki/Vaporware).

These are utilities to simplify the task of creating web applications. They may be idiosyncratic and are almost certainly based on ideas from a previous era, but I find them useful and put them here to use them more easily. 

Please don't blame me if you waste time trying to find something useful here and merely end up asking "why!!". You have been warned, the clue is in the name, perhaps there is nothing particularly useful here. 

**To minimise dependencies, Cache, Encryption and Xml classes were removed.** 


Features
--------

- Config. Static class to handle application settings. Consolidated configuration from environment (variables) and config file (which defaults to "web.config"). The Setup method reads in environment variables and config file settings. Thereafter settings can be accessed via Get and Set methods. Or using a number of built in properties: ContentRoot, WebRoot, AppName, AppURL, Env, Debug, LogFile.
- DateTime. Date and Time Extensions. Midnight (time in DateTime is zero), Noon and SetTime. DOW, Jan1WeekDay, DOY, First, Last, Next, IsLeapYear, ISOWeekNum, ISOWeekOne, ISOYear, ToDateStamp, ToLogStamp, ToDateString, ToISODateString.
- FileRead. StreamReader wrapper to read text file from filesystem or the internet. Can read the file a line at a time or the whole file at once.
- Log. A singleton that logs fatal, error, warning, information and debug messages from applications. Has a default rolling file and console logger, which can be overridden by any other logger that implements the ILog interface.
- MIME. Class with single static method (GetMimeType) that returns the MIME type associated with an extension (e.g. “application/pdf” for “.pdf”). If the extension is not found, “application/octet-stream” is returned.  
- String. String extensions: Truncate, Left, Mid, Right, ThrowOnNullOrEmpty, HasValue, IsNull, IsInteger, ToInteger, ToDouble, ToBool, ToEnum, CleanString, EscapeSingleQuote, StripQuotes, RemoveNonNumeric, ToDateTime, AsciiCtlStrip, MailMerge, NormalPath, Padding, MultiPart, ToDict, DictToString, Wordise, Match, ToStream, ToInList, ListTrim, ListToUpper, Validate, StringToList, ListToString.
- XmlFileRead. A simple XML parser used by the Config class. It is not recommended that this be used for any other purpose, so no documentation is provided. 


Documentation
--------

Each class has Intellisense documentation. There is also a MistwareUtils.doc file, with additional usage guidance.


Usage
--------

To add the nuget package to a .Net Core application:

```
dotnet add package Mistware.Utils 
```



Testing
---------------------
Mistware.Utils has a basic test suite in the test folder (MistwareUtilsTest.csproj)


