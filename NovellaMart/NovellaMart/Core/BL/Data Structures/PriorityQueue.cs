using System;

namespace NovellaMart.Core.BL.Data_Structures
{
    public class PriorityQueue<type>
    {
        private PriorityQueueNode<type> front;
        private int size;

        public PriorityQueue()
        {
            front = null;
            size = 0;
        }

        public bool IsEmpty()
        {
            return size == 0;
        }

        public int Size()
        {
            return size;
        }

        public void Enqueue(type value, int priority)
        {
            PriorityQueueNode<type> newNode = new PriorityQueueNode<type>(value, priority);

            if (front == null || priority > front.Priority)
            {
                newNode.Next = front;
                front = newNode;
            }
            else
            {
                PriorityQueueNode<type> tempNode = front;
                while (tempNode.Next != null && tempNode.Next.Priority >= priority)
                {
                    tempNode = tempNode.Next;
                }

                newNode.Next = tempNode.Next;
                tempNode.Next = newNode;
            }

            size = size + 1;
        }

        public type Dequeue()
        {
            if (IsEmpty())
            {
                Console.WriteLine("Priority Queue is empty! Returning default value.");
                return default(type);
            }

            type dequeuedValue = front.Data;
            front = front.Next;
            size = size - 1;
            return dequeuedValue;
        }

        public type Peek()
        {
            if (IsEmpty())
            {
                Console.WriteLine("Priority Queue is empty! Returning default value.");
                return default(type);
            }
            return front.Data;
        }

        public void Display()
        {
            if (IsEmpty())
            {
                Console.WriteLine("Priority Queue is empty!");
                return;
            }

            Console.Write("Priority Queue elements: ");
            PriorityQueueNode<type> tempNode = front;
            while (tempNode != null)
            {
                Console.Write(tempNode.Data + "(" + tempNode.Priority + ") ");
                tempNode = tempNode.Next;
            }
            Console.WriteLine();
        }

        public PriorityQueue<type> Clone()
        {
            PriorityQueue<type> copy = new PriorityQueue<type>();

            PriorityQueueNode<type> current = front;

            while (current != null)
            {
                copy.Enqueue(current.Data, current.Priority);
                current = current.Next;
            }

            return copy;
        }
    }

    
}
