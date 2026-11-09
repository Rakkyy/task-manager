using TaskManager.Core.Models;
using TaskManager.Core.ViewModels;

namespace TaskManager.Tests;

public class MainViewModelTests
{
    private static MainViewModel CreateWithTask(string title)
    {
        var vm = new MainViewModel { NewTaskTitle = title };
        vm.AddTaskCommand.Execute(null);
        return vm;
    }

    [Fact]
    public void AddTask_TrimsTitle_AddsItNotCompleted_AndClearsInput()
    {
        var vm = CreateWithTask("  Write report  ");

        var task = Assert.Single(vm.VisibleTasks);
        Assert.Equal("Write report", task.Title);
        Assert.False(task.IsCompleted);
        Assert.Equal(string.Empty, vm.NewTaskTitle);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void AddTask_EmptyOrWhitespace_IsRejectedWithError(string title)
    {
        var vm = CreateWithTask(title);

        Assert.Empty(vm.VisibleTasks);
        Assert.NotNull(vm.ErrorMessage);
    }

    [Fact]
    public void Typing_AfterValidationError_ClearsTheError()
    {
        var vm = CreateWithTask("");

        vm.NewTaskTitle = "a";

        Assert.Null(vm.ErrorMessage);
    }

    [Fact]
    public void Filter_ShowsOnlyMatchingTasks()
    {
        var vm = CreateWithTask("open");
        vm.NewTaskTitle = "done";
        vm.AddTaskCommand.Execute(null);
        vm.VisibleTasks.Single(t => t.Title == "done").IsCompleted = true;

        vm.SelectedFilter = TaskFilter.Completed;
        Assert.Equal("done", Assert.Single(vm.VisibleTasks).Title);

        vm.SelectedFilter = TaskFilter.NotCompleted;
        Assert.Equal("open", Assert.Single(vm.VisibleTasks).Title);

        vm.SelectedFilter = TaskFilter.All;
        Assert.Equal(2, vm.VisibleTasks.Count);
    }

    [Fact]
    public void CompletingTask_WhileFilteredToNotCompleted_RemovesItFromView()
    {
        var vm = CreateWithTask("task");
        vm.SelectedFilter = TaskFilter.NotCompleted;

        vm.VisibleTasks[0].IsCompleted = true;

        Assert.Empty(vm.VisibleTasks);
    }

    [Fact]
    public void AddTask_WhileFilteredToCompleted_DoesNotShowNewOpenTask()
    {
        var vm = new MainViewModel { SelectedFilter = TaskFilter.Completed };

        vm.NewTaskTitle = "new";
        vm.AddTaskCommand.Execute(null);

        Assert.Empty(vm.VisibleTasks);
        vm.SelectedFilter = TaskFilter.All;
        Assert.Single(vm.VisibleTasks);
    }
}
