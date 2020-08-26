using System;
using System.Collections.Generic;

namespace ticomp
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            System.IO.FileInfo fi = null;

            System.Text.Encoding input_enc = System.Text.Encoding.Default;
            System.Text.Encoding output_enc = System.Text.Encoding.Default;

            System.IO.FileInfo fo = null;

            bool verbose = false;

            if (args.Length == 0)
            {
                Console.WriteLine("");
                Console.WriteLine("TiComp. Text compilation. Copyright (C) 2018 - 2020 George A. Tsyrkov");
                Console.WriteLine("");
                Console.WriteLine("USAGE: ticomp [options] inputfile.md > outputfile.md");
                Console.WriteLine("        or");
                Console.WriteLine("       ticomp [options] inputfile.md -out=\"outputfile.md\"");
                Console.WriteLine("");
                Console.WriteLine("[options]:");
                Console.WriteLine("           -ie=code - set input file codepage number");
                Console.WriteLine("           -oe=code - set output file codepage number");
                Console.WriteLine("           -out=\"output file path\" - set output file path");
                Console.WriteLine("           -verb - verbose mode on (please, do not use it in pipeline mode)");
                Console.WriteLine("");

                return;
            }

            for (int i = 0; i < args.Length; i++)
            { 
                if (!args[i].StartsWith("-") &&
                    !args[i].StartsWith("--") &&
                    !args[i].StartsWith("\\"))
                {
                    fi = new System.IO.FileInfo(args[i]);
                }
                else
                {
                    string key = args[i].TrimStart('-').TrimStart('\\');
                    if (key.StartsWith("ie"))
                    {
                        if (key.Contains("="))
                        {
                            string[] split = key.Split('=');
                            try
                            {
                                int codepage = System.Text.Encoding.Default.CodePage;
                                int.TryParse(split[1], out codepage);
                                input_enc = System.Text.Encoding.GetEncoding(codepage);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("[ERROR]: " + ex.Message);
                            }
                        }
                    }
                    if (key.StartsWith("oe"))
                    {
                        if (key.Contains("="))
                        {
                            string[] split = key.Split('=');
                            try
                            {
                                int codepage = System.Text.Encoding.Default.CodePage;
                                int.TryParse(split[1], out codepage);
                                output_enc = System.Text.Encoding.GetEncoding(codepage);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("[ERROR]: " + ex.Message);
                            }
                        }
                    }
                    if (key.StartsWith("out"))
                    {
                        if (key.Contains("="))
                        {
                            string[] split = key.Split('=');
                            try
                            {
                                fo = new System.IO.FileInfo(split[1].Trim());
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("[ERROR]: " + ex.Message);
                            }
                        }
                    }
                    if (key.StartsWith("verb"))
                    {
                        verbose = true;                        
                    }
                }
            }

            if (fi != null && fi.Exists)
            {
                Console.OutputEncoding = output_enc;

                if (verbose)
                {
                    for (int i = 0; i < args.Length; i++)
                    {
                        Console.WriteLine(string.Format("[INFO]: args[{0}] = {1};", i, args[i]));
                    }
                    Console.WriteLine("[INFO]: Default codepage = " + System.Text.Encoding.Default.CodePage);
                    Console.WriteLine("[INFO]: Input codepage = " + input_enc.CodePage);
                    Console.WriteLine("[INFO]: Output codepage = " + output_enc.CodePage);
                    Console.WriteLine("[INFO]: Console input codepage = " + Console.InputEncoding.CodePage);
                    Console.WriteLine("[INFO]: Console output codepage = " + Console.OutputEncoding.CodePage);
                    Console.WriteLine("[INFO]: Input filename = " + fi.FullName);
                    if (fo != null)
                    {
                        Console.WriteLine("[INFO]: Output filename = " + fo.FullName);
                    }
                }

                List<string> compiled_lines = CompileDocument(fi.FullName, input_enc);

                if (fo != null)
                {
                    System.IO.File.WriteAllLines(fo.FullName, compiled_lines, output_enc);
                }
                else
                {
                    for (int i = 0; i < compiled_lines.Count; i++)
                    {
                        Console.WriteLine(compiled_lines[i]);
                    }
                }
            }
            else
            {
                Console.WriteLine("[ERROR]: не получилось считать файл " + fi != null ? fi.FullName : "null");
            }

#if DEBUG
            Console.ReadKey();
#endif
        }

        public static List<string> CompileDocument(string filePath, System.Text.Encoding input_enc)
        {
            List<string> ret = new List<string>();

            System.IO.FileInfo fi = new System.IO.FileInfo(filePath);

            if (!fi.Exists)
            {
                return ret;
            }

            List<string> lines = new List<string>(System.IO.File.ReadAllLines(fi.FullName, input_enc));

            Dictionary<string, int> RefNumbers = new Dictionary<string, int>();
            int LastRefNumber = 1;

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

                            // - Reference numbers in file
                            //
                            // mask "{ticomp|ref = mainPicture}"
                            //
                            if (splitted_line[i].Contains("ticomp|ref"))
                            {
                                if (splitted_line[i].Contains("="))
                                {
                                    string[] cmd = splitted_line[i].Split('=');
                                    string refKey = cmd[1].Trim();
                                    if (!RefNumbers.ContainsKey(refKey))
                                    {
                                        RefNumbers.Add(refKey, LastRefNumber);
                                        LastRefNumber++;
                                    }

                                    splitted_line[i] = RefNumbers[refKey];
                                    hasWorked = true;
                                }
                            }
                            
                            // - Including external file
                            //
                            // mask "{ticomp|include = C:\demo\someFileToInclude}"
                            //
                            if (splitted_line[i].Contains("ticomp|include"))
                            {
                                List<string> newLine = new List<string>();

                                if (splitted_line[i].Contains("="))
                                {
                                    string[] cmd = splitted_line[i].Split('=');

                                    newLine = CompileDocument(cmd[1].Trim(), input_enc);
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