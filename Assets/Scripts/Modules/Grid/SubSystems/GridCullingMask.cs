using CorePlugin.Attributes.Headers;
using Extensions;
using UnityEngine;

namespace Modules.Grid.SubSystems
{
    [RequireComponent(typeof(GridInitializer.GridInitializer))]
    public class GridCullingMask : MonoBehaviour
    {
        [SerializeField] private Vector3 cullingEnd;

        [SettingsHeader]
        [SerializeField] private Vector3 cullingStart;

        [ReferencesHeader]
        [SerializeField] private GridInitializer.GridInitializer gridInitializer;

        private void Update()
        {
            if (gridInitializer.IsInitialized) CheckMask();
        }

        private void CheckMask()
        {
            var buffer = gridInitializer.InstancedGrids;

            foreach (var gridConfiguration in buffer)
                gridConfiguration.CalculateCellInBound(position => position.Between(cullingStart, cullingEnd));
        }
    }
}
