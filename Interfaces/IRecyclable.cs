using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotionPlay.Interfaces
{
    public interface IRecyclable<T> where T : class
    {
        public void Recycle();
    }
}
