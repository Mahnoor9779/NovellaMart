using System;

namespace NovellaMart.Core.BL.Data_Structures
{
    public class StackArray<type>
    {
        private type[] arr;
        private int top;
        private int capacity;

        public StackArray(int stackCapacity)
        {
            if (stackCapacity <= 0)
            {
                capacity = 100;
            }
            else
            {
                capacity = stackCapacity;
            }

            arr = new type[capacity];
            top = -1;
        }

        public bool IsEmpty()
        {
            return top == -1;
        }

        public bool IsFull()
        {
            return top == capacity - 1;
        }

        public int Size()
        {
            return top + 1;
        }

        public void Push(type value)
        {
            if (IsFull())
            {
                Console.WriteLine("Stack Overflow! Cannot push value.");
                return;
            }

            top = top + 1;
            arr[top] = value;
        }

        public type Pop()
        {
            if (IsEmpty())
            {
                Console.WriteLine("Stack Underflow! Returning default value.");
                return default(type);
            }

            type poppedValue = arr[top];
            top = top - 1;
            return poppedValue;
        }

        public type Peek()
        {
            if (IsEmpty())
            {
                Console.WriteLine("Stack is empty! Returning default value.");
                return default(type);
            }

            return arr[top];
        }

        public void Display()
        {
            if (IsEmpty())
            {
                Console.WriteLine("Stack is empty!");
                return;
            }

            Console.Write("Stack elements (top to bottom): ");
            for (int i = top; i >= 0; i--)
            {
                Console.Write(arr[i] + " ");
            }
            Console.WriteLine();
        }
    }
}
