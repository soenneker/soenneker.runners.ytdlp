using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Soenneker.Managers.Runners.Abstract;
using Soenneker.Utils.Delay;
using Soenneker.Utils.File.Download.Abstract;

namespace Soenneker.Runners.ytdlp;

public class ConsoleHostedService : IHostedService
{
    private readonly ILogger<ConsoleHostedService> _logger;

    private readonly IHostApplicationLifetime _appLifetime;
    private readonly IRunnersManager _runnersManager;
    private readonly IFileDownloadUtil _fileDownloadUtil;

    private int? _exitCode;

    public ConsoleHostedService(ILogger<ConsoleHostedService> logger, IHostApplicationLifetime appLifetime, IFileDownloadUtil fileDownloadUtil, IRunnersManager runnersManager)
    {
        _logger = logger;
        _appLifetime = appLifetime;
        _fileDownloadUtil = fileDownloadUtil;
        _runnersManager = runnersManager;
    }

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        _appLifetime.ApplicationStarted.Register(() =>
        {
            Task.Run(async () =>
            {
                _logger.LogInformation("Running console hosted service ...");

                try
                {
                    string? filePath = await _fileDownloadUtil.Download("https://github.com/yt-dlp/yt-dlp-nightly-builds/releases/latest/download/yt-dlp.exe", fileExtension: ".exe", cancellationToken: cancellationToken);

                    await _runnersManager.PushIfChangesNeeded(filePath, Constants.FileName, Constants.Library, $"https://github.com/soenneker/{Constants.Library}", cancellationToken);

                    _logger.LogInformation("Complete!");

                    _exitCode = 0;
                }
                catch (Exception e)
                {
                    if (Debugger.IsAttached)
                        Debugger.Break();

                    _logger.LogError(e, "Unhandled exception");

                    await DelayUtil.Delay(2000, null, cancellationToken);
                    _exitCode = 1;
                }
                finally
                {
                    // Stop the application once the work is done
                    _appLifetime.StopApplication();
                }
            }, cancellationToken);
        });

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Exiting with return code: {exitCode}", _exitCode);

        // Exit code may be null if the user cancelled via Ctrl+C/SIGTERM
        Environment.ExitCode = _exitCode.GetValueOrDefault(-1);
        return Task.CompletedTask;
    }
}