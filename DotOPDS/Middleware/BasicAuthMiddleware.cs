using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotOPDS.Middleware
{
    public class BasicAuthMiddleware : OwinMiddleware
    {
        private static Dictionary<string, int> attempts = new Dictionary<string, int>();

        public BasicAuthMiddleware(OwinMiddleware next) : base(next)
        {
        }

        public async override Task Invoke(IOwinContext context)
        {
            var ip = context.Request.RemoteIpAddress;
            if (Settings.Instance.Authentication.Banned.Contains(ip))
            {
                context.Response.StatusCode = 451;
                return;
            }
            
            var auth = context.Request.Headers.Get("Authorization");
            if (!string.IsNullOrEmpty(auth) && auth.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
            {
                var token = auth.Substring("Basic ".Length).Trim();
                var userPass = Encoding.UTF8.GetString(Convert.FromBase64String(token)).Split(':');
                if (Settings.Instance.Authentication.Users.ContainsKey(userPass[0]))
                {
                    if (Settings.Instance.Authentication.Users[userPass[0]] == userPass[1])
                    {
                        lock (attempts)
                        {
                            if (attempts.ContainsKey(ip))
                            {
                                attempts.Remove(ip);
                            }
                        }
                        await Next.Invoke(context);
                        return;
                    }
                }
            }

            lock (attempts)
            {
                if (!attempts.ContainsKey(ip))
                {
                    attempts.Add(ip, 0);
                }
                else
                {
                    attempts[ip] += 1;
                }
            }
            if (attempts[ip] >= Settings.Instance.Authentication.Attempts)
            {
                lock (Settings.Instance.Authentication.Banned)
                {
                    Settings.Instance.Authentication.Banned.Add(ip);
                    Settings.Save();
                }
            }

            context.Response.StatusCode = 401;
            context.Response.Headers.Append("WWW-Authenticate", @"Basic realm=""DotOPDS""");
        }
    }

    public static class BasicAuthMiddlewareExtensions
    {
        public static IAppBuilder UseBasicAuthentication(this IAppBuilder app)
        {
            return app.Use(typeof(BasicAuthMiddleware));
        }
    }
}
