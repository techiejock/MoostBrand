﻿using Synchronizer.Helper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Synchronizer.Repository
{
    class RequisitionDetails : API
    {
        private async Task<List<RequisitionDetail>> GetUnsyc(string URL)
        {
            try
            {
                this.URL = URL;

                string content = string.Empty;

                var response = await this.Get("api/RequisitionDetails");

                content = await response.Content.ReadAsStringAsync();

                JavaScriptSerializer json_serializer = new JavaScriptSerializer();

                return json_serializer.Deserialize<List<RequisitionDetail>>(content);
            }
            catch
            {
            }

            return null;
        }

        public async Task<string> Post(string URL, RequisitionDetail _entity)
        {
            this.URL = URL;

            var response = await this.Post(_entity, "api/RequisitionDetails");

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<List<RequisitionDetail>> GetUnsyncLocal()
        {
            string URL = ConfigurationManager.AppSettings["urlLocal"].ToString();

            return await GetUnsyc(URL);
        }

        public async Task<List<RequisitionDetail>> GetUnsyncLive()
        {
            string URL = ConfigurationManager.AppSettings["urlLive"].ToString();

            return await GetUnsyc(URL);
        }

        public async Task<string> PostLocal(RequisitionDetail _entity)
        {
            string URL = ConfigurationManager.AppSettings["urlLocal"].ToString();

            return await Post(URL, _entity);
        }

        public async Task<string> PostLive(RequisitionDetail _entity)
        {
            string URL = ConfigurationManager.AppSettings["urlLive"].ToString();

            return await Post(URL, _entity);
        }
    }
}
