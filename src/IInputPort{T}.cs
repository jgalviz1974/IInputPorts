using Gasolutions.Core.Patterns.Result;

namespace Gasolutions.Core.Interfaces.Ports
{
    /// <summary>
    /// Interface for input port with a single generic parameter.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    public interface IInputPort<T>
    {
        /// <summary>
        /// Executes the input port action with the given entity.
        /// </summary>
        /// <param name="entity">The entity to process.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        ValueTask Execute(T entity);
    }

    /// <summary>
    /// Interface for input port without generic parameters.
    /// </summary>
    public interface IInputPort
    {
        /// <summary>
        /// Executes the input port action.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        ValueTask Execute();
    }

    /// <summary>
    /// Interface for output port with a single generic parameter.
    /// </summary>
    /// <typeparam name="T">The type of the result entity.</typeparam>
    public interface IOutputPort<T>
    {
        /// <summary>
        /// Handles the result of the output port action.
        /// </summary>
        /// <param name="resultEntity">The result entity to handle.</param>
        void Handle(Result<T> resultEntity);
    }

    /// <summary>
    /// Interface for output port without generic parameters.
    /// </summary>
    public interface IOutputPort
    {
        /// <summary>
        /// Handles the result of the output port action.
        /// </summary>
        /// <returns>The result of the action.</returns>
        Result Handle();
    }

    /// <summary>
    /// Interface for input port with two generic parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first entity.</typeparam>
    /// <typeparam name="T2">The type of the second entity.</typeparam>
    public interface IInputPort<T1, T2>
    {
        /// <summary>
        /// Executes the input port action with the given entities.
        /// </summary>
        /// <param name="entity1">The first entity to process.</param>
        /// <param name="entity2">The second entity to process.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        ValueTask Execute(T1 entity1, T2 entity2);
    }
}
