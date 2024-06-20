using Auth.Components;
using Crosscutting.Configuration.AuthPolicyConfiguration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using System.IO.Compression;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace API.Auth.Controllers
{
    [Route("API/Core/Auth")]
    public class UpdateController : Controller
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _configuration;

        public UpdateController(ILogger<AuthController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [Authorize(Policy = PolicyConfiguration.AuthUser)]
        [HttpPost("CheckForUpdates")]
        public async Task<IActionResult> CheckForUpdates([FromBody] List<FileSystemEntryDTO> model)
        {
            try
            {
                // Retrieve IP address of the client
                var ipAddress = Request.HttpContext.Connection.RemoteIpAddress;
                // Retrieve user agent string
                var userAgent = Request.HttpContext.Request.Headers["User-Agent"];

                // Log the IP address and user agent
                _logger.LogInformation($"[CheckForUpdates] Request from IP: {ipAddress}, User-Agent: {userAgent}");

                if (model == null || !Directory.Exists(_configuration["CoreFolderUpdatePath"]!))
                {
                    _logger.LogError("Invalid path or model is null: " + AppDomain.CurrentDomain.BaseDirectory);
                    return StatusCode(400, "Invalid path");
                }

                // Create a list to hold the file paths with folder structure
                var newerFiles = new List<FileWithFolder>();
                var ignoreFiles = new List<FileWithFolder>();

                foreach (var file in model)
                {
                    var sanitizedFileName = Path.GetFileName(@file.Path);

                    sanitizedFileName = Regex.Replace(sanitizedFileName!, @".*?(?=\\Core)", "");
                    sanitizedFileName = sanitizedFileName.Replace("\\", "/");

                    if (sanitizedFileName != null)
                    {
                        if (System.IO.File.Exists(sanitizedFileName))
                        {
                            var fileLastModified = System.IO.File.GetLastWriteTimeUtc(sanitizedFileName);

                            if (file.LastModified != null && fileLastModified > file.LastModified)
                            {
                                newerFiles.Add(new FileWithFolder(file.Name ?? "", sanitizedFileName ?? ""));
                            }
                            else
                            {
                                ignoreFiles.Add(new FileWithFolder(file.Name ?? "", sanitizedFileName ?? ""));
                            }
                        }
                    }
                }

                var getAllFiles = Directory.GetFiles("/Core", "*", SearchOption.AllDirectories);

                //get all files in the directory where file name doesnt match any in ignore list

                foreach (var allFiles in getAllFiles)
                {
                    var fileInfo = new FileInfo(allFiles);

                    if (ignoreFiles.Any(x => x.FolderPath.Replace("//", "/") == fileInfo.FullName))
                    {
                        continue;
                    }

                    newerFiles.Add(new FileWithFolder(fileInfo.Name, fileInfo.FullName));
                }

                newerFiles.RemoveAll(x => x.File == "rotation.cs");
                newerFiles.RemoveAll(x => x.File.Contains(".cs"));
                newerFiles.RemoveAll(x => x.File == "logs.txt");

                if (newerFiles.Count == 0)
                {
                    return StatusCode(204);
                }

                var zipMemoryStream = new MemoryStream();
                using (var zipArchive = new ZipArchive(zipMemoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var file in newerFiles)
                    {
                        var filePath = file.File;
                        var entry = zipArchive.CreateEntry(file.FolderPath ?? "");
                        using var entryStream = entry.Open();
                        using var fileStream = System.IO.File.OpenRead(file.FolderPath!);
                        await fileStream.CopyToAsync(entryStream);
                    }
                }

                zipMemoryStream.Position = 0;
                var contentType = "application/octet-stream";
                var fileName = "newer_files.zip";

                return File(zipMemoryStream, contentType, fileName);
            }
            catch (Exception ex)
            {
                // Handle any unexpected errors
                _logger.LogError(ex, "Error occurred while processing request");
                return StatusCode(500, "An error occurred while processing the request");
            }
        }
    }
}
