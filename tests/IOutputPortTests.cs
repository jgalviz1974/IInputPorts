namespace Gasolutions.Core.Interfaces.Ports.Tests;

/// <summary>
/// Test suite for IOutputPort interface (non-generic).
/// Tests output port implementations without generic parameters.
/// </summary>
public class IOutputPortTests
{
    /// <summary>
    /// Verifies that the output port can handle a result.
    /// </summary>
    [Fact]
    public void Handle_ReturnsSuccessResult()
    {
        // Arrange
        MockOutputPort port = new();

        // Act
        Result result = port.Handle();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
    }

    /// <summary>
    /// Verifies that multiple handle calls are tracked.
    /// </summary>
    [Fact]
    public void Handle_MultipleCalls_CountsAllCalls()
    {
        // Arrange
        MockOutputPort port = new();
        const int callCount = 5;

        // Act
        for (int i = 0; i < callCount; i++)
        {
            _ = port.Handle();
        }

        // Assert
        Assert.Equal(callCount, port.HandleCallCount);
    }

    /// <summary>
    /// Verifies that all handled results are stored.
    /// </summary>
    [Fact]
    public void Handle_StoresAllResults()
    {
        // Arrange
        MockOutputPort port = new();

        // Act
        for (int i = 0; i < 3; i++)
        {
            _ = port.Handle();
        }

        // Assert
        Assert.Equal(3, port.AllHandledResults.Count);
        Assert.All(port.AllHandledResults, r => Assert.True(r.IsSuccess));
    }

    /// <summary>
    /// Verifies that the last handled result is updated correctly.
    /// </summary>
    [Fact]
    public void Handle_UpdatesLastResult()
    {
        // Arrange
        MockOutputPort port = new();

        // Act
        _ = port.Handle();
        Result result2 = port.Handle();

        // Assert
        Assert.NotNull(port.LastHandledResult);
        Assert.Equal(result2, port.LastHandledResult);
    }

    /// <summary>
    /// Verifies that each handle call returns a result.
    /// </summary>
    [Fact]
    public void Handle_EachCallReturnsResult()
    {
        // Arrange
        MockOutputPort port = new();
        List<Result> results = [];

        // Act
        for (int i = 0; i < 3; i++)
        {
            results.Add(port.Handle());
        }

        // Assert
        Assert.Equal(3, results.Count);
        Assert.All(results, r => Assert.NotNull(r));
    }

    /// <summary>
    /// Verifies that results maintain consistency.
    /// </summary>
    [Fact]
    public void Handle_ResultsAreConsistent()
    {
        // Arrange
        MockOutputPort port = new();

        // Act
        Result result1 = port.Handle();
        Result result2 = port.Handle();

        // Assert
        Assert.True(result1.IsSuccess);
        Assert.True(result2.IsSuccess);
    }

    /// <summary>
    /// Verifies that the port can be called sequentially without issues.
    /// </summary>
    [Fact]
    public void Handle_SequentialCalls_Completes()
    {
        // Arrange
        MockOutputPort port = new();

        // Act & Assert - Should not throw
        for (int i = 0; i < 10; i++)
        {
            Result result = port.Handle();
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
        }
    }

    /// <summary>
    /// Mock implementation of IOutputPort for testing.
    /// </summary>
    private class MockOutputPort : IOutputPort
    {
        public Result? LastHandledResult { get; private set; }

        public List<Result> AllHandledResults { get; } = [];

        public int HandleCallCount { get; private set; }

        public Result Handle()
        {
            this.HandleCallCount++;
            Result result = Result.Success();
            this.LastHandledResult = result;
            this.AllHandledResults.Add(result);
            return result;
        }
    }
}
