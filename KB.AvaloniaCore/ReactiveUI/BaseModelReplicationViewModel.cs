using System.Reflection;

namespace KB.AvaloniaCore.ReactiveUI;

public abstract class BaseModelReplicationViewModel<TModel> : BaseViewModel
{
    private TModel _model;
    /// <summary>
    /// Collection of ViewModel properties and their replicated properties in the Model.
    /// <br/> ViewModel <see cref="PropertyInfo"/> -> Model <see cref="PropertyInfo"/>
    /// </summary>
    private readonly Dictionary<PropertyInfo, PropertyInfo> _propertiesToSync;

    public BaseModelReplicationViewModel(TModel model)
    {
        _propertiesToSync = new Dictionary<PropertyInfo, PropertyInfo>();
        Model = model ?? throw new ArgumentNullException(nameof(model));
        SyncFromModel();
    }

    public TModel Model
    {
        get { return _model; }
        set
        {
            m_SetProperty(ref _model, value);
            _UpdatePropertyMapFromModel();
        }
    }

    private void _UpdatePropertyMapFromModel()
    {
        _propertiesToSync.Clear();

        PropertyInfo[] viewModelProperties = this.GetType().GetProperties();

        foreach (PropertyInfo viewModelPropertyInfo in viewModelProperties)
        {
            object[] attributes = viewModelPropertyInfo.GetCustomAttributes(true);

            PropertyReplicationNameAttribute? attr = attributes.FirstOrDefault(attr => attr is PropertyReplicationNameAttribute) as PropertyReplicationNameAttribute;
            if (attr == null)
            {
                continue;
            }

            string modelPropertyName = attr.PropertyName;
            PropertyInfo? modelPropertyInfo = typeof(TModel).GetProperty(modelPropertyName);

            if(modelPropertyInfo == null)
            {
                continue;
            }

            _propertiesToSync.Add(viewModelPropertyInfo, modelPropertyInfo);
        }
    }

    public virtual void UpdateModel()
    {
        foreach ((PropertyInfo vmProp, PropertyInfo modelProp) in _propertiesToSync)
        {
            modelProp.SetValue(Model, vmProp.GetValue(this));
        }
    }

    public virtual void SyncFromModel()
    {
        foreach ((PropertyInfo vmProp, PropertyInfo modelProp) in _propertiesToSync)
        {
            vmProp.SetValue(this, modelProp.GetValue(Model));
        }
    }
}

[AttributeUsage(AttributeTargets.Property)]
public sealed class PropertyReplicationNameAttribute : Attribute
{
    public string PropertyName { get; private set; }

    public PropertyReplicationNameAttribute(string propertyName)
    {
        PropertyName = propertyName;
    }
}
