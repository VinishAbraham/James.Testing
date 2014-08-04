using System;
using System.Net.Http;
using System.Reflection;

namespace James.Testing.Rest
{
    internal static class HttpClientExtensions
    {
        internal static void AddHeaders(this HttpClient client, object headers)
        {
            if (headers == null) return;

            foreach (var property in headers.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                client.DefaultRequestHeaders.Add(property.Name.Replace("_", "-"),
                    property.GetValue(headers, null).ToString());
            }
        }

        internal static void AddQuery(this HttpClient client, object query)
        {
            if (query == null) return;
            if (client.BaseAddress == null) return;

            var uriBuilder = new UriBuilder(client.BaseAddress);

            foreach (var property in query.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                string queryToAppend = string.Format("{0}={1}", property.Name, property.GetValue(query, null));

                if (uriBuilder.Query != null && uriBuilder.Query.Length > 1)
                    uriBuilder.Query = uriBuilder.Query.Substring(1) + "&" + queryToAppend;
                else
                    uriBuilder.Query = queryToAppend;  
            }

            client.BaseAddress = uriBuilder.Uri;
        }

        internal static Uri With(this Uri uri, DynamicDictionary query)
        {
            if (query == null) return uri;
            if (uri == null) return null;

            var uriBuilder = new UriBuilder(uri);

            foreach (var name in query.GetMemberNames())
            {
                string queryToAppend = string.Format("{0}={1}", name, query.Get(name));

                if (uriBuilder.Query != null && uriBuilder.Query.Length > 1)
                    uriBuilder.Query = uriBuilder.Query.Substring(1) + "&" + queryToAppend;
                else
                    uriBuilder.Query = queryToAppend;
            }

            return uriBuilder.Uri;
        }
    }
}