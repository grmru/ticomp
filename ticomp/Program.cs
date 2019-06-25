using System;
using System.Collections.Generic;

namespace ticomp
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            System.IO.FileInfo fi = null;

            for (int i = 0; i < args.Length; i++)
            {
                if (!args[i].StartsWith("-") &&
                    !args[i].StartsWith("--") &&
                    !args[i].StartsWith("\\"))
                {
                    fi = new System.IO.FileInfo(args[0]);
                }
            }

            if (fi != null && fi.Exists)
            {
                List<string> compiled_lines = CompileDocument(fi.FullName);

                for (int i = 0; i < compiled_lines.Count; i++)
                {
                    Console.WriteLine(compiled_lines[i]);
                }
            }

#if DEBUG
            Console.ReadKey();
#endif
        }

        public static List<string> CompileDocument(string filePath)
        {
            List<string> ret = new List<string>();

            System.IO.FileInfo fi = new System.IO.FileInfo(filePath);

            if (!fi.Exists)
            {
                return ret;
            }

            List<string> lines = new List<string>(System.IO.File.ReadAllLines(fi.FullName));

            for (int l = 0; l < lines.Count; l++)
            {
                if (lines[l].Contains("{ticomp"))
                {
                    string[] splitted_line = lines[l].Split(new char[] { '{', '}' });
                    bool inside = false;
                    for (int i = 0; i < splitted_line.Length; i++)
                    {
                        if (inside)
                        {
                            bool hasWorked = false;
                            // mask "{ticomp|include = C:\demo\someFileToInclude}"
                            if (splitted_line[i].Contains("ticomp|include"))
                            {
                                List<string> newLine = new List<string>();

                                if (splitted_line[i].Contains("="))
                                {
                                    string[] cmd = splitted_line[i].Split('=');

                                    newLine = CompileDocument(cmd[1].Trim());
                                }

                                string result = string.Empty;

                                for (int r = 0; r < newLine.Count; r++)
                                {
                                    result += newLine[r];

                                    if (r < newLine.Count - 1)
                                    {
                                        result += System.Environment.NewLine;
                                    }
                                }

                                splitted_line[i] = result;

                                hasWorked = true;
                            }

                            if (!hasWorked)
                            {
                                splitted_line[i] = "{" + splitted_line[i] + "}";
                            }
                        }

                        inside = !inside;
                    }

                    lines[l] = string.Empty;
                    for (int i = 0; i < splitted_line.Length; i++)
                    {
                        lines[l] += splitted_line[i];
                    }
                }

                ret.Add(lines[l]);
            }

            return ret;
        }
    }
}