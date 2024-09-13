using System;


namespace SkycavePlayerService.Shared.Models.PlayerRecord
{
    /// <summary>
    /// The purpose of using the ValueObject base class, is simply
    /// to keep consistency i how values are retrieved from the domain primitives
    /// </summary>
    /// <typeparam name="T">Type of value that the object contains</typeparam>
    public abstract class ValueObjectBase<T>
    {
        public T Value { get; private set; }

        public ValueObjectBase(T value)
        {
            if (!IsValid(value))
            {
                throw new ArgumentException(
                        $"{this.GetType().Name} was initialized with value {value}, which is not within the constrains"
                    );
            }
            Value = value;
        }
        /// <summary>
        /// This methods ensures that all users of the ValueObjectBase
        /// have to consider what is a valid state for the value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected abstract bool IsValid(T value);

        

    }
}
