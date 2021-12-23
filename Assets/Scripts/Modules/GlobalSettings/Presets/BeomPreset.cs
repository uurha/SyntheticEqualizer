using System.Collections.Generic;
using System.Linq;
using CorePlugin.Attributes.Validation;
using Modules.Grid.Interfaces;
using SubModules.Beom;
using UnityEngine;

namespace Modules.GlobalSettings.Presets
{
    [CreateAssetMenu(menuName = "Presets/Beom Preset", fileName = "BeomPreset", order = 0)]
    public class BeomPreset : ScriptableObject
    {
        [HasComponent(typeof(IChunkComponent))]
        [SerializeField] private List<GameObject> beomCells;

        public BeomCells GetBeomEntities()
        {
            return new BeomCells(beomCells.Select(x => x.GetComponent<IChunkComponent>().Initialize()));
        }
    }
}
