using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using TaskManager.Core.Commands;
using TaskManager.Core.Models;

namespace TaskManager.Core.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly List<TaskItem> _allTasks = [];
    private string _newTaskTitle = string.Empty;
    private string? _errorMessage;
    private TaskFilter _selectedFilter = TaskFilter.All;

    public MainViewModel()
    {
        AddTaskCommand = new RelayCommand(AddTask);
    }

    public ObservableCollection<TaskItem> VisibleTasks { get; } = [];

    public IReadOnlyList<FilterOption> Filters { get; } =
    [
        new(TaskFilter.All, "All"),
        new(TaskFilter.Completed, "Completed"),
        new(TaskFilter.NotCompleted, "Not Completed")
    ];

    public ICommand AddTaskCommand { get; }

    public string NewTaskTitle
    {
        get => _newTaskTitle;
        set
        {
            if (SetProperty(ref _newTaskTitle, value))
            {
                // Hide the validation message once the user starts typing again
                ErrorMessage = null;
            }
        }
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        private set => SetProperty(ref _errorMessage, value);
    }

    public TaskFilter SelectedFilter
    {
        get => _selectedFilter;
        set
        {
            if (SetProperty(ref _selectedFilter, value))
            {
                ApplyFilter();
            }
        }
    }

    private void AddTask()
    {
        var title = NewTaskTitle.Trim();
        if (title.Length == 0)
        {
            ErrorMessage = "Please enter a task title.";
            return;
        }

        var task = new TaskItem(title);
        task.PropertyChanged += OnTaskPropertyChanged;
        _allTasks.Add(task);

        if (Matches(task))
        {
            VisibleTasks.Add(task);
        }

        NewTaskTitle = string.Empty;
    }

    private void OnTaskPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // Completing a task can move it in or out of the filtered list
        if (e.PropertyName == nameof(TaskItem.IsCompleted) && SelectedFilter != TaskFilter.All)
        {
            ApplyFilter();
        }
    }

    private void ApplyFilter()
    {
        VisibleTasks.Clear();
        foreach (var task in _allTasks.Where(Matches))
        {
            VisibleTasks.Add(task);
        }
    }

    private bool Matches(TaskItem task) => SelectedFilter switch
    {
        TaskFilter.Completed => task.IsCompleted,
        TaskFilter.NotCompleted => !task.IsCompleted,
        _ => true
    };
}
