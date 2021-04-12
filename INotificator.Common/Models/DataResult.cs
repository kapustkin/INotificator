namespace INotificator.Common.Models
{
    public class DataResult<T>
    {
        /// <summary>
        /// Result data
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Result has error
        /// </summary>
        public bool HasError => ErrorMessage != null;

        /// <summary>
        /// Error message
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}