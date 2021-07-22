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
    private ICellEntity[] Entities;

    public ICellEntity GetCell(EntityType entityType, Direction direction)
    {
        return direction switch
               {
                   Direction.In => Entities.FirstOrDefault(x => x.InDirection == entityType),
                   Direction.Out => Entities.FirstOrDefault(x => x.OutDirection == entityType),
                   _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
               };
    }
    
    public ICellEntity GetCell(EntityType inDir, EntityType outDir)
    {
        return Entities.FirstOrDefault(x => x.InDirection == inDir && x.OutDirection == outDir);
    }

    public BeomCells(IEnumerable<ICellEntity> cellEntities)
    {
        Entities = cellEntities as ICellEntity[] ?? cellEntities.ToArray();
    }
}
