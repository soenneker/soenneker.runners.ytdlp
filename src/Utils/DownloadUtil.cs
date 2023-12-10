using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Soenneker.Runners.ytdlp.Utils.Abstract;
using Soenneker.Utils.FileSync;
using Soenneker.Utils.HttpClientCache.Abstract;

namespace Soenneker.Runners.ytdlp.Utils;

///<inheritdoc cref="IDownloadUtil"/>
public class DownloadUtil : IDownloadUtil
{
    private readonly ILogger<DownloadUtil> _logger;
    private readonly IHttpClientCache _httpClientCache;

    public DownloadUtil(IHttpClientCache httpClientCache, ILogger<DownloadUtil> logger)
    {
        _httpClientCache = httpClientCache;
        _logger = logger;
    }

    public async ValueTask<string> Download()
    {
        HttpClient client = await _httpClientCache.Get(nameof(DownloadUtil));

        const string uri = "https://github.com/yt-dlp/yt-dlp-nightly-builds/releases/latest/download/yt-dlp.exe";

        string tempFile = FileUtilSync.GetTempFileName() + ".exe";

        _logger.LogInformation("Downloading file from uri ({uri}) to fileName ({fileName})...", uri, tempFile);

        HttpResponseMessage response = await client.GetAsync(uri);

        await using (var fs = new FileStream(tempFile, FileMode.CreateNew))
        {
            await response.Content.CopyToAsync(fs);
        }

        _logger.LogDebug("Finished downloading file from uri ({uri})", uri);

        return tempFile;
    }
}
