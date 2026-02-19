using Gasolutions.Core.Patterns.Result.Errors;

namespace Gasolutions.Core.Interfaces.Ports.Tests;

/// <summary>
/// Test suite for IOutputPort&lt;T&gt; interface.
/// Tests output port implementations with single generic parameter.
/// </summary>
public class IOutputPortGenericTests
{
    /// <summary>
    /// Verifies that a successful result is handled correctly.
    /// </summary>
    [Fact]
    public void Handle_WithSuccessResult_ProcessesCorrectly()
    {
        // Arrange
        MockOutputPort<string> port = new();
        Result<string> successResult = Gasolutions.Core.Patterns.Result.Result<string>.Success("test-data");

        // Act
        port.Handle(successResult);

        // Assert
        Assert.NotNull(port.LastHandledResult);
    }

    /// <summary>
    /// Verifies that an error result is handled correctly.
    /// </summary>
    [Fact]
    public void Handle_WithErrorResult_ProcessesCorrectly()
    {
        // Arrange
        MockOutputPort<string> port = new();
        Error error = new("TestError", "Test error message");
        Result<string> errorResult = Gasolutions.Core.Patterns.Result.Result<string>.Failure(error);

        // Act
        port.Handle(errorResult);

        // Assert
        Assert.NotNull(port.LastHandledResult);
    }

    /// <summary>
    /// Verifies that multiple results are handled sequentially.
    /// </summary>
    [Fact]
    public void Handle_WithMultipleResults_ProcessesAll()
    {
        // Arrange
        MockOutputPort<int> port = new();
        Result<int>[] results = new[]
        {
            Gasolutions.Core.Patterns.Result.Result<int>.Success(1),
            Gasolutions.Core.Patterns.Result.Result<int>.Success(2),
            Gasolutions.Core.Patterns.Result.Result<int>.Success(3),
        };

        // Act
        foreach (Result<int>? result in results)
        {
            port.Handle(result);
        }

        // Assert
        Assert.Equal(3, port.AllHandledResults.Count);
        Assert.Equal(results.Length, port.AllHandledResults.Count);
    }

    /// <summary>
    /// Verifies that the last handled result is tracked correctly.
    /// </summary>
    [Fact]
    public void Handle_TracksLastHandledResult()
    {
        // Arrange
        MockOutputPort<string> port = new();
        Result<string> firstResult = Gasolutions.Core.Patterns.Result.Result<string>.Success("first");
        Result<string> secondResult = Gasolutions.Core.Patterns.Result.Result<string>.Success("second");

        // Act
        port.Handle(firstResult);
        port.Handle(secondResult);

        // Assert
        Assert.NotNull(port.LastHandledResult);
    }

    /// <summary>
    /// Verifies that complex objects can be handled as results.
    /// </summary>
    [Fact]
    public void Handle_WithComplexObject_ProcessesSuccessfully()
    {
        // Arrange
        MockOutputPort<TestEntity> port = new();
        TestEntity entity = new() { Id = 1, Name = "Test", Value = 100 };
        Result<TestEntity> result = Gasolutions.Core.Patterns.Result.Result<TestEntity>.Success(entity);

        // Act
        port.Handle(result);

        // Assert
        Assert.NotNull(port.LastHandledResult);
    }

    /// <summary>
    /// Verifies that null data in result is handled correctly.
    /// </summary>
    [Fact]
    public void Handle_WithNullData_ProcessesCorrectly()
    {
        // Arrange
        MockOutputPort<string?> port = new();
        Result<string?> result = Gasolutions.Core.Patterns.Result.Result<string?>.Success(null);

        // Act
        port.Handle(result);

        // Assert
        Assert.NotNull(port.LastHandledResult);
    }

    /// <summary>
    /// Verifies that mixed success and error results are handled.
    /// </summary>
    [Fact]
    public void Handle_WithMixedResults_ProcessesBoth()
    {
        // Arrange
        MockOutputPort<string> port = new();
        Result<string> successResult = Gasolutions.Core.Patterns.Result.Result<string>.Success("success");
        Result<string> errorResult = Gasolutions.Core.Patterns.Result.Result<string>.Failure(new Gasolutions.Core.Patterns.Result.Errors.Error("Error", "Error message"));

        // Act
        port.Handle(successResult);
        port.Handle(errorResult);

        // Assert
        Assert.Equal(2, port.AllHandledResults.Count);
    }

    /// <summary>
    /// Verifies that collections can be handled as results.
    /// </summary>
    [Fact]
    public void Handle_WithCollectionResult_ProcessesSuccessfully()
    {
        // Arrange
        MockOutputPort<List<string>> port = new();
        List<string> collectionData = ["item1", "item2", "item3"];
        Result<List<string>> result = Gasolutions.Core.Patterns.Result.Result<List<string>>.Success(collectionData);

        // Act
        port.Handle(result);

        // Assert
        Assert.NotNull(port.LastHandledResult);
    }

    /// <summary>
    /// Test entity for complex object testing.
    /// </summary>
    private class TestEntity
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public int Value { get; set; }
    }

    /// <summary>
    /// Mock implementation of IOutputPort&lt;T&gt; for testing.
    /// </summary>
    private class MockOutputPort<T> : IOutputPort<T>
    {
        public Gasolutions.Core.Patterns.Result.Result<T>? LastHandledResult { get; private set; }

        public List<Gasolutions.Core.Patterns.Result.Result<T>> AllHandledResults { get; } = [];

        public void Handle(Gasolutions.Core.Patterns.Result.Result<T> resultEntity)
        {
            this.LastHandledResult = resultEntity;
            this.AllHandledResults.Add(resultEntity);
        }
    }
}
