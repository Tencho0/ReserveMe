namespace Shared.Providers
{
	using System.Collections.Generic;
	using System.Net;
	using System.Net.Http;
	using System.Net.Http.Headers;
	using System.Text;
	using System.Threading.Tasks;
	using System.Web;
	using Blazored.LocalStorage;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Converters;
	using Newtonsoft.Json.Serialization;
	using Shared.Exceptions;

	/// <summary>
	/// Core Api Request Provider
	/// </summary>
	public class ApiProvider : IApiProvider
	{
		private readonly JsonSerializerSettings serializerSettings;
		private readonly HttpClient httpClient;
		private readonly ILocalStorageService _localStorage;

		public ApiProvider(HttpClient httpClient, ILocalStorageService localStorage)
		{
			ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

			serializerSettings = new JsonSerializerSettings
			{
				ContractResolver = new DefaultContractResolver(),
				DateTimeZoneHandling = DateTimeZoneHandling.Local,
				NullValueHandling = NullValueHandling.Ignore,
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore
			};
			serializerSettings.Converters.Add(new StringEnumConverter());
			this.httpClient = httpClient;
			_localStorage = localStorage;
		}

		public async Task<TResult> GetAsync<TResult>(string uri, object[] uriParams, Dictionary<string, object> queryParams, string token = "")
		{
			try
			{
				await HttpClientAddHeader();
				var urlWithQueryParams = PrepareEndpoint(uri, uriParams, queryParams);
				var request = new HttpRequestMessage(HttpMethod.Get, urlWithQueryParams);
				httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				var response = await httpClient.SendAsync(request);
				await HandleResponse(response);
				string serialized = await response.Content.ReadAsStringAsync();

				TResult result = JsonConvert.DeserializeObject<TResult>(serialized, serializerSettings);

				return result;
			}
			catch (Exception ex)
			{
				//TODO: log error
				throw;
			}
		}

		public async Task<TResult> PostAsync<TRequest, TResult>(string uri, TRequest data, string token = "", string header = "")
		{
			try
			{
				await HttpClientAddHeader();
				var request = new HttpRequestMessage(HttpMethod.Post, uri);

				if (!string.IsNullOrEmpty(header))
				{
					AddHeaderParameter(httpClient, header);
				}

				var jsonData = JsonConvert.SerializeObject(data, serializerSettings);

				request.Content = new StringContent(jsonData, Encoding.UTF8, "application/json");
				httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				HttpResponseMessage response = await httpClient.SendAsync(request);

				await HandleResponse(response);

				string serialized = await response.Content.ReadAsStringAsync();

				TResult result = JsonConvert.DeserializeObject<TResult>(serialized, serializerSettings);

				return result;

			}
			catch (Exception ex)
			{
				//TODO: log error
				throw;
			}
		}

		public async Task<TResult> PutAsync<TRequest, TResult>(string uri, TRequest data, string token = "", string header = "")
		{
			try
			{
				await HttpClientAddHeader();
				var request = new HttpRequestMessage(HttpMethod.Put, uri);

				if (!string.IsNullOrEmpty(header))
				{
					AddHeaderParameter(httpClient, header);
				}

				var jsonData = JsonConvert.SerializeObject(data, serializerSettings);
				request.Content = new StringContent(jsonData, Encoding.UTF8, "application/json");
				httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				HttpResponseMessage response = await httpClient.SendAsync(request);

				await HandleResponse(response);

				string serialized = await response.Content.ReadAsStringAsync();

				TResult result = JsonConvert.DeserializeObject<TResult>(serialized, serializerSettings);

				return result;

			}
			catch (Exception ex)
			{
				//TODO: log error
				throw;
			}
		}

		public async Task<TResult> DeleteAsync<TRequest, TResult>(string uri, TRequest data, string token = "", string header = "")
		{
			try
			{
				await HttpClientAddHeader();
				var request = new HttpRequestMessage(HttpMethod.Delete, uri);

				if (!string.IsNullOrEmpty(header))
				{
					AddHeaderParameter(httpClient, header);
				}

				var jsonData = JsonConvert.SerializeObject(data, serializerSettings);
				request.Content = new StringContent(jsonData, Encoding.UTF8, "application/json");
				httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				HttpResponseMessage response = await httpClient.SendAsync(request);

				await HandleResponse(response);

				string serialized = await response.Content.ReadAsStringAsync();

				TResult result = JsonConvert.DeserializeObject<TResult>(serialized, serializerSettings);

				return result;
			}
			catch (Exception ex)
			{
				//TODO: log error
				throw;
			}
		}

		private async Task HttpClientAddHeader()
		{
			var savedToken = await _localStorage.GetItemAsync<string>("authToken");
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", savedToken);
		}

		private void AddHeaderParameter(HttpClient httpClient, string parameter)
		{
			if (httpClient == null)
				return;

			if (string.IsNullOrEmpty(parameter))
				return;

			httpClient.DefaultRequestHeaders.Add(parameter, Guid.NewGuid().ToString());

			//var arr = parameter.Split(": ", StringSplitOptions.RemoveEmptyEntries).ToArray();
			//httpClient.DefaultRequestHeaders.Add(arr[0], arr[1]);
		}

		/// <summary>
		/// Prepare URL
		/// </summary>
		/// <param name="uri">endpoint</param>
		/// <param name="uriParams">uri injected params</param>
		/// <param name="queryParams">query params</param>
		/// <returns></returns>
		private string PrepareEndpoint(string uri, object[] uriParams, Dictionary<string, object>? queryParams)
		{
			// Builds the final request URL by inserting route parameters and appending query parameters.

			if (uri.Contains("{0}") && (uriParams == null || uriParams.Length == 0))
				throw new ArgumentException("uri expects uriParams to be set");

			StringBuilder uriBuilder = new StringBuilder();

			if (uri.Contains("{0}"))
			{
				uriBuilder.Append(string.Format(uri, uriParams));
			}
			else
			{
				uriBuilder.Append(uri);
			}

			if (queryParams != null)
			{
				string delim = "?";
				foreach (var key in queryParams.Keys)
				{
					if (!uriBuilder.ToString().Contains("directions") && !uriBuilder.ToString().Contains("distancematrix"))
					{
						uriBuilder.Append($"{delim}{key}={HttpUtility.UrlEncode(queryParams[key].ToString())}");
					}
					else
					{
						uriBuilder.Append($"{delim}{key}={queryParams[key].ToString()}");
					}

					delim = "&";
				}
			}
			return uriBuilder.ToString();
		}

		/// <summary>
		/// Handle Response Success or Failiure
		/// </summary>
		/// <param name="response">Response to handle</param>
		/// <returns></returns>
		private async Task HandleResponse(HttpResponseMessage response)
		{
			if (!response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync();

				if (response.StatusCode == HttpStatusCode.Forbidden ||
				   response.StatusCode == HttpStatusCode.Unauthorized)
				{
					throw new ServiceAuthenticationException();
				}

				if (response.StatusCode == HttpStatusCode.BadRequest)
				{
					var validationTemplate = new
					{
						failures = new Dictionary<string, string[]>()
					};

					var validationData = JsonConvert.DeserializeAnonymousType(
						content,
						validationTemplate,
						serializerSettings);

					throw new ApiValidationException(validationData?.failures);
				}

				if (response.StatusCode == HttpStatusCode.InternalServerError)
				{
					var errorTemplate = new
					{
						title = string.Empty
					};

					var errorData = JsonConvert.DeserializeAnonymousType(
						content,
						errorTemplate,
						serializerSettings);

					throw new ApiRequestException(response.StatusCode, errorData?.title);
				}

				throw new ApiRequestException(response.StatusCode, response.ReasonPhrase);
			}
		}
	}
}
