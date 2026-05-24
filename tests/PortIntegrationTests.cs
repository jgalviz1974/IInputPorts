using Gasolutions.Core.Patterns.Result.Errors;

namespace Gasolutions.Core.Interfaces.Ports.Tests;

/// <summary>
/// Integration tests for Port interfaces.
/// Verifies real contracts: data integrity, result propagation and error handling
/// between input ports, use cases and output ports.
/// </summary>
public class PortIntegrationTests
{
    // -------------------------------------------------------------------------
    // IInputPort<T> — input contract
    // -------------------------------------------------------------------------

    /// <summary>
    /// Verifies that the exact request object reaches the input port (reference equality).
    /// </summary>
    [Fact]
    public async Task InputPort_Execute_ReceivesExactRequestReference()
    {
        // Arrange
        SpyInputPort<RequestDto> inputPort = new();
        RequestDto request = new() { Data = "exact-reference" };

        // Act
        await inputPort.Execute(request);

        // Assert — same object, not just an equal copy
        Assert.Same(request, inputPort.ReceivedEntity);
    }

    /// <summary>
    /// Verifies that IInputPort.Execute returns a completed ValueTask.
    /// </summary>
    [Fact]
    public void InputPort_Execute_ReturnsCompletedValueTask()
    {
        // Arrange
        SpyInputPort<RequestDto> inputPort = new();

        // Act
        ValueTask task = inputPort.Execute(new RequestDto { Data = "sync-check" });

        // Assert
        Assert.True(task.IsCompleted);
    }

    // -------------------------------------------------------------------------
    // IInputPort<T1, T2> — two-parameter contract
    // -------------------------------------------------------------------------

    /// <summary>
    /// Verifies that both parameters are received correctly by a two-parameter input port.
    /// </summary>
    [Fact]
    public async Task TwoParamInputPort_Execute_ReceivesBothParametersExactly()
    {
        // Arrange
        SpyInputPort<RequestDto, AuditDto> inputPort = new();
        RequestDto request = new() { Data = "main-data" };
        AuditDto audit = new() { UserId = "user-42" };

        // Act
        await inputPort.Execute(request, audit);

        // Assert
        Assert.Same(request, inputPort.ReceivedEntity1);
        Assert.Same(audit, inputPort.ReceivedEntity2);
    }

    /// <summary>
    /// Verifies that passing null as either parameter does not throw.
    /// </summary>
    [Fact]
    public async Task TwoParamInputPort_Execute_AcceptsNullParameters()
    {
        // Arrange
        SpyInputPort<RequestDto?, AuditDto?> inputPort = new();

        // Act
        await inputPort.Execute(null, null);

        // Assert
        Assert.Null(inputPort.ReceivedEntity1);
        Assert.Null(inputPort.ReceivedEntity2);
    }

    // -------------------------------------------------------------------------
    // IInputPort (non-generic) — parameterless contract
    // -------------------------------------------------------------------------

    /// <summary>
    /// Verifies that the non-generic IInputPort.Execute fires and completes.
    /// </summary>
    [Fact]
    public async Task NonGenericInputPort_Execute_FiresAndCompletes()
    {
        // Arrange
        SpyInputPort inputPort = new();

        // Act
        await inputPort.Execute();

        // Assert
        Assert.True(inputPort.WasExecuted);
    }

    /// <summary>
    /// Verifies that the non-generic IInputPort.Execute returns a completed ValueTask.
    /// </summary>
    [Fact]
    public void NonGenericInputPort_Execute_ReturnsCompletedValueTask()
    {
        // Arrange
        SpyInputPort inputPort = new();

        // Act
        ValueTask task = inputPort.Execute();

        // Assert
        Assert.True(task.IsCompleted);
    }

    // -------------------------------------------------------------------------
    // IOutputPort<T> — output contract with real Result<T> values
    // -------------------------------------------------------------------------

    /// <summary>
    /// Verifies that the output port receives a Success result whose Value
    /// is the exact object produced by the use case.
    /// </summary>
    [Fact]
    public async Task OutputPort_Handle_ReceivesSuccessResultWithCorrectPayload()
    {
        // Arrange
        SpyInputPort<RequestDto> inputPort = new();
        SpyOutputPort<ResponseDto> outputPort = new();
        GenericUseCase useCase = new(inputPort, outputPort);

        // Act
        await useCase.ExecuteAsync(new RequestDto { Data = "payload-check" });

        // Assert
        Assert.NotNull(outputPort.ReceivedResult);
        Assert.True(outputPort.ReceivedResult!.IsSuccess);
        Assert.Equal("Processed: payload-check", outputPort.ReceivedResult!.Value?.Message);
    }

    /// <summary>
    /// Verifies that when the use case produces a Failure, the output port receives
    /// a result whose IsFailure is true and the error code is preserved.
    /// </summary>
    [Fact]
    public async Task OutputPort_Handle_ReceivesFailureResult_WithCorrectErrorCode()
    {
        // Arrange
        SpyInputPort<RequestDto> inputPort = new();
        SpyOutputPort<ResponseDto> outputPort = new();
        FailingUseCase useCase = new(inputPort, outputPort);

        // Act
        await useCase.ExecuteAsync(new RequestDto { Data = "will-fail" });

        // Assert
        Assert.NotNull(outputPort.ReceivedResult);
        Assert.True(outputPort.ReceivedResult!.IsFailure);
        Assert.Equal("OtherErrors.NotDefined", outputPort.ReceivedResult!.Error?.Code);
    }

