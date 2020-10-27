using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace System.Net
{
  public class EMailAddress
  {
    #region Static Declarations

    private static Regex _emailValidator = new Regex(@"([a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*)@((?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)", RegexOptions.Compiled);
    private static Regex _emailParser = new Regex(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?", RegexOptions.Compiled);
    public static bool IsValid(string address)
    {
      return _emailValidator.IsMatch(address);
    }
    #endregion

    #region Fields
    private bool _isParsed;
    private string _recipient;
    private string _domain;
    private string _fullAddress;
    #endregion

    #region Properties

    public string Recipient
    {
      get
      {
        if (!_isParsed)
          Parse();
        return _recipient;
      }
    }

    public string Domain
    {
      get
      {
        if (!_isParsed)
          Parse();
        return _domain;
      }
    }

    public string FullAddress
    {
      get { return _fullAddress; }
      set { _fullAddress = value; }
    }

    #endregion


    public EMailAddress(string fullAddress)
    {
      #region Validation
      if (fullAddress == null)
        throw new ArgumentNullException("fullAddress");
      #endregion
      _fullAddress = fullAddress;
    }

    public EMailAddress(string recipient, string domain)
    {
      #region Validation
      if (recipient == null)
        throw new ArgumentNullException("recipient");
      if (domain == null)
        throw new ArgumentNullException("domain");
      #endregion
      _recipient = recipient;
      _domain = domain;
      _isParsed = true;
      _fullAddress = String.Format("{0}@{1}", _recipient, _domain);
    }

    public EMailAddress(string recipient, Uri domain) : this(recipient, domain.ToString()) { }

    #region Methods
    private void Parse()
    {
      if (_isParsed)
        throw new InvalidOperationException("Already Parsed");
      _isParsed = true;
      Match m = _emailParser.Match(_fullAddress);
      _recipient = m.Groups[1].Value;
      _domain = m.Groups[2].Value;
    }
    #endregion

    #region Overrides
    public override string ToString()
    {
      return _fullAddress;
    }
    #endregion
  }
}
