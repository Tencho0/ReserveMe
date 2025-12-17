namespace Shared.Services.Media
{
	using System.Net.Http.Json;
	using System.Threading.Tasks;
	using Microsoft.AspNetCore.Components.Forms;
	using Shared.ImageUpload;
	using Shared.Providers;

	public class MediaService : IMediaService
	{
		private readonly IApiProvider _provider;

		public MediaService(IApiProvider provider)
		{
			_provider = provider;
		}

		public async Task<UploadResult> UploadImage(IBrowserFile file)
		{
			try
			{
				if (file != null)
				{
					var formData = new MultipartFormDataContent();

					var extension = Path.GetExtension(file.Name);
					var newFileName = $"{Path.GetFileNameWithoutExtension(file.Name)}_{Guid.NewGuid()}{extension}";

					formData.Add(new StreamContent(file.OpenReadStream()), "file", newFileName);

					using (var httpClient = new HttpClient())
					{
						var response = await httpClient.PostAsync(Endpoints.UploadImage, formData);

						if (response.IsSuccessStatusCode)
						{
							var result = await response.Content.ReadFromJsonAsync<UploadResult>();

							if (result != null)
								return result;
						}
					}
				}
			}
			catch (Exception ex)
			{
				//TODO: Log error
				//_logger.LogError(ex, ex.Message);

				return new UploadResult();
			}

			return null;
		}
	}
}
