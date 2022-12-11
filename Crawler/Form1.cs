using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using System.IO;
using OpenQA.Selenium.Interactions;
using Keys = OpenQA.Selenium.Keys;
using System.Diagnostics;

namespace Crawler
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string pth = @"C:\chotot\ChototPhoneNumber.txt";
        string DriverPath= @"C:\Program Files\Google\Chrome\Application\";
        int MaxTimes =  0;
        List<string> lst = new List<string>();
        bool Is50000 = false;
        ChromeDriver cdr;
        private void button1_Click(object sender, EventArgs e)
        {
         
            Thread thread = new Thread(new ThreadStart(Start));
            thread.Start();
         
        }
        private void CheckStartDamain(ChromeDriver driverC)
        {
            driverC.Navigate().GoToUrl(textBox1.Text);

            if (textBox1.Text.ToLower().ToString().Contains("https://www.chotot.com"))
            {
                var lstelem = driverC.FindElements(By.TagName("a"));

               // var lstelem = driverC.FindElements(By.TagName("a"));
                Thread.Sleep(2000);
                if (lstelem.Count > 0 )
                {
                    foreach (var anchr in lstelem)
                    {
                        try
                        {

                            var an = anchr.GetAttribute("href");
                            if (an != null)
                                if (an.ToLower().ToString().Contains("htm") && an.ToLower().ToString().Contains(".com"))
                                {
                                    driverC.Navigate().GoToUrl(an.TrimEnd());
                                    Thread.Sleep(2000);
                                    break;
                                }
                        }
                        catch { }
                    }
                }
            }
        }
        private void Start()
        {
            if (String.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text) || !Int64.TryParse(textBox2.Text.Trim(), out long result) || String.IsNullOrEmpty(txtDriver.Text))
            {
                MessageBox.Show("Please fill the required correct value");
                return;
            }

            MaxTimes = Convert.ToInt32(textBox2.Text.Trim());

            //lst = File.ReadAllLines(pth).ToList<string>();
            // label2.Text = lst.Count.ToString();
            cdr = new ChromeDriver(txtDriver.Text);
            if (!textBox1.Text.Contains("htm"))
            {
                CheckStartDamain(cdr);
                Thread.Sleep(3000);
                textBox1.Text = cdr.Url;
            }
            while (!Is50000)
            { 
                if (lst.Count > MaxTimes || Is50000)
                {
                    MessageBox.Show("All "+ textBox2.Text +" Phone Numbers were Retrieved Successfully!");
                    break;
                }
                //textBox1.Text = driverC;
                try
                {
                    RunSele(cdr, textBox1.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            cdr.Quit();
            MessageBox.Show("All " + textBox2.Text + " Phone Numbers were Retrieved Successfully!"); 
        }
        private void RunSele(ChromeDriver driverC, string url)
        {
            try
            {
                driverC.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
                driverC.Manage().Window.Maximize();

                 //  var url = "https://xe.chotot.com/mua-ban-xe-tai-xe-ben-huyen-quoc-oai-ha-noi/101909944.htm?ctfp=3c15e79f-de73-41fa-ae40-4d7c8bb14ae7&_gl=1*1l0mli7*_ga*ODU2ODI5OTkyLjE2NzA2ODcwMDE.*_ga_XQVN5K27XX*MTY3MDY4NzAwMC4xLjEuMTY3MDY4NzYyMy41Ni4wLjA.&_ga=2.153775102.712544994.1670687001-856829992.1670687001";
                 // var url = textBox1.Text; //"https://www.nhatot.com/mua-ban-nha-dat-quan-8-tp-ho-chi-minh/98893753.htm?_gl=1*1apb9n6*_ga*NTM0NDM0NDYzLjE2NzA2OTM4ODI.*_ga_XQVN5K27XX*MTY3MDY5Mzg4MS4xLjAuMTY3MDY5Mzg4MS42MC4wLjA.&_ga=2.28176127.2128205471.1670693882-534434463.1670693882";
                 //var url = "https://www.chotot.com/mua-ban-dien-thoai-thi-xa-tan-uyen-binh-duong/100554829.htm";
                try
                {
                    driverC.Url = url;
                }
                catch (Exception ex)
                {
                    if (ex.Message.ToLower().Contains("out"))
                    {
                        driverC.Quit();
                        button1.PerformClick();
                    }
                } 
                Actions builder = new Actions(driverC);
                try
                {
                    var f = driverC.FindElement(By.Id("layers"));
                    if (f.GetAttribute("style").Contains("absolute"))
                    {
                        (driverC).ExecuteScript("arguments[0].style='display: relative;'", f);
                    }
                }
                catch
                {
                    try
                    {
                        builder.SendKeys(OpenQA.Selenium.Keys.Escape).Build().Perform();
                    }
                    catch { }
                }
                try
                {
                    RetrievePhoneNumberFirstapproch(driverC);
                }
                catch(Exception ex)
                {
                    RetrievePhoneNumber(driverC);
                }
            }
            finally
            {
                 driverC.FindElement(By.XPath("//button[.='Tin Tiếp']")).Click();
                Thread.Sleep(3000);
                textBox1.Text = driverC.Url;
            }
        }
        private void RetrievePhoneNumberFirstapproch(IWebDriver driverC)
        {
            var g = driverC.FindElement(By.CssSelector("div[class*='IntersectBox'] img"));
            var ele = driverC.FindElement(By.CssSelector("div[class*='IntersectBox'] img"));
             
            // var ele= driverC.FindElement(By.CssSelector("div:contains('showPhoneButton')"));
             
            var d = ele.GetAttribute("src").Trim();
            if (d == "https://static.chotot.com/storage/chotot-icons/svg/white-phone.svg")
            {
                try
                {
                    //  WebDriverWait 
                    ele.Location.Offset(ele.Location.X + 1, ele.Location.Y);
                    ele.Click();
                    var text = driverC.FindElements(By.CssSelector("div[class*='IntersectBox'] span"));
                    putEles(text);
                }
                catch { }
            }
        }
        private void putEles( System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> text )
        {
            foreach (var txt in text)
            {
                var str = txt.Text;
                if (Int64.TryParse(str, out long re))
                {
                    try
                    {
                        RegisterPhone(str);
                    }
                    catch
                    {

                    }

                }
            }
        }
        private void RegisterPhone(string phone)
        {
            if (!lst.Contains(phone))
            {
                lst.Add(phone);
                File.AppendAllText(pth, phone + Environment.NewLine);

                // File.WriteAllText(pth, File.ReadAllText(pth) + Environment.NewLine + phone);
                if (lst.Count == MaxTimes)
                {
                    Is50000 = true;
                }
            }
            lblCount.Text = lst.Count.ToString();

        }
        private void RetrievePhoneNumber(IWebDriver driverC)
        { 
            var elements = driverC.FindElements(By.TagName("img"));
            foreach (var c in elements)
            {
                bool IsSrc = true;
                var src = "";
                try
                {
                    src = c.GetAttribute("src").TrimEnd();
                }
                catch
                {
                    IsSrc = false;
                }
                if (IsSrc)
                {
                    //IntersectBox
                    //https://static.chotot.com/storage/chotot-icons/svg/white-phone.svg
                    if (src == "https://static.chotot.com/storage/chotot-icons/svg/white-phone.svg")
                    {
                        c.Click();
                       var spanlist= driverC.FindElements(By.TagName("span"));
                        foreach (var s in spanlist)
                        {
                            var valNumber = s.Text.Trim();
                            if (Int64.TryParse(valNumber, out long re) && valNumber.Length >= 9)
                            {
                                try
                                {
                                    RegisterPhone(valNumber);
                                    break;
                                }
                                catch
                                {

                                }

                            }
                        } 
                    }
                }
            } 
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            if (!Directory.Exists(@"C:\chotot"))
            {
                Directory.CreateDirectory(@"C:\chotot");
            }
            if (!File.Exists(pth))
            {
                File.Create(pth);
            }
            lst = File.ReadAllLines(pth).ToList<string>();
            lblCount.Text = lst.Count.ToString();
            txtData.Text = pth;
            txtDriver.Text = DriverPath;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start( Path.GetDirectoryName(pth));
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                cdr.Quit();
            }
            catch { }
        }

        private void button3_Click(object sender, EventArgs e)
        {
           
             
            openFileDialog1.Filter = "Chrome Driver file (*.exe)|*.exe|All files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtDriver.Text =openFileDialog1.FileName;
            }
        }
    }
}
