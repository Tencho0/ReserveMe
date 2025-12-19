namespace ReserveMe.Server.Controllers
{
	using Microsoft.AspNetCore.Mvc;

	public class MediaController : ApiControllerBase
	{
		private readonly IWebHostEnvironment _hostEnvironment;
		public MediaController(IWebHostEnvironment hostEnvironment)
		{
			_hostEnvironment = hostEnvironment;
		}

		[HttpPost("upload")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
		[ProducesDefaultResponseType]
		public async Task<IActionResult> Upload()
		{
			try
			{
				var file = Request.Form.Files[0];
				var folderPath = Path.Combine(_hostEnvironment.ContentRootPath, "StaticFiles", "Media");
				Directory.CreateDirectory(folderPath); // creates if missing

				var fileName = $"{Guid.NewGuid()}_{file.FileName}";
				var savePath = Path.Combine(folderPath, fileName);
				var fileUrl = Path.Combine("https://localhost:7118", "StaticFiles", "Media", fileName);

				using var stream = System.IO.File.Create(savePath);
				await file.CopyToAsync(stream);

				return Ok(new { fileName = file.FileName, fileUrl = fileUrl });
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex}");
			}
		}
	}
}
