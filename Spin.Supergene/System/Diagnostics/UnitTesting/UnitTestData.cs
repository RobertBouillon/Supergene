using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Data;

using excorlib;

namespace System.Diagnostics.UnitTesting
{
  public partial class UnitTestData
  {
    public partial class ActionsDataTable
    {
      public ActionsRow AddActionsRow(Type t)
      {
        List<MethodInfo> _taggedMethods = new List<MethodInfo>(3);

        //Find all methods in the class marked with the UnitTest attribute
        foreach (MemberInfo mi in t.FindMembers(MemberTypes.Method, BindingFlags.Public | BindingFlags.Static, null, null))
          if (Attribute.IsDefined(mi, typeof(UnitTest)))
            _taggedMethods.Add((MethodInfo)mi);

        MethodInfo create = GetUnitTestMethod(TestMethodType.CreateInstance, _taggedMethods);
        MethodInfo test = GetUnitTestMethod(TestMethodType.TestInstance, _taggedMethods);
        MethodInfo teststatic = GetUnitTestMethod(TestMethodType.TestStatic, _taggedMethods);
        MethodInfo destroy = GetUnitTestMethod(TestMethodType.DestroyInstance, _taggedMethods);

        ActionsRow row = AddActionsRow(
          t.Name,
          t.FullName,
          (create != null) ? excorlib.Properties.Resources.bullet_ball_glass_blue : excorlib.Properties.Resources.bullet_ball_glass_grey,
          (test != null) ? excorlib.Properties.Resources.bullet_ball_glass_blue : excorlib.Properties.Resources.bullet_ball_glass_grey,
          (teststatic != null) ? excorlib.Properties.Resources.bullet_ball_glass_blue : excorlib.Properties.Resources.bullet_ball_glass_grey,
          (destroy != null) ? excorlib.Properties.Resources.bullet_ball_glass_blue : excorlib.Properties.Resources.bullet_ball_glass_grey,
          //Properties.Resources.TestNotRun,
          //Properties.Resources.TestNotRun,
          //Properties.Resources.TestNotRun,
          null,
          null,
          null,
          null,
          (create==null)?0:create.GetParameters().Length,
          create != null || test != null || destroy != null,
          t,
          create != null,
          test != null,
          teststatic != null,
          destroy != null,
          null,
          create,
          test,
          teststatic,
          destroy);

        if (create == null)
        {
          row._dependencies = new Type[0];
          return row;
        }

        //Get the dependencies
        ParameterInfo[] dep = create.GetParameters();
        row._dependencies = new Type[dep.Length];
        int index = 0;
        foreach (ParameterInfo pi in dep)
        {
          if (pi.ParameterType == row.TargetType)
            throw new Exception(String.Format("Recursive dependancy found. Please create a static member called TestInstance in class '{0}' of type '{0}' to use for testing",row.TargetType.ToString()));

          //TODO: Allow a user to create a TestInstance type that allows the user to 
          //      either bypass CreateTest using the test instance, or to pass an instance
          //      of itself to the createtest method
          row._dependencies[index++] = pi.ParameterType;
        }

        return row;

      }

      /// <summary>
      /// Searches the list of tagged methods for a method matching the supplied name
      /// </summary>
      /// <param name="name">The name of the method to search</param>
      /// <returns>The method whose name matches the name supplied inthe search criteria</returns>
      private MethodInfo GetUnitTestMethod(string name, List<MethodInfo> _taggedMethods)
      {
        foreach (MethodInfo mi in _taggedMethods)
          if (mi.Name == name)
            return mi;

        return null;
      }

      /// <summary>
      /// Searches the list of tagged methods for a method matching the supplied name
      /// </summary>
      /// <param name="name">The name of the method to search</param>
      /// <returns>The method whose name matches the name supplied inthe search criteria</returns>
      private MethodInfo GetUnitTestMethod(TestMethodType type, List<MethodInfo> _taggedMethods)
      {
        foreach (MethodInfo mi in _taggedMethods)
        {
          UnitTest u = mi.GetCustomAttributes(typeof(UnitTest),false)[0] as UnitTest;
          if (u.Type == type)
            return mi;
        }

        return null;
      }
    }
 
    public partial class ActionsRow
    {
      #region Private Fields
      internal List<MethodInfo> _taggedMethods = new List<MethodInfo>(3);
      internal Type[] _dependencies;
      #endregion

      #region Constructors
      #endregion

      #region Private Methods



      /// <summary>
      /// Gets the objects this object depends on from the suppplied list and returns them in an array
      /// </summary>
      /// <param name="list"></param>
      /// <returns></returns>
      private object[] GetDependancies()
      {
        //TODO: Optimize this.... There's GOT to be a better way. (Can't query with JOINS).
        


