using System;
using UnityEngine;

namespace Cell.CellItem.Interfaces
{
    public class CellItem : MonoBehaviour, ICellItem
    {
        private ItemBehaviour _itemBehaviour;
        private IItemData _previousData;
        
        public void RunBehaviour()
        {
            throw new NotImplementedException();
        }

        public void RunBehaviour(IItemData data)
        {
            if(_previousData == data) return;
            _previousData = data;
            _itemBehaviour.Execute(data);
        }
    }
}
