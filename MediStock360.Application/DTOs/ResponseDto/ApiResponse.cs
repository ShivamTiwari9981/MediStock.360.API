namespace MediStock360.Application.DTOs.ResponseDto
{
    public class ApiResponse<T>
    {
        public int ErrorNo { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public bool IsSuccess => ErrorNo == 0;

        public static ApiResponse<T> Success(T data, string message = "Success")
        {
            return new ApiResponse<T> { ErrorNo = 0, Message = message, Data = data };
        }

        public static ApiResponse<T> Fail(int errorNo = 1,string message ="")
        {
            return new ApiResponse<T> { ErrorNo = errorNo, Message = message };
        }
    }
}
