using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace System.Threading.Workers
{
  public abstract class DirectoryWorker : QueueWorker<String>
  {
    #region Fields
    private readonly DirectoryInfo _directory;
    private FileSystemWatcher _watcher;
    private string _filter;
    #endregion
    #region Properties
    public DirectoryInfo Directory => _directory;
    #endregion
    #region Constructors

    public DirectoryWorker(string name, DirectoryInfo directory) : base(name)
    {
      #region Validation
      if (directory == null)
        throw new ArgumentNullException("directory");
      #endregion
      _directory = directory;
    }
    #endregion

    #region Overrides
    protected override void OnStarted(EventArgs e)
    {
      string filter = _filter ?? "*.*";

      //Enqueue all current files
      foreach (FileInfo fi in _directory.GetFiles(filter))
        Enqueue(fi.FullName);

      _watcher = new FileSystemWatcher(_directory.FullName, filter);
      _watcher.Created += (x, y) => { Enqueue(y.FullPath); };
    }

    protected override void OnStopped(EventArgs e)
    {
      _watcher.Dispose();
      _watcher = null;
    }
    #endregion
  }
}
