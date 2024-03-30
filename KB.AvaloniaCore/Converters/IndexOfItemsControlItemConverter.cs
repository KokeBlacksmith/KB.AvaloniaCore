using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using KB.AvaloniaCore.Injection;
using System.Globalization;

namespace KB.AvaloniaCore.Converters;

/// <summary>
/// To bind the index of the item in the list to the control.
/// <br></br>Can show the index of the item in the list.
/// <br/><br/>
/// It expects a <see cref="ContentPresenter"/> as value and returns the index of the item in the list.
/// 
/// </summary>
public class IndexOfItemsControlItemConverter : GenericValueConverter<ContentPresenter, int, object?>
{
    protected override int m_Convert(ContentPresenter value, Type targetType, object? parameter, CultureInfo culture)
    {
        ItemsControl? itemsControl = value.FindParentOfType<ItemsControl>();
        if (itemsControl == null)
        {
            throw new InvalidOperationException("ItemsControl not found");
        }
        
        int index = itemsControl.IndexFromContainer(value);
        return index;
    }

    protected override ContentPresenter m_ConvertBack(int value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
