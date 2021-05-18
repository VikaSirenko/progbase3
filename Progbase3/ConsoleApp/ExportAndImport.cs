using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System;

namespace ConsoleApp
{
    public static class ExportAndImport
    {
        public static void DoExport(List<Post> posts, string outputFile)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Post>));
            StreamWriter output = new StreamWriter(outputFile);
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineHandling = NewLineHandling.Entitize;
            XmlWriter writer = XmlWriter.Create(output, settings);
            serializer.Serialize(output, posts);
            output.Close();

        }

        public static List<Post> DoImport(string inputFile)
        {
            try
            {
                if (File.Exists(inputFile))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<Post>));
                    StreamReader reader = new StreamReader(inputFile);
                    List<Post> posts = (List<Post>)serializer.Deserialize(reader);
                    reader.Close();
                    return posts;


                    //TODO add to BD

                    
                }
                throw new FileNotFoundException($"Input file [{inputFile}] not found.");
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Unable to read data from file [{inputFile}]" + ex.Message);
            }

        }



    }
}