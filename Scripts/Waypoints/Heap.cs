using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Used by NodeGraph as part of path-calculation*/
public class Heap<T> where T : IHeapItem<T>
{
    T[] Items;
    int ItemCount;

    public Heap(int maxSize)
    {
        Items = new T[maxSize];
    }

    public void Add(int id, T item)
    {
        item.SetHeapIndex(id, ItemCount);
        Items[ItemCount] = item;
        SortUp(id, item);
        ItemCount++;
    }

    public T RemoveFirst(int id)
    {
        T firstItem = Items[0];
        ItemCount--;
        Items[0] = Items[ItemCount];
        Items[0].SetHeapIndex(id, 0);
        SortDown(id, Items[0]);
        return firstItem;
    }

    public void UpdateItem(int id, T item)
    {
        SortUp(id, item);
    }

    void SortUp(int id, T item)
    {
        int parentIndex = (item.GetHeapIndex(id) - 1) / 2;

        while (true)
        {
            T parentItem = Items[parentIndex];
            if (item.CompareTo(id, parentItem) > 0)
            {
                Swap(id, item, parentItem);
            }
            else
            {
                break;
            }
        }
    }

    void SortDown(int id, T item)
    {
        while (true)
        {
            int childIndexLeft = item.GetHeapIndex(id) * 2 + 1;
            int childIndexRight = item.GetHeapIndex(id) * 2 + 2;
            int swapIndex = 0;

            if (childIndexLeft < ItemCount)
            {
                swapIndex = childIndexLeft;

                if (childIndexRight < ItemCount)
                {
                    if (Items[childIndexLeft].CompareTo(id, Items[childIndexRight]) < 0)
                    {
                        swapIndex = childIndexRight;
                    }
                }

                if (item.CompareTo(id, Items[swapIndex]) < 0)
                {
                    Swap(id, item, Items[swapIndex]);
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }

    void Swap(int id, T item1, T item2)
    {
        Items[item1.GetHeapIndex(id)] = item2;
        Items[item2.GetHeapIndex(id)] = item1;
        int tempIndex = item1.GetHeapIndex(id);
        item1.SetHeapIndex(id, item2.GetHeapIndex(id));
        item2.SetHeapIndex(id, tempIndex);
    }

    public bool Contains(int id, T item)
    {
        return Equals(Items[item.GetHeapIndex(id)], item);
    }

    public int Count => ItemCount;
}

public interface IHeapItem<T>
{
    int GetHeapIndex(int id);

    void SetHeapIndex(int id, int index);

    int CompareTo(int id, T item);
}
