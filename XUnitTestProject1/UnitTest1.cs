using IdentityModel.Client;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Net;
using System.Net.Http;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace XUnitTestProject1
{
    public class UnitTest1
    {
        private readonly TestServer _myserver;
        private readonly TestServer _idsrvserver;

        public UnitTest1()
        {
            //create identityserver4 in a testserver
            var idsrvwebhost = new WebHostBuilder()
            .UseStartup<idsrvStartup>();
            _idsrvserver = new TestServer(idsrvwebhost);


            //build myApi TestServer
            var options = new JwtBearerOptions()
            {
                //totally garbage, becuase the test server httpclient doesnt care/use the proto/host/port ... only the path
                Authority = "http://localhost",
                Audience = "api1",    //from the scopes

                // IMPORTANT PART HERE
                 BackchannelHttpHandler = _idsrvserver.CreateHandler(),
                 RequireHttpsMetadata = false
            };

            var mywebhost = new WebHostBuilder()
            .UseStartup<myStartup>();

            //we need that backchannelhandler, into our api testserver
            mywebhost.ConfigureServices(c => c.AddSingleton(options));

            _myserver = new TestServer(mywebhost);
        }

        [Fact]
        public async void ShouldNotAllowAnonymousUser()
        {
            var httpClient = _myserver.CreateClient();

            //can use anything for the protocol/host/port ?????
            var result = await httpClient.GetAsync("http://XXXXXXXXXXXXXXXXXXXXXX/api/Thingie");
            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Fact]
        public async void ShouldReturnValuesForAuthenticatedUser()
        {
            var idsrvhttpClient = _idsrvserver.CreateClient();
            var httpClient = _myserver.CreateClient();

            var disco = await idsrvhttpClient.GetDiscoveryDocumentAsync();
            if (disco.IsError) throw new Exception(disco.Error);

            //notice we use the identityserver's httpclient ..... BECAUSE it actually connects to the identityserver testserver NOT the api test server
            //(tell them to FIX this)
            var tokenResponse = await idsrvhttpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "client",
                ClientSecret = "secret",
                Scope = "api1"
            });
            Assert.NotEmpty(tokenResponse.AccessToken);

            httpClient.SetBearerToken(token: tokenResponse.AccessToken);

            var result = await httpClient.GetAsync("http://XXXXXXXXXXXXXXXXXXXXXX/api/Thingie");
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }
    }
}
