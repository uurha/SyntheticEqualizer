using System.Collections.Generic;
using System.Linq;
using CorePlugin.Attributes.Validation;
using SubModules.Cell.Interfaces;
using UnityEngine;

namespace SubModules.Beom
{
    [CreateAssetMenu(menuName = "Beom System/Create Beom Preset", fileName = "BeomPreset", order = 0)]
    public class BeomPreset : ScriptableObject
    {
        [HasComponent(typeof(ICellEntity))]
        [SerializeField] private List<GameObject> beomCells;

        public BeomCells GetBeomEntities()
        {
            return new BeomCells(beomCells.Select(x => x.GetComponent<ICellEntity>()));
        }
    }
}
