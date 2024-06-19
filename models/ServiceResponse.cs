namespace AuthenBackend.models
{
    public class ServiceResponse<T> //Data เป็น property ที่มีประเภท T. T นี้คือ type parameter ที่คุณสามารถกำหนดเมื่อสร้าง instance ของ ServiceResponse<T>.
    {
        public T? Data { get; set; }
        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;
    }
}