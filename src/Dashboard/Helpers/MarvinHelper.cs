using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace DR.Marvin.Dashboard.Helpers
{
    public static class MarvinHelper
    {
        private static readonly Regex UrnDetection = new Regex(@"urn:dr:marvin:(?<type>(job)):(?<key>[^""\s&]+)");
        private static readonly Regex HttpLinkDetection = new Regex(@"http://(\S*\(\S+\))*[^)\s]+");
        public static MvcHtmlString PrettyPrintWithLinks<T>(this HtmlHelper<T> helper, string value, Uri apiRoot)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            value = value.Replace("\n", "<br />");
            try
            {
                {
                    // HTTP-LINK DETECTION:
                    var httpMatch = HttpLinkDetection.Match(value);
                    if (httpMatch.Success)
                    {
                        var anchor = new TagBuilder("a");
                        anchor.Attributes.Add("href", httpMatch.Value);
                        anchor.SetInnerText(httpMatch.Value);
                        value = HttpLinkDetection.Replace(value, anchor.ToString(TagRenderMode.Normal));
                        goto done;
                    }
                }

                {
                    // URN DECTECTION:
                    var urnMatch = UrnDetection.Match(value);
                    if (urnMatch.Success)
                    {
                        string target;
                        switch (urnMatch.Groups["type"].Value)
                        {
                            //TODO:
                            case "job":
                                target =
                                    $"<a href=\"{JobDebugLink(apiRoot,urnMatch.Value)}\">{urnMatch.Value}</a>";
                                break;
                            default:
                                target = null;
                                break;
                        }
                        if (target != null)
                        {
                            value = UrnDetection.Replace(value, target);
                            goto done;
                        }
                    }
                }

            }
            catch (Exception e)
            {
                value += "<br /><b>Pretty Print Failed: </b>" + e.ToString().Replace("\n", "<br />");
            }

            done:
            return new MvcHtmlString(value);
        }

        public static MvcHtmlString IdenticonFromString<T>(this HtmlHelper<T> helper, string value, int size=20)
        {
            var urnMatch = UrnDetection.Match(value);
            if (urnMatch.Success)
            {
                var key = urnMatch.Groups["key"].Value.Replace("-", "");
                var gravatarUrl =
                    $"https://www.gravatar.com/avatar/{key}?d=retro&s={size}";
                return MvcHtmlString.Create($"<img src={gravatarUrl} alt=\"{key}\" height=\"{size}\" width=\"{size}\"/>");
            }
            return MvcHtmlString.Empty;
        }

        public static Uri JobDebugLink(Uri apiRoot, string urn)
        {
            return new Uri($"{apiRoot}api/Debug/GetJob?urn={HttpUtility.UrlEncode(urn)}");

        }

        public static Uri OnLink(Uri ondemandRoot, string sourceId)
        {
            return new Uri($"{ondemandRoot}log/Home/filter/{HttpUtility.UrlEncode(sourceId)}");
        }
    }
}