using Ganss.Xss;

namespace LoginAPI.Services.HtmlSanitizerService
{
    public class HtmlSanitizerService
    {
        private readonly HtmlSanitizer _sanitizer;

        public HtmlSanitizerService()
        {
            _sanitizer = new HtmlSanitizer();
            _sanitizer.AllowedAttributes.Add("class");
            _sanitizer.AllowedAttributes.Add("style");
            _sanitizer.AllowedTags.Add("h1");
            _sanitizer.AllowedTags.Add("h2");
            _sanitizer.AllowedTags.Add("h3");
            _sanitizer.AllowedTags.Add("h4");
            _sanitizer.AllowedTags.Add("h5");
            _sanitizer.AllowedTags.Add("h6");
            _sanitizer.AllowedTags.Add("p");
            _sanitizer.AllowedTags.Add("a");
            _sanitizer.AllowedTags.Add("b");
            _sanitizer.AllowedTags.Add("i");
            _sanitizer.AllowedTags.Add("strong");
            _sanitizer.AllowedTags.Add("em");
            _sanitizer.AllowedTags.Add("u");
            _sanitizer.AllowedTags.Add("ul");
            _sanitizer.AllowedTags.Add("ol");
            _sanitizer.AllowedTags.Add("li");
            _sanitizer.AllowedTags.Add("blockquote");
            _sanitizer.AllowedTags.Add("code");
            _sanitizer.AllowedTags.Add("hr");
            _sanitizer.AllowedTags.Add("br");
            _sanitizer.AllowedTags.Add("div");
            _sanitizer.AllowedTags.Add("span");
            _sanitizer.AllowedTags.Add("img");
            _sanitizer.AllowedTags.Add("table");
            _sanitizer.AllowedTags.Add("thead");
            _sanitizer.AllowedTags.Add("tbody");
            _sanitizer.AllowedTags.Add("tfoot");
            _sanitizer.AllowedTags.Add("tr");
            _sanitizer.AllowedTags.Add("th");
            _sanitizer.AllowedTags.Add("td");
        }

        public string Sanitize(string input)
        {
            return _sanitizer.Sanitize(input);
        }
    }
}
