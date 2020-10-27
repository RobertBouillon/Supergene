using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.States
{
  public abstract class State
  {
    #region Fields
    private string _description;
    #endregion
    #region Properties
    public string Description
    {
      get { return _description; }
      set { _description = value; }
    }
    #endregion

    #region Constructors
    public State(string description)
    {
      #region Validation
      if (description == null)
        throw new ArgumentNullException("description");
      #endregion
      _description = description;
    }
    #endregion
    #region Methods
    public abstract State Evaluate(StateMachine machine);
    public abstract void OnEnter(StateMachine machine);
    public abstract void OnExit(StateMachine machine);
    #endregion
  }
}
