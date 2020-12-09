namespace web.professionaltranslator.net
{
    public class SiteSettings
    {
        public string Culture { get; set; }
        public int Lcid { get; set; }
        public string Site { get; set; }
        public int PagingSize { get; set; }
        public string Postmaster { get; set; }
        public string PostmasterDisplayName { get; set; }
        public string DefaultToDisplayName { get; set; }
        public string DefaultTo { get; set; }
        public string SmtpServer { get; set; }
        public string SmtpPassword { get; set; }
        public int SmtpPort { get; set; } 
        public int SmtpPortSsl { get; set; }
    }
}
