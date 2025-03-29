using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Data.Xml.Dom;
using Windows.Storage;


namespace Project_Radon.Helpers
{
	public class UrlValidater
	{
		public static async Task<bool> IsUrlReachable(Uri url)
		{
			using HttpClient httpClient = new();
			httpClient.BaseAddress = url;
			try
			{
				HttpResponseMessage response = await httpClient.GetAsync("");
				if (response.IsSuccessStatusCode)
				{
					Console.WriteLine($"URL is reachable: {url}");
					return true;
				}
				else
				{
					Console.WriteLine($"URL returned status code {response.StatusCode}: {url}");
					return false;
				}
			}
			catch (HttpRequestException e)
			{
				Console.WriteLine($"Request error: {e.Message}");
				return false;
			}
			catch (Exception e)
			{
				Console.WriteLine($"Unexpected error: {e.Message}");
				return false;
			}
		}
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
		public static Uri? GetValidateUrl(string queryText)
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
		{
			Regex regex = new(@"^(http|https|ms-appx|ms-appx-web|ftp|firebrowseruser|firebrowserwinui|firebrowserincog|firebrowser)\://|[a-zA-Z0-9\-\.]+\.[a-zA-Z](:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s]$", RegexOptions.IgnoreCase);

			Uri uriOut;
			//Regex($@"^(http|https|ms-appx|ms-appx-web|ftp|firebrowseruser|firebrowserwinui|firebrowserincog)\://|[a-zA-Z0-9\-\.]+\.[a-zA-Z](:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s]$", RegexOptions.IgnoreCase);

			if (IsUrlValid(queryText, regex, out Uri sendUri))
			{
				uriOut = sendUri != null ? new UriBuilder(sendUri.ToString()).Uri : new Uri(queryText);
			}
			else
			{
				return null;
			}

			return uriOut;
		}

		private static bool IsUrlValid(string url, Regex regex, out Uri uri)
		{
			uri = null;
			if (regex.IsMatch(url))
			{
				if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri))
				{
					return true;
				}
			}
			return false;
		}

		private static async Task<(string Name, string DisplayName)[]> GetProtocolsFromManifest()
		{
			StorageFolder packageFolder = Package.Current.InstalledLocation;
			StorageFile manifestFile = await packageFolder.GetFileAsync("AppxManifest.xml");
			string manifestContent = await FileIO.ReadTextAsync(manifestFile);
			XmlDocument xmlDoc = new();
			xmlDoc.LoadXml(manifestContent);

			XmlNodeList protocolNodes = xmlDoc.SelectNodesNS("//uap:Protocol", "xmlns:uap=\"http://schemas.microsoft.com/appx/manifest/uap/windows10\"");
			(string Name, string DisplayName)[] protocols = protocolNodes.Select(node => (
				Name: node.Attributes.GetNamedItem("Name").InnerText,
				DisplayName: node.SelectSingleNodeNS("uap:DisplayName", "xmlns:uap=\"http://schemas.microsoft.com/appx/manifest/uap/windows10\"").InnerText
			)).ToArray();

			return protocols;
		}
	}
}


