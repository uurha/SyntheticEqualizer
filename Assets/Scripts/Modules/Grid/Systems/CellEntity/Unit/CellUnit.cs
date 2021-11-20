using CorePlugin.Attributes.EditorAddons;
using CorePlugin.Attributes.Validation;
using Modules.GlobalSettings.Model;
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
        private static readonly int LowestColor = Shader.PropertyToID("_LowestColor");
        private static readonly int MiddleColor = Shader.PropertyToID("_MiddleColor");
        private static readonly int MaxColor = Shader.PropertyToID("_MaxColor");
        private static readonly int LowestDelta = Shader.PropertyToID("_LowestDelta");
        private static readonly int MiddleDelta = Shader.PropertyToID("_MiddleDelta");

        public Orientation InitialOrientation => _initialOrientation;

        public CellUnit Initialize()
        {
            _initialOrientation = new Orientation(transform, true);
            _meshRenderer = GetComponent<MeshRenderer>();
            InstantiateMaterials();
            return this;
        }

        public void SetCellUnitData(CellUnitData blockColor)
        {
            var count = _meshRenderer.materials.Length;

            for (var i = 0; i < count; i++)
            {
                _preparedMaterials[i].SetColor(LowestColor, blockColor.LowestColor);
                _preparedMaterials[i].SetColor(MiddleColor, blockColor.MiddleColor);
                _preparedMaterials[i].SetColor(MaxColor, blockColor.MaxColor);
                _preparedMaterials[i].SetFloat(LowestDelta, blockColor.LowDelta);
                _preparedMaterials[i].SetFloat(MiddleDelta, blockColor.MiddleDelta);
            }
            _meshRenderer.materials = _preparedMaterials;
        }

        [EditorButton]
        private void ResetShaderProperty()
        {
            var count = _meshRenderer.materials.Length;

            for (var i = 0; i < count; i++) _preparedMaterials[i].SetVector(InitialPosition, _initialOrientation.Position);
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
