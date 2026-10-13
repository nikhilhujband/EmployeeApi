
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeApi.Controllers;

[ApiController]
[Route("api/documents")]
public class DocumentsController(BlobServiceClient blobServiceClient, ILogger<DocumentsController> logger) : ControllerBase
{
    private readonly BlobServiceClient _blobServiceClient = blobServiceClient;
    private readonly ILogger<DocumentsController> _logger = logger;
    private const string ContainerName = "documents";
    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        _logger.LogInformation("Document upload started.");

        if (file == null || file.Length == 0)
        {
            _logger.LogWarning("Document upload rejected: file is empty.");
            return BadRequest("Please select a file.");
        }

        var container = _blobServiceClient
            .GetBlobContainerClient(ContainerName);

        await container.CreateIfNotExistsAsync();

        var blobName =
            $"{Guid.NewGuid():N}_{Path.GetFileName(file.FileName)}";

        var blob = container.GetBlobClient(blobName);

        try
        {
            await using var stream = file.OpenReadStream();

            await blob.UploadAsync(
                stream,
                new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders
                    {
                        ContentType = file.ContentType
                    }
                });

            _logger.LogInformation(
                "Document uploaded successfully. BlobName: {BlobName}",
                blobName);

            return Ok(new
            {
                blobName,
                message = "Document uploaded successfully."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Document upload failed. BlobName: {BlobName}",
                blobName);

            throw;
        }
    }


    [HttpPost("upload/{employeeId:int}")]
    public async Task<IActionResult> Upload(
        int employeeId,
        IFormFile file)
    {
        _logger.LogInformation(
            "Document upload started for EmployeeId: {EmployeeId}",
            employeeId);

        if (employeeId <= 0)
        {
            _logger.LogWarning(
                "Document upload rejected: invalid EmployeeId {EmployeeId}",
                employeeId);

            return BadRequest("Invalid employee ID.");
        }

        if (file == null || file.Length == 0)
        {
            _logger.LogWarning(
                "Document upload rejected because the file is empty. EmployeeId: {EmployeeId}",
                employeeId);

            return BadRequest("Please select a file.");
        }

        var container = _blobServiceClient
            .GetBlobContainerClient(ContainerName);

        await container.CreateIfNotExistsAsync();

        var fileName = Path.GetFileName(file.FileName);
        var blobName = $"employee/{employeeId}/{fileName}";
        var blob = container.GetBlobClient(blobName);

        try
        {
            await using var stream = file.OpenReadStream();

            await blob.UploadAsync(
                stream,
                new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders
                    {
                        ContentType = file.ContentType
                    }
                });

            _logger.LogInformation(
                "Document uploaded successfully. EmployeeId: {EmployeeId}, BlobName: {BlobName}",
                employeeId,
                blobName);

            return Ok(new
            {
                employeeId,
                blobName,
                message = "Document uploaded successfully."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Document upload failed. EmployeeId: {EmployeeId}, BlobName: {BlobName}",
                employeeId,
                blobName);

            throw;
        }
    }

    [HttpGet("{*blobName}")]
    public async Task<IActionResult> Download(string blobName)
    {
        _logger.LogInformation(
            "Document download requested. BlobName: {BlobName}",
            blobName);

        var container = _blobServiceClient
            .GetBlobContainerClient(ContainerName);

        var blob = container.GetBlobClient(blobName);

        try
        {
            if (!await blob.ExistsAsync())
            {
                _logger.LogWarning(
                    "Document not found. BlobName: {BlobName}",
                    blobName);

                return NotFound("Document not found.");
            }

            var download = await blob.DownloadContentAsync();

            _logger.LogInformation(
                "Document downloaded successfully. BlobName: {BlobName}",
                blobName);

            return File(
                download.Value.Content.ToArray(),
                download.Value.Details.ContentType
                    ?? "application/octet-stream",
                Path.GetFileName(blobName));
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Document download failed. BlobName: {BlobName}",
                blobName);

            throw;
        }
    }


    [HttpGet("download/{employeeId:int}/{fileName}")]
    public async Task<IActionResult> Download(
     int employeeId,
     string fileName)
    {
        _logger.LogInformation(
            "Document download requested. EmployeeId: {EmployeeId}, FileName: {FileName}",
            employeeId,
            fileName);

        if (employeeId <= 0)
        {
            _logger.LogWarning(
                "Document download rejected: invalid EmployeeId {EmployeeId}",
                employeeId);

            return BadRequest("Invalid employee ID.");
        }

        fileName = Path.GetFileName(fileName);

        var blobName = $"employee/{employeeId}/{fileName}";

        var container = _blobServiceClient
            .GetBlobContainerClient(ContainerName);

        var blob = container.GetBlobClient(blobName);

        try
        {
            if (!await blob.ExistsAsync())
            {
                _logger.LogWarning(
                    "Document not found. EmployeeId: {EmployeeId}, BlobName: {BlobName}",
                    employeeId,
                    blobName);

                return NotFound("Document not found.");
            }

            var download = await blob.DownloadContentAsync();

            _logger.LogInformation(
                "Document downloaded successfully. EmployeeId: {EmployeeId}, BlobName: {BlobName}",
                employeeId,
                blobName);

            return File(
                download.Value.Content.ToArray(),
                download.Value.Details.ContentType
                    ?? "application/octet-stream",
                fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Document download failed. EmployeeId: {EmployeeId}, BlobName: {BlobName}",
                employeeId,
                blobName);

            throw;
        }
    }
}