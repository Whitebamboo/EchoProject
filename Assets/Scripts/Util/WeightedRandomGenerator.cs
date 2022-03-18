using System;
using System.Collections.Generic;

//An generic weighted random generator used to get a random entry from the list it holds
public class WeightedRandomGenerator<T>
{
  //data class
  class Entry
  {
    public T Item;
    public double Weight;             //weight data of the item
    public double AccumulatedWeight;  //accumulated weight of item based on it's index
    public bool IsSelected;           //is selected for unique random list
  }

  private List<Entry> entryList = new List<Entry>(); //A list of entry for generator to choose from
  private double accumulatedWeight;                  //Weight for all entries
  private string randomType;                         
  private Random rand = new Random(); 

  public WeightedRandomGenerator(string randomType)
  {
    this.randomType = randomType;
  }

  //Add entry to the list for generator to choose from
  public void AddEntry(T item, double weight) 
  {
    accumulatedWeight += weight;
    entryList.Add(new Entry{Item = item, AccumulatedWeight = accumulatedWeight, 
    Weight = weight, IsSelected = false});    
  }

  //Get an entry based on the random type
  public T GetRandomEntry()
  {
    if(entryList.Count == 0)
    {
      Console.WriteLine(string.Format("Generator does not contain any entry. Did you add any entry to your table?"));
      return default(T);
    }

    if(randomType == "Random")
    {
      return GetRandom();
    }
    else if(randomType == "UniqueRandom")
    {
      return GetUniqueRandom();
    }
    
    return default(T);
  }

  //Reset the list for unique random
  public void Reset()
  {
    accumulatedWeight = 0;
    for (int i = 0; i < entryList.Count; i++) 
    {
        Entry e = entryList[i];
        accumulatedWeight += e.Weight;
        e.AccumulatedWeight = accumulatedWeight;
        e.IsSelected = false;
    }
  }

  //Return an entry based on normal weighted random algorithm
  T GetRandom() 
  {
    double r = rand.NextDouble() * accumulatedWeight;

    for (int i = 0; i < entryList.Count; i++) 
    {
        Entry e = entryList[i];
        if (e.AccumulatedWeight >= r) 
        {
          return e.Item;
        }
    }
    return default(T); //should only happen when there are no entries
  }

  //Return an entry based on unique weighted random algorithm
  T GetUniqueRandom() 
  {
    T result = default(T);
    double r = rand.NextDouble() * accumulatedWeight;

    bool findItem = false;
    double currItemWeight = 0;
    for (int i = 0; i < entryList.Count; i++) 
    {
        Entry e = entryList[i];
        if (!e.IsSelected && e.AccumulatedWeight >= r && !findItem)
        {
          result = e.Item;

          currItemWeight = e.Weight;
          accumulatedWeight -= currItemWeight; // Reduce the accumulated weight for the generrator
          e.IsSelected = true;
          findItem = true;
        }
        else
        {         
          //Reduce the accumulated weight of the rest of items for calculation
          e.AccumulatedWeight -= currItemWeight;
        }
    }
    return result; //should only happen when there are no entries
  }
}

