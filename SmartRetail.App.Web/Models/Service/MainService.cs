﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SmartRetail.App.DAL.DropBox;
using SmartRetail.App.DAL.Repository;
using SmartRetail.App.Web.Models.Interface;

namespace SmartRetail.App.Web.Models
{
    public class MainService : IInformationService
    {
        private static ImagesRepository _imagesRepo;
        private static ExpensesTypeRepository _exTypeRepo;
        private DropBoxBase dbClient;

        public MainService(string conn)
        {
            _imagesRepo = new ImagesRepository(conn);
            _exTypeRepo = new ExpensesTypeRepository(conn);
            dbClient = new DropBoxBase("o9340xsv2mzn7ws", "xzky2fzfnmssik1");
        }

        public async Task<JObject> GetDailyData(int whouse)
        {
            var urlPhoto = (await _imagesRepo.GetByIdAsync(1166)).img_url;
            var path = await dbClient.GetFileWithSharedLink(urlPhoto);
            var result = await dbClient.GetTempLink(path);

            var json = new JObject {new JProperty("imgs", new JArray { result, "Больше продаж не было"} )};
            var list = new Dictionary<string, float>();
            if (whouse == 0)
            {
                list = new Dictionary<string, float> { { "Карандаши", 0.34f }, { "Ручки", 0.16f }, { "Мячи", 0.5f } };
                json.Add("revenue", 265000);
                json.Add("profit", 36000);
                json.Add("salesCount", 15);
                json.Add("averageBill", 432.43);
            }
            else
            {
                list = new Dictionary<string, float> { { "Карандаши", 0.33f }, { "Ручки", 0.20f }, { "Мячи", 0.47f } };
                json.Add("revenue", 130000);
                json.Add("profit", 17000);
                json.Add("salesCount", 6);
                json.Add("averageBill", 432.43);
            }
           
            
            json.Add(new JProperty("goods", GetInfo(list)));
            return json;
        }

        public async Task<JObject> GetMonthData(int whouse)
        {
            var urlPhoto = (await _imagesRepo.GetByIdAsync(1166)).img_url;
            var path = await dbClient.GetFileWithSharedLink(urlPhoto);
            var result = await dbClient.GetTempLink(path);

            var json = new JObject { new JProperty("imgs", new JArray { result, "Больше продаж не было" }) };

            var list = new Dictionary<string, float>();
            if (whouse == 0)
            {
                list.Add("Карандаши", 0.34f);
                list.Add("Ручки", 0.16f);
                list.Add("Мячи", 0.5f);

                json.Add("revenue", 1240000);
                json.Add("profit", 456000);
                json.Add("salesCount", 650);
                json.Add("averageBill", 365.35);
            }
            else 
            {
                list.Add("Карандаши", 0.35f);
                list.Add("Ручки", 0.15f);
                list.Add("Мячи", 0.5f);

                json.Add("revenue", 600000);
                json.Add("profit", 200000);
                json.Add("salesCount", 350);
                json.Add("averageBill", 365.35);
            }

            json.Add(new JProperty("goods", GetInfo(list)));
            return json;
        }

        public JObject GetStocks(int whouse)
        {
            var json = new JObject();

            var list = new Dictionary<string, float>();

            if (whouse == 0)
            {
                list.Add("Карандаши", 55);
                list.Add("Ручки", 120);
                list.Add("Мячи", 64);

                json.Add("cost", 120000);
                json.Add("goodsCount", 950);
            }
            else
            {
                list.Add("Карандаши", 25);
                list.Add("Ручки", 50);
                list.Add("Мячи", 30);

                json.Add("cost", 50000);
                json.Add("goodsCount", 500);
            }

            json.Add(new JProperty("goods", GetInfo(list)));
            return json;
        }

        public JObject GetExpenses(int whouse)
        {
            var rnd = new Random();
            var json = new JObject();

            var exTypes = _exTypeRepo.GetAll();
            if (whouse == 0)
            {
                foreach (var type in exTypes)
                {
                    json.Add(new JProperty(type.type, rnd.Next(100, 10000)));
                }
            }
            else
            {
                foreach (var type in exTypes)
                {
                    json.Add(new JProperty(type.type, rnd.Next(100, 10000)));
                }
            }


            return json;

        }

        private static JArray GetInfo(Dictionary<string, float> dict)
        {
            var jarray = new JArray();
            foreach (var t in dict)
            {
                jarray.Add(new JObject(new JProperty(t.Key, t.Value)));
            }

            return jarray;
        }
    }
        
    
}
