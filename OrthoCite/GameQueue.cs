using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrthoCite
{
    public class GameQueue
    {
        readonly Queue<string[]> _messages;
        readonly object _lock;

        public GameQueue()
        {
            _messages = new Queue<string[]>();
            _lock = new object();
        }

        public void Push(string[] message)
        {
            lock (_lock)
            {
                _messages.Enqueue(message);
            }
        }

        public string[] Pull()
        {
            lock (_lock)
            {
                if (_messages.Count == 0) return null;
                return _messages.Dequeue();
            }
        }
    }
}
