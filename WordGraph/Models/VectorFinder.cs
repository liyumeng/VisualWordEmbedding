using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NewsSpectrum.Models
{
    public class VectorFinder
    {
        public static void LoadDict()
        {
            PageVector.LoadDict();
            foreach (var record in PageVector.Dicts)
            {
                double len = 0;
                for (int i = 0; i < PageVector.Dimension; ++i)
                {
                    len += record.Value[i] * record.Value[i];
                }
                len = Math.Sqrt(len);
                for (int i = 0; i < PageVector.Dimension; ++i)
                {
                    record.Value[i] = (float)(record.Value[i] / len);
                }
            }
        }

        private static string[] SPLITS = { " " };
        public static IEnumerable<ResultModel> FindWord(string src, CancellationToken token)
        {
            int dim = PageVector.Dimension;
            string[] words = src.Split(SPLITS, StringSplitOptions.RemoveEmptyEntries);
            var result = new List<ResultModel>(10);
            if (words.Length == 0) return result;
            var vecs = new double[dim];
            for (int i = 0; i < dim; i++)
                vecs[i] = 0;
            int t = 0;
            foreach (var w in words)
            {
                if (String.IsNullOrWhiteSpace(w))
                    continue;
                if (!PageVector.Dicts.ContainsKey(w))
                {
                    return result;
                }
                var vec = PageVector.Dicts[w];
                for (int i = 0; i < dim; i++)
                    vecs[i] += vec[i];
                t++;
            }
            if (token.IsCancellationRequested)
            {
                return null;
            }
            if (t == 0) return result;
            ResultModel minVal = null;
            foreach (var record in PageVector.Dicts)
            {
                if (token.IsCancellationRequested)
                {
                    return null;
                }
                if (record.Key == src)
                    continue;
                double dist = 0;
                for (int i = 0; i < dim; i++)
                {
                    dist += vecs[i] * record.Value[i];
                }

                if (result.Count >= 10)
                {
                    if (minVal == null)
                    {
                        double min = 2;
                        int mini = 0;
                        for (int i = 0; i < result.Count; i++)
                        {
                            if (result[i].Distance < min)
                            {
                                min = result[i].Distance;
                                mini = i;
                            }
                        }
                        minVal = result[mini];
                    }
                    if (dist > minVal.Distance)
                    {
                        minVal.Content = record.Key;
                        minVal.Distance = dist;
                        minVal = null;
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    result.Add(new ResultModel(record.Key, dist));
                }
            }
            result.Sort();

            return result;
        }

        public static void LoadPageVectors()
        {
            LoadDict();
            PageVector.LoadPageVector();
            foreach (var record in PageVector.PageVectors)
            {
                double len = 0;
                for (int i = 0; i < PageVector.Dimension; ++i)
                {
                    len += record.Value[i] * record.Value[i];
                }
                len = Math.Sqrt(len);
                if (len.Equals(0)) len = 1;
                for (int i = 0; i < PageVector.Dimension; ++i)
                {
                    record.Value[i] = (float)(record.Value[i] / len);
                }
            }
        }

        public static IEnumerable<ResultModel> FindPage(string src, CancellationToken token)
        {
            string[] words = src.Split(SPLITS, StringSplitOptions.RemoveEmptyEntries);
            var result = new List<ResultModel>(10);
            if (words.Length == 0) return result;
            int dim = PageVector.Dimension;

            double[] vecs = new double[dim];
            for (int i = 0; i < dim; i++)
                vecs[i] = 0;
            int t = 0;
            foreach (var w in words)
            {
                if (String.IsNullOrWhiteSpace(w))
                    continue;
                if (!PageVector.Dicts.ContainsKey(w))
                {
                    return result;
                }
                var vec = PageVector.Dicts[w];
                for (int i = 0; i < dim; i++)
                    vecs[i] += vec[i];
                t++;
            }

            if (t == 0) return result;

            double len = 0;
            for (int i = 0; i < dim; i++)
                len += vecs[i] * vecs[i];
            if (len == 0) len = 1;
            for (int i = 0; i < dim; i++)
                vecs[i] = vecs[i] / len;

            ResultModel minVal = null;
            foreach (var record in PageVector.PageVectors)
            {
                if (token.IsCancellationRequested)
                {
                    return null;
                }
                if (record.Key == src)
                    continue;
                double dist = 0;
                for (int i = 0; i < dim; i++)
                {
                    dist += vecs[i] * record.Value[i];
                }

                if (result.Count >= 10)
                {
                    if (minVal == null)
                    {
                        double min = 2;
                        int mini = 0;
                        for (int i = 0; i < result.Count; i++)
                        {
                            if (result[i].Distance < min)
                            {
                                min = result[i].Distance;
                                mini = i;
                            }
                        }
                        minVal = result[mini];
                    }
                    if (dist > minVal.Distance)
                    {
                        minVal.Content = record.Key;
                        minVal.Distance = dist;
                        minVal = null;
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    result.Add(new ResultModel(record.Key, dist));
                }
            }
            result.Sort();

            foreach (var r in result)
            {
                r.Type = PageVector.GetClass(r.Content);
            }

            return result.Take(10);
        }

        public static IEnumerable<ResultModel> Find(string src, CancellationToken token)
        {
            string[] words = src.Split(SPLITS, StringSplitOptions.RemoveEmptyEntries);
            var result = new List<ResultModel>(10);
            if (words.Length == 0) return result;
            int dim = PageVector.Dimension;

            double[] vecs = new double[dim];
            for (int i = 0; i < dim; i++)
                vecs[i] = 0;
            int t = 0;
            foreach (var w in words)
            {
                if (String.IsNullOrWhiteSpace(w))
                    continue;
                if (!PageVector.PageVectors.ContainsKey(w))
                {
                    return result;
                }
                var vec = PageVector.PageVectors[w];
                for (int i = 0; i < dim; i++)
                    vecs[i] += vec[i];
                t++;
            }

            if (t == 0) return result;

            double len = 0;
            for (int i = 0; i < dim; i++)
                len += vecs[i] * vecs[i];
            if (len == 0) len = 1;
            for (int i = 0; i < dim; i++)
                vecs[i] = vecs[i] / len;

            ResultModel minVal = null;
            foreach (var record in PageVector.PageVectors)
            {
                if (token.IsCancellationRequested)
                {
                    return null;
                }
                if (record.Key == src)
                    continue;
                double dist = 0;
                for (int i = 0; i < dim; i++)
                {
                    dist += vecs[i] * record.Value[i];
                }

                if (result.Count >= 10)
                {
                    if (minVal == null)
                    {
                        double min = 2;
                        int mini = 0;
                        for (int i = 0; i < result.Count; i++)
                        {
                            if (result[i].Distance < min)
                            {
                                min = result[i].Distance;
                                mini = i;
                            }
                        }
                        minVal = result[mini];
                    }
                    if (dist > minVal.Distance)
                    {
                        minVal.Content = record.Key;
                        minVal.Distance = dist;
                        minVal = null;
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    result.Add(new ResultModel(record.Key, dist));
                }
            }
            result.Sort();

            foreach (var r in result)
            {
                r.Type = PageVector.GetClass(r.Content);
            }

            return result.Take(10);
        }
    }
}
