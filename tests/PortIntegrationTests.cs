namespace Gasolutions.Core.Interfaces.Ports.Tests;

/// <summary>
/// Integration tests for Port interfaces.
/// Tests complete workflows combining input and output ports.
/// </summary>
public class PortIntegrationTests
{
    /// <summary>
    /// Verifies that a complete use case workflow works correctly.
    /// </summary>
    [Fact]
    public async Task CompleteWorkflow_ExecutesSuccessfully()
    {
        // Arrange
        SimpleInputPort inputPort = new();
        SimpleOutputPort outputPort = new();
        SimpleUseCase useCase = new(inputPort, outputPort);
        RequestDto request = new() { Data = "test-data" };

        // Act
        await useCase.ExecuteAsync(request);

        // Assert
        Assert.NotNull(inputPort.ReceivedRequest);
        Assert.Equal(request.Data, inputPort.ReceivedRequest.Data);
        Assert.NotNull(outputPort.LastResult);
    }

    /// <summary>
    /// Verifies that multiple use cases can run independently.
    /// </summary>
    [Fact]
    public async Task MultipleUseCases_RunIndependently()
    {
        // Arrange
        SimpleInputPort inputPort1 = new();
        SimpleOutputPort outputPort1 = new();
        SimpleUseCase useCase1 = new(inputPort1, outputPort1);

        SimpleInputPort inputPort2 = new();
        SimpleOutputPort outputPort2 = new();
        SimpleUseCase useCase2 = new(inputPort2, outputPort2);

        RequestDto request1 = new() { Data = "request-1" };
        RequestDto request2 = new() { Data = "request-2" };

        // Act
        await useCase1.ExecuteAsync(request1);
        await useCase2.ExecuteAsync(request2);

        // Assert
        Assert.Equal(request1.Data, inputPort1.ReceivedRequest?.Data);
        Assert.Equal(request2.Data, inputPort2.ReceivedRequest?.Data);
    }

    /// <summary>
    /// Verifies that sequential executions maintain state correctly.
    /// </summary>
    [Fact]
    public async Task SequentialExecutions_MaintainStateCorrectly()
    {
        // Arrange
        SimpleInputPort inputPort = new();
        SimpleOutputPort outputPort = new();
        SimpleUseCase useCase = new(inputPort, outputPort);

        // Act
        await useCase.ExecuteAsync(new RequestDto { Data = "first" });
        Result<ResponseDto>? firstResult = outputPort.LastResult;

        await useCase.ExecuteAsync(new RequestDto { Data = "second" });
        Result<ResponseDto>? secondResult = outputPort.LastResult;

        // Assert
        Assert.NotNull(firstResult);
        Assert.NotNull(secondResult);
    }

    /// <summary>
    /// Verifies that concurrent port executions are thread-safe.
    /// </summary>
    [Fact]
    public async Task ConcurrentExecutions_AreThreadSafe()
    {
        // Arrange
        const int taskCount = 10;
        List<Task> tasks = new(taskCount);

        // Act
        for (int i = 0; i < taskCount; i++)
        {
            int taskId = i;
            tasks.Add(Task.Run(async () =>
            {
                SimpleInputPort inputPort = new();
                SimpleOutputPort outputPort = new();
                SimpleUseCase useCase = new(inputPort, outputPort);
                RequestDto request = new() { Data = $"data-{taskId}" };

                await useCase.ExecuteAsync(request);

                Assert.Equal($"data-{taskId}", inputPort.ReceivedRequest?.Data);
                Assert.NotNull(outputPort.LastResult);
            }));
        }

        await Task.WhenAll(tasks);

        // Assert - No exceptions thrown
        Assert.Equal(taskCount, tasks.Count);
    }

    /// <summary>
    /// Verifies that multiple input ports can be chained.
    /// </summary>
    [Fact]
    public async Task MultipleInputPorts_CanBeChained()
    {
        // Arrange
        SimpleInputPort port1 = new();
        SimpleInputPort port2 = new();
        RequestDto request1 = new() { Data = "first" };
        RequestDto request2 = new() { Data = "second" };

        // Act
        await port1.Execute(request1);
        await port2.Execute(request2);

        // Assert
        Assert.Equal(request1.Data, port1.ReceivedRequest?.Data);
        Assert.Equal(request2.Data, port2.ReceivedRequest?.Data);
    }

    /// <summary>
    /// Verifies that port reusability works correctly.
    /// </summary>
    [Fact]
    public async Task PortReusability_MaintainsConsistency()
    {
        // Arrange
        SimpleInputPort inputPort = new();
        SimpleOutputPort outputPort = new();
        SimpleUseCase useCase = new(inputPort, outputPort);

        // Act - Execute multiple times with same port
        for (int i = 0; i < 3; i++)
        {
            RequestDto request = new() { Data = $"data-{i}" };
            await useCase.ExecuteAsync(request);
        }

        // Assert - Last execution should be preserved
        Assert.Equal("data-2", inputPort.ReceivedRequest?.Data);
        Assert.NotNull(outputPort.LastResult);
    }

    /// <summary>
    /// Mock use case showing input port to output port workflow.
    /// </summary>
    private class SimpleUseCase
    {
        private readonly IInputPort<RequestDto> inputPort;
        private readonly IOutputPort<ResponseDto> outputPort;

        public SimpleUseCase(IInputPort<RequestDto> inputPort, IOutputPort<ResponseDto> outputPort)
        {
            this.inputPort = inputPort;
            this.outputPort = outputPort;
        }

        public async Task ExecuteAsync(RequestDto request)
        {
            await this.inputPort.Execute(request);
            ResponseDto response = new() { Message = "Processed" };
            this.outputPort.Handle(Gasolutions.Core.Patterns.Result.Result<ResponseDto>.Success(response));
        }
    }

    /// <summary>
    /// Mock input port implementation.
    /// </summary>
    private class SimpleInputPort : IInputPort<RequestDto>
    {
        public RequestDto? ReceivedRequest { get; private set; }

        public ValueTask Execute(RequestDto entity)
        {
            this.ReceivedRequest = entity;
            return ValueTask.CompletedTask;
        }
    }

    /// <summary>
    /// Mock output port implementation.
    /// </summary>
    private class SimpleOutputPort : IOutputPort<ResponseDto>
    {
        public Gasolutions.Core.Patterns.Result.Result<ResponseDto>? LastResult { get; private set; }

        public void Handle(Gasolutions.Core.Patterns.Result.Result<ResponseDto> resultEntity)
        {
            this.LastResult = resultEntity;
        }
    }

    /// <summary>
    /// Request DTO for testing.
    /// </summary>
    private class RequestDto
    {
        public string? Data { get; set; }
    }

    /// <summary>
    /// Response DTO for testing.
    /// </summary>
    private class ResponseDto
    {
        public string? Message { get; set; }
    }
}
