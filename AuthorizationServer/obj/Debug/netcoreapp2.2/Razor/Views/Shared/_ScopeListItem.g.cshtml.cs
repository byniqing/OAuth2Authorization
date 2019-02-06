#pragma checksum "F:\OAuth2Authorization\AuthorizationServer\Views\Shared\_ScopeListItem.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "cf6e1284268ea31b28ab172ee6d5cec5cedd0374"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Shared__ScopeListItem), @"mvc.1.0.view", @"/Views/Shared/_ScopeListItem.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Shared/_ScopeListItem.cshtml", typeof(AspNetCore.Views_Shared__ScopeListItem))]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#line 1 "F:\OAuth2Authorization\AuthorizationServer\Views\_ViewImports.cshtml"
using AuthorizationServer;

#line default
#line hidden
#line 2 "F:\OAuth2Authorization\AuthorizationServer\Views\_ViewImports.cshtml"
using AuthorizationServer.Models;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"cf6e1284268ea31b28ab172ee6d5cec5cedd0374", @"/Views/Shared/_ScopeListItem.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"aba51f240eca2359e4cd8981e9837e96a8c2cb4e", @"/Views/_ViewImports.cshtml")]
    public class Views_Shared__ScopeListItem : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<AuthorizationServer.ViewModels.ScopeViewModel>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            BeginContext(54, 155, true);
            WriteLiteral("<li class=\"list-group-item\">\r\n    <label>\r\n        <input class=\"consent-scopecheck\"\r\n               type=\"checkbox\"\r\n               name=\"ScopesConsented\"");
            EndContext();
            BeginWriteAttribute("id", "\r\n               id=\"", 209, "\"", 248, 2);
            WriteAttributeValue("", 230, "scopes_", 230, 7, true);
#line 7 "F:\OAuth2Authorization\AuthorizationServer\Views\Shared\_ScopeListItem.cshtml"
WriteAttributeValue("", 237, Model.Name, 237, 11, false);

#line default
#line hidden
            EndWriteAttribute();
            BeginWriteAttribute("value", "\r\n               value=\"", 249, "\"", 284, 1);
#line 8 "F:\OAuth2Authorization\AuthorizationServer\Views\Shared\_ScopeListItem.cshtml"
WriteAttributeValue("", 273, Model.Name, 273, 11, false);

#line default
#line hidden
            EndWriteAttribute();
            BeginWriteAttribute("checked", "\r\n               checked=\"", 285, "\"", 325, 1);
#line 9 "F:\OAuth2Authorization\AuthorizationServer\Views\Shared\_ScopeListItem.cshtml"
WriteAttributeValue("", 311, Model.Checked, 311, 14, false);

#line default
#line hidden
            EndWriteAttribute();
            BeginWriteAttribute("disabled", "\r\n               disabled=\"", 326, "\"", 368, 1);
#line 10 "F:\OAuth2Authorization\AuthorizationServer\Views\Shared\_ScopeListItem.cshtml"
WriteAttributeValue("", 353, Model.Required, 353, 15, false);

#line default
#line hidden
            EndWriteAttribute();
            BeginContext(369, 5, true);
            WriteLiteral(" />\r\n");
            EndContext();
#line 11 "F:\OAuth2Authorization\AuthorizationServer\Views\Shared\_ScopeListItem.cshtml"
         if (Model.Required)
        {

#line default
#line hidden
            BeginContext(415, 75, true);
            WriteLiteral("            <input type=\"hidden\"\r\n                   name=\"ScopesConsented\"");
            EndContext();
            BeginWriteAttribute("value", "\r\n                   value=\"", 490, "\"", 529, 1);
#line 15 "F:\OAuth2Authorization\AuthorizationServer\Views\Shared\_ScopeListItem.cshtml"
WriteAttributeValue("", 518, Model.Name, 518, 11, false);

#line default
#line hidden
            EndWriteAttribute();
            BeginContext(530, 5, true);
            WriteLiteral(" />\r\n");
            EndContext();
#line 16 "F:\OAuth2Authorization\AuthorizationServer\Views\Shared\_ScopeListItem.cshtml"
        }

#line default
#line hidden
            BeginContext(546, 16, true);
            WriteLiteral("        <strong>");
            EndContext();
            BeginContext(563, 10, false);
#line 17 "F:\OAuth2Authorization\AuthorizationServer\Views\Shared\_ScopeListItem.cshtml"
           Write(Model.Name);

#line default
#line hidden
            EndContext();
            BeginContext(573, 27, true);
            WriteLiteral("</strong>\r\n        <strong>");
            EndContext();
            BeginContext(601, 17, false);
#line 18 "F:\OAuth2Authorization\AuthorizationServer\Views\Shared\_ScopeListItem.cshtml"
           Write(Model.DisplayName);

#line default
#line hidden
            EndContext();
            BeginContext(618, 11, true);
            WriteLiteral("</strong>\r\n");
            EndContext();
#line 19 "F:\OAuth2Authorization\AuthorizationServer\Views\Shared\_ScopeListItem.cshtml"
         if (Model.Emphasize)
        {

#line default
#line hidden
            BeginContext(671, 72, true);
            WriteLiteral("            <span class=\"glyphicon glyphicon-exclamation-sign\"></span>\r\n");
            EndContext();
#line 22 "F:\OAuth2Authorization\AuthorizationServer\Views\Shared\_ScopeListItem.cshtml"
        }

#line default
#line hidden
            BeginContext(754, 14, true);
            WriteLiteral("    </label>\r\n");
            EndContext();
#line 24 "F:\OAuth2Authorization\AuthorizationServer\Views\Shared\_ScopeListItem.cshtml"
     if (Model.Required)
    {

#line default
#line hidden
            BeginContext(801, 36, true);
            WriteLiteral("        <span><em>(必须)</em></span>\r\n");
            EndContext();
#line 27 "F:\OAuth2Authorization\AuthorizationServer\Views\Shared\_ScopeListItem.cshtml"
    }

#line default
#line hidden
            BeginContext(844, 4, true);
            WriteLiteral("    ");
            EndContext();
#line 28 "F:\OAuth2Authorization\AuthorizationServer\Views\Shared\_ScopeListItem.cshtml"
     if (Model.Description != null)
    {

#line default
#line hidden
            BeginContext(888, 61, true);
            WriteLiteral("        <div class=\"consent-description\">\r\n            <label");
            EndContext();
            BeginWriteAttribute("for", " for=\"", 949, "\"", 973, 2);
            WriteAttributeValue("", 955, "scopes_", 955, 7, true);
#line 31 "F:\OAuth2Authorization\AuthorizationServer\Views\Shared\_ScopeListItem.cshtml"
WriteAttributeValue("", 962, Model.Name, 962, 11, false);

#line default
#line hidden
            EndWriteAttribute();
            BeginContext(974, 1, true);
            WriteLiteral(">");
            EndContext();
            BeginContext(976, 17, false);
#line 31 "F:\OAuth2Authorization\AuthorizationServer\Views\Shared\_ScopeListItem.cshtml"
                                       Write(Model.Description);

#line default
#line hidden
            EndContext();
            BeginContext(993, 26, true);
            WriteLiteral("</label>\r\n        </div>\r\n");
            EndContext();
#line 33 "F:\OAuth2Authorization\AuthorizationServer\Views\Shared\_ScopeListItem.cshtml"
    }

#line default
#line hidden
            BeginContext(1026, 5, true);
            WriteLiteral("</li>");
            EndContext();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<AuthorizationServer.ViewModels.ScopeViewModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
