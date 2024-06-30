using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SharedAPI
{
	public class ResponseDto<T> where T : class
	{
		public bool IsSuccessful { get; set; }

		public int StatusCode { get; set; }

		public T? Data { get; set; }

		public List<string>? Errors { get; set; }

		public override string ToString()
		{
			return JsonSerializer.Serialize(this);
		}

		public static ResponseDto<T> Success(T data, int StatusCode)
		{
			return new ResponseDto<T>
			{
				IsSuccessful = true,
				StatusCode = StatusCode,
				Data = data
			};
		}

		public static ResponseDto<T> Failure(int StatusCode, string? error = null, List<string>? errors = null)
		{
			var response = new ResponseDto<T>();
			response.StatusCode = StatusCode;

			if (error is not null)
			{
				response.Errors = new List<string> { error };
			}
			else
			{
				response.Errors = errors;
			}

			return response;
		}
	}
}
