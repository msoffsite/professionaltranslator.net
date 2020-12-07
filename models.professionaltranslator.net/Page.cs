using System.Collections.Generic;

namespace Models.ProfessionalTranslator.Net
{
    public class Page : Base
    {
        public string Name { get; set; }
        public bool IsService { get; set; }
        public bool CanHaveImage { get; set; }
        public Image Image { get; set; }
        public List<Localized.Page> Bodies { get; set; }
        public List<Localized.PageHeader> Headers { get; set; }
    }
}
