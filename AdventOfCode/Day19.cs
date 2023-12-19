using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode
{

    internal class Day19 : BaseDay
    {

        enum RuleOperator
        {
            GreaterThan,
            LessThan
        }

        struct WorkflowRule
        {
            public string Property;
            public RuleOperator Operator;
            public int Value;
            public string Destination;

            public bool Evaluate(Dictionary<string, int> part)
            {
                var value = part[Property];
                switch (Operator)
                {
                    case RuleOperator.GreaterThan:
                        return value > Value;
                    case RuleOperator.LessThan:
                        return value < Value;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        struct Workflow
        {
            public Workflow() { }

            public Workflow(string workflowString) : this()
            {
                var parts = workflowString.Split('{');
                Name = parts[0];
                workflowString = parts[1][..^1];
                parts = workflowString.Split(',');
                for (var x = 0; x < parts.Length - 1; x++)
                {
                    var part = parts[x];
                    var property = part[0].ToString();
                    var op = part[1];

                    var ruleDestParts = part.Split(':');

                    var value = int.Parse(ruleDestParts[0][2..]);
                    var destination = ruleDestParts[1];
                    Rules.Add(new WorkflowRule() { Property = property, Operator = op == '<' ? RuleOperator.LessThan : RuleOperator.GreaterThan, Value = value, Destination = destination });
                }
                DefaultDestination = parts[^1];
            }

            public string Name;
            public List<WorkflowRule> Rules = new();
            public string DefaultDestination;

            public string GetDestination(Dictionary<string, int> part)
            {
                foreach (var rule in Rules)
                {
                    if (rule.Evaluate(part))
                    {
                        return rule.Destination;
                    }
                }
                return DefaultDestination;
            }
        }
        enum WorkflowResult
        {
            ACCEPT, REJECT
        }

        class WorkflowEngine
        {
            public Dictionary<string, Workflow> Workflows = new();

            public void AddWorkflow(string workflowString)
            {
                var workflow = new Workflow(workflowString);
                Workflows.Add(workflow.Name, workflow);
            }

            public WorkflowResult RunWorkflow(Dictionary<string, int> part)
            {
                var workflow = Workflows["in"];
                string destination = workflow.GetDestination(part);

                while (destination != "A" && destination != "R")
                {
                    workflow = Workflows[destination];
                    destination = workflow.GetDestination(part);
                }
                return destination == "A" ? WorkflowResult.ACCEPT : WorkflowResult.REJECT;
            }

            internal ulong GetAcceptedCount(string workflowName, Dictionary<string, (int Min, int Max)> ranges)
            {
                if (workflowName == "R")
                {
                    return 0;
                }
                if (workflowName == "A")
                {
                    ulong product = 1;
                    foreach (var range in ranges)
                    {
                        product *= (ulong)(range.Value.Max - range.Value.Min + 1);
                    }
                    return product;
                }

                var workflow = Workflows[workflowName];

                ulong total = 0;

                foreach(var rule in workflow.Rules)
                {
                    (int min, int max) = ranges[rule.Property];
                    (int min, int max) trueHalf = (0,0);
                    (int min, int max) falseHalf = (0,0);

                    if (rule.Operator == RuleOperator.LessThan)
                    {
                        trueHalf = (min, rule.Value - 1);
                        falseHalf = (rule.Value, max);
                    } else
                    {
                        trueHalf = (rule.Value + 1, max);
                        falseHalf = (min, rule.Value);
                    }


                    if (trueHalf.min < trueHalf.max)
                    {
                        var trueRanges = new Dictionary<string, (int Min, int Max)>(ranges);

                        trueRanges[rule.Property] = trueHalf;
                        total += GetAcceptedCount(rule.Destination, trueRanges);
                    }

                    if (falseHalf.min < falseHalf.max)
                    {
                        ranges = new Dictionary<string, (int Min, int Max)>(ranges);
                        ranges[rule.Property] = falseHalf;
                    }
                    else
                    {
                        break;
                    }
                }

                total += GetAcceptedCount(workflow.DefaultDestination, ranges);


                return total;
            }
        }
        WorkflowEngine Engine = new();
        List<Dictionary<string,int>> Parts = new();
        public Day19()
        {
            var lines = File.ReadAllLines(InputFilePath);
            bool parsingParts = false;
            foreach (var line in lines)
            {
                if (line == "")
                {
                    parsingParts = true;
                    continue;
                }

                if (!parsingParts)
                {
                    Engine.AddWorkflow(line);
                }
                else
                {
                    var part = new Dictionary<string, int>();
                    var parts = line[1..^1].Split(',');
                    foreach (var p in parts)
                    {
                        var kv = p.Split('=');
                        part.Add(kv[0], int.Parse(kv[1]));
                    }
                    Parts.Add(part);
                }
            }
        }

        public override ValueTask<string> Solve_1()
        {
            var answer = 0;
            foreach(var part in Parts)
            {
                var result = Engine.RunWorkflow(part);
                if (result == WorkflowResult.ACCEPT)
                {
                    var partSum = part["x"] + part["m"] + part["a"] + part["s"];
                    answer += partSum;
                }
            }

            return new(answer.ToString());
        }

        public override ValueTask<string> Solve_2()
        {
            var list = new List<WorkflowRule>();
            Dictionary<string, (int Min, int Max)> ranges = new() { { "x", (1, 4000) }, { "m", (1, 4000) }, { "a", (1, 4000) }, { "s", (1, 4000) } };
            var ans = Engine.GetAcceptedCount("in", ranges);

            return new("");
        }
    }
}
