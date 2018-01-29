using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Windows.Forms;
using System.Threading;
using HtmlAgilityPack;
using System.Drawing;
using System.Data;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;
using WebSocketSharp;
using System.IO;

namespace CSMONEY
{
    class Work
    {
        IWebDriver driver;
        int ID = 0;

        List<System.Net.Cookie> cook;
        List<System.Net.Cookie> cookAll;
        string apiKey = Properties.Settings.Default.ApiKey;
        string ga = "";
        CookieContainer cookies = new CookieContainer();
        HttpClientHandler handler = new HttpClientHandler();
        public struct Dat
        {
            public string Event { get; set; }
            inData data { get; set; }
        }
        public struct inData
        {

            public string m { get; set; }
            public string e { get; set; }
            public double p { get; set; }
        }
        public Work(int id)
        {
            ID = id;
        }
        //public void start()
        //{
        //    var firstFull = Convert.ToInt32(DateTime.Now.ToString("HHmmss"));
        //    while (true)
        //    {
        //        try
        //        {
        //            while (Program.pauseMoney==true)
        //                Thread.Sleep(200);

        //            var res = ClickItem(ITEMS);
        //            if (res == true)
        //            {
        //                var first = Convert.ToInt32(DateTime.Now.ToString("HHmmss"));
        //            }
        //            if (Convert.ToInt32(DateTime.Now.ToString("HHmmss")) - firstFull > 200)
        //            {
        //                bool cheek = false;
        //                var dt = DataTa.GetTable();
        //                foreach (DataRow item in dt.Rows)
        //                {
        //                    var s = item.ItemArray;
        //                    if (s[0].ToString() == Properties.Settings.Default.name && s[1].ToString() == Properties.Settings.Default.csmoney)
        //                    {
        //                        cheek = true;
        //                        if (s[3].ToString() != Properties.Settings.Default.csmoneyVersion)
        //                        {
        //                            MessageBox.Show("Версия ПО устарела");
        //                            Application.Exit();
        //                        }
        //                        Program.sleepIMONEY = Convert.ToInt32(s[2].ToString());
        //                    }
        //                }
        //                if (cheek == false)
        //                {
        //                    MessageBox.Show("Лицензия не активная.");
        //                    Application.Exit();
        //                }
        //                firstFull = Convert.ToInt32(DateTime.Now.ToString("HHmmss"));
        //                RefreshBotInventory();
        //                Program.Mess.Enqueue("БОТ[" + ID + "] " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|" + "{Правило 200сек.}Обновил инвентари!");

        //            }
        //            Thread.Sleep(Program.sleepMSecond);
        //        }
        //        catch (Exception ex) { }
        //    }
        //}


