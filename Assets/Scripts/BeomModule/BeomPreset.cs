using System.Collections.Generic;
using System.Linq;
using CellModule.Interfaces;
using CorePlugin.Attributes.Validation;
using UnityEngine;

namespace BeomModule
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