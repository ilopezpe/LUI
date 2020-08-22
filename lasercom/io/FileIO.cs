using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace lasercom.io
{
    public static class FileIO
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <throws>IOException, FormatException</throws>
        /// <returns></returns>
        public static IList<double> ReadTimesFile(string filename)
        {
            IList<double> times = new List<double>();
            foreach (string line in ReadLines(filename))
            {
                line.Trim();
                if (line != String.Empty && !line.StartsWith("#"))
                    times.Add(double.Parse(line));
            }
            return times;
        }

        public static IEnumerable<string> ReadLines(string filename)
        {
            using (TextReader tr = new StreamReader(filename))
            {
                string line;
                while ((line = tr.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }

        public static T[] ReadVector<T>(string FileName)
        {
            T[] vector;
            using (TextReader reader = File.OpenText(FileName))
            using (CsvReader csv = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture))
            {
                csv.Configuration.AllowComments = true;
                csv.Configuration.HasHeaderRecord = false;
                vector = csv.GetRecords<T>().ToArray<T>();
            }
            return vector;
        }

        public static void WriteVector<T>(string FileName, IEnumerable<T> Vector)
        {
            using (var writer = new StreamWriter(FileName))
            {
                using (var csv = new CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(Vector);
                }
                writer.Close();
            }
        }

        public static void WriteMatrix<T>(string FileName, T[,] Matrix)
        {
            using (var writer = new StreamWriter(FileName))
            {
                using (var csv = new CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture))
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
                writer.Close();
            }
        }
    }
}

