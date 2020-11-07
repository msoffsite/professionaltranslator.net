using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository.Professionaltranslator.Net;
using Repository.ProfessionalTranslator.Net;
using Image = Test.Professionaltranslator.Net.Repository.Image;
using models = Models.Professionaltranslator.Net;
using localized = Models.Professionaltranslator.Net.Localized;
using repository = Repository.ProfessionalTranslator.Net.Page;

namespace Test.Professionaltranslator.Net.Repository
{
    [TestClass]
    public class Page
    {
        [TestMethod]
        public async Task Delete()
        {
            models.Page item = WriteItem();
            Result result = await repository.Delete(Constants.Site, item.Id);
            Assert.IsTrue(result.Status == SaveStatus.Succeeded);
        }

        [TestMethod]
        public async Task ItemById()
        {
            List<models.Page> list = await SharedList();
            Guid? id = list[0].Id;
            if (!id.HasValue)
            {
                Assert.Fail("List is empty.");
            }

            models.Page item = await repository.Item(id);
            Assert.IsTrue(item != null);
        }

        [TestMethod]
        public async Task ItemByName()
        {
            models.Page item = await repository.Item(Constants.Site, "About");
            Assert.IsTrue(item != null);
        }

        [TestMethod]
        public async Task List()
        {
            List<models.Page> list = await SharedList();
            Assert.IsTrue(list.Count > 0);
        }

        [TestMethod]
        public async Task Save()
        {
            models.Page item = WriteItem();
            Result result = await repository.Save(Constants.Site, item);
            Assert.IsTrue(result.Status == SaveStatus.Succeeded);
        }

        private static async Task<List<models.Page>> SharedList()
        {
            return await repository.List(Constants.Site);
        }

        private static models.Page WriteItem()
        {
            var output = new models.Page
            {
                Id = Guid.Empty,
                Name = "Test",
                CanHaveImage = true,
                Image = Image.WriteItem(),
                IsService = true,
                Localization = LocalizedList()
            };
            return output;
        }

        private static List<localized.Page> LocalizedList()
        {
            var output = new List<localized.Page>();
            localized.Page item = new localized.Page
            {
                Lcid = 1033,
                Title = "Test Page",
                Html = "It is a truth universally acknowledged, that a single man in possession of a good fortune, must be in want of a wife."
            };
            output.Add(item);
            item = new localized.Page
            {
                Lcid = 3082,
                Title = "Página de prueba",
                Html = "Es una verdad universalmente reconocida, que un hombre soltero en posesión de una buena fortuna, debe estar necesitado de una esposa."
            };
            output.Add(item);
            return output;
        }
    }
}
