using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository.Professionaltranslator.Net;
using Repository.ProfessionalTranslator.Net;
using models = Models.Professionaltranslator.Net;
using repository = Repository.ProfessionalTranslator.Net.Image;

namespace Test.Professionaltranslator.Net.Repository
{
    [TestClass]
    public class Image
    {
        [TestMethod]
        public async Task Delete()
        {
            models.Image item = WriteItem();
            Result result = await repository.Delete(Constants.Site, item.Id);
            Assert.IsTrue(result.Status == SaveStatus.Succeeded);
        }

        [TestMethod]
        public async Task Item()
        {
            List<models.Image> list = await repository.List(Constants.Site);
            Guid? id = list[0].Id;
            if (!id.HasValue)
            {
                Assert.Fail("Id is null.");
            }
            
            models.Image item = await repository.Item(id.Value);
            Assert.IsTrue(item != null);
        }

        [TestMethod]
        public async Task List()
        {
            List<models.Image> list = await repository.List(Constants.Site);
            Assert.IsTrue(list.Any());
        }

        [TestMethod]
        public async Task SaveFailForInvalidSite()
        {
            models.Image item = WriteItem();
            Result result = await repository.Save(string.Empty, item);
            Assert.IsTrue(result.Status == SaveStatus.Failed);
        }

        [TestMethod]
        public async Task SaveFailForEmptyPath()
        {
            models.Image item = WriteItem();
            item.Path = string.Empty;
            Result result = await repository.Save(Constants.Site, item);
            Assert.IsTrue(result.Status == SaveStatus.Failed);
        }

        [TestMethod]
        public async Task SaveFailForPathLength()
        {
            models.Image item = WriteItem();
            item.Path = "Bygt2SRCOY4h1UeLu4cvIM9l4lTcfPLRb8CZvny91uhMDoUoL6jdbuPjSQDeKo3Xz2cOkChLVrZPqLu2ZfgToCB3OnVqFgWY8wlK2p6fZ253g2Gp982wgVL30eSEeYzVVozCkaIoBvnie6Bgds947aUoTLg4XFOsqOGRg8EtMCq7gnDteCQVNpxOdD5LGcKXmhsLdcKbRhVcbLe2fagg07Sz19JzWDMdz9JlQNXmwvlPabjfAFbyF0TRWjU0nIwXEyk5CX6uF5lZ8sZVeLSIHOFlPiVluTkRmec6Z7VaFFr0s0V4LHj5C0SZU7DRB8Sn6WilO7XHO0xgnBnxAk2H4WKppY9wab4mh23CTHkyf5gxkJoQIuvtAnuvcoZPgGdjy0TP0KfKlE5s0o3wYySzX6z4ScBygt2SRCOY4h1UeLu4cvIM9l4lTcfPLRb8CZvny91uhMDoUoL6jdbuPjSQDeKo3Xz2cOkChLVrZPqLu2ZfgToCB3OnVqFgWY8wlK2p6fZ253g2Gp982wgVL30eSEeYzVVozCkaIoBvnie6Bgds947aUoTLg4XFOsqOGRg8EtMCq7gnDteCQVNpxOdD5LGcKXmhsLdcKbRhVcbLe2fagg07Sz19JzWDMdz9JlQNXmwvlPabjfAFbyF0TRWjU0nIwXEyk5CX6uF5lZ8sZVeLSIHOFlPiVluTkRmec6Z7VaFFr0s0V4LHj5C0SZU7DRB8Sn6WilO7XHO0xgnBnxAk2H4WKppY9wab4mh23CTHkyf5gxkJoQIuvtAnuvcoZPgGdjy0TP0KfKlE5s0o3wYySzX6z4Sc";
            Result result = await repository.Save(Constants.Site, item);
            Assert.IsTrue(result.Status == SaveStatus.Failed);
        }

        [TestMethod]
        public async Task Save()
        {
            models.Image item = WriteItem();
            Result result = await repository.Save(Constants.Site, item);
            Assert.IsTrue(result.Status == SaveStatus.Succeeded);
        }

        internal static models.Image WriteItem()
        {
            var output = new models.Image
            {
                Id = Guid.Empty,
                Path = "https://test.com/test.jpg"
            };
            return output;
        }
    }
}
