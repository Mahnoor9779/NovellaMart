namespace NovellaMart.Core.BL.Data_Structures
{
    public class HeapPriorityQueue<type>
    {
        private (type Data, long Priority)[] heap;
        private int size;
        private int capacity;

        public HeapPriorityQueue(int initialCapacity = 100)
        {
            capacity = initialCapacity;
            heap = new (type, long)[capacity];
            size = 0;
        }

        public void Enqueue(type item, long priority)
        {
            if (size == capacity) Resize();

            // For Flash Sales, we treat the EARLIEST time as the HIGHEST priority.
            // We store the negative of ticks to use Max-Heap logic as a Min-Heap.
            heap[size] = (item, -priority);
            BubbleUp(size);
            size++;
        }

        private void BubbleUp(int index)
        {
            while (index > 0)
            {
                int parent = (index - 1) / 2;
                if (heap[index].Priority <= heap[parent].Priority) break;

                Swap(index, parent);
                index = parent;
            }
        }

        public type Dequeue()
        {
            if (size == 0) return default(type);

            type result = heap[0].Data;
            heap[0] = heap[size - 1];
            size--;
            BubbleDown(0);
            return result;
        }

        private void BubbleDown(int index)
        {
            while (true)
            {
                int left = 2 * index + 1;
                int right = 2 * index + 2;
                int largest = index;

                if (left < size && heap[left].Priority > heap[largest].Priority) largest = left;
                if (right < size && heap[right].Priority > heap[largest].Priority) largest = right;

                if (largest == index) break;

                Swap(index, largest);
                index = largest;
            }
        }

        private void Swap(int i, int j) => (heap[i], heap[j]) = (heap[j], heap[i]);

        private void Resize()
        {
            capacity *= 2;
            var newHeap = new (type, long)[capacity];
            Array.Copy(heap, newHeap, size);
            heap = newHeap;
        }

        public bool IsEmpty() => size == 0;
    }
}