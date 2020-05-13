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
using ArchiveTimeUtility.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace ArchiveTimeUtility.Jobs
{
    class Restore
    {
		// Just like in the Store class, the string should start with F* for files and D*
		// for directories.
		//
		// If, for some reason, there is an element in the provided file without the file
		// attribute, then the string should start with V* and then the index of the element should
		// be placed after the prefix.
		private Dictionary<string, InnerLogType> warningLogs;
		int modifiedFiles = 0,
			modifiedDirs = 0;
		public Restore(string rootDir, string xmlPath)
        {
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("WARNING! By running the restore job, you acknowledge that all items listed in the provided input file may have their creation, modification and/or access timestamps overwritten. Press any key to continue.");
			Console.ReadKey();
			Console.ResetColor();
			Console.WriteLine("Restore job started.");
			warningLogs = new Dictionary<string, InnerLogType>();
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("Loading input file...");
			XmlDocument d = new XmlDocument();
			try
			{
				d.Load(xmlPath);
			} catch (Exception)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("An unknown error occurred while attempting to load the provided file.");
				Console.WriteLine("Restore job failed.");
				Console.ResetColor();
				return;
			}
			for (int x = 0; x < d.DocumentElement.ChildNodes.Count; x++)
			{
				var rootElement = d.DocumentElement.ChildNodes[x];
				if (rootElement.Name.Equals("items"))
				{
					for (int y = 0; y < rootElement.ChildNodes.Count; y++)
					{
						var element = rootElement.ChildNodes[y];
						if (element.Name != "file" && element.Name != "directory")
						{
							warningLogs.Add("V*" + y, InnerLogType.UNKNOWN_ITEM_TYPE);
							continue;
						}
						// We're currently dealing with a file.
						// Get the path attribute.
						string path = null;
						bool isFile = element.Name.Equals("file");
						long ct = -1, mt = -1, at = -1;
						foreach (XmlAttribute attrib in element.Attributes)
						{
							if (attrib.Name.Equals("path"))
							{
								path = attrib.InnerText;
							}
						}
						if (path == null)
						{
							// The path attribute doesn't exist for some reason...
							warningLogs.Add("V*" + x, InnerLogType.MISSING_PATH);
							// Skip this element.
							continue;
						}
						foreach (XmlElement child in element.ChildNodes)
						{
							if (child.Name.Equals("timestamps"))
							{
								foreach (XmlElement tsChild in child.ChildNodes)
								{
									switch (tsChild.Name)
									{
										case "creationTime":
											ct = long.Parse(tsChild.InnerText);
											break;
										case "modifiedTime":
											mt = long.Parse(tsChild.InnerText);
											break;
										case "accessTime":
											at = long.Parse(tsChild.InnerText);
											break;
									}
								}
							}
						}
						// Check if the provided path exists.
						if (isFile)
						{
							if (!File.Exists(path))
							{
								warningLogs.Add("F*" + path, InnerLogType.MISSING_LOCAL_FILE);
								continue;
							}
						} else
						{
							if (!Directory.Exists(path))
							{
								warningLogs.Add("F*" + path, InnerLogType.MISSING_LOCAL_DIR);
								continue;
							}
						}
						// Update the timestamps.
						ushort layer = 0;
						foreach(char ch in path.ToCharArray())
						{
							if (ch == '\\')
								layer++;
						}
						
						WorkOnItem(path, ct, mt, at, element.Name.Equals("file"), layer);
					}
				}
			}
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("The restore job has been completed.");
			Console.WriteLine($"{modifiedFiles} files have been modified.");
			Console.WriteLine($"{modifiedDirs} directories have been modified.");
			Console.ForegroundColor = ConsoleColor.Yellow;
			foreach (var log in warningLogs)
			{
				string item = log.Key.Substring(2),
					rawItem = log.Key;
				switch (log.Value)
				{
					case InnerLogType.MISSING_LOCAL_DIR:
						Console.WriteLine($"Directory {item} could not be found on the local machine.");
						break;
					case InnerLogType.MISSING_LOCAL_FILE:
						Console.WriteLine($"File {item} could not be found on the local machine.");
						break;
					case InnerLogType.UNKNOWN_ITEM_TYPE:
						Console.WriteLine($"Item {item} is of an unknown type.");
						break;
					default:
						Console.WriteLine($"An unknown error occurred while attempting to work on item {item}.");
						break;
				}
			}
			Console.ResetColor();
		}
		private void WorkOnItem(string path, long ct = -1, long mt = -1, long at = -1, bool isFile = true, ushort layer = 0)
		{
			if (isFile) modifiedFiles++;
			else modifiedDirs++;
			if (layer != 0) for (int x = 0; x < layer; x++) { Console.Write(" "); }
			Console.ResetColor();
			if (!isFile)
				Console.ForegroundColor = ConsoleColor.Yellow;
			Console.Write(isFile ? $"Working on file ": "Working on directory ");
			Console.ForegroundColor = ConsoleColor.Magenta;
			Console.Write($"{path}\n");
			Console.ResetColor();
			if (ct != -1)
			{
				DateTime ctdt = Utils.ToDateTime(ct);
				if (isFile) File.SetCreationTimeUtc(path, ctdt);
				else Directory.SetCreationTimeUtc(path, ctdt);
				if (layer != 0) for (int x = 0; x < layer; x++) { Console.Write(" "); }
				Console.Write(" └───Defined creation time: ");
				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.Write($"{ ctdt}\n");
				Console.ResetColor();
			}
			if (mt != -1)
			{
				DateTime mtdt = Utils.ToDateTime(mt);
				if (isFile) File.SetLastWriteTimeUtc(path, Utils.ToDateTime(mt));
				else Directory.SetCreationTimeUtc(path, mtdt);
				if (layer != 0) for (int x = 0; x < layer; x++) { Console.Write(" "); }
				Console.Write(" └───Defined modified time: ");
				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.Write($"{ mtdt}\n");
				Console.ResetColor();
			}	
			if (at != -1)
			{
				DateTime atdt = Utils.ToDateTime(at);
				if (isFile) File.SetLastAccessTimeUtc(path, Utils.ToDateTime(at)); if (layer != 0) for (int x = 0; x < layer; x++) { Console.Write(" "); }
				else Directory.SetCreationTimeUtc(path, atdt);
				Console.Write(" └───Defined access time: ");
				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.Write($"{ atdt}\n");
				Console.ResetColor();
			}
		}
	}
}
