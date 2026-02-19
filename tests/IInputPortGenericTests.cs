namespace Gasolutions.Core.Interfaces.Ports.Tests;

/// <summary>
/// Test suite for IInputPort&lt;T&gt; interface.
/// Tests input port implementations with single parameter.
/// </summary>
public partial class IInputPortGenericTests
{
    /// <summary>
    /// Verifies that a valid entity can be executed through the input port.
    /// </summary>
    [Fact]
    public async Task Execute_WithValidEntity_CompletesSuccessfully()
    {
        // Arrange
        MockInputPort<string> port = new();
        const string testEntity = "test-entity";

        // Act
        await port.Execute(testEntity);

        // Assert
        _ = Assert.Single(port.ReceivedEntities);
        Assert.Equal(testEntity, port.ReceivedEntities[0]);
    }

    /// <summary>
    /// Verifies that multiple entities can be executed sequentially.
    /// </summary>
    [Fact]
    public async Task Execute_WithMultipleEntities_ProcessesAllSuccessfully()
    {
        // Arrange
        MockInputPort<int> port = new();
        int[] entities = new[] { 1, 2, 3, 4, 5 };

        // Act
        foreach (int entity in entities)
        {
            await port.Execute(entity);
        }

        // Assert
        Assert.Equal(entities.Length, port.ReceivedEntities.Count);
        Assert.Equal(entities, port.ReceivedEntities);
    }

    /// <summary>
    /// Verifies that null entity is handled correctly.
    /// </summary>
    [Fact]
    public async Task Execute_WithNullEntity_HandlesCorrectly()
    {
        // Arrange
        MockInputPort<string?> port = new();
        string? nullEntity = null;

        // Act
        await port.Execute(nullEntity);

        // Assert
        _ = Assert.Single(port.ReceivedEntities);
        Assert.Null(port.ReceivedEntities[0]);
    }

    /// <summary>
    /// Verifies that complex objects can be executed.
    /// </summary>
    [Fact]
    public async Task Execute_WithComplexObject_ProcessesSuccessfully()
    {
        // Arrange
        MockInputPort<TestEntity> port = new();
        TestEntity testEntity = new() { Id = 1, Name = "Test" };

        // Act
        await port.Execute(testEntity);

        // Assert
        _ = Assert.Single(port.ReceivedEntities);
        Assert.Equal(testEntity.Id, port.ReceivedEntities[0]!.Id);
        Assert.Equal(testEntity.Name, port.ReceivedEntities[0]!.Name);
    }

    /// <summary>
    /// Verifies that the port returns a completed ValueTask.
    /// </summary>
    [Fact]
    public void Execute_ReturnsCompletedValueTask()
    {
        // Arrange
        MockInputPort<string> port = new();

        // Act
        ValueTask result = port.Execute("test");

        // Assert
        Assert.True(result.IsCompleted);
    }

    /// <summary>
    /// Verifies that the same entity can be executed multiple times.
    /// </summary>
    [Fact]
    public async Task Execute_WithSameEntity_ProcessesMultipleTimes()
    {
        // Arrange
        MockInputPort<string> port = new();
        const string sameEntity = "same-entity";

        // Act
        for (int i = 0; i < 3; i++)
        {
            await port.Execute(sameEntity);
        }

        // Assert
        Assert.Equal(3, port.ReceivedEntities.Count);
        Assert.All(port.ReceivedEntities, entity => Assert.Equal(sameEntity, entity));
    }

    /// <summary>
    /// Verifies that collections can be executed as entities.
    /// </summary>
    [Fact]
    public async Task Execute_WithCollectionEntity_ProcessesSuccessfully()
    {
        // Arrange
        MockInputPort<List<string>> port = new();
        List<string> collectionEntity = ["item1", "item2", "item3"];

        // Act
        await port.Execute(collectionEntity);

        // Assert
        _ = Assert.Single(port.ReceivedEntities);
        Assert.Equal(3, port.ReceivedEntities[0]!.Count);
    }

    /// <summary>
    /// Test entity for complex object testing.
    /// </summary>
    private class TestEntity
    {
        public int Id { get; set; }

        public string? Name { get; set; }
    }

    private class MockInputPort<T> : IInputPort<T>
    {
        /// <summary>
        /// Gets the list of received entities.
        /// </summary>
        public List<T?> ReceivedEntities { get; } = [];

        /// <summary>
        /// Executes the input port with an entity.
        /// </summary>
        /// <param name="entity">The entity to process.</param>
        /// <returns>A completed ValueTask.</returns>
        public ValueTask Execute(T entity)
        {
            this.ReceivedEntities.Add(entity);
            return ValueTask.CompletedTask;
        }
    }
}
