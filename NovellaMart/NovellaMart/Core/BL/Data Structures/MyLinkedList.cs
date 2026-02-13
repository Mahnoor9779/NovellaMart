using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;


namespace NovellaMart.Core.BL.Data_Structures
{
    public class MyLinkedList<type> : IEnumerable<type>, ICollection<type>
    {
        public MyLinkedListNode<type> head;
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
            MyLinkedListNode<type> currentNode = head;
            while (currentNode != null)
            {
                MyLinkedListNode<type> nextNode = currentNode.Next;
                currentNode = nextNode;
            }
            head = null;
            size = 0;
        }

        public void InsertAtHead(type value)
        {
            MyLinkedListNode<type> newNode = new MyLinkedListNode<type>(value);
            newNode.Next = head;
            head = newNode;
            size = size + 1;
        }

        public void InsertAtEnd(type value)
        {
            MyLinkedListNode<type> newNode = new MyLinkedListNode<type>(value);
            if (head == null)
            {
                head = newNode;
                size = size + 1;
                return;
            }

            MyLinkedListNode<type> tempNode = head;
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

            MyLinkedListNode<type> tempNode = head;
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

            MyLinkedListNode<type> newNode = new MyLinkedListNode<type>(value);
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

            MyLinkedListNode<type> tempNode = head;
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

            MyLinkedListNode<type> tempNode = head;
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
            MyLinkedListNode<type> tempNode = head;
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


        public void Display()
        {
            MyLinkedListNode<type> tempNode = head;
            Console.Write("List elements: ");
            while (tempNode != null)
            {
                Console.Write(tempNode.Data);
                Console.Write(" ");
                tempNode = tempNode.Next;
            }
            Console.WriteLine();
        }

        public bool Remove(type value)
        {
            if (head == null)
            {
                return false;
            }

            if (head.Data.Equals(value))
            {
                head = head.Next;
                size = size - 1;
                return true;
            }

            MyLinkedListNode<type> tempNode = head;
            while (tempNode.Next != null)
            {
                if (tempNode.Next.Data.Equals(value))
                {
                    tempNode.Next = tempNode.Next.Next;
                    size = size - 1;
                    return true;
                }
                tempNode = tempNode.Next;
            }

            return false;
        }

        public IEnumerator<type> GetEnumerator()
        {
            var current = head;
            while (current != null)
            {
                yield return current.Data;
                current = current.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(type item)
        {
            InsertAtEnd(item);
        }

        public bool Contains(type item)
        {
            return FindNode(item);
        }

        public void CopyTo(type[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length - arrayIndex < size) throw new ArgumentException("Insufficient space in destination array.");

            var current = head;
            while (current != null)
            {
                array[arrayIndex++] = current.Data;
                current = current.Next;
            }
        }

        public bool IsReadOnly => false;

        int ICollection<type>.Count => size;
    }
}
