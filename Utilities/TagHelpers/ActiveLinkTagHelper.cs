#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;

namespace Utilities.TagHelpers
{
    [HtmlTargetElement(Attributes = ClassAttributeName)]
    [HtmlTargetElement(Attributes = PageAttributeName)]
    [HtmlTargetElement(Attributes = MatchLinkAttribute)]
    public class ActiveRouteTagHelper : TagHelper
    {
        private const string ClassAttributeName = "asp-active-class";
        private const string PageAttributeName = "asp-active-page";
        private const string MatchLinkAttribute = "asp-active-match";

        [HtmlAttributeName(PageAttributeName)] public string Page { get; set; } = default!;

        [HtmlAttributeName(ClassAttributeName)]
        public string Class { get; set; } = "active";

        [HtmlAttributeName(MatchLinkAttribute)]
        public MatchLinkType MatchType { get; set; }

        [HtmlAttributeNotBound] [ViewContext] public ViewContext ViewContext { get; set; } = default!;


        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            RouteValueDictionary routeValues = ViewContext.RouteData.Values;
            string? currentPage = routeValues["page"]?.ToString();

            if (string.IsNullOrEmpty(Page) && !string.IsNullOrEmpty(currentPage))
                Page = currentPage;

            string acceptedPage = Page.Trim();

            if (MatchType == MatchLinkType.All)
            {
                if (currentPage == acceptedPage)
                {
                    SetAttribute(output, "class", Class);
                }
            }
            else if (MatchType == MatchLinkType.Prefix)
            {
                if (currentPage.Contains(acceptedPage))
                {
                    SetAttribute(output, "class", Class);
                }
            }

            return base.ProcessAsync(context, output);
        }

        private void SetAttribute(TagHelperOutput output, string attributeName, string value, bool merge = true)
        {
            var v = value;
            if (output.Attributes.TryGetAttribute(attributeName, out TagHelperAttribute attribute))
            {
                if (merge)
                {
                    v = $"{attribute.Value} {value}";
                }
            }

            output.Attributes.SetAttribute(attributeName, v);
        }
    }

    public class LowerCaseComparer : IEqualityComparer<string?>
    {
        public bool Equals(string? x, string? y)
        {
            if (x == null || y == null)
            {
                return false;
            }

            return x.ToLowerInvariant().Equals(y.ToLowerInvariant());
        }

        public int GetHashCode(string obj)
        {
            return obj.GetHashCode();
        }
    }
}