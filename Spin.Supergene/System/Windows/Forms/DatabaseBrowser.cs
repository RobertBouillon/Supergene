using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace System.Windows.Forms
{
  
  public partial class DatabaseBrowser : Form
  {
    #region Private Fields
    private string _connectionString;
    private string _databaseName;
    private string _serverName;

    private List<String> _connections = new List<string>(8);
    #endregion
    #region Public Property Declarations


    public string ServerName
    {
      get { return _serverName; }
      set { _serverName = value; }
    }

    public string DatabaseName
    {
      get { return _databaseName; }
      set { _databaseName = value; }
    }

    public string ConnectionString
    {
      get { return _connectionString; }
    }
    #endregion

    #region Constructors
    public DatabaseBrowser()
    {
      InitializeComponent();
    }
    #endregion

    #region Private Methods
    private void Finish()
    {
      DialogResult = DialogResult.OK;

      TreeNode selectednode = tvwDatabases.SelectedNode;
      TreeNode databaseNode = selectednode.Parent;
      string originalconn = _connections[tvwDatabases.Nodes.IndexOf(selectednode.Parent)];

      StringBuilder ret = new StringBuilder();

      if (originalconn.IndexOf("Integrated Security=True") < 0)
        ret.Append(originalconn.Substring(0, originalconn.IndexOf("Data Source={0}")));
      else
        ret.Append("Integrated Security=True;");

      ret.Append(String.Format(@"Data Source={0};Initial Catalog={1}", databaseNode.Text, selectednode.Text));

      _connectionString = ret.ToString();
    }

    internal bool ConnectToSqlServer(string serverName, bool integratedAuth, string username, string password)
    {
      SqlConnection conn = null;
      SqlDataReader idr = null;
      SqlCommand cmd = null;
      //Connect to the server.
      try
      {
        if (integratedAuth)
        {
          conn = new SqlConnection(
            String.Format(@"Data Source={0};Initial Catalog=master;Integrated Security=True",
              serverName
            )
          );
        }
        else
        {
          conn = new SqlConnection(
            String.Format(
              @"user={1};password={2};Data Source={0};Initial Catalog=master",
              serverName,
              username,
              password
            )
          );
        }

        conn.Open();
        _connections.Add(conn.ConnectionString);

        cmd = new SqlCommand("SELECT @@VERSION",conn);
        string ver = (string)cmd.ExecuteScalar();

        if (ver.IndexOf("Microsoft SQL Server  2000") >= 0)
        {
          cmd = new SqlCommand("sp_databases", conn);
          cmd.CommandType = CommandType.StoredProcedure;
          idr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
        }
        else
        {
          cmd = new SqlCommand("Select name from sys.databases --where owner_sid <> 1", conn);
          idr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
        }

        TreeNode servernode = tvwDatabases.Nodes.Add(serverName);
        while (idr.Read())
          servernode.Nodes.Add(idr.GetString(0));

        servernode.Expand();

        conn.Close();
      }
      catch (SqlException ex)
      {
        MessageBox.Show("Error connecting to database: " + ex.Message);
        return false;
      }
      catch (Exception ex)
      {
        MessageBox.Show("Error connecting to database: " + ex.Message);
        return false;
      }
      finally
      {
        if (idr != null)
        {
          if (!idr.IsClosed)
            idr.Close();
        }

        if (cmd != null)
          cmd.Dispose();

        if (conn != null)
        {
          if (conn.State == ConnectionState.Open)
            conn.Close();
          conn.Dispose();
        }
      }
      return true;
    }
    #endregion
    #region Event Handles
    private void tsConnectButton_Click(object sender, EventArgs e)
    {
      ConnectToDatabaseDialog dialog = new ConnectToDatabaseDialog(this);
      dialog.ShowDialog();
    }


    private void btnOpen_Click(object sender, EventArgs e)
    {
      Finish();
    }

    private void tvwDatabases_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
    {
      Finish();
    }

    private void tvwDatabases_AfterSelect(object sender, TreeViewEventArgs e)
    {
      if (tvwDatabases.SelectedNode == null)
      {
        btnOpen.Enabled = false;
        return;
      }

      if (tvwDatabases.SelectedNode.Parent == null)
      {
        btnOpen.Enabled = false;
        return;
      }

      btnOpen.Enabled = true;
    }
    #endregion
  }
}