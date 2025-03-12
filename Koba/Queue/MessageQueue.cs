using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koba.Queue
{

    public delegate Task OnMessageAddedHandler(object message, Type messageType);

    public class MessageQueue<T>
    {
        public event OnMessageAddedHandler ItemAdded;
        private Queue<T> messages = new();

        public void Push(T message) {
            messages.Enqueue(message);
            ItemAdded.Invoke(message, typeof(T));
        }

        public T Pop() {
            return messages.Dequeue();
        }
    }
}
