/*
	MIT License

	Copyright (c) 2020 tehLuaX

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in all
	copies or substantial portions of the Software.

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
using ArchiveTimeUtility.Common;
using System.Xml;
using ArchiveTimeUtility.Common;

namespace ArchiveTimeUtility.Jobs
{
    class Store
    {
        // The string is the path of the file or directory and it should start with "F*" if it's a file or "D*" if it's a directory.
        Dictionary<string, ItemTimestampData> timestamps;
        public Store(string rootDir)
        {
            Console.WriteLine("Store job started.");
            Console.WriteLine($"Initiating logic on root directory ({rootDir})");
            timestamps = new Dictionary<string, ItemTimestampData>();
            StoreTimestamps(rootDir);
        }
        private void StoreTimestamps(string dir, int layer = 0)
        {
            foreach (string childFile in Directory.GetFiles(dir))
            {
                DateTime creationTime = File.GetCreationTimeUtc(childFile),
                    modifiedTime = File.GetLastWriteTimeUtc(childFile),
                    accessTime = File.GetLastAccessTimeUtc(childFile);
                timestamps.Add("F*" + childFile, new ItemTimestampData(creationTime, modifiedTime, accessTime));
                if (layer != 0) for (int x = 0; x < layer; x++) { Console.Write(" "); }
                Console.Write($"Found file: {childFile}\n");
                if (layer != 0) for (int x = 0; x < layer; x++) { Console.Write(" "); }
                Console.Write($" └───With creation timestamp: {creationTime}\n");
                if (layer != 0) for (int x = 0; x < layer; x++) { Console.Write(" "); }
                Console.Write($" └───With last modified timestamp: {modifiedTime}\n");
                if (layer != 0) for (int x = 0; x < layer; x++) { Console.Write(" "); }
                Console.Write($" └───With last access timestamp: {accessTime}\n");
            }
            foreach (string childDir in Directory.GetDirectories(dir))
            {
                DateTime creationTime = Directory.GetCreationTimeUtc(childDir),
                    modifiedTime = Directory.GetLastWriteTimeUtc(childDir),
                    accessTime = Directory.GetLastAccessTimeUtc(childDir);
                timestamps.Add("D*" + childDir, new ItemTimestampData(creationTime, modifiedTime, accessTime));
                if (layer != 0) for (int x = 0; x < layer; x++) { Console.Write(" "); }
                Console.Write($"Launching inner search for directory {childDir} in {dir}\n");
                if (layer != 0) for (int x = 0; x < layer; x++) { Console.Write(" "); }
                Console.Write($" └───With creation timestamp: {creationTime}\n");
                if (layer != 0) for (int x = 0; x < layer; x++) { Console.Write(" "); }
                Console.Write($" └───With last modified timestamp: {modifiedTime}\n");
                if (layer != 0) for (int x = 0; x < layer; x++) { Console.Write(" "); }
                Console.Write($" └───With last access timestamp: {accessTime}\n");
                StoreTimestamps(childDir, layer + 1);
            }
            if (layer == 0)
            {
                Console.WriteLine($"Done reading {timestamps.Count} items.");
                DateTime now = DateTime.Now;
                string filename = $"archiveTimestamps-{now.Year}{now.Month}{now.Day}{now.Second}{now.Minute}{now.Hour}.atf";
                Console.WriteLine($"Saving data as {filename}..."); 
                /*var stream = File.CreateText(filename);
                stream.WriteLine("<atufContents version=\"v1.0\"></atufContents>");
                stream.Close();*/
                XmlDocument d = new XmlDocument();
                //d.LoadXml(filename);
                XmlElement root = d.CreateElement("atufContents");
                root.SetAttribute("version", "v1.0");
                d.InnerXml = root.OuterXml;
                d.Save(filename);
                XmlElement items = d.CreateElement("items");
                foreach (var item in timestamps)
                {
                    XmlNode itemElement = d.CreateNode(XmlNodeType.Element,
                        item.Key.StartsWith("F*") ? "file" : "directory", null);

                    XmlAttribute fnAttrib = d.CreateAttribute("path");
                    fnAttrib.Value = item.Key.Substring(2);

                    itemElement.Attributes.Append(fnAttrib);
                    
                    XmlNode itemTimestamps = d.CreateNode(XmlNodeType.Element,"timestamps", null);
                    
                    XmlNode itemCreationTimestamp = d.CreateNode(XmlNodeType.Element,"creationTime", null);
                    itemCreationTimestamp.InnerText = Utils.ToUnixEpoch(item.Value.CreationTime).ToString();
                    XmlNode itemModifiedTimestamp = d.CreateNode(XmlNodeType.Element, "modifiedTime", null);
                    itemModifiedTimestamp.InnerText = Utils.ToUnixEpoch(item.Value.ModifiedTime).ToString();
                    XmlNode itemAccessTimestamp = d.CreateNode(XmlNodeType.Element, "accessTime", null);
                    itemAccessTimestamp.InnerText = Utils.ToUnixEpoch(item.Value.AccessTime).ToString();
                    
                    itemTimestamps.AppendChild(itemCreationTimestamp);
                    itemTimestamps.AppendChild(itemModifiedTimestamp);
                    itemTimestamps.AppendChild(itemAccessTimestamp);
                    
                    itemElement.AppendChild(itemTimestamps);
                    items.AppendChild(itemElement);
                }
                //root.AppendChild(items);
                d.DocumentElement.AppendChild(items);
                d.Save(filename);
                Console.WriteLine("Job done.");
            }
        }
    }
}
