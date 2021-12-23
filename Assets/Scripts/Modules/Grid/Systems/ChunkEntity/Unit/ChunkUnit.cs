using System;
using System.Diagnostics.Contracts;
using CorePlugin.Attributes.EditorAddons;
using CorePlugin.Attributes.EditorAddons.SelectAttributes;
using Extensions;
using Modules.GlobalSettings.Model;
using Modules.Grid.Model;
using Modules.Grid.Systems.ChunkEntity.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Modules.Grid.Systems.ChunkEntity.Unit
{
    [RequireComponent(typeof(MeshRenderer))]
    public class ChunkUnit : MonoBehaviour
    {
        [SerializeReference] [SelectImplementation(typeof(IOrientationOffset))]
        private IOrientationOffset positionOffset;

        private Orientation _initialOrientation;
        private MeshRenderer _meshRenderer;

        public Orientation InitialOrientation => _initialOrientation;

        public ChunkUnit Initialize(IOrientationOffsetParams offsetParams)
        {
            var thisTransform = transform;
            _initialOrientation = new Orientation(thisTransform, true);

            if (offsetParams != null)
            {
                _initialOrientation +=
                    positionOffset?.GetOffsetOrientation(new Orientation(thisTransform), offsetParams) ??
                    new Orientation();
            }
            
            transform.SetOrientation(_initialOrientation);
            _meshRenderer = GetComponent<MeshRenderer>();
            InstantiateMaterials();
            return this;
        }

        public void SetUnitData(ChunkUnitData blockColor)
        {
            var mpb = new MaterialPropertyBlock();
            mpb.SetColor(ShaderExtensions.LowestColor, blockColor.LowestColor);
            mpb.SetColor(ShaderExtensions.MiddleColor, blockColor.MiddleColor);
            mpb.SetColor(ShaderExtensions.MaxColor, blockColor.MaxColor);
            mpb.SetFloat(ShaderExtensions.LowestDelta, blockColor.LowDelta);
            mpb.SetFloat(ShaderExtensions.MiddleDelta, blockColor.MiddleDelta);
            mpb.SetVector(ShaderExtensions.InitialPosition, _initialOrientation.Position);
            _meshRenderer.SetPropertyBlock(mpb);
        }

        [EditorButton]
        private void ResetShaderProperty()
        {
            var mpb = new MaterialPropertyBlock();
            mpb.SetVector(ShaderExtensions.InitialPosition, _initialOrientation.Position);
            _meshRenderer.SetPropertyBlock(mpb);
        }

        private void InstantiateMaterials()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            ResetShaderProperty();
        }
    }
}
