namespace Gasolutions.Core.Interfaces.Ports.Tests;

/// <summary>
/// Test suite for IInputPort&lt;T1, T2&gt; interface.
/// Tests input port implementations with two generic parameters.
/// </summary>
public class IInputPortTwoParametersTests
{
    /// <summary>
    /// Verifies that two entities can be executed successfully.
    /// </summary>
    [Fact]
    public async Task Execute_WithTwoValidEntities_CompletesSuccessfully()
    {
        // Arrange
        MockInputPortTwoParameters<string, int> port = new();
        const string entity1 = "test";
        const int entity2 = 42;

        // Act
        await port.Execute(entity1, entity2);

        // Assert
        _ = Assert.Single(port.ReceivedEntities);
        Assert.Equal((entity1, entity2), port.ReceivedEntities[0]);
    }

    /// <summary>
    /// Verifies that multiple pairs of entities can be executed sequentially.
    /// </summary>
    [Fact]
    public async Task Execute_WithMultiplePairs_ProcessesAllSuccessfully()
    {
        // Arrange
        MockInputPortTwoParameters<string, int> port = new();
        (string, int)[] pairs = new[] { ("first", 1), ("second", 2), ("third", 3) };

        // Act
        foreach ((string? entity1, int entity2) in pairs)
        {
            await port.Execute(entity1, entity2);
        }

        // Assert
        Assert.Equal(pairs.Length, port.ReceivedEntities.Count);
        Assert.Equal(pairs.ToList(), port.ReceivedEntities!);
    }

    /// <summary>
    /// Verifies that the first parameter can be null.
    /// </summary>
    [Fact]
    public async Task Execute_WithNullFirstEntity_HandlesCorrectly()
    {
        // Arrange
        MockInputPortTwoParameters<string?, int> port = new();
        string? nullEntity = null;
        const int secondEntity = 42;

        // Act
        await port.Execute(nullEntity, secondEntity);

        // Assert
        _ = Assert.Single(port.ReceivedEntities);
        Assert.Null(port.ReceivedEntities[0].Value1);
        Assert.Equal(secondEntity, port.ReceivedEntities[0].Value2);
    }

    /// <summary>
    /// Verifies that the second parameter can be null.
    /// </summary>
    [Fact]
    public async Task Execute_WithNullSecondEntity_HandlesCorrectly()
    {
        // Arrange
        MockInputPortTwoParameters<int, string?> port = new();
        const int firstEntity = 42;
        string? nullEntity = null;

        // Act
        await port.Execute(firstEntity, nullEntity);

        // Assert
        _ = Assert.Single(port.ReceivedEntities);
        Assert.Equal(firstEntity, port.ReceivedEntities[0].Value1);
        Assert.Null(port.ReceivedEntities[0].Value2);
    }

    /// <summary>
    /// Verifies that both parameters can be null.
    /// </summary>
    [Fact]
    public async Task Execute_WithBothNull_HandlesCorrectly()
    {
        // Arrange
        MockInputPortTwoParameters<string?, int?> port = new();
        string? nullEntity1 = null;
        int? nullEntity2 = null;

        // Act
        await port.Execute(nullEntity1, nullEntity2);

        // Assert
        _ = Assert.Single(port.ReceivedEntities);
        Assert.Null(port.ReceivedEntities[0].Value1);
        Assert.Null(port.ReceivedEntities[0].Value2);
    }

    /// <summary>
    /// Verifies that complex objects can be used as both parameters.
    /// </summary>
    [Fact]
    public async Task Execute_WithComplexObjects_ProcessesSuccessfully()
    {
        // Arrange
        MockInputPortTwoParameters<TestEntity, TestEntity> port = new();
        TestEntity entity1 = new() { Id = 1, Name = "First" };
        TestEntity entity2 = new() { Id = 2, Name = "Second" };

        // Act
        await port.Execute(entity1, entity2);

        // Assert
        _ = Assert.Single(port.ReceivedEntities);
        Assert.Equal(entity1.Id, port.ReceivedEntities[0].Value1!.Id);
        Assert.Equal(entity2.Id, port.ReceivedEntities[0].Value2!.Id);
    }

    /// <summary>
    /// Verifies that collections can be used as parameters.
    /// </summary>
    [Fact]
    public async Task Execute_WithCollections_ProcessesSuccessfully()
    {
        // Arrange
        MockInputPortTwoParameters<List<string>, List<int>> port = new();
        List<string> collection1 = ["a", "b", "c"];
        List<int> collection2 = [1, 2, 3];

        // Act
        await port.Execute(collection1, collection2);

        // Assert
        _ = Assert.Single(port.ReceivedEntities);
        Assert.Equal(3, port.ReceivedEntities[0].Value1!.Count);
        Assert.Equal(3, port.ReceivedEntities[0].Value2!.Count);
    }

    /// <summary>
    /// Verifies that different data types work together.
    /// </summary>
    [Fact]
    public async Task Execute_WithDifferentTypes_ProcessesSuccessfully()
    {
        // Arrange
        MockInputPortTwoParameters<int, bool> port = new();
        const int intValue = 42;
        const bool boolValue = true;

        // Act
        await port.Execute(intValue, boolValue);

        // Assert
        _ = Assert.Single(port.ReceivedEntities);
        Assert.Equal(intValue, port.ReceivedEntities[0].Value1);
        Assert.Equal(boolValue, port.ReceivedEntities[0].Value2);
    }

    /// <summary>
    /// Verifies that the port returns a completed ValueTask.
    /// </summary>
    [Fact]
    public void Execute_ReturnsCompletedValueTask()
    {
        // Arrange
        MockInputPortTwoParameters<string, string> port = new();

        // Act
        ValueTask result = port.Execute("test1", "test2");

        // Assert
        Assert.True(result.IsCompleted);
    }

    /// <summary>
    /// Test entity for complex object testing.
    /// </summary>
    private class TestEntity
    {
        public int Id { get; set; }

        public string? Name { get; set; }
    }

    /// <summary>
    /// Mock implementation of IInputPort&lt;T1, T2&gt; for testing.
    /// </summary>
    private class MockInputPortTwoParameters<T1, T2> : IInputPort<T1, T2>
    {
        public List<(T1? Value1, T2? Value2)> ReceivedEntities { get; } = [];

        public ValueTask Execute(T1 entity1, T2 entity2)
        {
            this.ReceivedEntities.Add((entity1, entity2));
            return ValueTask.CompletedTask;
        }
    }
}
