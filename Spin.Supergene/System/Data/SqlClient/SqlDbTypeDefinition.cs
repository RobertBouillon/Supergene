using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace System.Data.SqlClient
{
  public class SqlDbTypeDefinition : DbTypeDefinition
  {
    #region SqlDbType Member List
    //SqlDbType.BigInt
    //SqlDbType.Binary
    //SqlDbType.Bit
    //SqlDbType.Char
    //SqlDbType.DateTime
    //SqlDbType.Decimal
    //SqlDbType.Float
    //SqlDbType.Image
    //SqlDbType.Int
    //SqlDbType.Money
    //SqlDbType.NChar
    //SqlDbType.NText
    //SqlDbType.NVarChar
    //SqlDbType.Real
    //SqlDbType.UniqueIdentifier
    //SqlDbType.SmallDateTime
    //SqlDbType.SmallInt
    //SqlDbType.SmallMoney
    //SqlDbType.Text
    //SqlDbType.Timestamp
    //SqlDbType.TinyInt
    //SqlDbType.VarBinary
    //SqlDbType.VarChar
    //SqlDbType.Variant
    //SqlDbType.Xml
    //SqlDbType.Udt


    //type==SqlDbType.BigInt||
    //type==SqlDbType.Binary||
    //type==SqlDbType.Bit||
    //type==SqlDbType.Char||
    //type==SqlDbType.DateTime||
    //type==SqlDbType.Decimal||
    //type==SqlDbType.Float||
    //type==SqlDbType.Image||
    //type==SqlDbType.Int||
    //type==SqlDbType.Money||
    //type==SqlDbType.NChar||
    //type==SqlDbType.NText||
    //type==SqlDbType.NVarChar||
    //type==SqlDbType.Real||
    //type==SqlDbType.UniqueIdentifier||
    //type==SqlDbType.SmallDateTime||
    //type==SqlDbType.SmallInt||
    //type==SqlDbType.SmallMoney||
    //type==SqlDbType.Text||
    //type==SqlDbType.Timestamp||
    //type==SqlDbType.TinyInt||
    //type==SqlDbType.VarBinary||
    //type==SqlDbType.VarChar||
    //type==SqlDbType.Variant||
    //type==SqlDbType.Xml||
    //type==SqlDbType.Udt||

    #endregion
    #region Private Members
    private string _userDefinedTypeName;
    #endregion
    #region Public Properties
    public new SqlDbType Type
    {
      get { return (SqlDbType)base.Type; }
    }
    #endregion
    #region Constructors
    public SqlDbTypeDefinition(SqlDbType type) : base((DbType)type)
    {
      #region Validation
      if (
          type == SqlDbType.Binary ||
          type == SqlDbType.Decimal ||
          type == SqlDbType.Int ||
          type == SqlDbType.NChar ||
          type == SqlDbType.NText ||
          type == SqlDbType.NVarChar ||
          type == SqlDbType.VarBinary ||
          type == SqlDbType.VarChar||
          type == SqlDbType.Udt
        )
      {
        throw new ArgumentException("SqlDbType '{0}' requires additional information. Consider using a different constructor.","type");
      }
      #endregion
    }
    public SqlDbTypeDefinition(SqlDbType type, int size) : base((DbType)type,size)
    {
      #region Validation
      if (type == SqlDbType.BigInt ||
          type == SqlDbType.Bit ||
          type == SqlDbType.Char ||
          type == SqlDbType.DateTime ||
          type == SqlDbType.Decimal ||
          type == SqlDbType.Float ||
          type == SqlDbType.Image ||
          type == SqlDbType.Int ||
          type == SqlDbType.Money ||
          type == SqlDbType.NText ||
          type == SqlDbType.Real ||
          type == SqlDbType.UniqueIdentifier ||
          type == SqlDbType.SmallDateTime ||
          type == SqlDbType.SmallInt ||
          type == SqlDbType.SmallMoney ||
          type == SqlDbType.Text ||
          type == SqlDbType.Timestamp ||
          type == SqlDbType.TinyInt ||
          type == SqlDbType.Xml ||
          type == SqlDbType.Udt
        )
      {
        throw new ArgumentException(String.Format("SqlDbType '{0}' is not a variable-length type. Consider another constructor.", type.ToString()), "type");
      }
      #endregion
    }
    public SqlDbTypeDefinition(SqlDbType type, int precision, int scale) : base((DbType)type,precision,scale)
    {
      #region Validation
      if (type != SqlDbType.Decimal)
      {
        throw new ArgumentException(String.Format("SqlDbType '{0}' does not support precision/scale attributes. Consider another constructor.", type.ToString()), "type");
      }
      #endregion
    }

    public SqlDbTypeDefinition(SqlDbType type, string userDefinedTypeName) : base((DbType)type)
    {
      #region Validation
      if (type != SqlDbType.Udt)
        throw new ArgumentException("This constructor is only for the User-Defined data type", "type");
      #endregion
      _userDefinedTypeName = userDefinedTypeName;
    }

    /// <summary>
    /// Creates a Type Definition with the attributes as they apply to the given type
    /// </summary>
    /// <param name="type"></param>
    /// <param name="size"></param>
    /// <param name="precision"></param>
    /// <param name="scale"></param>
    public SqlDbTypeDefinition(SqlDbType type, int size, int precision, int scale) : base((DbType)type,size,precision,scale)
    {
      #region Validation
      if (type == SqlDbType.Udt)
        throw new Exception("UDF not supported");
      #endregion
    }
    #endregion
    #region Overrides
    public string ToSql()
    {
      StringBuilder sb = new StringBuilder(256);
      ToSql(sb);
      return sb.ToString();
    }

    public override string ToString()
    {
      return ToSql();
    }

    /// <summary>
    /// Appends the SQL to the stringbuilder object.
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public void ToSql(StringBuilder target)
    {
      SqlDbType type = Type;

      //NOTE: Sometimes return is used in place of break if no other options are available
      switch (type)
      {
        case SqlDbType.BigInt:
          target.Append("bigint");
          return;
        case SqlDbType.Binary:
          target.Append("binary");
          return;
        case SqlDbType.Bit:
          target.Append("bit");
          return;
        case SqlDbType.Char:
          target.Append("char");
          break;
        case SqlDbType.DateTime:
          target.Append("datetime");
          return;
        case SqlDbType.Decimal:
          target.Append("decimal");
          break;
        case SqlDbType.Float:
          target.Append("float");
          return;
        case SqlDbType.Image:
          target.Append("image");
          return;
        case SqlDbType.Int:
          target.Append("int");
          return;
        case SqlDbType.Money:
          target.Append("money");
          return;
        case SqlDbType.NChar:
          target.Append("nchar");
          break;
        case SqlDbType.NText:
          target.Append("ntext");
          return;
        case SqlDbType.NVarChar:
          target.Append("nvarchar");
          break;
        case SqlDbType.Real:
          target.Append("real");
          return;
        case SqlDbType.UniqueIdentifier:
          target.Append("uniqueidentifier");
          return;
        case SqlDbType.SmallDateTime:
          target.Append("smalldatetime");
          return;
        case SqlDbType.SmallInt:
          target.Append("smallint");
          return;
        case SqlDbType.SmallMoney:
          target.Append("smallmoney");
          return;
        case SqlDbType.Text:
          target.Append("text");
          return;
        case SqlDbType.Timestamp:
          target.Append("timestamp");
          return;
        case SqlDbType.TinyInt:
          target.Append("tinyint");
          return;
        case SqlDbType.VarBinary:
          target.Append("varbinary");
          break;
        case SqlDbType.VarChar:
          target.Append("varchar");
          break;
        case SqlDbType.Variant:
          target.Append("variant");
          return;
        case SqlDbType.Xml:
          target.Append("xml");
          return;
        case SqlDbType.Udt:
          target.Append(_userDefinedTypeName);
          return;
        case SqlDbType.DateTime2:
          target.Append("datetime2");
          return;
        case SqlDbType.Time:
          target.Append("time");
          return;
        case SqlDbType.Date:
          target.Append("date");
          return;
        default:
          throw new InvalidOperationException(String.Format("Unknown SQL Type: '{0}'", type.ToString()));
      }

      if (
            type == SqlDbType.Char ||
            type == SqlDbType.NChar ||
            type == SqlDbType.NVarChar ||
            type == SqlDbType.VarChar            
          )
      {
        target.Append('(');
        if (Size == Int32.MaxValue)
          target.Append("max");
        else
          target.Append(Size);
        target.Append(')');
        return;
      }

      if (
            type==SqlDbType.VarBinary||
            type==SqlDbType.Binary
        )
      {
        target.Append('(');
        if (Size == Int32.MaxValue)
          target.Append("max");
        else
          target.Append(Size);
        target.Append(')');
        return;
      }

      if (type==SqlDbType.Decimal)
      {
        target.Append('(');
        target.Append(Precision);
        target.Append(',');
        target.Append(Scale);
        target.Append(')');
        return;
      }

      if (
        type == SqlDbType.Time||
        type == SqlDbType.DateTime2
       )
      {
        target.Append(Precision);
      }

    }
    #endregion
  }
}
