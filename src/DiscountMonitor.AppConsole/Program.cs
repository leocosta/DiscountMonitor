using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using DiscountMonitor.AppConsole.Data;
using DiscountMonitor.AppConsole.Models;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using RazorEngine;
using RazorEngine.Templating;
using XPlot.Plotly;

namespace DiscountMonitor.AppConsole
{
    class Program
    {
        const string Url = "https://www.magazineluiza.com.br/kit-fralda-pampers-premium-care-jumbo-tamanho-m-240-unidades/p/ffa6eej339/me/fdes/";
        const string ProductReportTemplate = "./Templates/ProductReport.cshtml";
        const string ProductReportFileName = "./Reports/ProductReport.pdf";

        static async Task Main(string[] args)
        {
            Console.Clear();

            CultureInfo.CurrentCulture = new CultureInfo("pt-BR");

            var webProduct = await GetProductFromWebAsync(Url);
            var product = await TrackPriceAndSaveProductAsync(webProduct);

            await GenerateProductReportInPdfAsync(product);

            await AlertIfDiscountWasRecheadAsync(product);
        }

        private static async Task<WebProduct> GetProductFromWebAsync(string url)
        {
            Console.WriteLine($"Obtendo dados do produto da página { url }.");

            using (var httpClient = new HttpClient())
            {
                var htmlContent = await httpClient.GetStringAsync(url);
                var htmlDocument = new HtmlDocument();

                htmlDocument.LoadHtml(htmlContent);

                var productName = htmlDocument.DocumentNode.SelectSingleNode("//h1");
                var productPrice = htmlDocument.DocumentNode.SelectSingleNode("//span[@class=\"price-template__text\"]");

                var ignoredTags = new[] { "h2", "table" };
                var productDescription = htmlDocument.DocumentNode
                    .SelectSingleNode("//div[@class=\"description__container-text\"]")
                    .ChildNodes
                    .Where(i => !ignoredTags.Contains(i.OriginalName) && !string.IsNullOrWhiteSpace(i.InnerText))
                    .FirstOrDefault();

                var product = new WebProduct
                {
                    Name = productName.InnerText,
                    Price = Convert.ToDouble(productPrice.InnerText),
                    Description = productDescription.InnerText,
                    Url = url
                };

                return product;
            }
        }

        private static async Task<Product> TrackPriceAndSaveProductAsync(WebProduct webProduct)
        {
            using (var dbContext = new DiscountMonitorContext())
            {
                var product = dbContext.Products
                    .Include(i => i.HistoryPrices)
                    .SingleOrDefault(i => i.Url.Equals(webProduct.Url));

                product ??= new Product(
                    webProduct.Name,
                    webProduct.Price,
                    webProduct.Description,
                    webProduct.Url
                );

                product.VerifyAndTrackPrice(webProduct.Price);
                if (product.PriceWasChanged)
                {
                    Console.WriteLine("O preço do produto foi alterado.");
                }

                dbContext.Products.Update(product);
                await dbContext.SaveChangesAsync();

                return product;
            }
        }

        private static string GetProductReport(Product product)
        {
            var htmlChart = GetHistoryPriceChartHtml(product);
            var model = new
            {
                Product = product,
                Chart = htmlChart
            };

            var htmlReport = GetProductReportHtml(model);

            return htmlReport;
        }

        private static string GetHistoryPriceChartHtml(Product product)
        {
            var groupedData = product.HistoryPrices
                .OrderBy(i => i.Date)
                .GroupBy(i => i.Date.ToString("dd/MM/yyyy"))
                .Select(g => new
                {
                    Date = g.Key,
                    AveragePrice = g.Average(a => a.Price)
                });

            var chart = Chart.Plot(new Graph.Scatter
            {
                x = groupedData.Select(i => i.Date),
                y = groupedData.Select(i => i.AveragePrice),
                fill = "tozeroy"
            });
            
            chart.WithTitle("Histórico de Variação de Preço");

            var htmlContent = chart.GetInlineHtml();

            return htmlContent;
        }

        private static string GetProductReportHtml(object model)
        {
            var htmlContent = RenderPage(ProductReportTemplate, model);

            return htmlContent;
        }

        private static string RenderPage(string templatePath, object model)
        {
            var templateContent = File.ReadAllText(templatePath);
            var htmlContent = Engine.Razor.RunCompile(templateContent, templatePath, null, model);

            return htmlContent;
        }

        private static async Task GenerateProductReportInPdfAsync(Product product)
        {
            var htmlContent = GetProductReport(product);

            await SaveToPdf(ProductReportFileName, htmlContent);
        }

        private static async Task SaveToPdf(string file, string htmlContent)
        {
            await new PuppeteerSharp.BrowserFetcher()
                .DownloadAsync(PuppeteerSharp.BrowserFetcher.DefaultRevision);

            var browser = await PuppeteerSharp.Puppeteer.LaunchAsync(new PuppeteerSharp.LaunchOptions
            {
                Headless = true
            });

            using (var page = await browser.NewPageAsync())
            {
                await page.SetContentAsync(htmlContent);
                await page.PdfAsync(file);
            }
        }

        private static async Task AlertIfDiscountWasRecheadAsync(Product product)
        {
            if (!product.DiscountWasReached())
            {
                return;
            }

            Console.WriteLine("Enviando alerta de desconto por e-mail.");

            await SendMailAsync("leonardo@growiz.com.br",
                $"Relatório do produto: { product.Name }",
                "Segue em anexo relatório do produto.",
                ProductReportFileName);
        }

        private async static Task SendMailAsync(string email, string subject, string message, string attachment = default)
        {
            try
            {
                var mailMessage = new MailMessage()
                {
                    From = new MailAddress("contato@growiz.com.br", "Growiz")
                };

                mailMessage.To.Add(new MailAddress(email));

                mailMessage.Subject = subject;
                mailMessage.Body = message;
                mailMessage.IsBodyHtml = true;

                mailMessage.Attachments.Add(new Attachment(attachment));

                using (var smtp = new SmtpClient("smtp.sendgrid.net", 587))
                {
                    smtp.Credentials = new NetworkCredential("<API_KEY>", "<AUTHENTICATION_TOKEN>");
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(mailMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}