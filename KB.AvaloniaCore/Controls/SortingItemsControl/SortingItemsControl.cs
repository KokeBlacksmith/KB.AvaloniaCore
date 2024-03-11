using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using KB.AvaloniaCore.ReactiveUI;

namespace KB.AvaloniaCore.Controls;

public sealed class SortingItemsControl : ItemsControl
{
    private class DraggedItemInfo
    {
        public DraggedItemInfo(ContentPresenter? draggedItem, int draggedItemIndex)
        {
            DraggedItem = draggedItem;
            DraggedItemIndex = draggedItemIndex;
        }

        public ContentPresenter? DraggedItem;
        public int DraggedItemIndex;
    }

    private AdornerLayer? _adornerLayer;
    /// <summary>
    /// Container of the dragged item. Just feedback.
    /// </summary>
    private ContentPresenter? _adornerElement;
    private DraggedItemInfo? _draggedItem;
    private Point _start;

    public SortingItemsControl()
    {
        AddHandler(DragDrop.DropEvent, _OnDrop);
        AddHandler(DragDrop.DragOverEvent, _OnDragOver);
    }

    /// <summary>
    /// Change positions of items from old index to new index. (int, int)
    /// </summary>
    public static readonly StyledProperty<GenericCommand<(int, int)>?> SwapItemsCommandProperty = AvaloniaProperty.Register<SortingItemsControl, GenericCommand<(int, int)>?>(nameof(SwapItemsCommand));

    public GenericCommand<(int, int)>? SwapItemsCommand
    {
        get { return GetValue(SwapItemsCommandProperty); }
        set { SetValue(SwapItemsCommandProperty, value); }
    }


    private ContentPresenter? GetItemFromPosition(Point point)
    {
        int totalItemsCount = Items.Count;
        for (int i = 0; i < totalItemsCount; ++i)
        {
            Control? item = this.ContainerFromIndex(i);
            if(item != null)
            {
                // item.IsPointerOver is not working
                bool isPointerOver = item.Bounds.Contains(point);
                if(isPointerOver)
                {
                    return (ContentPresenter)item;
                }
            }
        }

        return null;
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        if(_draggedItem != null)
        {
            return;
        }

        if(e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            EnableDragItemSwap(e);
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        if(_draggedItem != null)
        {
            _EndItemDrag();
        }
    }

    private void EnableDragItemSwap(PointerEventArgs e)
    {
        if(SwapItemsCommand == null)
        {
            return;
        }

        ContentPresenter? draggedItemPresenter = GetItemFromPosition(e.GetPosition(this));
        if(draggedItemPresenter == null)
        {
            return;
        }

        _draggedItem = new DraggedItemInfo(draggedItemPresenter, IndexFromContainer(draggedItemPresenter));
        DataObject dragData = new DataObject();
        dragData.Set("SortingItemsControlDrag", _draggedItem);
        DragDrop.DoDragDrop(e, dragData, DragDropEffects.Move);
    }

    private void _OnDragOver(object? sender, DragEventArgs e)
    {
        if(_draggedItem == null)
        {
            return;
        }

        Point newPos = e.GetPosition(this);
        _RequestItemSwap(newPos);
    }

    private void _OnDrop(object? sender, DragEventArgs e)
    {
        if(_draggedItem == null)
        {
            return;
        }

        _RequestItemSwap(e.GetPosition(this));
        _EndItemDrag();
    }

    private void _EndItemDrag()
    {
        _draggedItem = null;
    }

    private void _RequestItemSwap(Point pointerPosition)
    {
        ContentPresenter? overItem = GetItemFromPosition(pointerPosition);
        if (overItem != null && overItem != _draggedItem!.DraggedItem)
        {
            int overItemIndex = IndexFromContainer(overItem);
            if (overItemIndex != _draggedItem.DraggedItemIndex && SwapItemsCommand != null && SwapItemsCommand.CanExecute((_draggedItem.DraggedItemIndex, overItemIndex)))
            {
                SwapItemsCommand.Execute((_draggedItem.DraggedItemIndex, overItemIndex));
                _draggedItem.DraggedItemIndex = overItemIndex;
            }
        }
    }
}
