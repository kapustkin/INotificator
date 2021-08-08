using System;
using System.Net;
using System.Net.Http;
using INotificator.Common.Interfaces.Receivers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace INotificator.Services.Receivers
{
    public class DnsReceiver : BaseHttpReceiver, IDnsReceiver
    {
        private readonly IHttpClientFactory _clientFactory;

        public DnsReceiver(IHttpClientFactory clientFactory, ILogger<DnsReceiver> logger) : base(clientFactory, logger)
        {
            _clientFactory = clientFactory;
        }

        protected override HttpClient GetHttpClient()
        {
            var client = _clientFactory.CreateClient();

            return client;
        }

        protected override HttpRequestMessage HttpRequest(string path)
        {
            if (path == null)
            {
                throw new NotSupportedException("Path must be non null");
            }

            var request = new HttpRequestMessage(HttpMethod.Get, path);
            request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:86.0) Gecko/20100101 Firefox/86.0");
            request.Headers.Add("Cookie", "PHPSESSID=df8912ca86c43950901d4eabc538c3f5;current_path=862068630b7e0bd77a44b9920e007fc57b8b7f671a85ae5d38b54b7e3ba3ce85a%3A2%3A%7Bi%3A0%3Bs%3A12%3A%22current_path%22%3Bi%3A1%3Bs%3A64%3A%22%7B%22city%22%3A%22ee8c868d-187f-11e9-a206-00155d03332b%22%2C%22method%22%3A%22geoip%22%7D%22%3B%7D;phonesIdent=33ba51fb9e25c73b73c4bbe8a7428e39574de9c0d85baade601e259fd8a2dbdfa%3A2%3A%7Bi%3A0%3Bs%3A11%3A%22phonesIdent%22%3Bi%3A1%3Bs%3A36%3A%2265da0037-f903-466d-8c53-f3f5ad5960ad%22%3B%7D; rerf=AAAAAGC34rNFiTdmAyNOAg==;ipp_uid=1622663859337/QmuChHraeTGpmGna/nJZzGjiUmux+ie8zaUCjNA==;ipp_uid1=1622663859337;ipp_uid2=QmuChHraeTGpmGna/nJZzGjiUmux+ie8zaUCjNA==;cartUserCookieIdent_v3=b965bc71a0cd2894edacddcd91436ac4abe24be7dcc0efeeacbb93e77586f69ea%3A2%3A%7Bi%3A0%3Bs%3A22%3A%22cartUserCookieIdent_v3%22%3Bi%3A1%3Bs%3A36%3A%22ce54aa25-a10d-37e8-be79-1bbad670428f%22%3B%7D; orderCheckoutIdent=6f3dd1e7914eeeb9e3c8f67fd370f188de75c5a53ac9808709be497009bf8760a%3A2%3A%7Bi%3A0%3Bs%3A18%3A%22orderCheckoutIdent%22%3Bi%3A1%3Bs%3A36%3A%22ce54aa25-a10d-37e8-be79-1bbad670428f%22%3B%7D; wishlist-id=ab3e237d4fc5c3386aca52b949247415d988e4e88289b83ea5a8c95b52e26775a%3A2%3A%7Bi%3A0%3Bs%3A11%3A%22wishlist-id%22%3Bi%3A1%3Bs%3A36%3A%22febecf97-355b-4260-8ade-e92c9b81c010%22%3B%7D; _vi=63df4d7a165e500c4fd36f97ee2a36766e68886b9827826a6468422e14d6223ba%3A2%3A%7Bi%3A0%3Bs%3A3%3A%22_vi%22%3Bi%3A1%3Bs%3A32%3A%221491ab9a2293c4f525aa4bf5f8bcdb68%22%3B%7D; rsu-configuration-id=114719cea76f6ab4c5c938ef4d618fc2e2d697405c6f10e210db3e90fc3dd942a%3A2%3A%7Bi%3A0%3Bs%3A20%3A%22rsu-configuration-id%22%3Bi%3A1%3Bs%3A36%3A%222bf081ba-53bf-470b-a988-1cf2f3476f5d%22%3B%7D; configurationTutorialFinished=a204129f8e308b61d2cc5533331af3b5c8a6f96c3546a7f18b37a24e07723998a%3A2%3A%7Bi%3A0%3Bs%3A29%3A%22configurationTutorialFinished%22%3Bi%3A1%3Bi%3A1%3B%7D; preferProxy=a0db8deb6829c01648507cad3bfbfc7800c88e80b51dc605c5007e9b33740e28a%3A2%3A%7Bi%3A0%3Bs%3A11%3A%22preferProxy%22%3Bi%3A1%3Bs%3A42%3A%22https%3A%2F%2Fmow-proxy-asmsys.dns-shop.ru%3A17589%22%3B%7D; dnsauth_csrf=0bf1cffa0e13036b8a7a7567be9dd952ecc6d831c6441781b5265bef8e968cc4a%3A2%3A%7Bi%3A0%3Bs%3A12%3A%22dnsauth_csrf%22%3Bi%3A1%3Bs%3A36%3A%22defd64ab-a46e-413f-8d08-1947008738f1%22%3B%7D; _csrf=fb2f15358cda69ade5b55c40e8765997dad5e164b765474bbb0f32df701f3819a%3A2%3A%7Bi%3A0%3Bs%3A5%3A%22_csrf%22%3Bi%3A1%3Bs%3A32%3A%22YORBJuLhZ1z9fu8Sbl1_Ew1M91_cnLL5%22%3B%7D; ipp_key=v1623176924543/v3394bd400b5e53a13cfc65163aeca6afa04ab3/2dVLt9jrZJFHyT8QxSb0ww==");

            var cookieContainer = new CookieContainer();


            return request;
        }
    }
}