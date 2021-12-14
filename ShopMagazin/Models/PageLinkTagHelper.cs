using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;
using System.Text.Encodings.Web;

namespace ShopMagazin.Models
{
    public class PageLinkTagHelper : TagHelper
    {
        private IUrlHelperFactory urlHelperFactory;
        public PageLinkTagHelper(IUrlHelperFactory helperFactory)
        {
            urlHelperFactory = helperFactory;
        }
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }
        public PageViewModel PageModel { get; set; }
        public string PageAction { get; set; }

        [HtmlAttributeName(DictionaryAttributePrefix = "page-url-")]
        public Dictionary<string, object> PageUrlValues { get; set; } = new Dictionary<string, object>();

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "div";
            output.AddClass("catalog-pagination", HtmlEncoder.Default);

            // набор ссылок будет представлять список ul
            TagBuilder tag = new TagBuilder("nav");
            tag.AddCssClass("pagination");

            // формируем три ссылки - на текущую, предыдущую и следующую
            TagBuilder currentItem = CreateTag(PageModel.PageNumber, urlHelper);

            // создаем ссылку на предыдущую страницу, если она есть
            if (PageModel.HasPreviousPage)
            {
                TagBuilder link = new TagBuilder("a");
                link.AddCssClass("pagination__btn");
                PageUrlValues["page"] = PageModel.PageNumber-1;
                link.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
                link.InnerHtml.Append("Назад");
                tag.InnerHtml.AppendHtml(link);
            }

            for (int i = 1; i <= PageModel.TotalPages; i++)
            {
                TagBuilder tags = CreateTag(i, urlHelper);
                tag.InnerHtml.AppendHtml(tags);
            }

            //tag.InnerHtml.AppendHtml(currentItem);
            // создаем ссылку на следующую страницу, если она есть
            if (PageModel.HasNextPage)
            {
                TagBuilder link = new TagBuilder("a");
                link.AddCssClass("pagination__btn");
                PageUrlValues["page"] = PageModel.PageNumber + 1;
                link.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
                link.InnerHtml.Append("Вперёд");
                tag.InnerHtml.AppendHtml(link);
            }
            output.Content.AppendHtml(tag);
        }

        TagBuilder CreateTag(int pageNumber, IUrlHelper urlHelper)
        {
            TagBuilder link = new TagBuilder("a");
            if (pageNumber == this.PageModel.PageNumber)
                link.AddCssClass("pagination__btn_active");
            else
            {
                link.AddCssClass("pagination__btn");
                PageUrlValues["page"] = pageNumber;
                link.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
            }
            link.InnerHtml.Append(pageNumber.ToString());
            return link;
        }
    }
}
