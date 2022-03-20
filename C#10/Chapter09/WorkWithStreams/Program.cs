using System.Xml;
using static System.Console;
using static System.Environment;
using static System.IO.Path;

// WorkWithText();
WorkWithXml();

static void WorkWithText()
{
   // define a file to write to
   string textFile = Combine(CurrentDirectory, "streams.txt");

   // create a text file and return a helper writer
   StreamWriter text = File.CreateText(textFile);
   
   //enumerate the strings, writing each one to the stream as a separate line
   foreach (string item in Viper.CallSigns)
   {
      text.WriteLine(item);
   }
   text.Close(); // release resources 

   // output the contents of the file
   WriteLine("{0} contains {1:N0} bytes.", textFile, new FileInfo(textFile).Length);
   WriteLine(File.ReadAllText(textFile));

}

   static void WorkWithXml()
   {
      FileStream? xmlFileStream = null;
      XmlWriter? xml = null;
      try{      
         // define a file to write to
         string xmlFile = Combine(CurrentDirectory, "streams.xml");

         // create a file stream
         xmlFileStream = File.Create(xmlFile);

         // wrap the file stream in an XML writer helper and automatically indent nested elements
         xml = XmlWriter.Create(xmlFileStream, new XmlWriterSettings {Indent = true});

         // Write the XML declaration
         xml.WriteStartDocument();

         // Write a root element
         xml.WriteStartElement("callsigns");

         // enumarate the strings writing each one to the stream
         foreach( string item in Viper.CallSigns)
         {
            xml.WriteElementString("callsign", item);
         }

         // Write the close root element
         xml.WriteEndElement();

         // close helper and stream
         xml.Close();
         xmlFileStream.Close();

         // Output all elements of the file
         WriteLine("{0} contains {1:N0} bytes", xmlFile, new FileInfo(xmlFile).Length);
         WriteLine(File.ReadAllText(xmlFile));
      }
      catch (Exception ex)
      {
         // if the path doesn't exist the exception will be caught
         WriteLine($"{ex.GetType()} says {ex.Message}");
      }
      finally
      {
         if (xml != null)
         {
            xml.Dispose();
            WriteLine("The XML writer's unmanaged resources have been disposed.");
         }
         if (xmlFileStream != null)
         {
            xmlFileStream.Dispose();
            WriteLine("The file stream's unmanaged resources have been disposed.");
         }
      }
   }
static class Viper
{
   // define an array of Viper pilot call signs
   public static string[] CallSigns = new[]
   {
      "Husker", "Starbuck", "Apollo", "Boomer", "Bulldog", "Athena", "Helo", "Racetrack"
   };
}