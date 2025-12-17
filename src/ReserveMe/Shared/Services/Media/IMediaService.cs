namespace Shared.Services.Media
{
	using Microsoft.AspNetCore.Components.Forms;
	using Shared.ImageUpload;

	public interface IMediaService
	{
		Task<UploadResult> UploadImage(IBrowserFile file);
	}
}