    /// <summary>
    /// Verifies that passing two different results in sequence keeps only the last one.
    /// </summary>
    [Fact]
    public void OutputPort_Handle_LastResultOverwritesPrevious()
    {
        // Arrange
        SpyOutputPort<string> outputPort = new();
        Result<string> first = Result<string>.Success("first");
        Result<string> second = Result<string>.Success("second");

        // Act
        outputPort.Handle(first);
        outputPort.Handle(second);

        // Assert
        Assert.Equal("second", outputPort.ReceivedResult?.Value);
    }

    // -------------------------------------------------------------------------
    // IOutputPort (non-generic) — parameterless output contract
    // -------------------------------------------------------------------------

    /// <summary>
    /// Verifies that the non-generic IOutputPort.Handle returns a Success result.
    /// </summary>
    [Fact]
    public async Task NonGenericOutputPort_Handle_ReturnsSuccessResult()
    {
        // Arrange
        SpyInputPort inputPort = new();
        SpyOutputPort outputPort = new();
        NonGenericUseCase useCase = new(inputPort, outputPort);

        // Act
        await useCase.ExecuteAsync();

        // Assert
        Assert.NotNull(outputPort.ReceivedResult);
        Assert.True(outputPort.ReceivedResult!.IsSuccess);
    }

    // -------------------------------------------------------------------------
    // Exception propagation
    // -------------------------------------------------------------------------

    /// <summary>
    /// Verifies that an exception thrown inside IInputPort&lt;T&gt;.Execute propagates
    /// to the caller of the use case.
    /// </summary>
    [Fact]
    public async Task InputPort_WhenExecuteThrows_ExceptionPropagates()
    {
        // Arrange
        ThrowingInputPort inputPort = new();
        SpyOutputPort<ResponseDto> outputPort = new();
        GenericUseCase useCase = new(inputPort, outputPort);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => useCase.ExecuteAsync(new RequestDto { Data = "throw" }).AsTask());
    }

    /// <summary>
    /// Verifies that an exception thrown inside the non-generic IInputPort.Execute
    /// propagates to the caller of the use case.
    /// </summary>
    [Fact]
    public async Task NonGenericInputPort_WhenExecuteThrows_ExceptionPropagates()
    {
        // Arrange
        ThrowingNonGenericInputPort inputPort = new();
        SpyOutputPort outputPort = new();
        NonGenericUseCase useCase = new(inputPort, outputPort);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => useCase.ExecuteAsync().AsTask());
    }

    // =========================================================================
    // Test doubles
    // =========================================================================

    private class SpyInputPort<T> : IInputPort<T>
    {
        public T? ReceivedEntity { get; private set; }

        public ValueTask Execute(T entity)
        {
            this.ReceivedEntity = entity;
            return ValueTask.CompletedTask;
        }
    }

    private class SpyInputPort<T1, T2> : IInputPort<T1, T2>
    {
        public T1? ReceivedEntity1 { get; private set; }
        public T2? ReceivedEntity2 { get; private set; }

        public ValueTask Execute(T1 entity1, T2 entity2)
        {
            this.ReceivedEntity1 = entity1;
            this.ReceivedEntity2 = entity2;
            return ValueTask.CompletedTask;
        }
    }

    private class SpyInputPort : IInputPort
    {
        public bool WasExecuted { get; private set; }

        public ValueTask Execute()
        {
            this.WasExecuted = true;
            return ValueTask.CompletedTask;
        }
    }

    private class ThrowingInputPort : IInputPort<RequestDto>
    {
        public ValueTask Execute(RequestDto entity) =>
            throw new InvalidOperationException("Simulated failure in input port.");
    }

    private class ThrowingNonGenericInputPort : IInputPort
    {
        public ValueTask Execute() =>
            throw new InvalidOperationException("Simulated non-generic failure.");
    }

    private class SpyOutputPort<T> : IOutputPort<T>
    {
        public Result<T>? ReceivedResult { get; private set; }

        public void Handle(Result<T> resultEntity) =>
            this.ReceivedResult = resultEntity;
    }

    private class SpyOutputPort : IOutputPort
    {
        public Result? ReceivedResult { get; private set; }

        public Result Handle()
        {
            Result result = Result.Success();
            this.ReceivedResult = result;
            return result;
        }
    }

    // =========================================================================
    // Fake use cases
    // =========================================================================

    private class GenericUseCase(IInputPort<RequestDto> inputPort, IOutputPort<ResponseDto> outputPort)
    {
        public async ValueTask ExecuteAsync(RequestDto request)
        {
            await inputPort.Execute(request);
            ResponseDto response = new() { Message = $"Processed: {request.Data}" };
            outputPort.Handle(Result<ResponseDto>.Success(response));
        }
    }

    private class FailingUseCase(IInputPort<RequestDto> inputPort, IOutputPort<ResponseDto> outputPort)
    {
        public async ValueTask ExecuteAsync(RequestDto request)
        {
            await inputPort.Execute(request);
            outputPort.Handle(Result<ResponseDto>.Failure(OtherErrors.NotDefined("Use case failed deliberately.")));
        }
    }

    private class NonGenericUseCase(IInputPort inputPort, IOutputPort outputPort)
    {
        public async ValueTask ExecuteAsync()
        {
            await inputPort.Execute();
            outputPort.Handle();
        }
    }

    // =========================================================================
    // DTOs
    // =========================================================================

    private class RequestDto
    {
        public string? Data { get; set; }
    }

    private class AuditDto
    {
        public string? UserId { get; set; }
    }

    private class ResponseDto
    {
        public string? Message { get; set; }
    }
}
