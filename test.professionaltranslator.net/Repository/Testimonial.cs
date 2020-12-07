using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository.ProfessionalTranslator.Net;
using models = Models.ProfessionalTranslator.Net;
using localized = Models.ProfessionalTranslator.Net.Localized;
using repository = Repository.ProfessionalTranslator.Net.Testimonial;

namespace Test.ProfessionalTranslator.Net.Repository
{
    [TestClass]
    public class Testimonial
    {
        [TestMethod]
        public async Task Delete()
        {
            models.Testimonial item = WriteItem();
            Result result = await repository.Delete(Constants.Site, item.Id);
            Assert.IsTrue(result.Status == SaveStatus.Succeeded);
        }

        [TestMethod]
        public async Task Item()
        {
            List<models.Testimonial> list = await SharedList();
            Guid? id = list[0].Id;
            if (!id.HasValue)
            {
                Assert.Fail("List is empty.");
            }

            models.Testimonial item = await repository.Item(id);
            Assert.IsTrue(item != null);
        }

        [TestMethod]
        public async Task List()
        {
            List<models.Testimonial> list = await SharedList();
            Assert.IsTrue(list.Count > 0);
        }

        [TestMethod]
        public async Task Save()
        {
            models.Testimonial item = WriteItem();
            Result result = await repository.Save(Constants.Site, item);
            Assert.IsTrue(result.Status == SaveStatus.Succeeded);
        }

        private static async Task<List<models.Testimonial>> SharedList()
        {
            return await repository.List(Constants.Site);
        }

        private static models.Testimonial WriteItem()
        {
            var output = new models.Testimonial
            {
                Id = Guid.Empty,
                Work = Work.WriteItem(),
                Portrait = Image.WriteItem(),
                Name = "Test",
                EmailAddress = "test@emailaddress.com",
                DateCreated = DateTime.Now,
                Approved = true,
                Localization = LocalizedList()
            };
            return output;
        }

        private static List<localized.Testimonial> LocalizedList()
        {
            var output = new List<localized.Testimonial>();
            localized.Testimonial item = new localized.Testimonial
            {
                Lcid = 1033,
                Html = "It is a truth universally acknowledged, that a single man in possession of a good fortune, must be in want of a wife."
            };
            output.Add(item);
            item = new localized.Testimonial
            {
                Lcid = 3082,
                Html = "Es una verdad universalmente reconocida, que un hombre soltero en posesión de una buena fortuna, debe estar necesitado de una esposa."
            };
            output.Add(item);
            return output;
        }
    }
}
