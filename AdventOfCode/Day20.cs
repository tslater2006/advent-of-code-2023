using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode
{

    public enum PulseType
    {
        HIGH, LOW
    }

    enum FlipState
    {
        ON, OFF
    }
    class ButtonModule : Module
    {
        public override bool IsInInitialState()
        {
            return true;
        }

        public override void ProcessPulse(Module source, PulseType pulse)
        {
            SendPulse(PulseType.LOW);
        }
    }
    class BroadcasterModule : Module
    {
        public override bool IsInInitialState()
        {
            return true;
        }
        public override void ProcessPulse(Module source, PulseType pulse)
        {
            SendPulse(pulse);
        }
    }

    class FlipFlopModule : Module
    {
        FlipState State = FlipState.OFF;
        public override bool IsInInitialState()
        {
            return State == FlipState.OFF;
        }

        public override void ProcessPulse(Module source, PulseType pulse)
        {
            if (pulse == PulseType.LOW)
            {
                if (State == FlipState.OFF)
                {
                    State = FlipState.ON;
                    SendPulse(PulseType.HIGH);
                }
                else
                {
                    State = FlipState.OFF;
                    SendPulse(PulseType.LOW);
                }
            }
        }
        public override void Reset()
        {
            State = FlipState.OFF;
        }
    }

    class ConjuctionModule : Module
    {
        public override bool IsInInitialState()
        {
            return PulseStates.Values.All(p => p == PulseType.LOW);
        }
        public override void ProcessPulse(Module source, PulseType pulse)
        {

            PulseStates[source] = pulse;

            if (PulseStates.Values.All(p => p == PulseType.HIGH))
            {
                SendPulse(PulseType.LOW);
            }
            else
            {
                SendPulse(PulseType.HIGH);
            }
        }

        protected override void AddInput(Module m)
        {
            base.AddInput(m);
            PulseStates[m] = PulseType.LOW;
        }

        public override void Reset()
        {
            foreach (var kvp in PulseStates)
            {
                PulseStates[kvp.Key] = PulseType.LOW;
            }
        }

    }

    class OutputModule : Module
    {
        public OutputModule()
        {
            //Debugger.Break();
        }
        public List<PulseType> ReceivedPulses = new();
        public override bool IsInInitialState()
        {
            return true;
        }
        public override void ProcessPulse(Module source, PulseType pulse)
        {
            PulseStates[source] = pulse;
            ReceivedPulses.Add(pulse);
        }
        public override void Reset()
        {
            ReceivedPulses.Clear();
        }
    }
    public delegate void QueuePulseHandler(Module sender, PulseType pulse, Module destination);
    public abstract class Module
    {
        /* make an event to handle sending a pulse to a destination module */
        public event QueuePulseHandler OnQueueSignal;
        public List<Module> Inputs = new();
        public List<Module> Outputs = new();

        protected Dictionary<Module, PulseType> PulseStates = new();

        public abstract bool IsInInitialState();
        protected virtual void AddInput(Module m) { 
            Inputs.Add(m);      
        }

        public void AddOutput(Module m) { 
            Outputs.Add(m);
            m.AddInput(this);
        }


        public void SendPulse(PulseType pulse)
        {
            foreach (var output in Outputs)
            {
                OnQueueSignal?.Invoke(this, pulse, output);
            }
        }

        public virtual void Reset() { }

        public abstract void ProcessPulse(Module sender, PulseType pulse);
    }

    internal class Day20 : BaseDay
    {
        Dictionary<string, Module> ModuleMap = new();
        Queue<(Module source, PulseType pulse, Module dest)> processingQueue = new();
        public Day20()
        {
            Dictionary<string, List<string>> ConnectionMap = new();

            var lines = File.ReadAllLines(InputFilePath);

            foreach(var line in lines)
            {
                var halves = line.Split("->").Select(p => p.Trim()).ToArray();

                var moduleName = halves[0];
                if (moduleName[0] == '%')
                {
                    moduleName = moduleName[1..];
                    ModuleMap.Add(moduleName, new FlipFlopModule());
                }else if (moduleName[0] == '&')
                {
                    moduleName = moduleName[1..];
                    ModuleMap.Add(moduleName, new ConjuctionModule());
                } else
                {
                    switch (moduleName)
                    {
                        case "broadcaster":
                            ModuleMap.Add(moduleName, new BroadcasterModule());
                            break;
                    }
                }

                ConnectionMap.Add(moduleName, halves[1].Split(",").Select(l => l.Trim()).ToList());

            }
            var button = new ButtonModule();

            ModuleMap["button"] = button;
            ModuleMap["button"].AddOutput(ModuleMap["broadcaster"]);
            /* set up connections */
            foreach (var kvp in ConnectionMap)
            {
                var moduleName = kvp.Key;

                foreach(var conn in kvp.Value)
                {
                    if (ModuleMap.ContainsKey(conn))
                    {
                        ModuleMap[moduleName].AddOutput(ModuleMap[conn]);
                    }
                    else
                    {
                        ModuleMap[conn] = new OutputModule();
                        ModuleMap[moduleName].AddOutput(ModuleMap[conn]);
                    }
                }
            }
        }

        public override ValueTask<string> Solve_1()
        {
            foreach (var m in ModuleMap.Values)
            {
                m.OnQueueSignal += (Module source, PulseType pulse, Module dest) =>
                {
                    processingQueue.Enqueue((source, pulse, dest));
                };
            }
            var lowPulses = 0;
            var highPulses = 0;
            /* button push */
            for (var x = 0; x < 1000; x++)
            {
                processingQueue.Enqueue((ModuleMap["button"], PulseType.LOW, ModuleMap["broadcaster"]));
                while (processingQueue.Count > 0)
                {
                    (var source, var pulse, var dest) = processingQueue.Dequeue();
                    switch (pulse)
                    {
                        case PulseType.HIGH:
                            highPulses++;
                            break;
                        case PulseType.LOW:
                            lowPulses++;
                            break;
                    }
                    dest.ProcessPulse(source, pulse);
                }
            }
            var initStateStatus = ModuleMap.Values.Select(m => m.IsInInitialState()).ToArray();

            return new((lowPulses * highPulses).ToString());
        }

        public override ValueTask<string> Solve_2()
        {
            foreach (var module in ModuleMap.Values)
            {
                module.Reset();
            }

            var output = ModuleMap.Values.Where(m => m is OutputModule).First();
            var parent = output.Inputs[0];

            var dependencies = parent.Inputs.ToArray();

            ulong[] depNotInitialAt = new ulong[dependencies.Length];
            ulong[] depInitialAgainAt = new ulong[dependencies.Length];

            ulong buttonClickCount = 0;
            while (true)
            {
                buttonClickCount++;

                /* click the button */
                processingQueue.Enqueue((ModuleMap["button"], PulseType.LOW, ModuleMap["broadcaster"]));
                while (processingQueue.Count > 0)
                {
                    (var source, var pulse, var dest) = processingQueue.Dequeue();
                    dest.ProcessPulse(source, pulse);

                    for(var x = 0; x < dependencies.Length; x++)
                    {
                        var state = dependencies[x].IsInInitialState();
                        if (depNotInitialAt[x] == 0 && state == false)
                        {
                            depNotInitialAt[x] = buttonClickCount;
                        }

                        if (depNotInitialAt[x] > 0 && state == true && depInitialAgainAt[x] == 0)
                        {
                            depInitialAgainAt[x] = buttonClickCount;
                        }
                    }
                    
                }

                if (depInitialAgainAt.All(i => i > 0))
                {
                    break;
                }
            }

            /* multiply all depInitialAgainAt values together */
            var answer = depInitialAgainAt.Aggregate((ulong)1, (accum, next) => accum * next);


            return new(answer.ToString());
        }

    }
}
