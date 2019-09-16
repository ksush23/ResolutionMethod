using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Метод_резолюцій
{
    class Program
    {
        static Hashtable table;
        static void Main(string[] args)
        { 
            Console.WriteLine("Введiть вираз 1:");
            string line1 = Console.ReadLine();

            Console.WriteLine("Введiть вираз 2:");
            string line2 = Console.ReadLine();

            string line = ToLine(line1, line2);
            table = GetDyz(line);

            bool ok = false;
            int n1 = table.Count;
            int n = n1;
            do
            {
                n = n1;
                ok = Blake(table);
                n1 = table.Count;
            } while (n != n1);

            if (ok == true)
                Console.WriteLine("\nТвердження вiрне");
            else
                CounterExample(table);
            
            Console.ReadKey();
        }

        public static Hashtable GetDyz(string line)
        {
            Hashtable hashtable = new Hashtable();
            int n = 0;
            string current = "";
            int i = 0;

            while (i != line.Length)
            {
                if (line[i] != '&')
                {
                    if (line[i] != ' ')
                        current += line[i];
                }
                else
                {
                    hashtable.Add(n, current);
                    n++;

                    if (line[i + 2] == '&')
                        line = line.Remove(0, i + 2);
                    else
                        line = line.Remove(0, i + 1);
                    current = "";
                    i = 0;
                }
                i++;
            }

            hashtable.Add(n, current);
            return hashtable;
        }

        public static bool Blake(Hashtable table)
        {
            Hashtable hashtable = new Hashtable();
            
            foreach (DictionaryEntry el in table)
            {
                hashtable.Add(hashtable.Count, el.Value.ToString());
            }

            string current;
            bool indicator = true;
            foreach (DictionaryEntry el in hashtable)
            {
                current = el.Value.ToString();
                string line = "Склеюємо " + current;

                if (current.Length == 1 || current.Length == 2)
                {
                    bool ok = SearchForRezolventa(ref current, ref indicator, ref hashtable, "", "", ref line);
                    if (ok)
                        return true;
                }
                else
                {
                    string add = "";
                    string remember = "";
                    string currentLine = current;
                    while (currentLine !=  string.Empty)
                    {
                        string element = "";
                        int i = 0;

                        while (currentLine[i] != 'v' && currentLine[i] != '&')
                        {
                            if (currentLine[i] != ' ')
                                element += currentLine[i];
                            i++;

                            if (i == currentLine.Length)
                                break;
                        }
                        currentLine = currentLine.Remove(0, i);

                        if (currentLine != "")
                        {
                            add = currentLine;
                            remember = element;
                        }
                        else
                            add = "v" + remember;

                        indicator = true;
                        bool ok = SearchForRezolventa(ref element, ref indicator, ref hashtable, add, current, ref line);
                        if (ok)
                            return true;

                        if (currentLine != "")
                            currentLine = currentLine.Remove(0, 1);
                    }
                }
            }
            return false;
        }

        public static bool SearchForRezolventa(ref string current, ref bool indicator, ref Hashtable hashtable, string add, string actualLine, ref string line)
        {
            string secondCurrent;
            string element = current;
            if (current.Length == 2)
            {
                element = element.Remove(0, 1);
                indicator = false;
            }
            if (current.Length == 1)
            {
                indicator = true;
            }

            foreach (DictionaryEntry porEl in hashtable)
            {
                secondCurrent = porEl.Value.ToString();
                string addedLine = line + " i " + secondCurrent + "\n";

                if (secondCurrent == current)
                    continue;
                if (secondCurrent == actualLine)
                    continue;

                if (secondCurrent.Contains(element))
                {
                    if (secondCurrent.Length > 2)
                    {
                        int index = secondCurrent.IndexOf(element);
                        bool ok = false;

                        if (index != 0)
                            if ((secondCurrent[index - 1] == '-' && indicator) || (secondCurrent[index - 1] != '-' && !indicator))
                                ok = AddToHashTable(secondCurrent, indicator, index, add, ref addedLine);
                        if (index == 0)
                            if (!indicator)
                                ok = AddToHashTable(secondCurrent, indicator, index,add, ref addedLine);
                        if (ok)
                            return true;
                    }
                }
            }
            return false;
        }

        public static bool AddToHashTable(string secondCurrent, bool indicator, int index, string add, ref string addedLine)
        {
            int i = 3;
            string newLine = secondCurrent;
            if (index > 1 && index < newLine.Length)
            {
                if (newLine[index - 1] != '-')
                    newLine = newLine.Remove(index - 1);
                else
                {
                    try { newLine = newLine.Remove(index - 2, index); }
                    catch
                    {
                        try { newLine = newLine.Remove(index - 2, index - 1); }
                        catch
                        {
                            try { newLine = newLine.Remove(index - 2, index - 2); }
                            catch { newLine = newLine.Remove(index - 2, index - i); }
                        }
                    }    
                }
            }
            if (index == 0)
                newLine = newLine.Remove(index, index + 2);
            if (index == 1)
                newLine = newLine.Remove(index - 1, index + 2);

            if (add != "")
                newLine += add;

            if (!table.ContainsValue(newLine) && IsAddible(table, newLine))
            {
                Console.Write(addedLine);
                table.Add(table.Count, newLine);
                Console.Write("(");
                foreach (DictionaryEntry entry in table)
                    Console.Write(entry.Value.ToString() + ";");
                Console.Write(")");
                Console.WriteLine();
            }

            bool ok = CheckForResolventa(table);

                if (ok)
                return true;
            return false;
        }

        public static bool CheckForResolventa(Hashtable hashtable)
        {
            foreach (DictionaryEntry el in hashtable)
            {
                string current = el.Value.ToString();
                
                if (current.Length == 1 || current.Length == 2)
                {
                    foreach (DictionaryEntry el1 in hashtable)
                    {
                        string current1 = el1.Value.ToString();

                        if (current == current1)
                            continue;

                        if (current1.Length == 1 || current1.Length == 2)
                        {
                            if (current.Length == 1 && current1.Length == 2)
                            {
                                if (current == current1[1].ToString())
                                    return true;
                            }

                            if (current.Length == 2 && current1.Length == 1)
                            {
                                if (current1 == current[1].ToString())
                                    return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public static bool IsAddible(Hashtable hashtable, string addedLine)
        {
            char first;
            char second;
            int index = 0;

            try
            {
                if (addedLine[index] != '-')
                {
                    first = addedLine[index];
                    if (addedLine[index + 2] != '-')
                        second = addedLine[index + 2];
                    else
                        second = addedLine[index + 3];
                }
                else
                {
                    first = addedLine[index + 1];

                    if (addedLine[index + 3] != '-')
                        second = addedLine[index + 3];
                    else
                        second = addedLine[index + 4];
                }
            }
            catch { return true; }

            if (first == second)
                return false;

            string line = "";
            for (int i = addedLine.Length - 1; i >= 0; i--)
            {
                if (addedLine[i] == '-')
                {
                    char c = addedLine[i + 1];
                    line = line.Replace(addedLine[i + 1], '-');
                    line += addedLine[i + 1];
                }
                else
                    line += addedLine[i];
            }


            foreach (DictionaryEntry el in hashtable)
            {
                if (el.Value.ToString().Equals(line))
                    return false;
            }

            return true;
        }

        public static string ToLine(string line1, string line2)
        {
            string line = line1 + " & ";

            for (int i = 0; i < line2.Length; i++)
            {
                if (line2[i] == 'v')
                    line += " & ";
                if (line2[i] == '&')
                    line += " v ";

                if (line2[i] != 'v' && line2[i] != '&' && line2[i] != ' ' && line2[i] != '-')
                {
                    line += "-" + line2[i];
                }

                if (line2[i] == ' ')
                    line += " ";

                if (line2[i] == '-')
                    continue;
            }

            return line;
        }

        public static void CounterExample(Hashtable hashtable)
        {
            Console.WriteLine("\nКонтр-приклад:");

            foreach (DictionaryEntry el in hashtable)
            {
                if (el.Value.ToString().Length == 1 || el.Value.ToString().Length == 2)
                {
                    if (el.Value.ToString().Length == 1)
                        Console.WriteLine("I(" + el.Value.ToString() + ") = T");
                    else
                        Console.WriteLine("I(" + el.Value.ToString()[1] + ") = F");
                }
            }
        }
    }

}
