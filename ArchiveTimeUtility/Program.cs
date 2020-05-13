﻿/*
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

namespace ArchiveTimeUtility
{
	class Program
	{
		private static readonly string[] VALID_JOBS = { "store", "restore" };
		private static readonly string[] VALID_PARAMS = { "-d", "--dir" };
		static void Main(string[] args)
		{
			// Syntax: atu [store|restore] <--dir directory>
			string rootDir = null,
				currentJob = null;
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
							Console.WriteLine("An unknown parameter has been provided.");
							return;
						}
					}
					if (x == 0)
					{
						if (args[x].StartsWith('-'))
						{
							Console.WriteLine("The first argument cannot be a parameter.");
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
								Console.WriteLine("An unknown job has been provided and the program could not understand it.");
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
							Console.WriteLine("Although the directory argument was specified, its value could not be found.");
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
				Console.WriteLine("The program was unable to find the required parameters.");
				return;
			}
			if (rootDir == null)
			{
				Console.WriteLine("No root directory has been provided.");
				return;
			}
			Console.WriteLine($"Detected requested job: {currentJob}");
			Console.WriteLine($"Detected requested root directory: {rootDir}");
			if (currentJob.Equals("restore"))
			{
				Console.WriteLine("Firing up job \"restore\"");
				new Jobs.Restore(rootDir);
			} else
			{
				Console.WriteLine("Firing up job \"store\"");
				new Jobs.Store(rootDir);
			}
		}
	}
}
