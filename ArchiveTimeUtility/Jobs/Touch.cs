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
using System.Text;

namespace ArchiveTimeUtility.Jobs
{
    class Touch
    {
        private string path;
        /// <summary>
        /// Initiates a new touch job in the specified context.
        /// </summary>
        /// <param name="path">The context path.</param>
        public Touch(string path)
        {
            this.path = path;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Touch job started.");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Initiating logic on root directory ({path})...");
            Console.ResetColor();
            StoreTimestamps(path);
        }


        private void StoreTimestamps(string rootDir, int layer = 1)
        {
            foreach (string childFile in Directory.GetFiles(rootDir))
            {
                if (layer != 0) for (int x = 0; x < layer; x++) { Console.Write(" "); }
                Console.Write("Found file: ");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write($"{childFile}\n");
                Console.ResetColor();

                File.SetCreationTimeUtc(childFile, DateTime.Now);
                File.SetLastWriteTimeUtc(childFile, DateTime.Now);
                File.SetLastAccessTimeUtc(childFile, DateTime.Now);
            }

            foreach (string childDir in Directory.GetDirectories(rootDir))
            {
                if (layer != 0) for (int x = 0; x < layer; x++) { Console.Write(" "); }
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Launching inner search for directory ");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write($"{childDir} in {rootDir}\n");
                Console.ResetColor();

                File.SetCreationTimeUtc(childDir, DateTime.Now);
                File.SetLastWriteTimeUtc(childDir, DateTime.Now);
                File.SetLastAccessTimeUtc(childDir, DateTime.Now);

                StoreTimestamps(childDir, layer + 1);
            }

            /*foreach (string childDir in Directory.GetDirectories(rootDir))
                StoreTimestamps(childDir, layer + 1);*/
        }
    }
}
