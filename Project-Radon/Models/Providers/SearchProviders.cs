using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Media.Imaging;

namespace Project_Radon.ViewModels.Providers
{
    public class SearchProviders
    {
        public BitmapImage Image { get; set; }
        public string ProviderUrl { get; }
        public string ProviderName { get; }
        public int Index { get; }

        public SearchProviders(BitmapImage image, string providerUrl, string providerName, int index)
        {
            Image = image;
            ProviderUrl = providerUrl;
            ProviderName = providerName;
            Index = index;
        }

        public static List<SearchProviders> ProvidersList = new()
        {
            new SearchProviders(new BitmapImage(new Uri("https://www.google.com/favicon.ico")), "https://www.google.com/search?q=", "Google", 0),
            new SearchProviders(new BitmapImage(new Uri("https://www.bing.com/favicon.ico")), "https://www.bing.com/search?q=", "Bing", 1),
            new SearchProviders(new BitmapImage(new Uri("https://search.yahoo.com/favicon.ico")), "https://search.yahoo.com/search?p=", "Yahoo!", 2),
            new SearchProviders(new BitmapImage(new Uri("https://www.duckduckgo.com/favicon.ico")), "https://duckduckgo.com/?q=", "DuckDuckGo", 3),
            new SearchProviders(new BitmapImage(new Uri("https://www.ask.com/favicon.ico")), "https://www.ask.com/web?q=", "Ask", 4),
            //new SearchProviders(new BitmapImage(new Uri("https://www.baidu.com/favicon.ico")), "https://www.baidu.com/s?ie=utf-8&f=8&rsv_bp=1&rsv_idx=1&tn=baidu&wd=", "Baidu", 5),
            //new SearchProviders(new BitmapImage(new Uri("https://www.ecosia.org/favicon.ico")), "https://www.ecosia.org/search?q=", "Ecosia", 6),
            //new SearchProviders(new BitmapImage(new Uri("https://www.startpage.com/favicon.ico")), "https://www.startpage.com/search?q=", "Startpage", 7),
            new SearchProviders(new BitmapImage(new Uri("https://www.qwant.com/favicon.ico")), "https://www.qwant.com/?q=", "Qwant", 8),

            //new SearchProviders(new BitmapImage(new Uri("https://www.google.com/s2/favicons?sz=64&domain=https://lite.qwant.com")), "https://lite.qwant.com/?q=", "Qwant Lite", 9),

            //new SearchProviders(new BitmapImage(new Uri("https://presearch.com/favicon.ico")), "https://presearch.com/search?q=", "Presearch", 10),
            //new SearchProviders(new BitmapImage(new Uri("https://swisscows.com/favicon.ico")), "https://swisscows.com/web?query=", "Swisscows", 11),

            //new SearchProviders(new BitmapImage(new Uri("https://www.dogpile.com/static/info.dogpile.com/favicon.ico")), "https://www.dogpile.com/serp?q=", "Dogpile", 12),
            //new SearchProviders(new BitmapImage(new Uri("https://www.webcrawler.com/static/www.webcrawler.com/favicon.ico")), "https://www.webcrawler.com/serp?q=", "Webcrawler", 13),
            //new SearchProviders(new BitmapImage(new Uri("https://you.com/favicon/favicon-32x32.png?v=2")), "https://you.com/search?q=", "You", 14),
            //new SearchProviders(new BitmapImage(new Uri("https://results.excite.com/static/excite/455/favicon.ico")), "https://results.excite.com/serp?q=", "Excite", 15),

            //new SearchProviders(new BitmapImage(new Uri("https://search20.lycos.com/favicon.ico")), "https://search20.lycos.com/web/?q=", "Lycos", 16),
            //new SearchProviders(new BitmapImage(new Uri("https://www.metacrawler.com/static/www.metacrawler.com/favicon.ico")), "https://www.metacrawler.com/serp?q=", "Metacrawler", 17),
            //new SearchProviders(new BitmapImage(new Uri("https://www.mojeek.com/favicon.ico")), "https://www.mojeek.com/search?q=", "Mojeek", 18),

            new SearchProviders(new BitmapImage(new Uri("https://cdn.search.brave.com/serp/v2/_app/immutable/assets/favicon.acxxetWH.ico")), "https://search.brave.com/search?q=", "BraveSearch", 19)
        };


        public static string GetSearchProviderUrl(string providerName)
        {
            SearchProviders provider = SearchProviders.ProvidersList
                .FirstOrDefault(p => p.ProviderName.Equals(providerName, StringComparison.OrdinalIgnoreCase));

            return provider?.ProviderUrl ?? "https://www.google.com/search?q="; // Return a default URL if no match is found
        }
    }
}