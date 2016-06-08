using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Rank
{
    class Program
    {
        static void Main(string[] args)
        {
            string []filenames = Directory.GetFiles(@"./score");
            Dictionary<string, Tuple<double, double>> score = new Dictionary<string, Tuple<double, double>> ();
            foreach(string filename in filenames)
            {
                string[] split = filename.Split('_');
                string name = split[0];
                string s_max = split[split.Length - 2];
                string s_mean = split[split.Length - 1];
                s_mean = s_mean.Substring(0, s_mean.LastIndexOf('.'));

                double max = Convert.ToDouble(s_max);
                double mean = Convert.ToDouble(s_mean);
                score.Add(name.Substring(8), new Tuple<double, double>(max, mean));
            }

            var maxRank = score.ToList();
            maxRank.Sort((pair1, pair2) => pair1.Value.Item1.CompareTo(pair2.Value.Item1));
            var meanRank = score.ToList();
            meanRank.Sort((pair1, pair2) => pair1.Value.Item2.CompareTo(pair2.Value.Item2));

            StreamWriter sw_maxRank = new StreamWriter("maxRank.csv");
            StreamWriter sw_meanRank = new StreamWriter("meanRank.csv");
            for(int i = maxRank.Count - 1; i >= 0; i--)
            {
                sw_maxRank.WriteLine(maxRank.ElementAt(i).Key + "," + maxRank.ElementAt(i).Value.Item1);
                sw_meanRank.WriteLine(meanRank.ElementAt(i).Key + "," + meanRank.ElementAt(i).Value.Item2);
            }
            sw_maxRank.Close();
            sw_meanRank.Close();
        }
    }
}
