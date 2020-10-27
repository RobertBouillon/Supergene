namespace System.Windows.Forms
{
  partial class ConnectToDatabaseDialog : Form
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.txtUserName = new System.Windows.Forms.TextBox();
      this.txtPassword = new System.Windows.Forms.TextBox();
      this.txtServerName = new System.Windows.Forms.TextBox();
      this.btnConnect = new System.Windows.Forms.Button();
      this.chkIntegratedAuth = new System.Windows.Forms.CheckBox();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 15);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(69, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Server Name";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Enabled = false;
      this.label2.Location = new System.Drawing.Point(12, 39);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(55, 13);
      this.label2.TabIndex = 1;
      this.label2.Text = "Username";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Enabled = false;
      this.label3.Location = new System.Drawing.Point(12, 65);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(53, 13);
      this.label3.TabIndex = 2;
      this.label3.Text = "Password";
      // 
      // txtUserName
      // 
      this.txtUserName.Enabled = false;
      this.txtUserName.Location = new System.Drawing.Point(117, 36);
      this.txtUserName.Name = "txtUserName";
      this.txtUserName.Size = new System.Drawing.Size(196, 20);
      this.txtUserName.TabIndex = 3;
      // 
      // txtPassword
      // 
      this.txtPassword.Enabled = false;
      this.txtPassword.Location = new System.Drawing.Point(117, 62);
      this.txtPassword.Name = "txtPassword";
      this.txtPassword.Size = new System.Drawing.Size(196, 20);
      this.txtPassword.TabIndex = 4;
      // 
      // txtServerName
      // 
      this.txtServerName.Location = new System.Drawing.Point(117, 12);
      this.txtServerName.Name = "txtServerName";
      this.txtServerName.Size = new System.Drawing.Size(196, 20);
      this.txtServerName.TabIndex = 5;
      this.txtServerName.Text = ".";
      // 
      // btnConnect
      // 
      this.btnConnect.Location = new System.Drawing.Point(238, 91);
      this.btnConnect.Name = "btnConnect";
      this.btnConnect.Size = new System.Drawing.Size(75, 23);
      this.btnConnect.TabIndex = 6;
      this.btnConnect.Text = "&Connect";
      this.btnConnect.UseVisualStyleBackColor = true;
      this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
      // 
      // chkIntegratedAuth
      // 
      this.chkIntegratedAuth.AutoSize = true;
      this.chkIntegratedAuth.Checked = true;
      this.chkIntegratedAuth.CheckState = System.Windows.Forms.CheckState.Checked;
      this.chkIntegratedAuth.Location = new System.Drawing.Point(12, 95);
      this.chkIntegratedAuth.Name = "chkIntegratedAuth";
      this.chkIntegratedAuth.Size = new System.Drawing.Size(145, 17);
      this.chkIntegratedAuth.TabIndex = 7;
      this.chkIntegratedAuth.Text = "Integrated Authentication";
      this.chkIntegratedAuth.UseVisualStyleBackColor = true;
      // 
      // ConnectToDatabaseDialog
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(325, 126);
      this.Controls.Add(this.chkIntegratedAuth);
      this.Controls.Add(this.btnConnect);
      this.Controls.Add(this.txtServerName);
      this.Controls.Add(this.txtPassword);
      this.Controls.Add(this.txtUserName);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ConnectToDatabaseDialog";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Connect To Database";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox txtUserName;
    private System.Windows.Forms.TextBox txtPassword;
    private System.Windows.Forms.TextBox txtServerName;
    private System.Windows.Forms.Button btnConnect;
    private System.Windows.Forms.CheckBox chkIntegratedAuth;
  }
}