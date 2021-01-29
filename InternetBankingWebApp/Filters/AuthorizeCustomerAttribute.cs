using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using InternetBankingWebApp.Models;

namespace InternetBankingWebApp.Filters
{
    public class AuthorizeCustomerAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Get session of customer ID from login page
            var customerID = context.HttpContext.Session.GetInt32(nameof(Customer.CustomerID));

            // If cannot get session from login page, redirect to login page.
            if(!customerID.HasValue)
                context.Result = new RedirectToActionResult("Login", "Login", null);
        }
    }
}
