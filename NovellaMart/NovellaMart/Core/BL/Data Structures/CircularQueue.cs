using System;

namespace NovellaMart.Core.BL.Data_Structures
{
    public class CircularQueue<type>
    {
        private type[] arr;
        private int front;
        private int rear;
        private int capacity;
        private int size;

        public CircularQueue(int queueCapacity)
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
            front = -1;
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
                Console.WriteLine("Circular Queue Overflow! Cannot enqueue value.");
                return;
            }

            if (front == -1)
            {
                front = 0;
            }

            rear = (rear + 1) % capacity;
            arr[rear] = value;
            size = size + 1;
        }

        public type Dequeue()
        {
            if (IsEmpty())
            {
                Console.WriteLine("Circular Queue Underflow! Returning default value.");
                return default(type);
            }

            type dequeuedValue = arr[front];
            front = (front + 1) % capacity;
            size = size - 1;
            if (size == 0)
            {
                front = -1;
                rear = -1;
            }
            return dequeuedValue;
        }

        public type Peek()
        {
            if (IsEmpty())
            {
                Console.WriteLine("Circular Queue is empty! Returning default value.");
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
                Console.WriteLine("Circular Queue is empty!");
                return;
            }

            Console.Write("Circular Queue elements: ");
            for (int i = 0; i < size; i++)
            {
                int index = (front + i) % capacity;
                Console.Write(arr[index] + " ");
            }
            Console.WriteLine();
        }
    }
}
