namespace RetailInMotion.Model.Dto.Message
{
    public class ResultDto<T>
        where T : class
    {
        public string Error { get; set; }
        public T Result { get; set; }

        public ResultDto() { }

        public ResultDto(T result)
        {
            Result = result;
            Error = null;
        }
        public ResultDto(string error)
        {
            Result = null;
            Error = error;
        }
    }
}
