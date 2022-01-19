using Microsoft.AspNetCore.Http;

namespace src.Extensions
{
    public static class HttpContextExtensions
    {
        public static string GetTenantId(this HttpContext httpContext)
        {
            // ? pontosys.com/tenant-1/product -> " " / "tenant-1" / "product"
            var tenant = httpContext.Request.Path.Value.Split('/', System.StringSplitOptions.RemoveEmptyEntries)[0];

            // * pontosys.com/product/?tenantId=tenant-1

            // ? var tenant = httpContext.Request.Headers["tenant-id"];

            return tenant;
        }
    }
}