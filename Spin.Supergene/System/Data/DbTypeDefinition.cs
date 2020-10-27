using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace System.Data
{
  /// <summary>
  /// Describes a data type and meta-data describing that type
  /// </summary>
  /// <remarks>
  /// Most databases recognize data types with different meta-data as 
  /// completely different data types that must be cast. For example,
  /// a decimal(10,2) must be explicitly cast to a decimal(14,2).
  /// 
  /// This class allows that compare to happen by providing detailed 
  /// information about the type.
  /// </remarks>
  public abstract class DbTypeDefinition
  {
    #region DbType List
    //DbType.AnsiString
    //DbType.Binary
    //DbType.Byte
    //DbType.Boolean
    //DbType.Currency
    //DbType.Date
    //DbType.DateTime
    //DbType.Decimal
    //DbType.Double
    //DbType.Guid
    //DbType.Int16
    //DbType.Int32
    //DbType.Int64
    //DbType.Object
    //DbType.SByte
    //DbType.Single
    //DbType.String
    //DbType.Time
    //DbType.UInt16
    //DbType.UInt32
    //DbType.UInt64
    //DbType.VarNumeric
    //DbType.AnsiStringFixedLength
    //DbType.StringFixedLength
    //DbType.Xml


    //type==DbType.AnsiString||
    //type==DbType.Binary||
    //type==DbType.Byte||
    //type==DbType.Boolean||
    //type==DbType.Currency||
    //type==DbType.Date||
    //type==DbType.DateTime||
    //type==DbType.Decimal||
    //type==DbType.Double||
    //type==DbType.Guid||
    //type==DbType.Int16||
    //type==DbType.Int32||
    //type==DbType.Int64||
    //type==DbType.Object||
    //type==DbType.SByte||
    //type==DbType.Single||
    //type==DbType.String||
    //type==DbType.Time||
    //type==DbType.UInt16||
    //type==DbType.UInt32||
    //type==DbType.UInt64||
    //type==DbType.VarNumeric||
    //type==DbType.AnsiStringFixedLength||
    //type==DbType.StringFixedLength||
    //type==DbType.Xml

    #endregion

    #region Private Members
    private DbType _type;
    private int _size;
    private int _precision;
    private int _scale;
    #endregion
    #region Public Property Declarations

    public DbType Type
    {
      get { return _type; }
      protected set { _type = value; }
    }

    /// <summary>
    /// The size of a fixed-length type, or the max size of a variable-length type.
    /// </summary>
    public int Size
    {
      get { return _size; }
      protected set { _size = value; }
    }

    public int Precision
    {
      get { return _precision; }
      protected set { _precision = value; }
    }

    public int Scale
    {
      get { return _scale; }
      protected set { _scale = value; }
    }

    #endregion

    #region Constructors
    protected DbTypeDefinition(DbType type)
    {
      //NOTE: Validation should be handled by the derived class.
      #region Validation
      //if (type == DbType.AnsiString ||
      //  type == DbType.AnsiStringFixedLength |
      //  type == DbType.Binary ||
      //  type == DbType.String ||
      //  type == DbType.StringFixedLength ||
      //  type == DbType.Decimal ||
      //  type == DbType.VarNumeric
      //  )
      //  throw new ArgumentException(String.Format("DbType '{0}' requires additional information. Use another constructor.", type.ToString()), "type");
      #endregion

      _type = type;
    }

    protected DbTypeDefinition(DbType type, int size)
    {
      #region Validation
      //if (
      //  type == DbType.Byte ||
      //  type == DbType.Boolean ||
      //  type == DbType.Currency ||
      //  type == DbType.Date ||
      //  type == DbType.DateTime ||
      //  type == DbType.Decimal ||
      //  type == DbType.Double ||
      //  type == DbType.Guid ||
      //  type == DbType.Int16 ||
      //  type == DbType.Int32 ||
      //  type == DbType.Int64 ||
      //  type == DbType.SByte ||
      //  type == DbType.Single ||
      //  type == DbType.Time ||
      //  type == DbType.UInt16 ||
      //  type == DbType.UInt32 ||
      //  type == DbType.UInt64 ||
      //  type == DbType.Xml
      //)
      //{
      //  throw new ArgumentException(String.Format("DbType '{0}' is not a variable-length type",type.ToString()), "type");
      //}
      #endregion

      _type = type;
      _size = size;
    }

    protected DbTypeDefinition(DbType type, int precision, int scale)
    {
      _type = type;
      _scale = scale;
      _precision = precision;
    }

    protected DbTypeDefinition(DbType type, int size, int precision, int scale)
    {
      _type = type;
      _scale = scale;
      _precision = precision;
      _size = size;
    }


    #endregion

    #region Abstract Methods
    /// <summary>
    /// Creates a SQL syntactically-correct string representing the data type
    /// </summary>
    /// <returns>A SQL code fragment representing the data type</returns>
    /// <remarks>
    /// This was almost taken out, but since the Type property is a SqlDbType,
    /// it's implied that the data type will be Sql-compatable. If not,
    /// then a NotSupportedException should be thrown.
    /// 
    /// Actually, it's DbType :)
    /// </remarks>
    //public abstract string ToSql();
    #endregion
  }
}