        //Get the objects that we need
        object[] ret = new object[_dependencies.Length];
        int index = 0;

        foreach (Type t in _dependencies)
        {
          foreach(ActionsRow row in Table.Rows)
          {
            if (row.TargetType == t)
            {
              if (row.IsActiveInstanceNull())
                throw new Exception(String.Format("Execution Order Error: {0} requires type {1}, which has not been created", TargetType.ToString(),row.TargetType.ToString()));

              ret[index++] = row.ActiveInstance;
            }
          }
        }

        return ret;
      }


      /// <summary>
      /// True if the array contains the types this object is dependent on
      /// </summary>
      /// <param name="search"></param>
      /// <returns></returns>
      public bool HasDependancies(Type[] search)
      {

        foreach (Type t in _dependencies)
        {
          bool found = false;
          foreach (Type st in search)
          {
            if (st == null)
              continue;
            if (st == t)
            {
              found = true;
              break;
            }
          }
          if (!found)
            return false;
        }

        return true;
      }
      #endregion

      #region Private Methods (Actions)
      internal void Validate()
      {
        if (CanCreate)
          UnitTest.ValidateUnitTestSignature(TestMethodType.CreateInstance, CreateMethod);

        if (CanTest)
          UnitTest.ValidateUnitTestSignature(TestMethodType.TestInstance, TestMethod);

        if (CanTestStatic)
          UnitTest.ValidateUnitTestSignature(TestMethodType.TestStatic, TestStaticMethod);

        if (CanDestroy)
          UnitTest.ValidateUnitTestSignature(TestMethodType.DestroyInstance, DestroyMethod);
      }

      internal bool Create()
      {
        try
        {
          ActiveInstance = CreateMethod.Invoke(null, GetDependancies());
        }
        catch (Exception ex)
        {
          CreateException = ex;
          CreateImage = Properties.Resources.statusred;
          return false;
        }

        CreateException = null;
        CreateImage = Properties.Resources.statusgreen;
        return true;
      }

      internal bool Test()
      {
        try
        {
          if (IsActiveInstanceNull())
            throw new Exception("Cannot test unit: create failed");
          TestMethod.Invoke(null, new object[] { ActiveInstance });
        }
        catch (Exception ex)
        {
          TestException = ex;
          TestImage = Properties.Resources.statusred;
          return false;
        }

        TestException = null;
        TestImage = Properties.Resources.statusgreen;
        return true;
      }


      internal bool TestStatic()
      {
        try
        {
          TestStaticMethod.Invoke(null, new object[] { });
        }
        catch (Exception ex)
        {
          TestStaticException = ex;
          TestImage = Properties.Resources.statusred;
          return false;
        }

        TestStaticException = null;
        TestImage = Properties.Resources.statusgreen;
        return true;
      }


      internal bool Destroy()
      {
        try
        {
          if (IsActiveInstanceNull())
            throw new Exception("Cannot destroy unit: create failed");
          DestroyMethod.Invoke(null, new object[] { ActiveInstance });
        }
        catch (Exception ex)
        {
          DestroyException = ex;
          DestroyImage = Properties.Resources.statusred;
          return false;
        }

        DestroyException = null;
        DestroyImage = Properties.Resources.statusgreen;
        return true;
      }
      #endregion
    }

    #region Public Methods
    public int[] GetExecutionOrder(DataView list)
    {
      int index = 0;
      int thispass = 0;
      int lastpass = 0;
      int total = 0;

      int[] executionorder = new int[list.Count];
      Type[] executed = new Type[list.Count];
      bool[] added = new bool[list.Count];

      while (total < list.Count)
      {
        lastpass = thispass;
        thispass = 0;

        int itemnum = 0;
        foreach (DataRowView row in list)
        {
          int thisindex = Actions.Rows.IndexOf(row.Row);
          if (added[itemnum])
            continue;

          if (((ActionsRow)row.Row).HasDependancies(executed))
          {
            executed[index] = ((ActionsRow) row.Row).TargetType;
            executionorder[index] = thisindex;
            added[itemnum] = true;
            index++;
            thispass++;
            total++;
          }

          itemnum++;
        }

        if (thispass == 0)
        {
          //Object dependancies were missing...
          List<String> missing = new List<string>(list.Count - total);
          int totallen = 0;

          for (int i = 0; i < total; i++)
            if (!added[i])
            {
              UnitTestData.ActionsRow strongrow = (UnitTestData.ActionsRow)list[i-1].Row;
              totallen += strongrow.Name.Length + 2;
              missing.Add(strongrow.Name);
            }

          StringBuilder sb = new StringBuilder(totallen);
          foreach (string s in missing)
            sb.AppendLine(s);

          throw new Exception("The following objects were missing dependencies:\n\n" + sb.ToString());
        }
      }
      return executionorder;
    }
    #endregion
  }
}
