using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSMONEY
{
    class BotStarter
    {
        public void Start()
        {
            IWebDriver driver;

            var driverService = ChromeDriverService.CreateDefaultService();  //скрытие 
            driverService.HideCommandPromptWindow = true;                    //консоли

            driver = new ChromeDriver(driverService);
            driver.Navigate().GoToUrl("https://steamcommunity.com/login/home/?goto=");
            var login = driver.FindElement(By.Id("steamAccountName"));
            login.SendKeys("helpertrader");
            var pass = driver.FindElement(By.Id("steamPassword"));
            pass.SendKeys("Bogrinof114");
            var button = driver.FindElement(By.Id("SteamLogin"));
            button.Click();
            MessageBox.Show("1");
            HttpClientHandler handler2 = new HttpClientHandler();
            var _cookies = driver.Manage().Cookies.AllCookies;
            foreach (var item in _cookies)
            {
                try
                {
                    handler2.CookieContainer.Add(new System.Net.Cookie(item.Name, item.Value) { Domain = item.Domain });
                }
                catch (Exception ex){ }
            }

            int id = 0;
            foreach (var botId in Program.ListBotMod)
            {
                new System.Threading.Thread(delegate () { Bot bot = new Bot(handler2.CookieContainer, botId, id); }).Start();
                Thread.Sleep(500);
                id++;
            }
            new System.Threading.Thread(delegate () { ChekProxy(); }).Start();
        }
        private void ChekProxy()
        {
            while (true)
            {
                try
                {
                    if (Program.ProxyList.Count < 30)
                    {
                        foreach (var item in Program.ProxyListFix)
                        {
                            Program.ProxyList.Enqueue(item);
                        }

                    }
                }
                catch (Exception ex) { }
                Thread.Sleep(1000);
            }
        }
      
    }
}
