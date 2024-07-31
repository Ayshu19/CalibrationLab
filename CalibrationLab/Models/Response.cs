namespace CalibrationLab.Models
{
    public class Response<T>
    {
        public bool IsSuccess { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }
    }

    public class Response
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; }
    }
}