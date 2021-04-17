using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dbRead = Repository.ProfessionalTranslator.Net.DatabaseOperations.dbo.Read;
using dbLocalizedRead = Repository.ProfessionalTranslator.Net.DatabaseOperations.Localization.Read.Testimonial;
using dbWrite = Repository.ProfessionalTranslator.Net.DatabaseOperations.dbo.Write.Testimonial;
using models = Models.ProfessionalTranslator.Net;

namespace Repository.ProfessionalTranslator.Net
{
    public class Testimonial
    {
        public static async Task<Result> Delete(string site, Guid? id)
        {
            if (!id.HasValue)
            {
                return new Result(ResultStatus.Failed, "Id must be a valid GUID.");
            }

            return await dbWrite.Delete(site, id.Value);
        }

        public static async Task<models.Testimonial> Item(Guid? id)
        {
            if (!id.HasValue) return null;
            Tables.dbo.Testimonial testimonial = await dbRead.Testimonial.Item(id.Value);
            return await Item(testimonial);
        }

        public static async Task<models.Testimonial> Item(string site, Guid? workId)
        {
            if (!workId.HasValue) return null;
            models.Site siteItem = await Site.Item(site);
            if (siteItem?.Id == null) return null;
            Tables.dbo.Testimonial testimonial = await dbRead.Testimonial.Item(siteItem.Id, workId.Value);
            if (testimonial == null) return null;
            return await Item(testimonial);
        }


        // ReSharper disable once UnusedMember.Local
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        private static async Task<Tables.dbo.Testimonial> Item(Guid siteId, Guid workId)
        {
            Tables.dbo.Testimonial testimonial = await dbRead.Testimonial.Item(siteId, workId);
            return testimonial;
        }

        private static async Task<models.Testimonial> Item(Tables.dbo.Testimonial testimonial)
        {
            try
            {
                models.Image portrait = await Image.Item(testimonial.PortraitImageId);
                models.Work work = await Work.Item(testimonial.WorkId);
                if ((portrait == null) && (work?.Cover?.Id != null))
                {
                    portrait = await Image.Item(work.Cover.Id);
                }
                List<Tables.Localization.Testimonial> localizedList = await dbLocalizedRead.List(testimonial.Id);
                var output = new models.Testimonial
                {
                    Id = testimonial.Id,
                    Work = work,
                    Portrait = portrait,
                    Name = testimonial.Name,
                    EmailAddress = testimonial.EmailAddress,
                    DateCreated = testimonial.DateCreated,
                    Approved = testimonial.Approved,
                    Entries = localizedList.Select(n => new models.Localized.Testimonial
                    {
                        Lcid = n.Lcid,
                        Html = n.Html.Trim()
                    }).ToList()
                };
                return output;
            }
            catch (System.Exception ex)
            {
                Console.Write(ex.Message);
                return null;
            }
        }

        public static async Task<List<models.Testimonial>> List(string site)
        {
            List<Task<models.Testimonial>> taskList = await TaskList(site);
            if (taskList.Count == 0) return new List<models.Testimonial>();
            var output = new List<models.Testimonial>();
            for (var i = 0; 0 < taskList.Count; i++)
            {
                if (i == taskList.Count) break;
                models.Testimonial item = taskList[i].Result;
                if (!output.Contains(item))
                {
                    output.Add(item);
                }
            }

            return output;
        }

        private static async Task<List<Task<models.Testimonial>>> TaskList(string site)
        {
            if (string.IsNullOrEmpty(site)) return new List<Task<models.Testimonial>>();
            List<Tables.dbo.Testimonial> list = await dbRead.Testimonial.List(site);
            return Complete(list);
        }

        public static async Task<List<models.Testimonial>> List(string site, bool approved)
        {
            List<Task<models.Testimonial>> taskList = await TaskList(site, approved);
            if (taskList.Count == 0) return new List<models.Testimonial>();
            var output = new List<models.Testimonial>();
            for (var i = 0; 0 < taskList.Count; i++)
            {
                if (i == taskList.Count) break;
                models.Testimonial item = taskList[i].Result;
                if (!output.Contains(item))
                {
                    output.Add(item);
                }
            }

            return output;
        }

        private static async Task<List<Task<models.Testimonial>>> TaskList(string site, bool approved)
        {
            if (string.IsNullOrEmpty(site)) return new List<Task<models.Testimonial>>();
            List<Tables.dbo.Testimonial> list = await dbRead.Testimonial.List(site, approved);
            return Complete(list);
        }

