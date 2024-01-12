namespace AVR
{
    [System.Serializable]
    public class Response<T>
    {
        public T data;
        public AVR.ResponseError error;
    }

    [System.Serializable]
    public class ResponseError
    {
        public string message;
        public string code;
        public string status;
    }
}