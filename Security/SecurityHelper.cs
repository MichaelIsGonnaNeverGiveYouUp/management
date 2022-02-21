using Model;
using System;
using System.Collections.Generic;

namespace Security;

public class SecurityHelper
{
    public static Response SecureResponse(Response response) {
        response = ApplyHSTS(response);
        return response;
    }
    public static Response ApplyHSTS(Response response) {
        if (response.headers.ContainsKey("Strict-Transport-Security")) {
            // We do the change to https if starts with http
            if (response.request_url.StartsWith("http://")) {
                response.request_url = response.request_url.Replace("http://","https://");
            }
        }
        return response;
    }
}