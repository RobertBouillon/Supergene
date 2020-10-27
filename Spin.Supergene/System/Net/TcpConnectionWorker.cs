using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net
{
  public class TcpConnectionWorker : Worker
  {
    #region Fields
    private TcpListener _tcpListener;
    #endregion

    #region Constructors
    public TcpConnectionWorker(string name, IPEndPoint ep) : base(name)
    {
      #region Validation
      if (ep == null)
        throw new ArgumentNullException(nameof(ep));
      #endregion
      _tcpListener = new TcpListener(ep);
    }

    public TcpConnectionWorker(string name, IPEndPoint ep, Action<TcpClient> connected) : this(name, ep)
    {
      #region Validation
      if (connected == null)
        throw new ArgumentNullException(nameof(connected));
      #endregion
      ClientConnected += (x, y) => connected(y.Client);
    }
    #endregion

    #region Overrides
    protected override void OnStarting(CancelEventArgs e)
    {
      _tcpListener.Start();
      base.OnStarting(e);
    }

    protected override void OnStopping(CancelEventArgs e)
    {
      _tcpListener.Stop();
      base.OnStopping(e);
    }

    protected override bool HasWork => _tcpListener.Pending();

    protected override void Work()
    {
      var client = _tcpListener.AcceptTcpClient();
      OnClientConnected(client);
    }
    #endregion


    #region ClientConnectedEventArgs Subclass
    public class ClientConnectedEventArgs : EventArgs
    {
      #region Properties
      public TcpClient Client { get; private set; }
      #endregion
      #region Constructors
      internal ClientConnectedEventArgs(TcpClient client)
      {
        #region Validation
        if (client == null)
          throw new ArgumentNullException("client");
        #endregion
        Client = client;
      }
      #endregion
    }
    #endregion

    public event global::System.EventHandler<ClientConnectedEventArgs> ClientConnected;
    protected void OnClientConnected(TcpClient client) => OnClientConnected(new ClientConnectedEventArgs(client));
    protected virtual void OnClientConnected(ClientConnectedEventArgs e) => ClientConnected?.Invoke(this, e);
  }
}
