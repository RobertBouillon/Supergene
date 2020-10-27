using System;

namespace System.Diagnostics.UnitTesting
{
	/// <summary>
	/// Tags a method as a method used in Unit Testing.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
  public class UnitTest : Attribute
	{
    #region Public Fields
    public static readonly string NewTestString = "UNIT TEST GENERATED";
    public static readonly string UpdatedTestString = "UNIT TEST UPDATED";
    #endregion
    #region Private Variable Declarations
    private int p_Timeout;
    private bool p_BackgroundAction;
    #endregion
    #region Public Property Declarations
    /// <summary>
    /// The least amount of time the method should take to execute. 0 is no timeout (default: 0)
    /// </summary>
    public int Timeout
    {
      get{return p_Timeout;}
      set{p_Timeout = value;}
    }

    /// <summary>
    /// True if this test should be run asynchronously to the other tests in the project. (default:false)
    /// </summary>
    public bool BackgroundAction
    {
      get{return p_BackgroundAction;}
      set{p_BackgroundAction = value;}
    }

    #endregion
    #region ctors
    /// <summary>
    /// Tags a method as a method used in Unit Testing.
    /// </summary>
    public UnitTest()
		{
		}

    /// <summary>
    /// Tags a method as a method used in Unit Testing.
    /// </summary>
    /// <param name="timeout">The least amount of time the method should take to execute. 0 is no timeout (default: 0)</param>
    public UnitTest(int timeout)
    {
      p_Timeout = timeout;
    }

    /// <summary>
    /// Tags a method as a method used in Unit Testing.
    /// </summary>
    /// <param name="timeout">The least amount of time the method should take to execute. 0 is no timeout (default: 0)</param>
    /// <param name="backgroundAction">True if this test should be run asynchronously to the other tests in the project. (default:false)</param>
    public UnitTest(int timeout, bool backgroundAction) : this(timeout)
    {
      p_BackgroundAction = backgroundAction;
    }
    #endregion
    #region Public Static Methods
    public static Exception DestroyTest(object o)
    {
      Type t = o.GetType();
      System.Reflection.MethodInfo mi = t.GetMethod("DestroyTest");
      if(mi==null)
        throw new Exception("Class does not contain a definition for 'DestroyTest'.");
      
      try
      {
        mi.Invoke(null,new object[]{o});
      }
      catch(Exception ex)
      {
        return ex.InnerException;
      }
      return null;
    }

    #endregion
	}
}
