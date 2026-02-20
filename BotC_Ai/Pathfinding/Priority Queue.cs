using System.Collections.Generic;
//using Codice.CM.Common;
//using PlasticPipe.PlasticProtocol.Messages;

namespace utils
{
    public class Priority_Queue<T>
    {
        
      private QueueNode<T> head;

        public void enqueue(int priority, T value)
        {
            QueueNode<T> nodeToInsert = new QueueNode<T>(priority, value, null);
            QueueNode<T> current = head;

            if (current == null)
            {
                head = nodeToInsert;
                return;
            }
            if (head.priority > priority)
            {
                nodeToInsert.next = head;
                head = nodeToInsert;
                return;
            }

            while (current.hasNext() && current.priority < nodeToInsert.priority)
            {
                current = current.next;
            }

            nodeToInsert.next = current.next;
            current.next = nodeToInsert;
        }


        public static List<T> asList(Priority_Queue<T> queue)
        {
            List<T> priorityList = new List<T>();
            while (!queue.isEmpty())
            {
                priorityList.Add(queue.dequeue());
            }
            return priorityList;
        }

        public T dequeue()
        {
            T temp = head.value;
            head = head.next;
            return temp;
        }

        public bool isEmpty()
        {
            return head == null;
        }

        public bool contains(T node)
        {
            if (this.isEmpty()) return false;
            QueueNode<T> current = head;
            while (!(current.next == null))
            {
                if (current.value.Equals(node)) return true;
                current = current.next;
            }

            return false;
        }

        public void updatePriority(T node, int priority)
        {
            var current = head;
            while (!current.value.Equals(node))
            {
                current = current.next;
            }
 
            var prev = head;
            while (!prev.next.Equals(current))
            {
                prev = prev.next;
            }

            prev.next = current.next;
            enqueue(priority,current.value);

        }

    }
    
    public class QueueNode<T>
    {
        public int priority;
        public T value;
        public QueueNode<T> next;
        public bool hasNext()
        {
            return next != null;
        }
        
        public QueueNode(int priority, T value, QueueNode<T> next)
        {
            this.priority = priority;
            this.next = next;
            this.value = value;
        }
    }
    
}