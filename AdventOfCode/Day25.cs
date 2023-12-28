using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode
{
    internal class Day25 : BaseDay
    {

        Dictionary<string, List<string>> Nodes = new();
        Random random = new(6);
        public Day25()
        {
            var lines = File.ReadAllLines(InputFilePath);
            foreach(var line in lines)
            {
                var parts = line.Replace(":","").Split(" ",StringSplitOptions.RemoveEmptyEntries);
                var node = parts[0];
                foreach(var part in parts.Skip(1))
                {
                    if (!Nodes.ContainsKey(node))
                        Nodes[node] = new List<string>();
                    Nodes[node].Add(part);

                    if (!Nodes.ContainsKey(part))
                        Nodes[part] = new List<string>();
                    Nodes[part].Add(node);
                }
            }
        }

        (int cutSize, int a, int b) FindMinCut()
        {
            int mergeID = 0;
            /* Copy the Nodes dictionary to a temporary one */
            var nodes = new Dictionary<string, List<string>>();
            foreach (var node in Nodes)
                nodes[node.Key] = new List<string>(node.Value);

            var componentSize = nodes.Keys.ToDictionary( k => k, v => 1);

            var mergeNode = string (string node1, string node2) =>
            {
                var newNode = $"merge{mergeID++}";
                nodes[newNode] = new List<string>();
                nodes[newNode].AddRange(nodes[node1].Where(n => n != node2));
                nodes[newNode].AddRange(nodes[node2].Where(n => n != node1));

                /* update back references */
                foreach(var child in nodes[node1])
                {
                    while (nodes[child].Remove(node1)){
                        nodes[child].Add(newNode);
                    }
                }

                foreach (var child in nodes[node2])
                {
                    while (nodes[child].Remove(node2))
                    {
                        nodes[child].Add(newNode);
                    }
                }

                return newNode;
            };  

            while (nodes.Count > 2)
            {
                var u = nodes.Keys.ElementAt(random.Next(nodes.Count));
                var v = nodes[u].ElementAt(random.Next(nodes[u].Count));

                var newNode = mergeNode(u, v);

                componentSize[newNode] = componentSize[u] + componentSize[v];

                nodes.Remove(u);
                nodes.Remove(v);
            }

            return (nodes[nodes.Keys.First()].Count, componentSize[nodes.Keys.First()], componentSize[nodes.Keys.Last()]);
        }

        public override ValueTask<string> Solve_1()
        {
            var (count, sizeA, sizeB) = FindMinCut();
            while (count != 3)
            {
                (count, sizeA, sizeB) = FindMinCut();
            }
            return new((sizeA * sizeB).ToString());
        }

        public override ValueTask<string> Solve_2()
        {
            return new("Merry Christmas!");
        }
    }
}
