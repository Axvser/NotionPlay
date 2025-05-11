using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotionPlay.Interfaces
{
    public interface ILockableNode
    {
        public bool IsLocked { get; set; }
    }
}
