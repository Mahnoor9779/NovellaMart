using System;

namespace NovellaMart.Core.BL.Data_Structures
{
    public class SimpleQueue<type>
    {
        private type[] arr;
        private int front;
        private int rear;
        private int capacity;
        private int size;

        public SimpleQueue(int queueCapacity)
        {
            if (queueCapacity <= 0)
            {
                capacity = 100;
            }
            else
            {
                capacity = queueCapacity;
            }

            arr = new type[capacity];
            front = 0;
            rear = -1;
            size = 0;
        }

        public bool IsEmpty()
        {
            return size == 0;
        }

        public bool IsFull()
        {
            return size == capacity;
        }

        public void Enqueue(type value)
        {
            if (IsFull())
            {
                Console.WriteLine("Queue Overflow! Cannot enqueue value.");
                return;
            }

            rear = rear + 1;
            arr[rear] = value;
            size = size + 1;
        }

        public type Dequeue()
        {
            if (IsEmpty())
            {
                Console.WriteLine("Queue Underflow! Returning default value.");
                return default(type);
            }

            type dequeuedValue = arr[front];
            front = front + 1;
            size = size - 1;
            return dequeuedValue;
        }

        public type Peek()
        {
            if (IsEmpty())
            {
                Console.WriteLine("Queue is empty! Returning default value.");
                return default(type);
            }
            return arr[front];
        }

        public int Size()
        {
            return size;
        }

        public void Display()
        {
            if (IsEmpty())
            {
                Console.WriteLine("Queue is empty!");
                return;
            }

            Console.Write("Queue elements: ");
            for (int i = 0; i < size; i++)
            {
                Console.Write(arr[front + i] + " ");
            }
            Console.WriteLine();
        }
    }
}
