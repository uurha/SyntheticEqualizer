using CorePlugin.Attributes.EditorAddons;
using CorePlugin.Attributes.Validation;
using CorePlugin.Logger;
using Modules.Grid.Model;
using UnityEngine;

namespace Modules.Grid.Systems.CellEntity.Unit
{
    [RequireComponent(typeof(MeshRenderer))]
    public class CellUnit : MonoBehaviour
    {
        [NotNull] [SerializeField] private Material materialPrefab;

        private Orientation _initialOrientation;
        private Material[] _preparedMaterials;
        private MeshRenderer _meshRenderer;
        private static readonly int InitialPosition = Shader.PropertyToID("_InitialPosition");

        public Orientation InitialOrientation => _initialOrientation;

        public CellUnit Initialize()
        {
            _initialOrientation = new Orientation(transform, true);
            _meshRenderer = GetComponent<MeshRenderer>();
            InstantiateMaterials();
            return this;
        }

        [EditorButton]
        private void ResetShaderProperty()
        {
            var count = _meshRenderer.materials.Length;
            for (var i = 0; i < count; i++)
            {
                _preparedMaterials[i].SetVector(InitialPosition, transform.position);
            }
            _meshRenderer.materials = _preparedMaterials;
        }

        private void InstantiateMaterials()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            var count = _meshRenderer.materials.Length;
            _preparedMaterials = new Material[count];

            for (var i = 0; i < count; i++)
            {
                var preparedMaterial = Instantiate(materialPrefab);
                _preparedMaterials[i] = preparedMaterial;
            }
            ResetShaderProperty();
        }
    }
}
