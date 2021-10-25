using System;
using System.Linq;
using System.Threading.Tasks;
using Base.Deque;
using Extensions;
using SubModules.Cell.Model;
using UnityEngine;
using Random = System.Random;
using TaskExtensions = CorePlugin.Extensions.TaskExtensions;

namespace SubModules.Route.Generator
{
    public class RouteGenerator
    {
        private readonly int _lenght;
        private readonly Random _random;
        private readonly Deque<EntityRoute> _routes;
        private readonly Deque<EntityRoute> _turns;
        private int _seed;

        public RouteGenerator(int lenght, int seed = 0)
        {
            _lenght = lenght;
            _seed = seed == 0 ? Environment.TickCount : seed;
            _random = new Random(_seed);
            _routes = new Deque<EntityRoute>();
            _turns = new Deque<EntityRoute>();
        }

        public Task<Deque<EntityRoute>> GeneratePathAsync()
        {
            return TaskExtensions.CreateTask(GeneratePath);
        }

        public Deque<EntityRoute> GeneratePath()
        {
            var prevStep = EntityRoute.None;

            for (var i = 0; i < _lenght; i++)
            {
                if (i <= 3)
                {
                    _routes.AddFirst(EntityRoute.North);

                    if (i == 3)
                    {
                        _turns.AddFirst(EntityRoute.North);
                        prevStep = EntityRoute.North;
                    }
                    continue;
                }
                var bufferStep = GetValidRoute(prevStep);
                _routes.AddFirst(bufferStep);
                prevStep = bufferStep;
            }
            Debug.Log($"{nameof(_seed)}: {_seed} {nameof(_turns.Count)}: {_turns.Count} Sum: {_turns.Sum(x => (int) x)}");
            return _routes;
        }

        private EntityRoute GetValidRoute(EntityRoute prevStep)
        {
            EntityRoute bufferStep;
            var breakCount = 0;

            while (true)
            {
                breakCount++;
                if (breakCount >= 1000) throw new Exception();
                bufferStep = RandomDirection(prevStep.Negative());

                if (bufferStep != prevStep)
                {
                    if (ValidateLastTurns(_turns, out var prohibitedRoute))
                        if (prohibitedRoute == bufferStep)
                            continue;

                    if (_routes.LastDiffer() > 4)
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

        private bool ValidateLastTurns(Deque<EntityRoute> turns, out EntityRoute prohibitedRoute)
        {
            if (turns.Count < 3)
            {
                prohibitedRoute = EntityRoute.None;
                return false;
            }
            var array = turns.ToList().GetRange(0, 3);

            switch (array[0], array[1], array[2])
            {
                case (EntityRoute.North, EntityRoute.West, EntityRoute.South):
                    prohibitedRoute = EntityRoute.East;
                    return true;
                case (EntityRoute.South, EntityRoute.West, EntityRoute.North):
                    prohibitedRoute = EntityRoute.East;
                    return true;
                case (EntityRoute.North, EntityRoute.East, EntityRoute.South):
                    prohibitedRoute = EntityRoute.West;
                    return true;
                case (EntityRoute.South, EntityRoute.East, EntityRoute.North):
                    prohibitedRoute = EntityRoute.West;
                    return true;
                case (EntityRoute.East, EntityRoute.South, EntityRoute.West):
                    prohibitedRoute = EntityRoute.North;
                    return true;
                case (EntityRoute.East, EntityRoute.North, EntityRoute.West):
                    prohibitedRoute = EntityRoute.South;
                    return true;
                case (EntityRoute.West, EntityRoute.South, EntityRoute.East):
                    prohibitedRoute = EntityRoute.North;
                    return true;
                case (EntityRoute.West, EntityRoute.North, EntityRoute.East):
                    prohibitedRoute = EntityRoute.South;
                    return true;
            }
            prohibitedRoute = EntityRoute.None;
            return false;
        }

        private EntityRoute RandomDirection(EntityRoute exclude = EntityRoute.None)
        {
            var except = new[] {EntityRoute.None, exclude};
            var values = (Enum.GetValues(typeof(EntityRoute)) as EntityRoute[])!.Except(except).ToArray();
            return values[_random.Next(0, values.Length)];
        }
    }
}
