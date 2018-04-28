using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace Sounds_Packing
{
    class mergeSort
    {
        static public int count = 0;

        public static void MergeSort(int[] input, int low, int high)
        {
            {
                if (low >= high)
                {
                    return;
                }
                int mid = (low + high) >> 1;
                if (count < 4)
                {
                    count += 2;
                    Thread t1 = new Thread(() => MergeSort(input, low, mid));
                    t1.Start();
                    Thread t2 = new Thread(() => MergeSort(input, mid + 1, high));
                    t2.Start();
                    t1.Abort();
                    t2.Abort();
                    count -= 2;
                }
                else
                {
                    MergeSort(input, low, mid);
                    MergeSort(input, mid + 1, high);
                }

                Merge(input, low, mid, high);
            }
        }

        public static void MergeSort(int[] input)
        {
            MergeSort(input, 0, input.Length - 1);  //complexity:O(1)
        }

        private static void Merge(int[] input, int low, int middle, int high)
        {

            int left = low;   //complexity:O(1)
            int right = middle + 1; //complexity:O(1)
            int[] tmp = new int[(high - low) + 1];//complexity:O(1)
            int tmpIndex = 0; //complexity:O(1)

            while ((left <= middle) && (right <= high))//complexity:O(n/2)
            {
                if (input[left] > input[right])
                {
                    tmp[tmpIndex] = input[left];  //complexity:O(1)
                    left = left + 1;  //complexity:O(1)
                }
                else
                {
                    tmp[tmpIndex] = input[right]; //complexity:O(1)
                    right = right + 1; //complexity:O(1)
                }
                tmpIndex = tmpIndex + 1; //complexity:O(1)
            }

            if (left <= middle)
            {
                while (left <= middle) //complexity:O(n/2)
                {
                    tmp[tmpIndex] = input[left]; //complexity:O(1)
                    left = left + 1; //complexity:O(1)
                    tmpIndex = tmpIndex + 1; //complexity:O(1)
                }
            }

            if (right <= high) //complexity:O(1)
            {
                while (right <= high) //complexity:O(n/2)
                {
                    tmp[tmpIndex] = input[right]; //complexity:O(1)
                    right = right + 1; //complexity:O(1)
                    tmpIndex = tmpIndex + 1; //complexity:O(1)
                }
            }

            for (int i = 0; i < tmp.Length; i++) //complexity:O(n)
            {
                input[low + i] = tmp[i]; //complexity:O(1)
            }

        }


        public static void MergeSortWithout(int[] input, int low, int high)
        {
            if (low >= high)        //complexity:O(1)
            {
                return;
            }
            int mid = (low + high) >> 1;  //complexity:O(1)
            MergeSortWithout(input, low, mid); //complexity:O(1),  recurrence: T(n)
            MergeSortWithout(input, mid + 1, high); //complexity:O(1),  recurrence: T(n)
            MergeWithout(input, low, mid, high); //complexity:O(1)
        }

        public static void MergeSortWithout(int[] input)
        {
            MergeSortWithout(input, 0, input.Count() - 1);  //complexity:O(1)
        }

        private static void MergeWithout(int[] input, int low, int middle, int high)
        {

            int left = low;   //complexity:O(1)
            int right = middle + 1; //complexity:O(1)
            int[] tmp = new int[(high - low) + 1];//complexity:O(1)
            int tmpIndex = 0; //complexity:O(1)

            while ((left <= middle) && (right <= high))//complexity:O(n/2)
            {
                if (input[left] > input[right])
                {
                    tmp[tmpIndex] = input[left];  //complexity:O(1)
                    left = left + 1;  //complexity:O(1)
                }
                else
                {
                    tmp[tmpIndex] = input[right]; //complexity:O(1)
                    right = right + 1; //complexity:O(1)
                }
                tmpIndex = tmpIndex + 1; //complexity:O(1)
            }

            if (left <= middle)
            {
                while (left <= middle) //complexity:O(n/2)
                {
                    tmp[tmpIndex] = input[left]; //complexity:O(1)
                    left = left + 1; //complexity:O(1)
                    tmpIndex = tmpIndex + 1; //complexity:O(1)
                }
            }

            if (right <= high) //complexity:O(1)
            {
                while (right <= high) //complexity:O(n/2)
                {
                    tmp[tmpIndex] = input[right]; //complexity:O(1)
                    right = right + 1; //complexity:O(1)
                    tmpIndex = tmpIndex + 1; //complexity:O(1)
                }
            }

            for (int i = 0; i < tmp.Length; i++) //complexity:O(n)
            {
                input[low + i] = tmp[i]; //complexity:O(1)
            }


        }
    }
}