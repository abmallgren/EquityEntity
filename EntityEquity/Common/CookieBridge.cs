using System.Net;

namespace EntityEquity.Common
{
    public class CookieBridge
    {
        private IConfiguration _configuration;
        public string Name { 
            get
            {
                return _configuration["CookieBridge:CookieName"];
            }
        }
        private string Domain {
            get
            {
                return _configuration["CookieBridge:CookieDomain"];
            }
        }
        public string? Value { get; set; }
        public CookieBridge(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public CookieContainer CookieContainer
        {
            get
            {
                CookieContainer container = new();
                container.Add(Cookie);
                return container;
            }
        }
        private Cookie Cookie
        {
            get
            {
                if (String.IsNullOrEmpty(Value))
                {
                    throw new Exception("CookieBridge Value must be specified.");
                }
                return new Cookie()
                {
                    Name = Name,
                    Domain = Domain,
                    Value = Value
                };
            }
        }
    }
}
