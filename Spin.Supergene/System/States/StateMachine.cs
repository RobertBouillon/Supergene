using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.States
{
  public abstract class StateMachine
  {
    #region Fields
    private State _currentState;
    private readonly State _initialState;
    #endregion
    #region 
    public State CurrentState
    {
      get { return _currentState; }
      set
      {
        ChangeState(value);
      }
    }

    public State InitialState
    {
      get { return _initialState; }
    }
    #endregion

    #region Constructors
    public StateMachine(State initialState)
    {
      #region Validation
      if (initialState == null)
        throw new ArgumentNullException("initialState");
      #endregion
      _initialState = initialState;
    }
    #endregion


    #region Methods
    public virtual void ChangeState(State newState)
    {
      #region Validation
      if (newState == null)
        throw new ArgumentNullException("newState");
      #endregion

      if (_currentState != null)
        _currentState.OnExit(this);
      _currentState = newState;
      _currentState.OnEnter(this);
    }

    public virtual void Reset()
    {
      if (_currentState != null)
        _currentState.OnExit(this);
      _currentState = null;
      //_initialState.OnEnter(this);
    }

    public virtual void Evaluate()
    {
      if (_currentState == null)
        ChangeState(_initialState);

      for (var state = _currentState; state != null; state = state.Evaluate(this))
        if (_currentState != state)
          ChangeState(state);
    }
    #endregion



  }
}
