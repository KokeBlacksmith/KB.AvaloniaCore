using Avalonia.Data.Converters;
using System.Globalization;

namespace KB.AvaloniaCore.Converters;
public abstract class GenericValueConverter<TInput, TOuput, TParameter> : IValueConverter
{
    /// <summary>
    /// Converts a value.
    /// <br/>If the value is not convertible, throw exception.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <returns>Converted value</returns>
    protected abstract TOuput m_Convert(TInput value, Type targetType, TParameter parameter, CultureInfo culture);

    /// <summary>
    /// Converts back a value.
    /// <br/>If the value is not convertible, throw exception.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <returns>Converted value</returns>
    protected abstract TInput m_ConvertBack(TOuput value, Type targetType, TParameter parameter, CultureInfo culture);

    /// <summary>
    /// Converts a value.
    /// <br/>This method should not throw exceptions. If the value is not convertible, return
    /// <br/>a <see cref="Avalonia.Data.BindingNotification"/> in an error state. Any exceptions thrown
    /// <br/>will be treated as an application exception.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <returns>Converted value</returns>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        try
        {
            return m_Convert((TInput)value, targetType, (TParameter)parameter, culture);
        }
        catch (Exception ex)
        {
            return Avalonia.Data.BindingNotification.ExtractError(ex.Message);
        }
    }

    /// <summary>
    /// Converts back a value.
    /// <br/>This method should not throw exceptions. If the value is not convertible, return
    /// <br/>a <see cref="Avalonia.Data.BindingNotification"/> in an error state. Any exceptions thrown
    /// <br/>will be treated as an application exception.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <returns>Converted value</returns>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        try
        {
            return m_ConvertBack((TOuput)value, targetType, (TParameter)parameter, culture);
        }
        catch (Exception ex)
        {
            return Avalonia.Data.BindingNotification.ExtractError(ex.Message);
        }
    }
}
