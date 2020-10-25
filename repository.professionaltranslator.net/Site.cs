﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.Professionaltranslator.Net;
using dbRead = Repository.ProfessionalTranslator.Net.DatabaseOperations.dbo.Read.Site;
using dbWrite = Repository.ProfessionalTranslator.Net.DatabaseOperations.dbo.Write.Site;
using models = Models.Professionaltranslator.Net;

namespace Repository.ProfessionalTranslator.Net
{
    public class Site
    {
        internal static Tables.dbo.Site Convert(models.Site inputItem)
        {
            if (inputItem == null) return null;
            var output = new Tables.dbo.Site
            {
                Id = inputItem.Id ?? Guid.NewGuid(),
                Name = inputItem.Name
            };
            return output;
        }

        public static async Task<models.Site> Item(string enumerator)
        {
            Tables.dbo.Site item = await dbRead.Item(enumerator);
            if (item == null) return null;
            var output = new models.Site
            {
                Id = item.Id,
                Name = item.Name
            };
            return output;
        }

        public static async Task<List<models.Site>> List()
        {
            List<Tables.dbo.Site> list = await dbRead.List();
            return list.Select(n => new models.Site
            {
                Id = n.Id,
                Name = n.Name
            }).ToList();
        }

        public static async Task<Result> Save(models.Site item)
        {
            if (item == null)
            {
                return new Result(SaveStatus.Failed, "Site cannot be null.");
            }

            if (string.IsNullOrEmpty(item.Name))
            {
                return new Result(SaveStatus.Failed, "Name cannot be empty.");
            }

            if (item.Name.Length > 25)
            {
                return new Result(SaveStatus.Failed, "Name must be 25 characters or fewer.");
            }

            Tables.dbo.Site convertedSite = Convert(item);
            if (convertedSite == null)
            {
                return new Result(SaveStatus.Failed, "Could not convert Site model to table.");
            }

            Result saveResult = await dbWrite.Item(convertedSite);
            return saveResult;
        }
    }
}
