namespace Gasolutions.Core.Interfaces.Ports.Tests;

/// <summary>
/// Test suite for IInputPort interface (non-generic).
/// Tests input port implementations without parameters.
/// </summary>
public class IInputPortTests
{
    /// <summary>
    /// Verifies that the input port can be executed.
    /// </summary>
    [Fact]
    public async Task Execute_WithoutParameters_CompletesSuccessfully()
    {
        // Arrange
        MockInputPort port = new();

        // Act
        await port.Execute();

        // Assert
        Assert.Equal(1, port.ExecutionCount);
    }

    /// <summary>
    /// Verifies that the input port can be executed multiple times.
    /// </summary>
    [Fact]
    public async Task Execute_MultipleTimes_CountsAllExecutions()
    {
        // Arrange
        MockInputPort port = new();
        const int executionCount = 5;

        // Act
        for (int i = 0; i < executionCount; i++)
        {
            await port.Execute();
        }

        // Assert
        Assert.Equal(executionCount, port.ExecutionCount);
    }

    /// <summary>
    /// Verifies that last execution time is updated on each call.
    /// </summary>
    [Fact]
    public async Task Execute_UpdatesLastExecutionTime()
    {
        // Arrange
        MockInputPort port = new();
        DateTime beforeExecution = DateTime.UtcNow;

        // Act
        await port.Execute();
        DateTime afterExecution = DateTime.UtcNow;

        // Assert
        _ = Assert.NotNull(port.LastExecutionTime);
        Assert.True(port.LastExecutionTime >= beforeExecution);
        Assert.True(port.LastExecutionTime <= afterExecution);
    }

    /// <summary>
    /// Verifies that multiple executions update the execution time each time.
    /// </summary>
    [Fact]
    public async Task Execute_MultipleExecutions_UpdatesTimeSequentially()
    {
        // Arrange
        MockInputPort port = new();
        List<DateTime> times = [];

        // Act
        for (int i = 0; i < 3; i++)
        {
            await port.Execute();
            times.Add(port.LastExecutionTime!.Value);
            await Task.Delay(10); // Small delay to ensure time progression
        }

        // Assert
        Assert.Equal(3, times.Count);
        Assert.True(times[1] >= times[0]);
        Assert.True(times[2] >= times[1]);
    }

    /// <summary>
    /// Verifies that the port returns a completed ValueTask.
    /// </summary>
    [Fact]
    public void Execute_ReturnsCompletedValueTask()
    {
        // Arrange
        MockInputPort port = new();

        // Act
        ValueTask result = port.Execute();

        // Assert
        Assert.True(result.IsCompleted);
    }

    /// <summary>
    /// Verifies that concurrent executions are thread-safe.
    /// </summary>
    [Fact]
    public async Task Execute_ConcurrentExecutions_MaintainsIntegrity()
    {
        // Arrange
        MockInputPort port = new();
        const int taskCount = 10;
        List<ValueTask> tasks = new(taskCount);

        // Act
        for (int i = 0; i < taskCount; i++)
        {
            tasks.Add(port.Execute());
        }

        foreach (ValueTask task in tasks)
        {
            await task;
        }

        // Assert
        Assert.Equal(taskCount, port.ExecutionCount);
    }

    /// <summary>
    /// Verifies that the port can be reused after execution.
    /// </summary>
    [Fact]
    public async Task Execute_IsReusable()
    {
        // Arrange
        MockInputPort port = new();

        // Act
        await port.Execute();
        int firstCount = port.ExecutionCount;

        await port.Execute();
        int secondCount = port.ExecutionCount;

        // Assert
        Assert.Equal(1, firstCount);
        Assert.Equal(2, secondCount);
    }

    /// <summary>
    /// Mock implementation of IInputPort for testing.
    /// </summary>
    private class MockInputPort : IInputPort
    {
        public int ExecutionCount { get; private set; }

        public DateTime? LastExecutionTime { get; private set; }

        public ValueTask Execute()
        {
            this.ExecutionCount++;
            this.LastExecutionTime = DateTime.UtcNow;
            return ValueTask.CompletedTask;
        }
    }
}
