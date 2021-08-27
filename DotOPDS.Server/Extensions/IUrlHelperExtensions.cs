using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace DotOPDS.Extensions;

internal static class IUrlHelperExtensions
{
    public static string RouteUrlRaw(this IUrlHelper helper, string routeName, object values)
    {
        var properties = from p in values.GetType().GetProperties()
                            where p.GetValue(values, null) != null
                            select $"{p.Name}={p.GetValue(values, null)}";

        return $"{helper.RouteUrl(routeName)}?{string.Join("&", properties.ToArray())}";
    }
}
