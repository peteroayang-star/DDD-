using DddTemplate.Domain.TodoItems;

namespace DddTemplate.Domain.Tests.TodoItems;

public class TodoItemTests
{
    [Fact]
    public void Create_ShouldCreateTodoItem_WithValidData()
    {
        var title = "Test Todo";
        var description = "Test Description";

        var todoItem = TodoItem.Create(title, description);

        Assert.NotEqual(Guid.Empty, todoItem.Id);
        Assert.Equal(title, todoItem.Title);
        Assert.Equal(description, todoItem.Description);
        Assert.False(todoItem.IsCompleted);
    }

    [Fact]
    public void Create_ShouldThrowException_WhenTitleIsEmpty()
    {
        Assert.Throws<ArgumentException>(() => TodoItem.Create(""));
    }

    [Fact]
    public void MarkCompleted_ShouldSetIsCompletedToTrue()
    {
        var todoItem = TodoItem.Create("Test");

        todoItem.MarkCompleted();

        Assert.True(todoItem.IsCompleted);
    }

    [Fact]
    public void Rename_ShouldUpdateTitle()
    {
        var todoItem = TodoItem.Create("Old Title");
        var newTitle = "New Title";

        todoItem.Rename(newTitle);

        Assert.Equal(newTitle, todoItem.Title);
    }
}
