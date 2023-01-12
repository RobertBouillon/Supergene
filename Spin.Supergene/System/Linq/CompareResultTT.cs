using System;
using System.Collections.Generic;
using System.Text;

namespace System.Linq;

public class CompareResult<TLeft, TRight>
{
  #region Fields
  private List<TLeft> _onlyInLeftList = new List<TLeft>();
  private List<TRight> _onlyInRightList = new List<TRight>();
  private Dictionary<TLeft, TRight> _different = new Dictionary<TLeft, TRight>();
  private Dictionary<TLeft, TRight> _equal = new Dictionary<TLeft, TRight>();
  #endregion
  #region Properties
  public Dictionary<TLeft, TRight> Equal
  {
    get { return _equal; }
  }

  public Dictionary<TLeft, TRight> Different
  {
    get { return _different; }
  }

  /// <summary>
  /// Items in the left list no also in the right list
  /// </summary>
  public List<TLeft> Removed
  {
    get { return _onlyInLeftList; }
  }

  /// <summary>
  /// Items in the right list not in the left list
  /// </summary>
  public List<TRight> Added
  {
    get { return _onlyInRightList; }
  }

  public bool IsSame
  {
    get { return TotalDifferences == 0; }
  }

  public int TotalDifferences
  {
    get { return _onlyInLeftList.Count + _onlyInRightList.Count + _different.Count; }
  }
  #endregion
  #region Constructor
  public CompareResult()
  {

  }
  #endregion
  #region Test Code
  //private static void Test1()
  //{
  //  List<String> left = new List<string>(new string[] { "John", "Joe", "Jim", "Bob" });
  //  List<String> right = new List<string>(new string[] { "John", "Sally", "Bob", "Jim" });

  //  CollectionDifference<string, string> diff = left.Compare(right);
  //}

  //private static void Test3()
  //{
  //  List<String> left = new List<string>(new string[] { "John", "Joe", null, "Bob" });
  //  List<String> right = new List<string>(new string[] { "John", "Sally", "Bob", null });

  //  CollectionDifference<string, string> diff = left.Compare(right);
  //}

  //private static void Test2()
  //{
  //  List<String> left = new List<string>(new string[] { "1", "4", "8", "3" });
  //  List<int> right = new List<int>(new int[] { 3, 9, 1, 4 });

  //  CollectionDifference<string, int> diff = left.Compare(right, (x, y) => x == y.ToString());
  //}

  //private static void Test4()
  //{
  //  List<Employee> left = new List<Employee>(new Employee[] { new Employee(1, "John"), new Employee(2, "Jim"), new Employee(3, "Sally"), new Employee(4, "Bob") });
  //  List<Employee> right = new List<Employee>(new Employee[] { new Employee(1, "Johnny"), new Employee(2, "Jim"), new Employee(3, "Sally"), new Employee(5, "Sally") });

  //  CollectionDifference<Employee, Employee> diff = left.Compare(right, (x, y) => x.Name == y.Name, (x, y) => x.ID == y.ID);
  //}

  //class Employee
  //{
  //  public int ID;
  //  public string Name;

  //  public Employee(int id, string name)
  //  {
  //    ID = id; Name = name;
  //  }
  //}
  #endregion
}
