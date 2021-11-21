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
            var mpb = new MaterialPropertyBlock();
            mpb.SetColor(LowestColor, blockColor.LowestColor);
            mpb.SetColor(MiddleColor, blockColor.MiddleColor);
            mpb.SetColor(MaxColor, blockColor.MaxColor);
            mpb.SetFloat(LowestDelta, blockColor.LowDelta);
            mpb.SetFloat(MiddleDelta, blockColor.MiddleDelta);
            mpb.SetVector(InitialPosition, _initialOrientation.Position);
            _meshRenderer.SetPropertyBlock(mpb);
        }

        [EditorButton]
        private void ResetShaderProperty()
        {
            var mpb = new MaterialPropertyBlock();
            mpb.SetVector(InitialPosition, _initialOrientation.Position);
            _meshRenderer.SetPropertyBlock(mpb);
        }

        private void InstantiateMaterials()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            ResetShaderProperty();
        }
    }
}