        public void INI()
        {
            try
            {
                var driverService = ChromeDriverService.CreateDefaultService();  //скрытие 
                driverService.HideCommandPromptWindow = true;                    //консоли
                driver = new ChromeDriver(driverService);
                driver.Navigate().GoToUrl("https://cs.money/ru");
                MessageBox.Show("Введите все данные , после этого программа продолжит работу!");

                var _cookies = driver.Manage().Cookies.AllCookies;
             //   var ws = new WebSocket("wss://cs.money/ws");

                foreach (var item in _cookies)
                {
                    if (item.Name == "_ga")
                    {
                        string[] tmp = item.Value.Split('.');
                        ga = tmp[2] + "." + tmp[3];
                    }
                    handler.CookieContainer.Add(new System.Net.Cookie(item.Name, item.Value) { Domain = item.Domain });
                  //  ws.SetCookie(new WebSocketSharp.Net.Cookie(item.Name, item.Value));
                }

                driver.Manage().Window.Position = new Point(5000, 5000);
                driver.Navigate().GoToUrl("https://steamcommunity.com/id/me/tradeoffers/privacy#trade_offer_access_url");
                IWebElement offer = driver.FindElement(By.Id("trade_offer_access_url"));
                if (offer.GetAttribute("value") != Properties.Settings.Default.csmoney)
                {
                    MessageBox.Show("Выберите аккаунт к которому привязана программа!");
                    driver.Quit();
                    return;
                }

                driver.Navigate().GoToUrl("https://store.steampowered.com/account/");

                var _cookiesSteam = driver.Manage().Cookies.AllCookies;
                cook = new List<System.Net.Cookie>();
                cookAll = new List<System.Net.Cookie>();
                foreach (var item in _cookiesSteam)
                {
                    if (item.Name == "sessionid" || item.Name == "steamLogin" || item.Name == "steamLoginSecure")
                    {
                        cook.Add(new System.Net.Cookie(item.Name, item.Value, string.Empty, "steamcommunity.com"));

                    }

                    if (item.Name != "timezoneOffset")
                    {
                        cookAll.Add(new System.Net.Cookie(item.Name, item.Value, string.Empty, "steamcommunity.com"));
                    }

                }
                cookAll.Add(new System.Net.Cookie("bCompletedTradeOfferTutorial", "true", string.Empty, "steamcommunity.com"));
                new System.Threading.Thread(delegate () {
                    try
                    {
                        while (true)
                        {
                            try
                            {
                                Thread.Sleep(240000);
                                if (Program.AutoRefreshPage)
                                {
                                    driver.Navigate().Refresh();
                                    Program.Mess.Enqueue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|Обновил страницу");
                                }
                                
                            }
                            catch (Exception ex) { }
                        }

                    }
                    catch (Exception ex) { }
                }).Start();
                


                driver.Navigate().GoToUrl("https://cs.money/ru");
                driver.Manage().Window.Position = new Point(0, 0);
                var _cookies1 = driver.Manage().Cookies.AllCookies;
                List<KeyValuePair<string, string>> cook11 = new List<KeyValuePair<string, string>>();
                foreach (var item in _cookies1)
                {
                    cook11.Add(new KeyValuePair<string, string>(item.Name, item.Value));
                }
               
                WebSocket4Net.WebSocket websocket = new WebSocket4Net.WebSocket("wss://cs.money/ws", "", cook11, version:WebSocket4Net.WebSocketVersion.Rfc6455,userAgent: Properties.Settings.Default.UsetAgent);
                websocket.MessageReceived += new EventHandler<WebSocket4Net.MessageReceivedEventArgs>(websocket_Opened);

                websocket.Open();
                new System.Threading.Thread(delegate () {
                    try
                    {
                        while (true)
                        {
                            try
                            {
                                if (websocket.State.ToString() == "Closing" || websocket.State.ToString() == "Closed")
                                {
                                    //driver.Navigate().Refresh();
                                    Thread.Sleep(10000);
                                    var _cookies2 = driver.Manage().Cookies.AllCookies;
                                    List<KeyValuePair<string, string>> cook12 = new List<KeyValuePair<string, string>>();
                                    foreach (var item in _cookies2)
                                    {
                                        cook12.Add(new KeyValuePair<string, string>(item.Name, item.Value));
                                    }
                                    websocket = new WebSocket4Net.WebSocket("wss://cs.money/ws", "", cook12, version: WebSocket4Net.WebSocketVersion.Rfc6455, userAgent: Properties.Settings.Default.UsetAgent);
                                    websocket.MessageReceived += new EventHandler<WebSocket4Net.MessageReceivedEventArgs>(websocket_Opened);
                                    websocket.Open();
                                    Program.Mess.Enqueue("Статус подключения к сервру:" + websocket.State.ToString());
                                }
                                Thread.Sleep(1000);
                            }
                            catch (Exception ex) { }
                        }
                        
                    }
                    catch (Exception ex) { }
                }).Start();
                //ws.Connect();        
                //HttpClientHandler handler1 = new HttpClientHandler();
                //CookieContainer cookies1 = new CookieContainer();

                //              var firstFull = Convert.ToInt32(DateTime.Now.ToString("HHmmss"));
                //              while (true)
                //              {
                //                  if (Convert.ToInt32(DateTime.Now.ToString("HHmmss")) - firstFull > 100)
                //                  {
                //                      firstFull = Convert.ToInt32(DateTime.Now.ToString("HHmmss"));
                //                      ws.Close();
                //                      ws.Connect();
                //                      Program.Mess.Enqueue("Поднял подключение к серверу CSMONEY");
                //                  }
                //                  if (ws.ReadyState.ToString() == "Closed")
                //                  {
                //                      ws.Compression = CompressionMethod.Deflate;
                //                      ws.SslConfiguration.ServerCertificateValidationCallback =
                //(sender, certificate, chain, sslPolicyErrors) => {

                //  return true; // If the server certificate is valid.
                //};
                //                      ws.Connect();
                //                      Program.Mess.Enqueue("Поднял подключение к серверу CSMONEY");
                //                  }

                //                 // Program.Mess.Enqueue("Статус подключения к сервру:" + ws.ReadyState.ToString());
                //                  Thread.Sleep(3000);
                //              }
            }
            catch (Exception ex) { Program.Mess.Enqueue(ex.Message); }

        }
        private void websocket_Opened(object sender, WebSocket4Net.MessageReceivedEventArgs e)
        {
            try
            {
                var da = JsonConvert.DeserializeObject<dynamic>(e.Message.Replace("event", "Event"));
                if (da.Event.Value == "add_items")
                {
                    var countAddItems = da.data.Count;
                    Program.Mess.Enqueue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|Добавлено предметов на сайт:" + countAddItems);
                    ClickItem(da);
                }
            }
            catch (Exception ex)
            {
                string ss = "";
            }

        }
        private void Post(HttpClientHandler handler, int id,string botId,string itemsId,string wallet)
        {
            HttpClientHandler handler1 = handler;
                try
                {
                    HttpClient client = null;
                    client = new HttpClient(handler1);
                    client.Timeout = TimeSpan.FromSeconds(60);
                    client.DefaultRequestHeaders.Add("User-Agent",
                                                     Properties.Settings.Default.UsetAgent);
                    client.DefaultRequestHeaders.Add("Accept", "*/*");
                    client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
                    //Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;/* unixTimestamp.ToString() + DateTime.Now.Millisecond.ToString()*/
                     StringContent content = new StringContent("{\"steamid\":\""+ botId + "\",\"peopleItems\":[],\"botItems\":[\""+ itemsId + "\"],\"onWallet\":-"+ wallet + ",\"gid\":\""  + "\"}", Encoding.UTF8, "application/json");
                     Program.Mess.Enqueue("БОТ[" + ID + "] " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "| Оправил Запрос1|Бот:" + botId + "|IDItems:" + itemsId + "|wallet:" + wallet);
                     var response = client.PostAsync("https://cs.money/send_offer", content).Result;
                     Program.Mess.Enqueue("БОТ[" + ID + "] " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "| Завершил Запрос1");
                     if (response.IsSuccessStatusCode)
                     {
                        var responseContent = response.Content;
                        string responseString = responseContent.ReadAsStringAsync().Result;
                     }
                }
                catch (Exception ex) { }

        }
        private bool chekFinishDownload(string kode)
        {
            try
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(driver.PageSource);
                var item = doc.DocumentNode.SelectNodes("//div[@id=\"inventory_bot\"]").FirstOrDefault().InnerHtml;
                if (item != "") { return true; }
            }
            catch (Exception ex) { }
            return false;
        }
        private bool ClickItem(dynamic json)
        {

            try
            {
                var da = json;
                var aa = da.Event.Value;
                foreach (var item in da.data)
                {

                    var b = item.b[0].Value;
                    var id = item.id[0].Value;
                    var m = item.m.Value;
                    var p = item.p.Value;
                    var f = item.f.First.Value;
                    string ee = "0";
                    string s = "0";
                    if (item.e != null)
                    {
                        try
                        {
                            ee = item.e.Value;
                        }
                        catch (Exception ex) { Program.Mess.Enqueue("БОТ[" + ID + "] " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|Ошибка1 :" + ex.Message); }
                    }
                    if (item.s != null)
                    {
                        try
                        {
                            s = item.s.Count.ToString();
                        }
                        catch (Exception ex) { Program.Mess.Enqueue("БОТ[" + ID + "] " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|Ошибка1 :" + ex.Message); }
                    }
                    string factory = "";
                    switch (ee)
                    {
                        case "MW": factory = " (Minimal Wear)"; break;
                        case "FN": factory = " (Factory New)"; break;
                        case "FT": factory = " (Field-Tested)"; break;
                        case "BS": factory = " (Battle-Scarred)"; break;
                        case "WW": factory = " (Well-Worn)"; break;
                    }
                    foreach (var name in Program.Data)
                    {
                        if (m.Replace(" ", "") == (name.Name).Replace(" ", "") && ee == name.Factory)
                        {
                            Program.MessageForSendItems.Enqueue("|_____________________________________________________________|");
                            Program.Mess.Enqueue("БОТ[" + ID + "] " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "| Нашел предмет :" + m + "|Цена_Сайта:" + p + "|Цена_Наша:" + name.Price);
                            
                            if (p <= name.Price)
                            {
                                if (name.count > 0)
                                {
                                    // Program.MessTelegram.Enqueue("БОТ[CsMoney]" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|Подошел предмет:" + m + "|Цена:" + p);
                                    Program.MessageForSendItems.Enqueue( DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "| Оправил Запрос |Предмет:" + m + "|Цена_Сайта:" + p + "|Цена_Наша:" + name.Price);
                                    if (Program.BrowesrQuery == false)
                                    {
                                        Post(handler, 1, b.ToString(), id.ToString(), p.ToString().Replace(",", "."));
                                    }
                                    else
                                    {
                                        string ff = "";
                                        if (f.ToString() != "0")
                                        {
                                            ff = "\\\"" + f.ToString() + "\\\"";
                                        }
                                        else { ff = f.ToString(); }
                                        IJavaScriptExecutor js = driver as IJavaScriptExecutor;
                                        string ss1 = "function test() {var xhr = new XMLHttpRequest();" +
                                       //     "var body = \"{\\\"steamid\\\":\\\"" + b.ToString() + "\\\",\\\"peopleItems\\\":[],\\\"botItems\\\":[\\\"" + id.ToString() + "\\\"],\\\"onWallet\\\":-" + p.ToString().Replace(",", ".") + ",\\\"gid\\\":\\\"" + "\\\"}\";" +
                                           "var body = \"{\\\"peopleItems\\\":[],\\\"botItems\\\":[{\\\"assetid\\\":\\\""+ id.ToString() + "\\\",\\\"local_price\\\":\\\""+ p.ToString().Replace(",", ".") + "\\\",\\\"price\\\":"+ p.ToString().Replace(",", ".") + ",\\\"float\\\":"+ff + ",\\\"stickers_count\\\":"+s.ToString() + ",\\\"market_hash_name\\\":\\\""+ m.ToString() + factory + ""+ "\\\",\\\"bot\\\":\\\""+ b.ToString() + "\\\"}],\\\"onWallet\\\":-"+ p.ToString().Replace(",", ".") + "}\";" +
                                            " xhr.open(\"POST\", 'https://cs.money/send_offer', false); " +
                                            "xhr.setRequestHeader('Content-Type', 'application/json');" +
                                            " xhr.setRequestHeader('accept', '*/*');" +
                                            " xhr.send(body);return xhr.status+'|'+xhr.responseText; } return test();";
                                        var title = js.ExecuteScript(ss1);
                                        Program.MessageForSendItems.Enqueue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|Результат отправленного обмена:" + title.ToString());
                                    }
                                    Program.OfferSend++;
                                    Program.MessageForSendItems.Enqueue( DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|Завершил Запрос");
                                    Program.Dat NewItem = new Program.Dat()
                                    {
                                        Id = name.Id,
                                        Name = name.Name,
                                        Factory = name.Factory,
                                        Price = name.Price,
                                        AutoAddItem = name.AutoAddItem,
                                        SiteName = name.SiteName,
                                        count = name.count - 1
                                    };
                                    Program.Data.Remove(name);
                                    Program.Data.Add(NewItem);
                                    Program.MessageForSendItems.Enqueue( DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|Осталось количество предметов"+ NewItem.count.ToString());
                                    if (Program.autoConfirm == true)
                                    {
                                        TradingBot tradeBot = new TradingBot(cook, cookAll, apiKey);
                                    }

                                    //      TradingBot tradeBot = new TradingBot(cook, apiKey);
                                    Thread.Sleep(2000);
                                }
                                else { Program.DontOfferSend++; Program.MessageFailed.Enqueue( DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|Не удалось запустить запрос, максимальное количество предметов:"+ m); }
                                
                                return true;//main-trade-button
                            }
                            else {
                                Program.DontOfferSend++;
                                Program.MessageFailed.Enqueue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|Цена не подошла "  + m + "|Цена_Сайта:" + p + "|Цена_Наша:" + name.Price);
                                SetListBadPrice(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"), "cs.money", m.ToString() + " [" + ee + "]", name.Price.ToString(), p.ToString());
                            }

                        }
                    }

                }
            }
            catch (Exception ex) { Program.MessageFailed.Enqueue("БОТ[" + ID + "] " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|Ошибка2 :" + ex.Message); }
            return false;
        }
        private bool ChekItemsOffer(string name)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(driver.PageSource);
            string url = "";
            try
            {
                var nodes = doc.DocumentNode.SelectNodes("//div[@id=\"offer_inventory_bot\"]").FirstOrDefault();
                var nodes1 = nodes.ChildNodes.Where(n => (n.Name == "div"));
                int cc = 0;
                foreach (var item1 in nodes1)
                {
                    string name1 = item1.Attributes["hash"].Value.Replace(" ", "");
                    if (name1 == name)
                    {
                        return true;
                    }
                    cc++;
                }
            }
            catch (Exception ex) { }
            Program.Mess.Enqueue("БОТ[" + ID + "] " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "| Зарезервированная вещь:" + name);
            return false;
        }
        private void RefreshBotInventory()
        {
            IJavaScriptExecutor js = driver as IJavaScriptExecutor;

            Program.Mess.Enqueue("БОТ[" + ID + "] " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "| Обновил инвентарь бота");
            string title = (string)js.ExecuteScript("document.getElementById('refresh_bot_inventory').click();");
        }
        private void CloseModalFrm()
        {
            IJavaScriptExecutor js = driver as IJavaScriptExecutor;

            Program.Mess.Enqueue("БОТ[" + ID + "] " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "| Закрыл модальную форму");
            string title = (string)js.ExecuteScript("document.getElementsByClassName('modal__close')[3].click();");
        }
        private void RefreshUserInventory()
        {
            IJavaScriptExecutor js = driver as IJavaScriptExecutor;

            Program.Mess.Enqueue("БОТ[" + ID + "] " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "| Обновил инвентарь пользователя");
            string title = (string)js.ExecuteScript("document.getElementById('refresh_user_inventory').click();");
        }
        private void SetListBadPrice(string _Data, string _Site, string _Name, string _OldPrice, string _NewPrice)
        {
            Program.DataViewBadPrice item = new Program.DataViewBadPrice()
            {
                Date = _Data,
                Site = _Site,
                Name =_Name,
                OldPrice = _OldPrice,
                NewPrice =_NewPrice
            };
            Program.BadPrice.Add(item);
        }
    }
}
