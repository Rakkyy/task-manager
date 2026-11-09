using TaskManager.Core.ViewModels;

namespace TaskManager.Core.Models;

public sealed class TaskItem : ViewModelBase
{
    private bool _isCompleted;

    public TaskItem(string title)
    {
        Title = title;
    }

    public string Title { get; }

    public bool IsCompleted
    {
        get => _isCompleted;
        set
        {
            if (SetProperty(ref _isCompleted, value))
            {
                // StatusText depends on IsCompleted, so notify the UI about it as well
                OnPropertyChanged(nameof(StatusText));
            }
        }
    }

    public string StatusText => IsCompleted ? "Completed" : "Not Completed";
}
