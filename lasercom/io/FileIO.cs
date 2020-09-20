using CsvHelper;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace LuiHardware.io
{
    public static class FileIO
    {
        /// <summary>
        /// </summary>
        /// <param name="filename"></param>
        /// <throws>IOException, FormatException</throws>
        /// <returns></returns>
        public static IList<double> ReadTimesFile(string filename)
        {
            IList<double> times = new List<double>();
            foreach (var line in ReadLines(filename))
            {
                line.Trim();
                if (line != string.Empty && !line.StartsWith("#"))
                    times.Add(double.Parse(line));
            }

            return times;
        }

        public static IEnumerable<string> ReadLines(string filename)
        {
            using (TextReader tr = new StreamReader(filename))
            {
                string line;
                while ((line = tr.ReadLine()) != null) yield return line;
            }
        }

        public static T[] ReadVector<T>(string FileName)
        {
            T[] vector;
            using (var reader = File.OpenText(FileName))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Configuration.AllowComments = true;
                csv.Configuration.HasHeaderRecord = false;
                vector = csv.GetRecords<T>().ToArray();
            }
            return vector;
        }

        public static void WriteVector<T>(string FileName, IEnumerable<T> vector)
        {
            using (var writer = new StreamWriter(FileName))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(vector);
            }
        }

        public static void WriteMatrix<T>(string FileName, T[,] Matrix)
        {
            using (var writer = new StreamWriter(FileName))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                for (int i = 0; i < Matrix.GetLength(0); i++)
                {
                    for (int j = 0; j < Matrix.GetLength(1); j++)
                    {
                        csv.WriteField(Matrix[i, j].ToString());
                    }
                    csv.NextRecord();
                }
            }
        }
    }
}