using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSMONEY
{
    class MoneyGet
    {
        IWebDriver driver;
        int idBot = 0;

        public MoneyGet( int _idBot)
        {
            idBot = _idBot;
            ArgumentsChek.ValueChanged += Data_ValueChanged; //подписываем метод к событию
        }
        public void INI()
        {
            try
            {
                var driverService = ChromeDriverService.CreateDefaultService();  //скрытие 
                driverService.HideCommandPromptWindow = true;                    //консоли
                driver = new ChromeDriver(driverService);
                driver.Navigate().GoToUrl("https://cs.money/ru");
                MessageBox.Show("Введите все данные , после этого программа продолжит работу!");

                driver.Manage().Window.Position = new Point(5000, 5000);
                //driver.Navigate().GoToUrl("https://steamcommunity.com/id/me/tradeoffers/privacy#trade_offer_access_url");
                //IWebElement offer = driver.FindElement(By.Id("trade_offer_access_url"));
                //if (offer.GetAttribute("value") != Properties.Settings.Default.csmoney)
                //{
                //    MessageBox.Show("Выберите аккаунт к которому привязана программа!");
                //    driver.Quit();
                //    return;
                //}
                //driver.Navigate().GoToUrl("https://store.steampowered.com/account/");

                new System.Threading.Thread(delegate () {
                    try
                    {
                        while (true)
                        {
                            try
                            {
                                Thread.Sleep(150000);
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
            }
            catch (Exception ex) { Program.Mess.Enqueue(ex.Message); }
        }
        private void Data_ValueChanged(object sender, EventArgs e) //этот метод будет вызываться при изменении переменной в классе SomeClass
        {
            try
            {
                Program.MessageForSendItems.Enqueue($"|[{idBot}]-------------------------------------------------------------|");
                Program.MessageForSendItems.Enqueue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + $"[{idBot}]Отправил Запрос");
                IJavaScriptExecutor js = driver as IJavaScriptExecutor;
                var title = js.ExecuteScript(ArgumentsChek.Value);
                Program.MessageForSendItems.Enqueue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + $"[{idBot}]Завершил Запрос");
                Program.MessageForSendItems.Enqueue($"|[{idBot}]-------------------------------------------------------------|");
            }
            catch (Exception ex) { Program.MessageFailed.Enqueue($"|[{idBot}]|" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "|Ошибка :" + ex.Message); }
        }
    }
}
