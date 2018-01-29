using Newtonsoft.Json;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSMONEY
{
    class Bot
    {
        CookieContainer cookies = new CookieContainer();
        string BotId = "";
        public struct Data
        {
            public string _classid { get; set; }
            //     public List<RgInventory> rgInventory { get; set; }
            public string _id { get; set; }
            public string _instanceid { get; set; }
        }
        public struct PropertyItemsFloat
        {
            public string floatt { get; set; }
            //     public List<RgInventory> rgInventory { get; set; }
            public string stickerCount { get; set; }
            
        }
        int ID = 0;
        int pp = 0;
        public Bot(CookieContainer _cookies, string _botId,  int _ID)
        {
            try
            {
                cookies = _cookies;
                BotId = _botId;
                ID = _ID;
                for (int i = 0; i < 1; i++)
                {
                    Program.Mess.Enqueue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "ID={" + ID + "}|BOT:{" + BotId + "}|Запустил бота!");
                    //Types.LogInfo items = new Types.LogInfo() { Id = _ID, Site = _site };
                    //Program.MessLog.Enqueue(items);
                    //new System.Threading.Thread(delegate ()
                    //{
                    Work();
                    //}).Start();
                    new System.Threading.Thread(delegate ()
                    {
                        CheckWork();
                    }).Start();
                    Thread.Sleep(1000);
                }

            }
            catch (Exception ex)
            {
                Program.MessageFailed.Enqueue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "ID={" + ID + "}|BOT:{" + BotId + "}Main|" + ex.Message);
            }
        }
        private void CheckWork()
        {
            int temp = pp;
            while (true)
            {
                try
                {
                    Thread.Sleep(40000);
                    if (temp == pp)
                    {
                        Program.Mess.Enqueue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "ID={" + ID + "}|BOT:{" + BotId + "}|Перезапустил");
                        //Types.LogInfo log = new Types.LogInfo() { Id = ID, Text = "Перезапустил", Time = DateTime.Now.ToString("HH:mm:ss:fff") };
                        //Program.MessLog.Enqueue(log);
                        temp = pp;
                        //new System.Threading.Thread(delegate ()
                        //{
                        Work();
                        //}).Start();
                    }
                    temp = pp;
                }
                catch (Exception ex)
                {
                    Program.MessageFailed.Enqueue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "ID={" + ID + "}|BOT:{" + BotId + "}CheckWork|" + ex.Message);
                }
            }
        }


        private void Work()
        {
            try
            {
                while (true)
                {
                    try
                    {
                        pp++;
                        Random rr = new Random();
                        int sl = rr.Next(100, 500);
                        Thread.Sleep(sl);


                        var items = Get();
                        if (items != null)
                        {
                            List<Data> list = new List<Data>();
                            int k = 0;
                            foreach (var item in items.rgInventory)
                            {
                                Data d = new Data {_classid = item.First.classid.ToString(),_id = item.First.id.ToString(), _instanceid= item.First.instanceid.ToString() };
                                list.Add(d);
                            }
                            foreach (var item in items.rgDescriptions)
                            {
                                foreach (var it in Program.Data)
                                {
                                    string factory = "";
                                    switch (it.Factory)
                                    {
                                        case "MW": factory = "(MinimalWear)"; break;
                                        case "FN": factory = "(FactoryNew)"; break;
                                        case "FT": factory = "(Field-Tested)"; break;
                                        case "BS": factory = "(Battle-Scarred)"; break;
                                        case "WW": factory = "(Well-Worn)"; break;
                                    }
                                    string FullName = it.Name + factory;
                                    if ( item.First.market_hash_name.Value.ToString().Replace(" ", "") == FullName.Replace(" ", ""))
                                    {

                                        Program.MessageForSendItems.Enqueue("|_____________________________________________________________|");
                                        try
                                        {
                                            Program.MessageForSendItems.Enqueue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + $"Нашел предмет {item.First.market_hash_name.Value.ToString()}");
                                            string id = list.Find(N => (N._classid == item.First.classid.Value.ToString()))._id;
                                            var linkView = item.First.actions.First["link"].Value.ToString().Replace("%owner_steamid%", BotId).Replace("%assetid%", id);
                                            Program.MessageForSendItems.Enqueue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + $"Получил ID {id}");
                                            double price = Program.DataMoney.Find(N => (N.Name.Replace(" ", "") == FullName.Replace(" ", ""))).Price;
                                            Program.MessageForSendItems.Enqueue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + $"Получил Price {price}");
                                        
                                        if (price <= it.Price)
                                        {
                                                if (it.count > 0)
                                                {
                                                    //DataStruct.GoodItems.Enqueue(it);
                                                    PropertyItemsFloat PIF = GetFloat(linkView);
                                                    Program.MessageForSendItems.Enqueue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + $"Получил Float {PIF.floatt}");


                                                    IJavaScriptExecutor js = Program._driver as IJavaScriptExecutor;
                                                    string ss1 = "function test() {var xhr = new XMLHttpRequest();" +
                                                       //     "var body = \"{\\\"steamid\\\":\\\"" + b.ToString() + "\\\",\\\"peopleItems\\\":[],\\\"botItems\\\":[\\\"" + id.ToString() + "\\\"],\\\"onWallet\\\":-" + p.ToString().Replace(",", ".") + ",\\\"gid\\\":\\\"" + "\\\"}\";" +
                                                       "var body = \"{\\\"peopleItems\\\":[],\\\"botItems\\\":[{\\\"assetid\\\":\\\"" + id + "\\\",\\\"local_price\\\":\\\"" + price.ToString().Replace(",", ".") + "\\\",\\\"price\\\":" + price.ToString().Replace(",", ".") + ",\\\"float\\\":\\\"" + Math.Round(Convert.ToDouble(PIF.floatt), 8).ToString().Replace(",", ".") + "\\\",\\\"stickers_count\\\":" + PIF.stickerCount + ",\\\"market_hash_name\\\":\\\"" + item.First.market_hash_name.Value.ToString() + "" + "\\\",\\\"bot\\\":\\\"" + BotId + "\\\"}],\\\"onWallet\\\":-" + price.ToString().Replace(",", ".") + "}\";" +
                                                        " xhr.open(\"POST\", 'https://cs.money/send_offer', false); " +
                                                        "xhr.setRequestHeader('Content-Type', 'application/json');" +
                                                        " xhr.setRequestHeader('accept', '*/*');" +
                                                        " xhr.send(body);return xhr.status+'|'+xhr.responseText; } return test();";
                                                    ArgumentsChek.Value = ss1;//передаем строку запроса на дочерние программы через событие
                                                    var title = js.ExecuteScript(ss1);
                                                    Program.MessageForSendItems.Enqueue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|Результат отправленного обмена:" + title.ToString());
                                                    Program.OfferSend++;
                                                    Program.Dat NewItem = new Program.Dat()
                                                    {
                                                        Id = it.Id,
                                                        Name = it.Name,
                                                        Factory = it.Factory,
                                                        Price = it.Price,
                                                        AutoAddItem = it.AutoAddItem,
                                                        SiteName = it.SiteName,
                                                        count = it.count - 1
                                                    };
                                                    Program.Data.Remove(it);
                                                    Program.Data.Add(NewItem);
                                                    Program.MessageForSendItems.Enqueue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|Осталось количество предметов" + NewItem.count.ToString());
                                                }
                                                else
                                                {
                                                    Program.MessageForSendItems.Enqueue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|Лимит на взятие предметов исчерпан!");
                                                }
                                        }
                                        else
                                            {
                                                Program.DontOfferSend++;
                                                Program.MessageForSendItems.Enqueue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + $"Цена не подошла PriceMoney:{price.ToString()}-PriceData:{it.Price.ToString()}");
                                            }
                                        }
                                        catch (Exception ex) { Program.MessageFailed.Enqueue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "ID={" + ID + "}|BOT:{" + BotId + "}Work1|" + ex.Message); }
                                        Program.MessageForSendItems.Enqueue("|_____________________________________________________________|");
                                        //   Program.MessageForSendItems.Enqueue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|Результат отправленного обмена:" + title.ToString());
                                    }
                                }
                                k++;
                            }
                            Program.Mess.Enqueue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "ID={" + ID + "}|BOT:{" + BotId + "}|Количсетво предметов:{-" + k + "-}");
                        }
                        else
                        {
                            Program.Mess.Enqueue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "ID={" + ID + "}|BOT:" + BotId + "|" + "|null");
                        }
                    }
                    catch (Exception ex)
                    {
                        Program.MessageFailed.Enqueue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "ID={" + ID + "}|BOT:{" + BotId + "}Work1|" + ex.Message);
                        //Types.LogInfo er = new Types.LogInfo() { Id = ID, Error = "Work1|"+ex.Message, Timeerror = DateTime.Now.ToString("HH:mm:ss:fff") };
                        //Program.MessLog.Enqueue(er);
                    }

                }
            }
            catch (Exception ex)
            {
                Program.MessageFailed.Enqueue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "ID={" + ID + "}|BOT:{" + BotId + "}Work2|" + ex.Message);
                //Types.LogInfo er = new Types.LogInfo() { Id = ID, Error = "Work2|" + ex.Message, Timeerror = DateTime.Now.ToString("HH:mm:ss:fff") };
                //Program.MessLog.Enqueue(er);
            }
        }
        private dynamic Get()
        {

            try
            {
                string newProxy = Program.ProxyList.Dequeue();
                HttpClient client = null;
                HttpClientHandler handler2 = new HttpClientHandler();
                handler2.CookieContainer = cookies;
                handler2.Proxy = null;
                client = Prox(client, handler2, newProxy);
              //  client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(20);
                client.DefaultRequestHeaders.Add("User-Agent",
     "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.108 Safari/537.36");

                var response = client.GetAsync("http://steamcommunity.com/profiles/" + BotId + "/inventory/json/730/2").Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content;
                    string responseString = responseContent.ReadAsStringAsync().Result;
                    if (responseString != "null")
                    {
                        return JsonConvert.DeserializeObject<dynamic>(responseString);
                    }
                    //else { Program.Mess.Enqueue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "ID={" + ID + "}|BOT:" + BotId + "|" + "null" ); }
                }
                client.Dispose();
                handler2.Dispose();
            }
            catch (Exception ex)
            {
                //Types.LogInfo er = new Types.LogInfo() { Id = ID, Error = "GET|"+ex.Message, Timeerror = DateTime.Now.ToString("HH:mm:ss:fff") };
                //Program.MessLog.Enqueue(er);
            }
            return null;

        }
        private PropertyItemsFloat GetFloat(string link)
        {

            try
            {
                //       string newProxy = Program.ProxyList.Dequeue();
                HttpClient client = null;

                //handler2.CookieContainer = cookies;
                //handler2.Proxy = null;
                //client = Prox(client, handler2, newProxy);
                client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(3);
                //client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36");
                //client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
                //client.DefaultRequestHeaders.Add("Accept-Language", "ru,uk;q=0.9,en;q=0.8");
                //client.DefaultRequestHeaders.Add("Connection", "keep-alive");
                //client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");

                var response = client.GetAsync("https://api.csgofloat.com:1738/?url=" + link ).Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content;
                    string responseString = responseContent.ReadAsStringAsync().Result;
                    if (responseString != "null")
                    {
                        var res =  JsonConvert.DeserializeObject<dynamic>(responseString);
                        PropertyItemsFloat PIF = new PropertyItemsFloat { floatt = res.iteminfo.floatvalue.Value.ToString(),stickerCount = res.iteminfo.stickers.Count.ToString() };
                        return PIF;
                    }
                    //else { Program.Mess.Enqueue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "ID={" + ID + "}|BOT:" + BotId + "|" + "null" ); }
                }
                client.Dispose();

            }
            catch (Exception ex)
            {
                //Types.LogInfo er = new Types.LogInfo() { Id = ID, Error = "GET|"+ex.Message, Timeerror = DateTime.Now.ToString("HH:mm:ss:fff") };
                //Program.MessLog.Enqueue(er);
            }
            return new PropertyItemsFloat();

        }
        public HttpClient Prox(HttpClient client1, HttpClientHandler handler, string paroxyu)
        {

            HttpClient client = client1;
            try
            {
                string
                httpUserName = "webminidead",
                httpPassword = "159357Qq";
                string proxyUri = paroxyu;
                NetworkCredential proxyCreds = new NetworkCredential(
                    httpUserName,
                    httpPassword
                );
                WebProxy proxy = new WebProxy(proxyUri, false)
                {
                    UseDefaultCredentials = false,
                    Credentials = proxyCreds,
                };
                try
                {
                    handler.Proxy = null;
                    handler.Proxy = proxy;
                    handler.PreAuthenticate = true;
                    handler.UseDefaultCredentials = false;
                    handler.Credentials = new NetworkCredential(httpUserName, httpPassword);
                    handler.AllowAutoRedirect = true;
                }
                catch (Exception ex)
                {
                    //Types.LogInfo er = new Types.LogInfo() {Id = ID, Error = "1|"+ex.Message, Timeerror = DateTime.Now.ToString("HH:mm:ss:fff") };
                    //Program.MessLog.Enqueue(er);
                }
                client = new HttpClient(handler);
            }
            catch (Exception ex)
            {
                //Types.LogInfo er = new Types.LogInfo() { Id = ID, Error = "2|" + ex.Message, Timeerror = DateTime.Now.ToString("HH:mm:ss:fff") };
                //Program.MessLog.Enqueue(er);
            }
            return client;
        }
    }
}