        private static List<Task<models.Testimonial>> Complete(IEnumerable<Tables.dbo.Testimonial> testimonialList)
        {
            return testimonialList.Select(async n => await Item(n) ).ToList();
            /*
            return testimonialList.Select(async n => new models.Testimonial
            {
                Id = n.Id,
                Work = await Work.Item(n.WorkId),
                Portrait = await Image.Item(n.PortraitImageId),
                Name = n.Name,
                EmailAddress = n.EmailAddress,
                DateCreated = n.DateCreated,
                Approved = n.Approved,
                Localization = localizedList.Where(x => x.Id == n.Id).Select(t => new models.Localized.Testimonial
                {
                    Lcid = t.Lcid,
                    Html = t.Html
                }).ToList()
            }).ToList();
            */
        }

        /// <summary>
        /// Saves testimonial and child items.
        /// </summary>
        /// <instructions>
        /// Set inputItem.Id to null when creating a new object.
        /// </instructions>
        /// <param name="site">Name of site related to testimonial.</param>
        /// <param name="inputItem">Testimonial object.</param>
        /// <returns>Returns save status and messages. If successful, returns an identifier via ReturnId.</returns>
        public static async Task<Result> Save(string site, models.Testimonial inputItem)
        {
            var saveStatus = ResultStatus.Undetermined;
            var messages = new List<string>();

            if (inputItem == null)
            {
                return new Result(ResultStatus.Failed, "Testimonial cannot be null.");
            }

            Tables.dbo.Site siteItem = await dbRead.Site.Item(site);
            if (siteItem == null)
            {
                return new Result(ResultStatus.Failed, "No site was found with that name.");
            }

            Tables.dbo.Work convertedWork = Work.Convert(inputItem.Work, siteItem.Id);
            if (convertedWork == null)
            {
                return new Result(ResultStatus.Failed, "Could not convert work model to table.");
            }

            Tables.dbo.Image convertedPortrait = Image.Convert(inputItem.Portrait, siteItem.Id);
            if (convertedPortrait == null)
            {
                return new Result(ResultStatus.Failed, "Could not convert portrait model to table.");
            }

            // Disabled until portraits are used for testimonials rather than covers.
            //Result savePortraitResult = await Image.Save(site, inputItem.Portrait);
            //if (savePortraitResult.Status == SaveStatus.Failed)
            //{
            //    return savePortraitResult;
            //}

            inputItem.Portrait.Id = convertedPortrait.Id;
            
            Rules.StringRequiredMaxLength(inputItem.Name, "Name", 100, ref messages);

            if (Rules.StringRequiredMaxLength(inputItem.EmailAddress, "Email Address", 256, ref messages) ==
                Rules.Passed.Yes)
            {
                Rules.ValidateEmailAddress(inputItem.EmailAddress, "Email Address", ref messages);
            }

            if (messages.Any())
            {
                return new Result(ResultStatus.Failed, messages);
            }

            var saveItem = new Tables.dbo.Testimonial
            {
                Id = inputItem.Id,
                SiteId = siteItem.Id,
                WorkId = convertedWork.Id,
                PortraitImageId = convertedPortrait.Id,
                Name = inputItem.Name,
                EmailAddress = inputItem.EmailAddress,
                Approved = inputItem.Approved
            };

            Result saveTestimonialResult = await dbWrite.Item(site, saveItem);
            if (saveTestimonialResult.Status == ResultStatus.Failed)
            {
                return saveTestimonialResult;
            }

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (models.Localized.Testimonial localizedPage in inputItem.Entries)
            {
                var saveLocalization = new Tables.Localization.Testimonial
                {
                    Id = saveItem.Id,
                    Html = localizedPage.Html,
                    Lcid = localizedPage.Lcid
                };
                Result localizedResult = await DatabaseOperations.Localization.Write.Testimonial.Item(site, saveLocalization);
                if (localizedResult.Status != ResultStatus.Failed) continue;
                saveStatus = ResultStatus.PartialSuccess;
                messages.AddRange(localizedResult.Messages);
            }

            if (saveStatus == ResultStatus.Undetermined)
            {
                saveStatus = ResultStatus.Succeeded;
            }
            
            return new Result(saveStatus, messages, inputItem.Id);

        }
    }
}
