using System;
using NovellaMart.Core.BL.Data_Structures;

namespace DSA
{
    public class BinaryTreeGeneric<type>
    {
        public BinaryTreeNodeGeneric<type> Root;

        public BinaryTreeGeneric()
        {
            Root = null;
        }

        public bool IsEmpty()
        {
            return Root == null;
        }

        // Insert in level order
        public void Insert(type val)
        {
            BinaryTreeNodeGeneric<type> newNode = new BinaryTreeNodeGeneric<type>(val);

            if (Root == null)
            {
                Root = newNode;
                return;
            }

            QueueBinaryTreeGeneric<type> queue = new QueueBinaryTreeGeneric<type>();
            queue.Enqueue(Root);

            while (!queue.IsEmpty())
            {
                BinaryTreeNodeGeneric<type> currentNode = queue.Dequeue();

                if (currentNode.Left == null)
                {
                    currentNode.Left = newNode;
                    return;
                }
                else
                {
                    queue.Enqueue(currentNode.Left);
                }

                if (currentNode.Right == null)
                {
                    currentNode.Right = newNode;
                    return;
                }
                else
                {
                    queue.Enqueue(currentNode.Right);
                }
            }
        }

        public void Inorder()
        {
            InorderHelper(Root);
            Console.WriteLine();
        }

        private void InorderHelper(BinaryTreeNodeGeneric<type> node)
        {
            if (node == null)
            {
                return;
            }

            InorderHelper(node.Left);
            Console.Write(node.Data);
            Console.Write(" ");
            InorderHelper(node.Right);
        }

        public void Preorder()
        {
            PreorderHelper(Root);
            Console.WriteLine();
        }

        private void PreorderHelper(BinaryTreeNodeGeneric<type> node)
        {
            if (node == null)
            {
                return;
            }

            Console.Write(node.Data);
            Console.Write(" ");
            PreorderHelper(node.Left);
            PreorderHelper(node.Right);
        }

        public void Postorder()
        {
            PostorderHelper(Root);
            Console.WriteLine();
        }

        private void PostorderHelper(BinaryTreeNodeGeneric<type> node)
        {
            if (node == null)
            {
                return;
            }

            PostorderHelper(node.Left);
            PostorderHelper(node.Right);
            Console.Write(node.Data);
            Console.Write(" ");
        }
    }

    // Simple Queue for Binary Tree Node Generic
    public class QueueBinaryTreeGeneric<type>
    {
        private BinaryTreeNodeGeneric<type>[] data;
        private int front;
        private int rear;
        private int capacity;

        public QueueBinaryTreeGeneric()
        {
            capacity = 500;
            data = new BinaryTreeNodeGeneric<type>[capacity];
            front = 0;
            rear = -1;
        }

        public bool IsEmpty()
        {
            return rear < front;
        }

        public void Enqueue(BinaryTreeNodeGeneric<type> node)
        {
            rear = rear + 1;

            if (rear >= capacity)
            {
                int newCapacity = capacity + 500;
                BinaryTreeNodeGeneric<type>[] newArray = new BinaryTreeNodeGeneric<type>[newCapacity];
                int i = 0;
                while (i < capacity)
                {
                    newArray[i] = data[i];
                    i = i + 1;
                }

                data = newArray;
                capacity = newCapacity;
            }

            data[rear] = node;
        }

        public BinaryTreeNodeGeneric<type> Dequeue()
        {
            if (IsEmpty())
            {
                return null;
            }

            BinaryTreeNodeGeneric<type> val = data[front];
            front = front + 1;
            return val;
        }
    }
}
