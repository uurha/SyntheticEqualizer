namespace Cell.CellItem.Interfaces
{
    public interface ICellItem
    {
        public void RunBehaviour();
        public void RunBehaviour(IItemData data);
    }
}
