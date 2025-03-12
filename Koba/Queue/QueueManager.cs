using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koba.Queue
{
/*
    public class QueueManager<T> 
    {
        private readonly Dictionary<string ,MessageQueue<T>> messageQueues = new();

        public MessageQueue<T> this[string key] {
            get {
                return GetQueue(key);
            }
        }

        public void CreateQueue(string name) {
            if(!messageQueues.TryAdd(name, new MessageQueue<T>())){
                throw new InvalidOperationException("Já existe uma fila com este indíce");
            }
        }

        public MessageQueue<T> GetQueue(string name) {
            MessageQueue<T> requestedQueue;
            bool queueExists = messageQueues.TryGetValue(name, out requestedQueue);
            if (queueExists) {
                return requestedQueue;
            } else {
                throw new IndexOutOfRangeException("A fila solicitada não existe");
            }
        }
    }
*/

    public class QueueManager
    {
        private readonly Dictionary<string, object> messageQueues = new();

        public void CreateQueue<T>(string name) {
            if (!messageQueues.TryAdd(name, new MessageQueue<T>())) {
                throw new InvalidOperationException("Já existe uma fila com este indíce");
            }
        }

        public MessageQueue<T> GetQueue<T>(string name) {
            object unknowQueue;
            if (messageQueues.TryGetValue(name, out unknowQueue)) {
                if (typeof(MessageQueue<T>) != typeof(object)) {
                    MessageQueue<T> requestedQueue = (MessageQueue<T>)unknowQueue;
                    return requestedQueue;
                } else {
                    throw new InvalidOperationException($"O tipo especifícado para a fila \"{name}\" está incorreto.");
                }
            } else {
                throw new IndexOutOfRangeException("A fila solicitada não existe");
            }
        }
    }


}
