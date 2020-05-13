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
using System.IO;

namespace ArchiveTimeUtility
{
	class Program
	{
		private static readonly string[] VALID_JOBS = { "store", "restore" };
		private static readonly string[] VALID_PARAMS = { "-d", "--dir" };
		static void Main(string[] args)
		{
			ShowLicense();
			// Syntax: atu [store|restore] <--dir directory> [atfPath]
			string rootDir = null,
				currentJob = null,
				atfPath = null;
			bool currentArgIsValue = false;
			if (args.Length != 0)
			{
				for (ushort x = 0; x < args.Length; x++)
				{
					if (!currentArgIsValue)
					{
						bool unknownParam = true;
						if (args[x].StartsWith('-'))
							foreach (string param in VALID_PARAMS)
							{
								if (param.Equals(args[x]))
								{
									unknownParam = false;
									break;
								}
							}
						else
							unknownParam = false;
						if (unknownParam)
						{
							Console.ForegroundColor = ConsoleColor.Red;
							Console.WriteLine("An unknown parameter has been provided.");
							Console.ResetColor();
							return;
						}
					}
					if (x == 0)
					{
						if (args[x].StartsWith('-'))
						{
							Console.ForegroundColor = ConsoleColor.Red;
							Console.WriteLine("The first argument cannot be a parameter.");
							Console.ResetColor();
							return;
						}
						else
						{
							bool isValidJob = false;
							foreach(string job in VALID_JOBS)
							{
								if (job == args[x])
								{
									isValidJob = true;
									currentJob = args[x];
									break;
								}
							}
							if (!isValidJob)
							{
								Console.ForegroundColor = ConsoleColor.Red;
								Console.WriteLine("An unknown job has been provided and the program could not understand it.");
								Console.ResetColor();
								return;
							}
						}
					}
					if (args[x].Equals("--dir") || args[x].Equals("-d"))
					{
						try
						{
							rootDir = args[x + 1];
						} catch (IndexOutOfRangeException)
						{
							Console.ForegroundColor = ConsoleColor.Red;
							Console.WriteLine("Although the directory argument was specified, its value could not be found.");
							Console.ResetColor();
							return;
						}
					}
					if (args[x].StartsWith('-'))
						currentArgIsValue = true;
					else if (currentArgIsValue)
						currentArgIsValue = false;
				}
			} else
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("The program was unable to find the required parameters.");
				Console.ResetColor();
				return;
			}
			if (rootDir == null)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("No root directory has been provided.");
				Console.ResetColor();
				return;
			}

			if (currentJob.Equals("restore"))
			{
				atfPath = args[args.Length - 1];
			}
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine($"Detected requested job: {currentJob}");
			Console.WriteLine($"Detected requested root directory: {rootDir}");
			Console.ForegroundColor = ConsoleColor.Yellow;
			if (currentJob.Equals("restore"))
			{
				Console.WriteLine("Firing up job \"restore\"");
				new Jobs.Restore(rootDir, atfPath);
			} else
			{
				Console.WriteLine("Firing up job \"store\"");
				new Jobs.Store(rootDir);
			}
		}
		private static void ShowLicense()
		{
			try
			{
				string[] licenseLines = File.ReadAllLines("./LICENSE.txt");

				foreach (string line in licenseLines)
				{
					Console.WriteLine(line);
				}
			} catch (IOException)
			{}
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("This program is licensed under the MIT license. Press any key to accept the license.");
			Console.ReadKey();
			Console.ResetColor();
		}
	}
}
