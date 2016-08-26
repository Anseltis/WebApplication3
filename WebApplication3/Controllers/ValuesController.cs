using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Xml;

namespace WebApplication3.Controllers
{
    public class ValuesController : ApiController
    {
        private void ProtectSection(string sectionName, string provider)
        {
            Configuration config = WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath);
            ConfigurationSection section = config.GetSection(sectionName);
            var f = (ClientSettingsSection)section;

            SettingElement addMe = new SettingElement("propertyName", SettingsSerializeAs.String);
            XmlElement element = new XmlDocument().CreateElement("value");
            element.InnerText = "800";
            addMe.Value.ValueXml = element;
            f.Settings.Add(addMe);

            var file = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            config.SaveAs(file);
            File.Delete(file);

            var ff = f.Settings.OfType<SettingElement>().ToArray();
            var v = ff[1];

            var d = section.SectionInformation.GetRawXml();
        }

        //Decrypting the connstring in the Web.Config File
        private void UnProtectSection(string sectionName)
        {
            Configuration config =
                WebConfigurationManager.
                    OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath);

            ConfigurationSection section =
                      config.GetSection(sectionName);

            if (section != null &&
                  section.SectionInformation.IsProtected)
            {
                section.SectionInformation.UnprotectSection();
                config.Save();
            }
        }

        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            ProtectSection("CompanyAccountSectionGroup/RemedySettings", "DataProtectionConfigurationProvider");
            ConfigurationManager.RefreshSection("CompanyAccountSectionGroup/RemedySettings");
            var d = (ClientSettingsSection)ConfigurationManager.GetSection("CompanyAccountSectionGroup/RemedySettings");
            var f = d.Settings.OfType<SettingElement>().ToArray();
            var v = f[1];

            //            
            //d.SectionInformation.ForceSave = true;
            //d.SectionInformation.ProtectSection("CompanyAccountSectionGroup/RemedySettings");

            

            return v.Value.ValueXml.InnerText;
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
