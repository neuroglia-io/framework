using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Neuroglia;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Mvc.Testing
{

    /// <summary>
    /// Represents the base class for all API tests
    /// </summary>
    public abstract class ApiControllerTest
    {

        /// <summary>
        /// Initializes a new <see cref="ApiControllerTest"/>
        /// </summary>
        /// <param name="apiFactory">The <see cref="IApiFactory"/> used to create test <see cref="HttpClient"/>s</param>
        protected ApiControllerTest(IApiFactory apiFactory)
        {
            this.ApiFactory = apiFactory;
            this.LinkGenerator = this.Services.GetRequiredService<LinkGenerator>();
            this.SerializerSettings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        /// <summary>
        /// Gets the <see cref="IApiFactory"/> used to create test <see cref="HttpClient"/>s
        /// </summary>
        protected IApiFactory ApiFactory { get; }

        /// <summary>
        /// Gets the service used to generate links
        /// </summary>
        protected LinkGenerator LinkGenerator { get; }

        /// <summary>
        /// Gets the <see cref="JsonSerializerSettings"/> to use
        /// </summary>
        protected JsonSerializerSettings SerializerSettings { get; }

        /// <summary>
        /// Gets the current <see cref="IServiceProvider"/>
        /// </summary>
        protected IServiceProvider Services
        {
            get
            {
                return this.ApiFactory.Services;
            }
        }

        /// <summary>
        /// Executes a GET request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TController">°The type of the <see cref="ControllerBase"/> to request</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task GetAsync<TController>(string action, object routeValues = null)
            where TController : ControllerBase
        {
            using (HttpClient httpClient = this.ApiFactory.CreateClient())
            {
                using (HttpResponseMessage response = await httpClient.GetAsync(this.BuildUrl<TController>(action, routeValues)))
                {
                    await EnsureSuccessStatusCodeForAsync(response);
                }
            }
        }

        /// <summary>
        /// Executes a GET request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TController">°The type of the <see cref="ControllerBase"/> to request</typeparam>
        /// <typeparam name="TResult">The type of expected result</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task<TResult> GetAsync<TController, TResult>(string action, object routeValues = null)
            where TController : ControllerBase
        {
            using (HttpClient httpClient = this.ApiFactory.CreateClient())
            {
                using (HttpResponseMessage response = await httpClient.GetAsync(this.BuildUrl<TController>(action, routeValues)))
                {
                    string json = await response.Content?.ReadAsStringAsync();
                    await EnsureSuccessStatusCodeForAsync(response);
                    if (string.IsNullOrWhiteSpace(json))
                        return default;
                    else
                        return this.Deserialize<TResult>(json);
                }
            }
        }

        /// <summary>
        /// Executes a POST request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TController">°The type of the <see cref="ControllerBase"/> to request</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task PostAsync<TController>(string action, object routeValues = null)
             where TController : ControllerBase
        {
            using (HttpClient httpClient = this.ApiFactory.CreateClient())
            {
                using (HttpResponseMessage response = await httpClient.PostAsync(this.BuildUrl<TController>(action, routeValues), null))
                {
                    await EnsureSuccessStatusCodeForAsync(response);
                }
            }
        }

        /// <summary>
        /// Executes a POST request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TController">°The type of the <see cref="ControllerBase"/> to request</typeparam>
        /// <typeparam name="TResult">The type of expected result</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task<TResult> PostAsync<TController, TResult>(string action, object routeValues = null)
             where TController : ControllerBase
        {
            using (HttpClient httpClient = this.ApiFactory.CreateClient())
            {
                using (HttpResponseMessage response = await httpClient.PostAsync(this.BuildUrl<TController>(action, routeValues), null))
                {
                    string json = await response.Content?.ReadAsStringAsync();
                    await EnsureSuccessStatusCodeForAsync(response);
                    if (string.IsNullOrWhiteSpace(json))
                        return default;
                    else
                        return this.Deserialize<TResult>(json);
                }
            }
        }

        /// <summary>
        /// Executes a POST request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TController">°The type of the <see cref="ControllerBase"/> to request</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="payload">The payload to send as a JSON content</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task PostAsJsonAsync<TController>(string action, object payload = default, object routeValues = null)
             where TController : ControllerBase
        {
            using (HttpClient httpClient = this.ApiFactory.CreateClient())
            {
                using (HttpResponseMessage response = await httpClient.PostAsJsonAsync(this.BuildUrl<TController>(action, routeValues), payload))
                {
                    string json = await response.Content?.ReadAsStringAsync();
                    await EnsureSuccessStatusCodeForAsync(response);
                }
            }
        }

        /// <summary>
        /// Executes a POST request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TController">°The type of the <see cref="ControllerBase"/> to request</typeparam>
        /// <typeparam name="TResult">The type of expected result</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="payload">The payload to send as a JSON content</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task<TResult> PostAsJsonAsync<TController, TResult>(string action, object payload = default, object routeValues = null)
             where TController : ControllerBase
        {
            using (HttpClient httpClient = this.ApiFactory.CreateClient())
            {
                using (HttpResponseMessage response = await httpClient.PostAsJsonAsync(this.BuildUrl<TController>(action, routeValues), payload))
                {
                    string json = await response.Content?.ReadAsStringAsync();
                    await EnsureSuccessStatusCodeForAsync(response);
                    if (string.IsNullOrWhiteSpace(json))
                        return default;
                    else
                        return this.Deserialize<TResult>(json);
                }
            }
        }

        /// <summary>
        /// Executes a POST request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TController">°The type of the <see cref="ControllerBase"/> to request</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="payload">The payload to send as a form URL-encoded content</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task PostAsFormAsync<TController>(string action, object payload = default, object routeValues = null)
            where TController : ControllerBase
        {
            using (HttpClient httpClient = this.ApiFactory.CreateClient())
            {

                using (HttpResponseMessage response = await httpClient.PostAsync(this.BuildUrl<TController>(action, routeValues), GetFormContent(payload)))
                {
                    string json = await response.Content?.ReadAsStringAsync();
                    await EnsureSuccessStatusCodeForAsync(response);
                }
            }
        }

        /// <summary>
        /// Executes a POST request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TController">°The type of the <see cref="ControllerBase"/> to request</typeparam>
        /// <typeparam name="TResult">The type of expected result</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="payload">The payload to send as a form URL-content content</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task<TResult> PostAsFormAsync<TController, TResult>(string action, object payload = default, object routeValues = null)
            where TController : ControllerBase
        {
            using (HttpClient httpClient = this.ApiFactory.CreateClient())
            {
                using (HttpResponseMessage response = await httpClient.PostAsync(this.BuildUrl<TController>(action, routeValues), GetFormContent(payload)))
                {
                    string json = await response.Content?.ReadAsStringAsync();
                    await EnsureSuccessStatusCodeForAsync(response);
                    if (string.IsNullOrWhiteSpace(json))
                        return default;
                    else
                        return this.Deserialize<TResult>(json);
                }
            }
        }

        /// <summary>
        /// Executes a POST request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TController">°The type of the <see cref="ControllerBase"/> to request</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="payload">The payload to send as a form multipart content</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task PostAsMultipartAsync<TController>(string action, object payload = default, object routeValues = null)
           where TController : ControllerBase
        {
            using (HttpClient httpClient = this.ApiFactory.CreateClient())
            {
                using (HttpResponseMessage response = await httpClient.PostAsync(this.BuildUrl<TController>(action, routeValues), GetMultipartContent(payload)))
                {
                    string json = await response.Content?.ReadAsStringAsync();
                    await EnsureSuccessStatusCodeForAsync(response);
                }
            }
        }

        /// <summary>
        /// Executes a POST request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TController">°The type of the <see cref="ControllerBase"/> to request</typeparam>
        /// <typeparam name="TResult">The type of expected result</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="payload">The payload to send as a form multipart content</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task<TResult> PostAsMultipartAsync<TController, TResult>(string action, object payload = default, object routeValues = null)
            where TController : ControllerBase
        {
            using (HttpClient httpClient = this.ApiFactory.CreateClient())
            {
                using (HttpResponseMessage response = await httpClient.PostAsync(this.BuildUrl<TController>(action, routeValues), GetMultipartContent(payload)))
                {
                    string json = await response.Content?.ReadAsStringAsync();
                    await EnsureSuccessStatusCodeForAsync(response);
                    if (string.IsNullOrWhiteSpace(json))
                        return default;
                    else
                        return this.Deserialize<TResult>(json);
                }
            }
        }

        /// <summary>
        /// Executes a PATCH request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TController">°The type of the <see cref="ControllerBase"/> to request</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task PatchAsync<TController>(string action, object routeValues = null)
             where TController : ControllerBase
        {
            using (HttpClient httpClient = this.ApiFactory.CreateClient())
            {
                using (HttpResponseMessage response = await httpClient.PatchAsync(this.BuildUrl<TController>(action, routeValues), null))
                {
                    await EnsureSuccessStatusCodeForAsync(response);
                }
            }
        }

        /// <summary>
        /// Executes a PATCH request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TController">°The type of the <see cref="ControllerBase"/> to request</typeparam>
        /// <typeparam name="TResult">The type of expected result</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task<TResult> PatchAsync<TController, TResult>(string action, object routeValues = null)
             where TController : ControllerBase
        {
            using (HttpClient httpClient = this.ApiFactory.CreateClient())
            {
                using (HttpResponseMessage response = await httpClient.PatchAsync(this.BuildUrl<TController>(action, routeValues), null))
                {
                    string json = await response.Content?.ReadAsStringAsync();
                    await EnsureSuccessStatusCodeForAsync(response);
                    if (string.IsNullOrWhiteSpace(json))
                        return default;
                    else
                        return this.Deserialize<TResult>(json);
                }
            }
        }

        /// <summary>
        /// Executes a PATCH request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TController">°The type of the <see cref="ControllerBase"/> to request</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="payload">The payload to send as a JSON content</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task PatchAsJsonAsync<TController>(string action, object payload = default, object routeValues = null)
            where TController : ControllerBase
        {
            string json = payload == null ? null : JsonConvert.SerializeObject(payload);
            HttpContent content = null;
            if (!string.IsNullOrWhiteSpace(json))
                content = new StringContent(json, Encoding.UTF8, "application/json");
            using (HttpClient httpClient = this.ApiFactory.CreateClient())
            {
                using (HttpResponseMessage response = await httpClient.PatchAsync(this.BuildUrl<TController>(action, routeValues), content))
                {
                    await EnsureSuccessStatusCodeForAsync(response);
                }
            }
        }

        /// <summary>
        /// Executes a PATCH request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TController">°The type of the <see cref="ControllerBase"/> to request</typeparam>
        /// <typeparam name="TResult">The type of expected result</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="payload">The payload to send as a JSON content</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task<TResult> PatchAsJsonAsync<TController, TResult>(string action, object payload = default, object routeValues = null)
             where TController : ControllerBase
        {
            string json = payload == null ? null : JsonConvert.SerializeObject(payload);
            HttpContent content = null;
            if (!string.IsNullOrWhiteSpace(json))
                content = new StringContent(json, Encoding.UTF8, "application/json");
            using (HttpClient httpClient = this.ApiFactory.CreateClient())
            {
                using (HttpResponseMessage response = await httpClient.PatchAsync(this.BuildUrl<TController>(action, routeValues), content))
                {
                    json = await response.Content?.ReadAsStringAsync();
                    await EnsureSuccessStatusCodeForAsync(response);
                    if (string.IsNullOrWhiteSpace(json))
                        return default;
                    else
                        return this.Deserialize<TResult>(json);
                }
            }
        }

        /// <summary>
        /// Executes a PATCH request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TController">°The type of the <see cref="ControllerBase"/> to request</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="payload">The payload to send as a form URL-content content</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task PatchAsFormAsync<TController>(string action, object payload = default, object routeValues = null)
            where TController : ControllerBase
        {
            using (HttpClient httpClient = this.ApiFactory.CreateClient())
            {

                using (HttpResponseMessage response = await httpClient.PatchAsync(this.BuildUrl<TController>(action, routeValues), GetFormContent(payload)))
                {
                    string json = await response.Content?.ReadAsStringAsync();
                    await EnsureSuccessStatusCodeForAsync(response);
                }
            }
        }

        /// <summary>
        /// Executes a PATCH request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TController">°The type of the <see cref="ControllerBase"/> to request</typeparam>
        /// <typeparam name="TResult">The type of expected result</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="payload">The payload to send as a form URL-content content</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task<TResult> PatchAsFormAsync<TController, TResult>(string action, object payload = default, object routeValues = null)
            where TController : ControllerBase
        {
            using (HttpClient httpClient = this.ApiFactory.CreateClient())
            {
                using (HttpResponseMessage response = await httpClient.PatchAsync(this.BuildUrl<TController>(action, routeValues), GetFormContent(payload)))
                {
                    string json = await response.Content?.ReadAsStringAsync();
                    await EnsureSuccessStatusCodeForAsync(response);
                    if (string.IsNullOrWhiteSpace(json))
                        return default;
                    else
                        return this.Deserialize<TResult>(json);
                }
            }
        }

        /// <summary>
        /// Executes a PATCH request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TController">°The type of the <see cref="ControllerBase"/> to request</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="payload">The payload to send as a form multipart content</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task PatchAsMultipartAsync<TController>(string action, object payload = default, object routeValues = null)
           where TController : ControllerBase
        {
            using (HttpClient httpClient = this.ApiFactory.CreateClient())
            {
                using (HttpResponseMessage response = await httpClient.PatchAsync(this.BuildUrl<TController>(action, routeValues), GetMultipartContent(payload)))
                {
                    string json = await response.Content?.ReadAsStringAsync();
                    await EnsureSuccessStatusCodeForAsync(response);
                }
            }
        }

        /// <summary>
        /// Executes a PATCH request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TController">°The type of the <see cref="ControllerBase"/> to request</typeparam>
        /// <typeparam name="TResult">The type of expected result</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="payload">The payload to send as a form multipart content</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task<TResult> PatchAsMultipartAsync<TController, TResult>(string action, object payload = default, object routeValues = null)
            where TController : ControllerBase
        {
            using (HttpClient httpClient = this.ApiFactory.CreateClient())
            {
                using (HttpResponseMessage response = await httpClient.PatchAsync(this.BuildUrl<TController>(action, routeValues), GetMultipartContent(payload)))
                {
                    string json = await response.Content?.ReadAsStringAsync();
                    await EnsureSuccessStatusCodeForAsync(response);
                    if (string.IsNullOrWhiteSpace(json))
                        return default;
                    else
                        return this.Deserialize<TResult>(json);
                }
            }
        }

        /// <summary>
        /// Executes a PUT request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TController">°The type of the <see cref="ControllerBase"/> to request</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task PutAsync<TController>(string action, object routeValues = null)
            where TController : ControllerBase
        {
            using (HttpClient httpClient = this.ApiFactory.CreateClient())
            {
                using (HttpResponseMessage response = await httpClient.PutAsync(this.BuildUrl<TController>(action, routeValues), null))
                {
                    await EnsureSuccessStatusCodeForAsync(response);
                }
            }
        }

        /// <summary>
        /// Executes a PUT request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TController">°The type of the <see cref="ControllerBase"/> to request</typeparam>
        /// <typeparam name="TResult">The type of expected result</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task<TResult> PutAsync<TController, TResult>(string action, object routeValues = null)
            where TController : ControllerBase
        {
            using (HttpClient httpClient = this.ApiFactory.CreateClient())
            {
                using (HttpResponseMessage response = await httpClient.PutAsync(this.BuildUrl<TController>(action, routeValues), null))
                {
                    string json = await response.Content?.ReadAsStringAsync();
                    await EnsureSuccessStatusCodeForAsync(response);
                    if (string.IsNullOrWhiteSpace(json))
                        return default;
                    else
                        return this.Deserialize<TResult>(json);
                }
            }
        }

        /// <summary>
        /// Executes a PUT request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TController">°The type of the <see cref="ControllerBase"/> to request</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="payload">The payload to send as a JSON content</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task PutAsJsonAsync<TController>(string action, object payload = default, object routeValues = null)
             where TController : ControllerBase
        {
            using (HttpClient httpClient = this.ApiFactory.CreateClient())
            {
                using (HttpResponseMessage response = await httpClient.PutAsJsonAsync(this.BuildUrl<TController>(action, routeValues), payload))
                {
                    await EnsureSuccessStatusCodeForAsync(response);
                }
            }
        }

        /// <summary>
        /// Executes a PUT request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TController">°The type of the <see cref="ControllerBase"/> to request</typeparam>
        /// <typeparam name="TResult">The type of expected result</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="payload">The payload to send as a JSON content</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task<TResult> PutAsJsonAsync<TController, TResult>(string action, object payload = default, object routeValues = null)
             where TController : ControllerBase
        {
            using (HttpClient httpClient = this.ApiFactory.CreateClient())
            {
                using (HttpResponseMessage response = await httpClient.PutAsJsonAsync(this.BuildUrl<TController>(action, routeValues), payload))
                {
                    string json = await response.Content?.ReadAsStringAsync();
                    await EnsureSuccessStatusCodeForAsync(response);
                    if (string.IsNullOrWhiteSpace(json))
                        return default;
                    else
                        return this.Deserialize<TResult>(json);
                }
            }
        }

        /// <summary>
        /// Executes a PUT request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TController">°The type of the <see cref="ControllerBase"/> to request</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="payload">The payload to send as a form URL-content content</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task PutAsFormAsync<TController>(string action, object payload = default, object routeValues = null)
            where TController : ControllerBase
        {
            using (HttpClient httpClient = this.ApiFactory.CreateClient())
            {

                using (HttpResponseMessage response = await httpClient.PutAsync(this.BuildUrl<TController>(action, routeValues), GetFormContent(payload)))
                {
                    string json = await response.Content?.ReadAsStringAsync();
                    await EnsureSuccessStatusCodeForAsync(response);
                }
            }
        }

        /// <summary>
        /// Executes a PUT request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TController">°The type of the <see cref="ControllerBase"/> to request</typeparam>
        /// <typeparam name="TResult">The type of expected result</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="payload">The payload to send as a form URL-content content</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task<TResult> PutAsFormAsync<TController, TResult>(string action, object payload = default, object routeValues = null)
            where TController : ControllerBase
        {
            using (HttpClient httpClient = this.ApiFactory.CreateClient())
            {
                using (HttpResponseMessage response = await httpClient.PutAsync(this.BuildUrl<TController>(action, routeValues), GetFormContent(payload)))
                {
                    string json = await response.Content?.ReadAsStringAsync();
                    await EnsureSuccessStatusCodeForAsync(response);
                    if (string.IsNullOrWhiteSpace(json))
                        return default;
                    else
                        return this.Deserialize<TResult>(json);
                }
            }
        }

        /// <summary>
        /// Executes a PUT request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TController">°The type of the <see cref="ControllerBase"/> to request</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="payload">The payload to send as a form multipart content</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task PutAsMultipartAsync<TController>(string action, object payload = default, object routeValues = null)
           where TController : ControllerBase
        {
            using (HttpClient httpClient = this.ApiFactory.CreateClient())
            {
                using (HttpResponseMessage response = await httpClient.PutAsync(this.BuildUrl<TController>(action, routeValues), GetMultipartContent(payload)))
                {
                    string json = await response.Content?.ReadAsStringAsync();
                    await EnsureSuccessStatusCodeForAsync(response);
                }
            }
        }

        /// <summary>
        /// Executes a PUT request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TController">°The type of the <see cref="ControllerBase"/> to request</typeparam>
        /// <typeparam name="TResult">The type of expected result</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="payload">The payload to send as a form multipart content</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task<TResult> PutAsMultipartAsync<TController, TResult>(string action, object payload = default, object routeValues = null)
            where TController : ControllerBase
        {
            using (HttpClient httpClient = this.ApiFactory.CreateClient())
            {
                using (HttpResponseMessage response = await httpClient.PutAsync(this.BuildUrl<TController>(action, routeValues), GetMultipartContent(payload)))
                {
                    string json = await response.Content?.ReadAsStringAsync();
                    await EnsureSuccessStatusCodeForAsync(response);
                    if (string.IsNullOrWhiteSpace(json))
                        return default;
                    else
                        return this.Deserialize<TResult>(json);
                }
            }
        }

        /// <summary>
        /// Executes a DELETE request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TController">°The type of the <see cref="ControllerBase"/> to request</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task DeleteAsync<TController>(string action, object routeValues = null)
             where TController : ControllerBase
        {
            using (HttpClient httpClient = this.ApiFactory.CreateClient())
            {
                using (HttpResponseMessage response = await httpClient.DeleteAsync(this.BuildUrl<TController>(action, routeValues)))
                {
                    await EnsureSuccessStatusCodeForAsync(response);
                }
            }
        }

        /// <summary>
        /// Executes a DELETE request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TController">°The type of the <see cref="ControllerBase"/> to request</typeparam>
        /// <typeparam name="TResult">The type of expected result</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task<TResult> DeleteAsync<TController, TResult>(string action, object routeValues = null)
             where TController : ControllerBase
        {
            using (HttpClient httpClient = this.ApiFactory.CreateClient())
            {
                using (HttpResponseMessage response = await httpClient.DeleteAsync(this.BuildUrl<TController>(action, routeValues)))
                {
                    string json = await response.Content?.ReadAsStringAsync();
                    await EnsureSuccessStatusCodeForAsync(response);
                    if (string.IsNullOrWhiteSpace(json))
                        return default;
                    else
                        return this.Deserialize<TResult>(json);
                }
            }
        }

        /// <summary>
        /// Executes a DELETE request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TController">°The type of the <see cref="ControllerBase"/> to request</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="payload">The payload to send as a JSON content</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task DeleteAsJsonAsync<TController>(string action, object payload = default, object routeValues = null)
             where TController : ControllerBase
        {
            using HttpClient httpClient = this.ApiFactory.CreateClient();
            using HttpRequestMessage request = new(HttpMethod.Delete, this.BuildUrl<TController>(action, routeValues));
            if (payload != null)
                request.Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, MediaTypeNames.Application.Json);
            using HttpResponseMessage response = await httpClient.SendAsync(request);
            string json = await response.Content?.ReadAsStringAsync();
            await EnsureSuccessStatusCodeForAsync(response);
        }

        /// <summary>
        /// Executes a DELETE request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TController">°The type of the <see cref="ControllerBase"/> to request</typeparam>
        /// <typeparam name="TResult">The type of expected result</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="payload">The payload to send as a JSON content</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task<TResult> DeleteAsJsonAsync<TController, TResult>(string action, object payload = default, object routeValues = null)
             where TController : ControllerBase
        {
            using HttpClient httpClient = this.ApiFactory.CreateClient();
            using HttpRequestMessage request = new(HttpMethod.Delete, this.BuildUrl<TController>(action, routeValues));
            if (payload != null)
                request.Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, MediaTypeNames.Application.Json);
            using HttpResponseMessage response = await httpClient.SendAsync(request);
            string json = await response.Content?.ReadAsStringAsync();
            await EnsureSuccessStatusCodeForAsync(response);
            if (string.IsNullOrWhiteSpace(json))
                return default;
            else
                return this.Deserialize<TResult>(json);
        }

        /// <summary>
        /// Executes a DELETE request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TController">°The type of the <see cref="ControllerBase"/> to request</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="payload">The payload to send as a form URL-encoded content</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task DeleteAsFormAsync<TController>(string action, object payload = default, object routeValues = null)
            where TController : ControllerBase
        {
            using HttpClient httpClient = this.ApiFactory.CreateClient();
            using HttpRequestMessage request = new(HttpMethod.Delete, this.BuildUrl<TController>(action, routeValues));
            if (payload != null)
                request.Content = GetFormContent(payload);
            using HttpResponseMessage response = await httpClient.SendAsync(request);
            string json = await response.Content?.ReadAsStringAsync();
            await EnsureSuccessStatusCodeForAsync(response);
        }

        /// <summary>
        /// Executes a DELETE request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TController">°The type of the <see cref="ControllerBase"/> to request</typeparam>
        /// <typeparam name="TResult">The type of expected result</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="payload">The payload to send as a form URL-content content</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task<TResult> DeleteAsFormAsync<TController, TResult>(string action, object payload = default, object routeValues = null)
            where TController : ControllerBase
        {
            using HttpClient httpClient = this.ApiFactory.CreateClient();
            using HttpRequestMessage request = new(HttpMethod.Delete, this.BuildUrl<TController>(action, routeValues));
            if (payload != null)
                request.Content = GetFormContent(payload);
            using HttpResponseMessage response = await httpClient.SendAsync(request);
            string json = await response.Content?.ReadAsStringAsync();
            await EnsureSuccessStatusCodeForAsync(response);
            if (string.IsNullOrWhiteSpace(json))
                return default;
            else
                return this.Deserialize<TResult>(json);
        }

        /// <summary>
        /// Executes a DELETE request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TController">°The type of the <see cref="ControllerBase"/> to request</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="payload">The payload to send as a form multipart content</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task DeleteAsMultipartAsync<TController>(string action, object payload = default, object routeValues = null)
           where TController : ControllerBase
        {
            using HttpClient httpClient = this.ApiFactory.CreateClient();
            using HttpRequestMessage request = new(HttpMethod.Delete, this.BuildUrl<TController>(action, routeValues));
            if (payload != null)
                request.Content = GetMultipartContent(payload);
            using HttpResponseMessage response = await httpClient.SendAsync(request);
            string json = await response.Content?.ReadAsStringAsync();
            await EnsureSuccessStatusCodeForAsync(response);
        }

        /// <summary>
        /// Executes a DELETE request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TController">°The type of the <see cref="ControllerBase"/> to request</typeparam>
        /// <typeparam name="TResult">The type of expected result</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="payload">The payload to send as a form multipart content</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task<TResult> DeleteAsMultipartAsync<TController, TResult>(string action, object payload = default, object routeValues = null)
            where TController : ControllerBase
        {
            using HttpClient httpClient = this.ApiFactory.CreateClient();
            using HttpRequestMessage request = new(HttpMethod.Delete, this.BuildUrl<TController>(action, routeValues));
            if (payload != null)
                request.Content = GetMultipartContent(payload);
            using HttpResponseMessage response = await httpClient.SendAsync(request);
            string json = await response.Content?.ReadAsStringAsync();
            await EnsureSuccessStatusCodeForAsync(response);
            if (string.IsNullOrWhiteSpace(json))
                return default;
            else
                return this.Deserialize<TResult>(json);
        }

        /// <summary>
        /// Builds a request url based on the specified <see cref="ControllerBase"/>, action and route values
        /// </summary>
        /// <typeparam name="TController">The type of <see cref="ControllerBase"/> to request</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>The resulting request url</returns>
        protected string BuildUrl<TController>(string action, object routeValues = null)
            where TController : ControllerBase
        {
            string url = this.LinkGenerator.GetPathByAction(action, MvcHelper.NameOf<TController>(), routeValues);
            string cultureQueryParam = $"ui-culture={CultureInfo.CurrentCulture.Name}";
            if (!string.IsNullOrWhiteSpace(url))
            {
                if (url.Contains("?"))
                    url += $"&{cultureQueryParam}";
                else
                    url += $"?{cultureQueryParam}";
            }
            return url;
        }

        private TResult Deserialize<TResult>(string json)
        {
            if (typeof(TResult) == typeof(string))
                return (TResult)(object)json;
            else
                return JsonConvert.DeserializeObject<TResult>(json, this.SerializerSettings);
        }

        private static FormUrlEncodedContent GetFormContent(object payload)
        {
            IDictionary<string, string> nameValueCollection;
            if (payload == null)
                nameValueCollection = new Dictionary<string, string>();
            else
                nameValueCollection = payload.ToDictionary<string>();
            return new FormUrlEncodedContent(nameValueCollection);
        }

        private static MultipartContent GetMultipartContent(object payload)
        {
            MultipartFormDataContent multipart = new MultipartFormDataContent();
            if (payload != null)
            {
                foreach (PropertyInfo property in payload.GetType().GetProperties())
                {
                    if (property.PropertyType == typeof(IFormFile))
                    {
                        IFormFile formFile = (IFormFile)property.GetValue(payload);
                        HttpContent content = new StreamContent(formFile.OpenReadStream());
                        content.Headers.ContentType = new MediaTypeHeaderValue(formFile.ContentType);
                        multipart.Add(content, property.Name, formFile.FileName);
                    }
                    else if (property.PropertyType == typeof(IFormFileCollection))
                    {
                        IFormFileCollection formFileCollection = (IFormFileCollection)property.GetValue(payload);
                        foreach (IFormFile formFile in formFileCollection)
                        {
                            HttpContent content = new StreamContent(formFile.OpenReadStream());
                            content.Headers.ContentType = new MediaTypeHeaderValue(formFile.ContentType);
                            multipart.Add(content, property.Name, formFile.FileName);
                        }
                    }
                    else
                    {
                        object value = property.GetValue(payload);
                        multipart.Add(new StringContent(value.ToString()), property.Name);
                    }
                }
            }
            return multipart;
        }

        private static async Task EnsureSuccessStatusCodeForAsync(HttpResponseMessage response)
        {
            string content = await response.Content?.ReadAsStringAsync();
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                throw new ApiRequestException(ex, content);
            }
        }

    }

    /// <summary>
    /// Represents the base class for all API controller tests
    /// </summary>
    /// <typeparam name="TController">The type of <see cref="ControllerBase"/> to test</typeparam>
    public abstract class ApiControllerTest<TController>
        : ApiControllerTest
        where TController : ControllerBase
    {

        /// <summary>
        /// Initializes a new <see cref="ApiControllerTest{TController}"/>
        /// </summary>
        /// <param name="apiFactory">The service used to create test <see cref="HttpClient"/>s</param>
        public ApiControllerTest(IApiFactory apiFactory)
            : base(apiFactory)
        {

        }

        /// <summary>
        /// Executes a GET request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <param name="action">The action to request</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task GetAsync(string action, object routeValues = null)
        {
            await base.GetAsync<TController>(action, routeValues);
        }

        /// <summary>
        /// Executes a GET request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TResult">The type of expected result</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public new async Task<TResult> GetAsync<TResult>(string action, object routeValues = null)
        {
            return await base.GetAsync<TController, TResult>(action, routeValues);
        }

        /// <summary>
        /// Executes a POST request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <param name="action">The action to request</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task PostAsync(string action, object routeValues = null)
        {
            await base.PostAsync<TController>(action, routeValues);
        }

        /// <summary>
        /// Executes a POST request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TResult">The type of expected result</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public new async Task<TResult> PostAsync<TResult>(string action, object routeValues = null)
        {
            return await base.PostAsync<TController, TResult>(action, routeValues);
        }

        /// <summary>
        /// Executes a POST request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <param name="action">The action to request</param>
        /// <param name="payload">The payload to send as a JSON content</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task PostAsJsonAsync(string action, object payload = default, object routeValues = null)
        {
            await base.PostAsJsonAsync<TController>(action, payload, routeValues);
        }

        /// <summary>
        /// Executes a POST request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TResult">The type of expected result</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="payload">The payload to send as a JSON content</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public new async Task<TResult> PostAsJsonAsync<TResult>(string action, object payload = default, object routeValues = null)
        {
            return await base.PostAsJsonAsync<TController, TResult>(action, payload, routeValues);
        }

        /// <summary>
        /// Executes a POST request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <param name="action">The action to request</param>
        /// <param name="payload">The payload to send as a form URL-encoded content</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task PostAsFormAsync(string action, object payload = default, object routeValues = null)
        {
            await base.PostAsFormAsync<TController>(action, payload, routeValues);
        }

        /// <summary>
        /// Executes a POST request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TResult">The type of expected result</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="payload">The payload to send as a form URL-content content</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public new async Task<TResult> PostAsFormAsync<TResult>(string action, object payload = default, object routeValues = null)
        {
            return await base.PostAsFormAsync<TController, TResult>(action, payload, routeValues);
        }

        /// <summary>
        /// Executes a POST request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <param name="action">The action to request</param>
        /// <param name="payload">The payload to send as a form multipart content</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task PostAsMultipartAsync(string action, object payload = default, object routeValues = null)
        {
            await base.PostAsMultipartAsync<TController>(action, payload, routeValues);
        }

        /// <summary>
        /// Executes a POST request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TResult">The type of expected result</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="payload">The payload to send as a form multipart content</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public new async Task<TResult> PostAsMultipartAsync<TResult>(string action, object payload = default, object routeValues = null)
        {
            return await base.PostAsMultipartAsync<TController, TResult>(action, payload, routeValues);
        }

        /// <summary>
        /// Executes a PATCH request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <param name="action">The action to request</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task PatchAsync(string action, object routeValues = null)
        {
            await base.PatchAsync<TController>(action, routeValues);
        }

        /// <summary>
        /// Executes a PATCH request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TResult">The type of expected result</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public new async Task<TResult> PatchAsync<TResult>(string action, object routeValues = null)
        {
            return await base.PatchAsync<TController, TResult>(action, routeValues);
        }

        /// <summary>
        /// Executes a PATCH request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <param name="action">The action to request</param>
        /// <param name="payload">The payload to send as a JSON content</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task PatchAsJsonAsync(string action, object payload = default, object routeValues = null)
        {
            await base.PatchAsJsonAsync<TController>(action, payload, routeValues);
        }

        /// <summary>
        /// Executes a PATCH request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TResult">The type of expected result</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="payload">The payload to send as a JSON content</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public new async Task<TResult> PatchAsJsonAsync<TResult>(string action, object payload = default, object routeValues = null)
        {
            return await base.PatchAsJsonAsync<TController, TResult>(action, payload, routeValues);
        }

        /// <summary>
        /// Executes a PATCH request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <param name="action">The action to request</param>
        /// <param name="payload">The payload to send as a form URL-content content</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task PatchAsFormAsync(string action, object payload = default, object routeValues = null)
        {
            await base.PatchAsFormAsync<TController>(action, payload, routeValues);
        }

        /// <summary>
        /// Executes a PATCH request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TResult">The type of expected result</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="payload">The payload to send as a form URL-content content</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public new async Task<TResult> PatchAsFormAsync<TResult>(string action, object payload = default, object routeValues = null)
        {
            return await base.PatchAsFormAsync<TController, TResult>(action, payload, routeValues);
        }

        /// <summary>
        /// Executes a PATCH request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <param name="action">The action to request</param>
        /// <param name="payload">The payload to send as a form multipart content</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task PatchAsMultipartAsync(string action, object payload = default, object routeValues = null)
        {
            await base.PatchAsMultipartAsync<TController>(action, payload, routeValues);
        }

        /// <summary>
        /// Executes a PATCH request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TResult">The type of expected result</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="payload">The payload to send as a form multipart content</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public new async Task<TResult> PatchAsMultipartAsync<TResult>(string action, object payload = default, object routeValues = null)
        {
            return await base.PatchAsMultipartAsync<TController, TResult>(action, payload, routeValues);
        }

        /// <summary>
        /// Executes a PUT request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <param name="action">The action to request</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task PutAsync(string action, object routeValues = null)
        {
            await base.PutAsync<TController>(action, routeValues);
        }

        /// <summary>
        /// Executes a PUT request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TResult">The type of expected result</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public new async Task PutAsync<TResult>(string action, object routeValues = null)
        {
            await base.PutAsync<TController, TResult>(action, routeValues);
        }

        /// <summary>
        /// Executes a PUT request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <param name="action">The action to request</param>
        /// <param name="payload">The payload to send as a JSON content</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task PutAsJsonAsync(string action, object payload = default, object routeValues = null)
        {
            await base.PutAsJsonAsync<TController>(action, payload, routeValues);
        }

        /// <summary>
        /// Executes a PUT request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TResult">The type of expected result</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="payload">The payload to send as a JSON content</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public new async Task<TResult> PutAsJsonAsync<TResult>(string action, object payload = default, object routeValues = null)
        {
            return await base.PutAsJsonAsync<TController, TResult>(action, payload, routeValues);
        }

        /// <summary>
        /// Executes a PUT request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <param name="action">The action to request</param>
        /// <param name="payload">The payload to send as a form URL-content content</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task PutAsFormAsync(string action, object payload = default, object routeValues = null)
        {
            await base.PutAsFormAsync<TController>(action, payload, routeValues);
        }

        /// <summary>
        /// Executes a PUT request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TResult">The type of expected result</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="payload">The payload to send as a form URL-content content</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public new async Task<TResult> PutAsFormAsync<TResult>(string action, object payload = default, object routeValues = null)
        {
            return await base.PatchAsFormAsync<TController, TResult>(action, payload, routeValues);
        }

        /// <summary>
        /// Executes a PUT request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <param name="action">The action to request</param>
        /// <param name="payload">The payload to send as a form multipart content</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task PutAsMultipartAsync(string action, object payload = default, object routeValues = null)
        {
            await base.PutAsMultipartAsync<TController>(action, payload, routeValues);
        }

        /// <summary>
        /// Executes a PUT request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TResult">The type of expected result</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="payload">The payload to send as a form multipart content</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public new async Task<TResult> PutAsMultipartAsync<TResult>(string action, object payload = default, object routeValues = null)
        {
            return await base.PutAsMultipartAsync<TController, TResult>(action, payload, routeValues);
        }

        /// <summary>
        /// Executes a DELETE request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <param name="action">The action to request</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task DeleteAsync(string action, object routeValues = null)
        {
            await base.DeleteAsync<TController>(action, routeValues);
        }

        /// <summary>
        /// Executes a DELETE request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TResult">The type of expected result</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public new async Task<TResult> DeleteAsync<TResult>(string action, object routeValues = null)
        {
            return await base.DeleteAsync<TController, TResult>(action, routeValues);
        }

        /// <summary>
        /// Executes a DELETE request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <param name="action">The action to request</param>
        /// <param name="payload">The payload to send as a JSON content</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task DeleteAsJsonAsync(string action, object payload = default, object routeValues = null)
        {
            await base.DeleteAsJsonAsync<TController>(action, payload, routeValues);
        }

        /// <summary>
        /// Executes a DELETE request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TResult">The type of expected result</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="payload">The payload to send as a JSON content</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public new async Task<TResult> DeleteAsJsonAsync<TResult>(string action, object payload = default, object routeValues = null)
        {
            return await base.DeleteAsJsonAsync<TController, TResult>(action, payload, routeValues);
        }

        /// <summary>
        /// Executes a DELETE request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <param name="action">The action to request</param>
        /// <param name="payload">The payload to send as a form URL-encoded content</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task DeleteAsFormAsync(string action, object payload = default, object routeValues = null)
        {
            await base.DeleteAsFormAsync<TController>(action, payload, routeValues);
        }

        /// <summary>
        /// Executes a DELETE request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TResult">The type of expected result</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="payload">The payload to send as a form URL-content content</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public new async Task<TResult> DeleteAsFormAsync<TResult>(string action, object payload = default, object routeValues = null)
        {
            return await base.DeleteAsFormAsync<TController, TResult>(action, payload, routeValues);
        }

        /// <summary>
        /// Executes a DELETE request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <param name="action">The action to request</param>
        /// <param name="payload">The payload to send as a form multipart content</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public async Task DeleteAsMultipartAsync(string action, object payload = default, object routeValues = null)
        {
            await base.DeleteAsMultipartAsync<TController>(action, payload, routeValues);
        }

        /// <summary>
        /// Executes a DELETE request on the specified <see cref="ControllerBase"/>
        /// </summary>
        /// <typeparam name="TResult">The type of expected result</typeparam>
        /// <param name="action">The action to request</param>
        /// <param name="payload">The payload to send as a form multipart content</param>
        /// <param name="routeValues">An object representing the route values of the action to request</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        public new async Task<TResult> DeleteAsMultipartAsync<TResult>(string action, object payload = default, object routeValues = null)
        {
            return await base.DeleteAsMultipartAsync<TController, TResult>(action, payload, routeValues);
        }

    }

}
