using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System.IO;

namespace CSMONEY
{
    class AutoRefresh
    {
        IWebDriver driver;
        double minPercent;
        double addPercent;
        bool prope = false;
        string siteName;
        int countMax;
        public AutoRefresh(double _minPercent,bool _prope,double _addPercent,string _siteName,int _countMax)
        {
            minPercent = _minPercent;
            prope = _prope;
            addPercent = _addPercent;
            siteName = _siteName;
            countMax = _countMax;
        }
        public void Login()
        {
            try
            {
                var driverService = ChromeDriverService.CreateDefaultService();  //скрытие 
                driverService.HideCommandPromptWindow = true;                    //консоли
                driver = new ChromeDriver(driverService);
                driver.Navigate().GoToUrl("http://csgoback.net/comparison");
                MessageBox.Show("Проведите все НАСТРОЙКИ , после этого программа продолжит работу!");


                if (prope == false)
                {
                    try
                    {
                        IJavaScriptExecutor js = driver as IJavaScriptExecutor;
                        string ss1 = "$('body, html').scrollTop($(document).height());";
                        for (int i = 0; i < 100; i++)
                        {
                            string title = (string)js.ExecuteScript(ss1);
                            Thread.Sleep(100);
                        }
                        WorkerOnes();
                        driver.Quit();
                    }
                    catch (Exception ex) { Program.Mess.Enqueue($"Ошибка 1{ex.Message}"); }
                }
                else
                {
                    IJavaScriptExecutor js = driver as IJavaScriptExecutor;
                    string ss1 = "$('body, html').scrollTop($(document).height());";
                    for (int i = 0; i < 100; i++)
                    {
                        string title = (string)js.ExecuteScript(ss1);
                        Thread.Sleep(100);
                    }
                    Worker(); }
            }
            catch (Exception ex) { Program.Mess.Enqueue($"Ошибка 6{ex.Message}"); }
        }
        private void Worker()
        {
            while (true)
            {
                try
                {
                   
                    
                    string kode = driver.PageSource;
                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(kode);
                    var node = doc.DocumentNode.SelectNodes("//tr");
                    foreach (var n in node)
                    {
                        try
                        {
                            var adsd = n.ChildNodes;
                            var name = adsd[0].InnerText;
                            var price = Convert.ToDouble(adsd[4].ChildNodes.ElementAt(0).InnerText.Replace(".", ","));
                            var price2 = adsd[6].ChildNodes.ElementAt(0).Attributes[0].Value.Contains("unavailable");
                            var percent = Convert.ToDouble(adsd[8].InnerText.Replace("%", "").Replace(".", ","));
                            if (price2 == false)
                            {
                                WorkWithData(name, price,percent);
                            }
                        }
                        catch (Exception ex) { }
                    }
                    string json = JsonConvert.SerializeObject(Program.Data);
                    File.WriteAllText("data.txt", json);

                }
                catch (Exception ex)
                {
                    Program.MessAutoRefresh.Enqueue($"Ошибка 2{ex.Message}");
                }
                Thread.Sleep(3000);
            }
        }
        private void WorkerOnes()
        {

                try
                {
                   

                    string kode = driver.PageSource;
                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(kode);
                    var node = doc.DocumentNode.SelectNodes("//tr");
                    foreach (var n in node)
                    {
                        try
                        {
                            var adsd = n.ChildNodes;
                            var name = adsd[0].InnerText;
                            var price = Convert.ToDouble(adsd[4].ChildNodes.ElementAt(0).InnerText.Replace(".", ","));
                            var price2 = adsd[6].ChildNodes.ElementAt(0).Attributes[0].Value.Contains("unavailable");
                            var percent = Convert.ToDouble(adsd[8].InnerText.Replace("%", "").Replace(".", ","));
                            if (price2 == false )
                            {
                                WorkWithData(name, price, percent);
                            }
                        }
                        catch (Exception ex) { }
                    }
                    string json = JsonConvert.SerializeObject(Program.Data);
                    File.WriteAllText("data.txt", json);

                }
                catch (Exception ex)
                {
                Program.MessAutoRefresh.Enqueue($"Ошибка 3{ex.Message}");

            }


        }
        private void WorkWithData(string Name,double price,double percent)
        {
            try
            {
                string factory = "";
                if (Name.Contains("(Well-Worn)") == true || Name.Contains("(Field-Tested)") == true || Name.Contains("(Battle-Scarred)") == true || Name.Contains("(Minimal Wear)") == true || Name.Contains("(Factory New)") == true)
                {
                    string tms = Name.Split('(')[1].Replace(")", "").Replace("(", "").Replace(" ", "");
                    switch (tms)
                    {
                        case "MinimalWear": factory = "MW"; break;
                        case "FactoryNew": factory = "FN"; break;
                        case "Field-Tested": factory = "FT"; break;
                        case "Battle-Scarred": factory = "BS"; break;
                        case "Well-Worn": factory = "WW"; break;
                    }
                    var item = new Program.Dat
                    {
                        Name = Name.Split('(')[0],
                        Factory = factory,
                        Price = price + price * addPercent / 100,
                        AutoAddItem = true,
                        count = countMax,
                        SiteName = siteName

                    };

                    var itemFind = Program.Data.Find(N => (N.Name == item.Name && N.Factory == item.Factory));
                    if (itemFind.Name == null && percent > minPercent)
                    {
                        Program.Data.Add(item);
                        Program.MessAutoRefresh.Enqueue($"{ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") }|Добавил Предмет:{Name} с ценой: {price} (учет х%:{item.Price}) -- процент:{percent}%");
                    }
                    else
                    {
                        if (itemFind.Price != item.Price && percent > minPercent && itemFind.AutoAddItem == true)
                        {
                            var index = Program.Data.FindIndex(N => (N.Name == itemFind.Name && N.Factory == itemFind.Factory && N.Price == itemFind.Price));
                            Thread.Sleep(3000);
                            Program.Data.RemoveAt(index);
                            Program.Data.Add(item);
                            Program.MessageForEditData.Enqueue($"{ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") }|Изменил Предмет:{Name} с ценой: {itemFind.Price} на новую цену:{item.Price}-- процент:{percent}");
                        }
                        else
                        if ( percent < minPercent && itemFind.AutoAddItem == true && itemFind.SiteName == siteName)
                        {
                            var index = Program.Data.FindIndex(N => (N.Name == itemFind.Name && N.Factory == itemFind.Factory && N.Price == itemFind.Price));
                            Program.Data.RemoveAt(index);
                            Program.MessageForEditData.Enqueue($"{ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") }|Удалил Предмет:{Name} с ценой: {price} (учет х%:{item.Price}) -- процент:{percent}%");
                        }
                    }

                }
                else
                {
                    var item = new Program.Dat
                    {
                        Name = Name,
                        Factory = factory,
                        Price = price + price * addPercent / 100,
                        AutoAddItem = true,
                        count = countMax,
                        SiteName = siteName
                    };
                    var itemFind = Program.Data.Find(N => (N.Name == item.Name && N.Factory == item.Factory));
                    if (itemFind.Name == null && percent > minPercent)
                    {
                        Program.Data.Add(item);
                        Program.MessAutoRefresh.Enqueue($"{ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") }|Добавил Предмет:{Name} с ценой: {price} (учет х%:{item.Price})-- процент:{percent}%");
                    }
                    else
                    {
                        if (itemFind.Price != item.Price && percent > minPercent && itemFind.AutoAddItem == true)
                        {
                            var index = Program.Data.FindIndex(N => (N.Name == itemFind.Name && N.Factory == itemFind.Factory && N.Price == itemFind.Price));
                            Thread.Sleep(3000);
                            Program.Data.RemoveAt(index);
                            Program.Data.Add(item);
                            Program.MessageForEditData.Enqueue($"{ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") }|Изменил Предмет:{Name} с ценой: {itemFind.Price} на новую цену:{item.Price}-- процент:{percent}");
                        }
                        else
                        if (percent < minPercent && itemFind.AutoAddItem==true && itemFind.SiteName == siteName)
                        {
                            var index = Program.Data.FindIndex(N => (N.Name == itemFind.Name && N.Factory == itemFind.Factory && N.Price == itemFind.Price));
                            Program.Data.RemoveAt(index);
                            Program.MessageForEditData.Enqueue($"{ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") }|Удалил Предмет:{Name} с ценой: {price} (учет х%:{item.Price}) -- процент:{percent}%");
                        }
                    }
                }
            }
            catch (Exception ex) { Program.MessAutoRefresh.Enqueue($"Ошибка 4{ex.Message}"); }
        }
       
    }
}
