using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace System.Windows.Forms
{
  public partial class ConnectToDatabaseDialog : Form
  {
    DatabaseBrowser _parent;

    public ConnectToDatabaseDialog(DatabaseBrowser parent)
    {
      InitializeComponent();

      _parent = parent;
    }

    private void btnConnect_Click(object sender, EventArgs e)
    {
      btnConnect.Enabled = false;
      Application.DoEvents();

      if (_parent.ConnectToSqlServer(txtServerName.Text, chkIntegratedAuth.Checked, txtUserName.Text, txtPassword.Text))
        Close();
      else
        btnConnect.Enabled = true;
    }
  }
}