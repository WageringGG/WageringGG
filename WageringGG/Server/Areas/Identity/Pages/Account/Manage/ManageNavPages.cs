using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace WageringGG.Server.Areas.Identity.Pages.Account.Manage
{
    public static class ManageNavPages
    {
        public static string Email => "Email";
        public static string ChangePassword => "ChangePassword";
        public static string ExternalLogins => "ExternalLogins";
        public static string TwoFactorAuthentication => "TwoFactorAuthentication";
        public static string EmailNavClass(ViewContext viewContext) => PageNavClass(viewContext, Email);
        public static string ChangePasswordNavClass(ViewContext viewContext) => PageNavClass(viewContext, ChangePassword);
        public static string ExternalLoginsNavClass(ViewContext viewContext) => PageNavClass(viewContext, ExternalLogins);
        public static string TwoFactorAuthenticationNavClass(ViewContext viewContext) => PageNavClass(viewContext, TwoFactorAuthentication);

        private static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "is-active" : string.Empty;
        }
    }
}
