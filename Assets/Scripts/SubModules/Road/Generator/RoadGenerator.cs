using System;
using System.Linq;
using System.Threading.Tasks;
using Base.Deque;
using Extensions;
using SubModules.Cell.Model;
using UnityEngine;
using Random = System.Random;
using TaskExtensions = CorePlugin.Extensions.TaskExtensions;

namespace SubModules.Road.Generator
{
    public class RoadGenerator
    {
        private readonly int _lenght;
        private readonly Random _random;
        private readonly Deque<RoadDirection> _roadDirections;
        private readonly Deque<RoadDirection> _turns;
        private int _seed;

        public RoadGenerator(int lenght, int seed = 0)
        {
            _lenght = lenght;
            _seed = seed == 0 ? Environment.TickCount : seed;
            _random = new Random(_seed);
            _roadDirections = new Deque<RoadDirection>();
            _turns = new Deque<RoadDirection>();
        }

        public Task<Deque<RoadDirection>> GeneratePathAsync()
        {
            return TaskExtensions.CreateTask(GeneratePath);
        }

        public Deque<RoadDirection> GeneratePath()
        {
            var prevStep = RoadDirection.None;

            for (var i = 0; i < _lenght; i++)
            {
                if (i <= 3)
                {
                    _roadDirections.AddFirst(RoadDirection.North);

                    if (i == 3)
                    {
                        _turns.AddFirst(RoadDirection.North);
                        prevStep = RoadDirection.North;
                    }
                    continue;
                }
                var bufferStep = GetValidRoad(prevStep);
                _roadDirections.AddFirst(bufferStep);
                prevStep = bufferStep;
            }
            Debug.Log($"{nameof(_seed)}: {_seed} {nameof(_turns.Count)}: {_turns.Count} Sum: {_turns.Sum(x => (int) x)}");
            return _roadDirections;
        }

        private RoadDirection GetValidRoad(RoadDirection prevStep)
        {
            RoadDirection bufferStep;
            var breakCount = 0;

            while (true)
            {
                breakCount++;
                if (breakCount >= 1000) throw new Exception();
                bufferStep = RandomDirection(prevStep.Negative());

                if (bufferStep != prevStep)
                {
                    if (ValidateLastTurns(_turns, out var prohibitedRoad))
                        if (prohibitedRoad == bufferStep)
                            continue;

                    if (_roadDirections.LastDiffer() > 4)
                    {
                        var count = _turns.Count(x => x == bufferStep);
                        var intPr = _turns.Count % 2 == 0 ? _turns.Count / 2 : 2;

                        if (count > 0 &&
                            count % intPr == 0)
                            continue;
                    }
                    _turns.AddFirst(bufferStep);
                }
                break;
            }
            return bufferStep;
        }

        private bool ValidateLastTurns(Deque<RoadDirection> turns, out RoadDirection prohibitedRoad)
        {
            if (turns.Count < 3)
            {
                prohibitedRoad = RoadDirection.None;
                return false;
            }
            var array = turns.ToList().GetRange(0, 3);

            switch (array[0], array[1], array[2])
            {
                case (RoadDirection.North, RoadDirection.West, RoadDirection.South):
                    prohibitedRoad = RoadDirection.East;
                    return true;
                case (RoadDirection.South, RoadDirection.West, RoadDirection.North):
                    prohibitedRoad = RoadDirection.East;
                    return true;
                case (RoadDirection.North, RoadDirection.East, RoadDirection.South):
                    prohibitedRoad = RoadDirection.West;
                    return true;
                case (RoadDirection.South, RoadDirection.East, RoadDirection.North):
                    prohibitedRoad = RoadDirection.West;
                    return true;
                case (RoadDirection.East, RoadDirection.South, RoadDirection.West):
                    prohibitedRoad = RoadDirection.North;
                    return true;
                case (RoadDirection.East, RoadDirection.North, RoadDirection.West):
                    prohibitedRoad = RoadDirection.South;
                    return true;
                case (RoadDirection.West, RoadDirection.South, RoadDirection.East):
                    prohibitedRoad = RoadDirection.North;
                    return true;
                case (RoadDirection.West, RoadDirection.North, RoadDirection.East):
                    prohibitedRoad = RoadDirection.South;
                    return true;
            }
            prohibitedRoad = RoadDirection.None;
            return false;
        }

        private RoadDirection RandomDirection(RoadDirection exclude = RoadDirection.None)
        {
            var except = new[] {RoadDirection.None, exclude};
            var values = (Enum.GetValues(typeof(RoadDirection)) as RoadDirection[])!.Except(except).ToArray();
            return values[_random.Next(0, values.Length)];
        }
    }
}
