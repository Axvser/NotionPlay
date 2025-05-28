using NotionPlay.Interfaces;

namespace NotionPlay.Tools
{
    public class ObjectPool<T> where T : class, IRecyclable<T>
    {
        private readonly Queue<T> _pool = [];

        public void Initialize(int count)
        {
            for (int i = 0; i < count; i++)
            {
                _pool.Enqueue(Activator.CreateInstance<T>());
            }
        }

        public T Get()
        {
            if (_pool.Count == 0)
            {
                return Activator.CreateInstance<T>();
            }
            return _pool.Dequeue();
        }

        public void Return(T item)
        {
            if (item != null)
            {
                item.Recycle();
                _pool.Enqueue(item);
            }
        }
    }
}
