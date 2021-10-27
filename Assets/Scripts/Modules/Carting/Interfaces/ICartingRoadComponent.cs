using Modules.Grid.Model;

namespace Modules.Carting.Interfaces
{
    public interface ICartingRoadComponent
    {
        public RoadDirection InDirection { get; }

        public RoadDirection OutDirection { get; }
    }
}
