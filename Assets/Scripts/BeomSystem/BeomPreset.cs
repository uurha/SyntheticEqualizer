using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CorePlugin.Attributes.Validation;
using Extensions;
using Grid;
using UnityEngine;

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

[Serializable] 
public struct BeomCells
{
    public ICellEntity[] UnmovableEntities { get; }
    public ICellEntity[] MovableEntities { get; }

    public ICellEntity GetCell(EntityType entityType)
    {
        if (entityType.IsMovable())
        {
            return MovableEntities.FirstOrDefault(x => x.CellID == entityType);
        }

        if (entityType.IsUnMovable())
        {
            return UnmovableEntities.FirstOrDefault(x => x.CellID == entityType);
        }
        return null;
    }
    
    public BeomCells(ICellEntity[] cellEntities)
    {
        UnmovableEntities = cellEntities.Where(x => x.CellID.IsUnMovable()).ToArray();
        MovableEntities = cellEntities.Where(x=> x.CellID.IsMovable()).ToArray();
    }
    
    public BeomCells(IEnumerable<ICellEntity> cellEntities)
    {
        var entities = cellEntities as ICellEntity[] ?? cellEntities.ToArray();
        UnmovableEntities = entities.Where(x => x.CellID.IsUnMovable()).ToArray();
        MovableEntities = entities.Where(x=> x.CellID.IsMovable()).ToArray();
    }
}
