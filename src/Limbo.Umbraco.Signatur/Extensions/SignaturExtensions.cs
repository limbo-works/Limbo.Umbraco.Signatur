using System;
using System.Collections.Generic;
using System.Globalization;

namespace Limbo.Umbraco.Signatur.Extensions;

public static class SignaturExtensions {

    public static void Add(this List<KeyValuePair<string, IEnumerable<object?>>> list, string key, long value) {
        list.Add(new KeyValuePair<string, IEnumerable<object?>>(key, new object[] { value }));
    }

    public static void Add(this List<KeyValuePair<string, IEnumerable<object?>>> list, string key, string value) {
        list.Add(new KeyValuePair<string, IEnumerable<object?>>(key, new object[] { value }));
    }

    public static void Add(this List<KeyValuePair<string, IEnumerable<object?>>> list, string key, DateTimeOffset value) {
        list.Add(key, value.ToString("yyyyMMddHHmmss000", CultureInfo.InvariantCulture));
    }

}