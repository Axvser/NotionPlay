namespace NotionPlay.Interfaces
{
    public interface IRecyclable<T> where T : class
    {
        public void Recycle();
    }
}
