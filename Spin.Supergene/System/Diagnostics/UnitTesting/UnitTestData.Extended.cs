using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Data;

namespace System.Diagnostics.UnitTesting
{
  public partial class UnitTestData
  {
    public partial class ActionsRow
    {
      #region Private Fields
      private List<MethodInfo> _taggedMethods = new List<MethodInfo>(3);
      private Type[] _dependencies;
      #endregion

      #region Constructors
      /// <summary>
      /// Creates a Unit Test Action based upon type information provided by reflection
      /// </summary>
      /// <param name="t"></param>
      public ActionsRow(Type t)
      {
        p_Type = t;

        //Find all methods in the class marked with the UnitTest attribute
        foreach (MemberInfo mi in t.FindMembers(MemberTypes.Method, BindingFlags.Public | BindingFlags.Static, null, null))
          if (Attribute.IsDefined(mi, typeof(UnitTest)))
            _taggedMethods.Add((MethodInfo)mi);


        //Determine what we can perform on the type
        CanTest = GetUnitTestMethod("TestUnit") != null;
        CanDestroy = GetUnitTestMethod("DestroyTest") != null;

        MethodInfo createtest = GetUnitTestMethod("CreateTest", t);
        if (createtest == null)
        {
          CanCreate = false;
          return;
        }

        CanCreate = true;

        //Catalog the method dependencies
        ParameterInfo[] dep = createtest.GetParameters();

        _dependencies = new Type[dep.Length];
        int index = 0;
        foreach (ParameterInfo pi in dep)
          _dependencies[index++] = pi.ParameterType;

        Dependencies = _dependencies.Length;


        //Set the defaults
        EnableTest = CanTest || CanCreate || CanDestroy;

        CreateImage = (CanCreate) ? Properties.Resources.statusblue : Properties.Resources.statuswhite;
        TestImage = (CanTest) ? Properties.Resources.statusblue : Properties.Resources.statuswhite;
        DestroyImage = (CanDestroy) ? Properties.Resources.statusblue : Properties.Resources.statuswhite;

      }
      #endregion

      #region Private Methods

      /// <summary>
      /// Searches the list of tagged methods for a method matching the supplied name
      /// </summary>
      /// <param name="name">The name of the method to search</param>
      /// <returns>The method whose name matches the name supplied inthe search criteria</returns>
      private MethodInfo GetUnitTestMethod(string name)
      {
        foreach (MethodInfo mi in _taggedMethods)
          if (mi.Name == name)
            return mi;

        return null;
      }


      /// <summary>
      /// Gets the objects this object depends on from the suppplied list and returns them in an array
      /// </summary>
      /// <param name="list"></param>
      /// <returns></returns>
      private object[] GetDependancies(object[] list)
      {
        DataView depsearch = new DataView(Table);
        DataView objects = new DataView(Table);

        objects.RowFilter = "not Isnull(ActiveInstance)";

        object[] dep = new object[p_Dependencies.Length];
        int index = 0;
        bool found;
        foreach (Type t in p_Dependencies)
        {
          depsearch.RowFilter = String.Format("Type = '{0}'", t.ToString());
          if(depsearch.Count==0)
            throw new Exception(String.Format("{0} is a dependency and was not loaded.", t.FullName));

          object o = ((ActionsRow)depsearch[0]).ActiveInstance;
          if(o==null)
            throw new Exception(String.Format("{0} is a dependency and failed to be created.", t.FullName));

          dep[index++] = o;
        }

        return dep;
      }


      /// <summary>
      /// True if the array contains the types this object is dependent on
      /// </summary>
      /// <param name="search"></param>
      /// <returns></returns>
      public bool HasDependancies(Type[] search)
      {
        foreach (Type t in _dependencies)
          foreach (Type st in search)
            if (st == t)
              return true;

        return false;
      }
      #endregion

      #region Private Methods (Actions)
      private void Create()
      {
        CreateMethod.Invoke(null,GetDependancies(p_TestObjects));
      }
      #endregion
    }


    #region Public Methods
    public int[] GetExecutionOrder()
    {
      int index = 0;
      int thispass = 0;
      int lastpass = 0;
      int total = 0;

      int[] executionorder = new int[Actions.Rows.Count];
      Type[] executed = new Type[Actions.Rows.Count];
      bool[] added = new bool[Actions.Rows.Count];

      while (total < Actions.Rows.Count)
      {
        lastpass = thispass;
        thispass = 0;

        foreach (ActionsRow row in Actions.Rows)
        {
          int thisindex = Actions.Rows.IndexOf(row);
          if (added[thisindex])
            continue;

          if (row.HasDependancies(executed))
          {
            executed[index] = row.TargetType;
            executionorder[index] = thisindex;
            added[thisindex] = true;
            index++;
            thispass++;
            total++;
          }
        }

        if (thispass == 0)
          throw new Exception("Some object dependancies were missing");
      }
      return executionorder;
    }
    #endregion
  }
}
