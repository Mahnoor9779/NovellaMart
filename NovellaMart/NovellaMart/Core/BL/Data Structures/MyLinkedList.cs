using System;
using System.Drawing;

namespace NovellaMart.Core.BL.Data_Structures
{
    public class MyLinkedList<type>
    {
        public LinkedListNode<type> head;
        private int size;

        public MyLinkedList()
        {
            head = null;
            size = 0;
        }

        public bool IsEmpty()
        {
            return head == null;
        }

        public void Clear()
        {
            LinkedListNode<type> currentNode = head;
            while (currentNode != null)
            {
                LinkedListNode<type> nextNode = currentNode.Next;
                currentNode = nextNode;
            }
            head = null;
        }

        public void InsertAtHead(type value)
        {
            LinkedListNode<type> newNode = new LinkedListNode<type>(value);
            newNode.Next = head;
            head = newNode;
            size = size + 1;
        }

        public void InsertAtEnd(type value)
        {
            LinkedListNode<type> newNode = new LinkedListNode<type>(value);
            if (head == null)
            {
                head = newNode;
                size = size + 1;
                return;
            }

            LinkedListNode<type> tempNode = head;
            while (tempNode.Next != null)
            {
                tempNode = tempNode.Next;
            }
            tempNode.Next = newNode;
            size = size + 1;
        }

        public bool InsertAtIndex(int index, type value)
        {
            if (index < 0)
            {
                return false;
            }

            if (index == 0)
            {
                InsertAtHead(value);
                return true;
            }

            LinkedListNode<type> tempNode = head;
            int count = 0;
            while (tempNode != null && count < index - 1)
            {
                tempNode = tempNode.Next;
                count = count + 1;
            }

            if (tempNode == null)
            {
                return false;
            }

            LinkedListNode<type> newNode = new LinkedListNode<type>(value);
            newNode.Next = tempNode.Next;
            tempNode.Next = newNode;
            return true;
        }

        public bool DeleteFromStart()
        {
            if (head == null)
            {
                return false;
            }

            head = head.Next;
            return true;
        }

        public bool DeleteFromEnd()
        {
            if (head == null)
            {
                return false;
            }

            if (head.Next == null)
            {
                head = null;
                return true;
            }

            LinkedListNode<type> tempNode = head;
            while (tempNode.Next.Next != null)
            {
                tempNode = tempNode.Next;
            }

            tempNode.Next = null;
            return true;
        }

        public bool DeleteAtIndex(int index)
        {
            if (index < 0 || index >= size || head == null)
            {
                return false;
            }

            if (index == 0)
            {
                head = head.Next;
                size = size - 1;
                return true;
            }

            LinkedListNode<type> tempNode = head;
            int count = 0;
            while (tempNode != null && count < index - 1)
            {
                tempNode = tempNode.Next;
                count = count + 1;
            }

            if (tempNode.Next == null)
            {
                return false;
            }

            tempNode.Next = tempNode.Next.Next;
            size = size - 1;
            return true;
        }

        public bool FindNode(type value)
        {
            LinkedListNode<type> tempNode = head;
            while (tempNode != null)
            {
                if (tempNode.Data.Equals(value))
                {
                    return true;
                }
                tempNode = tempNode.Next;
            }
            return false;
        }

        public int Count()
        {
            return size;
        }

        //public int Count()
        //{
        //    int count = 0;
        //    LinkedListNode<type> tempNode = head;
        //    while (tempNode != null)
        //    {
        //        count = count + 1;
        //        tempNode = tempNode.Next;
        //    }
        //    return count;
        //}

        public void Display()
        {
            LinkedListNode<type> tempNode = head;
            Console.Write("List elements: ");
            while (tempNode != null)
            {
                Console.Write(tempNode.Data);
                Console.Write(" ");
                tempNode = tempNode.Next;
            }
            Console.WriteLine();
        }
    }
}
