using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;

namespace System.Diagnostics.UnitTesting
{
  /// <summary>
  /// An execution order with calculated dependencies
  /// </summary>
  /// <remarks>
  /// Makes multiple passes over the source set, building a "pyramid" of objects, starting with
  /// the objects that do not have any dependencies. The second pass includes objects that have dependencies
  /// in the previous pass, so on and so forth.
  /// </remarks>
  public class MultipassExecutionOrder
  {
    #region Fields
    private List<TypeTestProfileCollection> _passes = new List<TypeTestProfileCollection>();
    private readonly TypeTestProfileCollection _source;
    private ValidationErrorCollection _validationErrors = new ValidationErrorCollection();
    #endregion
    #region Properties

    public ValidationErrorCollection ValidationErrors
    {
      get { return _validationErrors; }
      set { _validationErrors = value; }
    }

    public TypeTestProfileCollection Source
    {
      get { return _source; }
    }

    public List<TypeTestProfileCollection> Passes
    {
      get { return _passes; }
      set { _passes = value; }
    }
    #endregion

    #region Constructors
    public MultipassExecutionOrder(TypeTestProfileCollection source)
    {
      #region Validation
      if (source == null)
        throw new ArgumentNullException("source");
      #endregion
      _source = source;
      ValidateDependencies();

      if(_validationErrors.Count==0)
        GenerateExecutionOrder();
    }

    private void ValidateDependencies()
    {
      foreach (TypeTestProfile profile in _source)
      {
        if (!profile.HasDependencies(_source))
        {
          StringCollection missing = new StringCollection();
          foreach(Type t in profile.Dependencies)
            if(!_source.Contains(t))
              missing.Add(t.FullName);
            else
              if(!_source[t].CanCreateInstance)
                missing.Add(t.FullName);

          string[] m = new string[missing.Count];
          missing.CopyTo(m,0);
          
          _validationErrors.Add(new ValidationError(profile, String.Format("Type '{0}' missing unit test dependencies: {1}", profile.Type.FullName, String.Join(",",m))));
        }
      }
    }
    #endregion
    #region Private Methods

    private void GenerateExecutionOrder()
    {
      TypeTestProfileCollection _unprofiled = new TypeTestProfileCollection();
      TypeTestProfileCollection _profiled = new TypeTestProfileCollection();

      _source.CopyTo(_unprofiled);

      while(_unprofiled.Count>0)
        _passes.Add(GeneratePass(_unprofiled, _profiled));
    }

    private TypeTestProfileCollection GeneratePass(TypeTestProfileCollection unprofiled, TypeTestProfileCollection profiled)
    {
      TypeTestProfileCollection ret = new TypeTestProfileCollection();
      foreach (TypeTestProfile profile in unprofiled)
        if (profile.HasDependencies(profiled))
          ret.Add(profile);

      foreach (TypeTestProfile profile in ret)
      {
        unprofiled.Remove(profile);
        profiled.Add(profile);
      }
      return ret;
    }
    #endregion
    #region Public Methods
    public TypeTestProfileCollection GetDisabledDependencies()
    {
      TypeTestProfileCollection ret = new TypeTestProfileCollection();

      foreach (TypeTestProfile profile in _source)
        if (profile.Enabled)
          foreach (Type dependency in profile.Dependencies)
            if (!_source[dependency].Enabled)
              if (!ret.Contains(profile))
                ret.Add(_source[dependency]);

      return ret;
    }
    #endregion
  }
}
