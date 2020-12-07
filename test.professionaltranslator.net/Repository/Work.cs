using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository.ProfessionalTranslator.Net;
using models = Models.ProfessionalTranslator.Net;
using repository = Repository.ProfessionalTranslator.Net.Work;

namespace Test.ProfessionalTranslator.Net.Repository
{
    [TestClass]
    public class Work
    {
        [TestMethod]
        public async Task Delete()
        {
            models.Work item = WriteItem();
            Result result = await repository.Delete(Constants.Site, item.Id);
            Assert.IsTrue(result.Status == SaveStatus.Succeeded);
        }

        [TestMethod]
        public async Task Item()
        {
            List<models.Work> list = await SharedList();
            Guid? id = list[0].Id;
            if (!id.HasValue)
            {
                Assert.Fail("List is empty.");
            }

            models.Work item = await repository.Item(id);
            Assert.IsTrue(item != null);
        }

        [TestMethod]
        public async Task List()
        {
            List<models.Work> list = await SharedList();
            Assert.IsTrue(list.Count > 0);
        }

        [TestMethod]
        public async Task Save()
        {
            models.Work item = WriteItem();
            Result result = await repository.Save(Constants.Site, item);
            Assert.IsTrue(result.Status == SaveStatus.Succeeded);
        }

        private static async Task<List<models.Work>> SharedList()
        {
            return await repository.List(Constants.Site);
        }

        internal static models.Work WriteItem()
        {
            var output = new models.Work
            {
                Id = Guid.Empty,
                Title = "Test",
                Authors = "Jane Austen Test",
                Cover = Image.WriteItem(),
                Href = "https://testurl.com",
                DateCreated = DateTime.Now,
                Display = false,
                TestimonialLink = "https://testlink.com"
            };
            return output;
        }
    }
}
