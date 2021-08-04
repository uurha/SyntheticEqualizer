using CorePlugin.Attributes.Headers;
using Extensions;
using UnityEngine;

namespace Grid
{
    [RequireComponent(typeof(GridInitializer))]
    public class GridCullingMask : MonoBehaviour
    {
        [ReferencesHeader]
        [SerializeField] private GridInitializer gridInitializer;

        [SettingsHeader]
        [SerializeField] private Vector3 cullingStart;
        [SerializeField] private Vector3 cullingEnd;

        private void Update()
        {
            if (gridInitializer.IsInitialized) CheckMask();
        }

        private void CheckMask()
        {
            var buffer = gridInitializer.InstancedGrid;

            buffer.CalculateCellInBound(position => position.Between(cullingStart, cullingEnd));
        }
    }
}
