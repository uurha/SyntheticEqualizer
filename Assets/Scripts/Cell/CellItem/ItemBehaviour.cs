namespace Cell.CellItem.Interfaces
{
    public abstract class ItemBehaviour
    {
        protected ICellItem _item;
        
        public ItemBehaviour(ICellItem item)
        {
            _item = item;
        }

        public abstract void Execute(IItemData data);
    }
}