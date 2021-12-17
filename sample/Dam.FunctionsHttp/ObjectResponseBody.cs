using Microsoft.AspNetCore.Mvc;

using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Dam.Model.WebModels
{
    public class ObjectResponseBody<T> : IActionResult
    {
        public static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
        {
#if DEBUG
            WriteIndented = true
#endif
        };
        private readonly T value;
        private readonly HttpStatusCode statusCode;

        public ObjectResponseBody(T value, HttpStatusCode statusCode)
        {
            this.value = value;
            this.statusCode = statusCode;
        }
        public async Task ExecuteResultAsync(ActionContext context)
        {
            context.HttpContext.Response.StatusCode = (int)this.statusCode;
            await JsonSerializer.SerializeAsync(context.HttpContext.Response.Body, this.value, SerializerOptions, context.HttpContext.RequestAborted);            
        }
    }
}
