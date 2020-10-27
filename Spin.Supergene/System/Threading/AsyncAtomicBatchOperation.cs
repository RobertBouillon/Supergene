using System;
using System.Collections.Generic;
using System.Text;

namespace System.Threading
{
  class AsyncAtomicBatchOperation : AsyncOperation
  {
    #region Fields
    private CollectionBase<StatelessAsyncMethod> _operations;
    #endregion
    #region Constructors
    public AsyncAtomicBatchOperation(params StatelessAsyncMethod[] operations)
    {
      throw new NotImplementedException();
      
      _operations = new CollectionBase<StatelessAsyncMethod>(operations);
    }
    #endregion

    #region Methods
    public void Execute()
    {

    }
    #endregion
    #region Events

    public event EventHandler Executing;

    protected void OnExecuting()
    {
      OnExecuting(EventArgs.Empty);
    }

    protected virtual void OnExecuting(EventArgs e)
    {
      if (Executing != null)
        Executing(this, e);
    }


    public event EventHandler Executed;

    protected void OnExecuted()
    {
      OnExecuted(EventArgs.Empty);
    }

    protected virtual void OnExecuted(EventArgs e)
    {
      if (Executed != null)
        Executed(this, e);
    }



    #endregion

  }
}
