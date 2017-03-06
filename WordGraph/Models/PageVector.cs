using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordGraph.Models
{
    public class PageVector
    {
        private static string m_db = Environment.CurrentDirectory + Path.DirectorySeparatorChar;
        public static Dictionary<string, float[]> Dicts = new Dictionary<string, float[]>();
        public static Dictionary<string, List<double>> PageVectors = new Dictionary<string, List<double>>();
        public static int Dimension = 100;

        public static void LoadDict()
        {
            string path = String.Format("{0}wordemb.vec", m_db);
            if (!File.Exists(path))
            {
                throw new IOException($"{path} doesn't exist.");
            }

            Dicts.Clear();
            using (StreamReader reader = new StreamReader(path))
            {
                var items = reader.ReadLine().Split(' ');
                int vocabCnt = int.Parse(items[0]);
                int dim = int.Parse(items[1]);
                Dimension = dim;
                for (int i = 0; i < vocabCnt; i++)
                {
                    items = reader.ReadLine().TrimEnd().Split(' ');
                    if (items.Length - 1 == dim)
                    {
                        string word = items[0];
                        var values = new float[dim];
                        for (int j = 0; j < dim; j++)
                        {
                            values[j] = float.Parse(items[1 + j]);
                        }
                        Dicts[word] = values;
                    }
                }
            }
        }

        public static void ConvertDict()
        {
            var reader = new StreamReader(String.Format("{0}dict.txt", m_db), Encoding.UTF8);
            var stream = new FileStream(String.Format("{0}dict.bin", m_db), FileMode.Create);
            var writer = new BinaryWriter(stream, Encoding.UTF8);

            var line = reader.ReadLine();
            Dicts.Clear();

            long count = 0;
            writer.Write(count);
            while (line != null)
            {
                string[] data = line.Split(',');
                if (data.Length == 201)
                {
                    var list = ConvertToFloats(data);
                    data[0] = data[0].Trim();

                    var codes = Encoding.UTF8.GetBytes(data[0]);
                    Int16 len = (Int16)codes.Length;
                    writer.Write(len);
                    writer.Write(codes);

                    foreach (var f in list)
                    {
                        writer.Write(f);
                    }
                    count++;
                    //Dicts[data[0]] = list;
                }
                line = reader.ReadLine();
            }
            writer.Seek(0, SeekOrigin.Begin);
            writer.Write(count);
            writer.Close();
            stream.Close();
            reader.Close();
        }


        public static void LoadPageVector()
        {
            var stream = new FileStream(String.Format("{0}pagevector.bin", m_db), FileMode.Open);
            var reader = new BinaryReader(stream, Encoding.UTF8);
            long count = reader.ReadInt64();
            PageVectors.Clear();
            int m = 0;

            for (m = 0; m < count; m++)
            {
                int len = reader.ReadInt32();
                var bytes = reader.ReadBytes(len);
                string name = Encoding.UTF8.GetString(bytes);
                reader.ReadByte();
                var data = new List<double>(200);
                for (int i = 0; i < 200; i++)
                {
                    data.Add(reader.ReadDouble());
                }
                var t = reader.ReadByte();
                PageVectors[name] = data;
            }

            reader.Close();
            stream.Close();
        }

        private static List<float> ConvertToFloats(string[] data)
        {
            var result = new List<float>(200);

            for (int i = 1; i < 201; i++)
            {
                result.Add(float.Parse(data[i]));
            }
            return result;
        }

        public static string GetClass(string dir)
        {
            int index = dir.IndexOf('\\');
            if (index > 0)
                dir = dir.Remove(index, dir.Length - index);
            switch (dir)
            {
                case "C000007": return "汽车类";
                case "C000008": return "财经类";
                case "C000010": return "IT类";
                case "C000013": return "健康类";
                case "C000014": return "体育类";
                case "C000016": return "旅游类";
                case "C000020": return "教育类";
                case "C000022": return "招聘类";
                case "C000023": return "文化类";
                case "C000024": return "军事类";
            }
            return "未知类";
        }

        public static string ReadFile(string path)
        {
            StreamReader reader = new StreamReader(String.Format(@"d:\SmallData\source\{0}", path), Encoding.Default);
            string tmp = reader.ReadToEnd();
            reader.Close();
            return tmp;
        }
    }
}
