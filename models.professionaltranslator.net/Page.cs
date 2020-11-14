using System.Collections.Generic;

namespace Models.Professionaltranslator.Net
{
    public class Page : Base
    {
        public string Name { get; set; }
        public bool IsService { get; set; }
        public bool CanHaveImage { get; set; }
        public Image Image { get; set; }
        public List<Localized.Page> Localization { get; set; }
    }
}
