using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Runners.ytdlp.Utils.Abstract;

public interface IFileOperationsUtil
{
    ValueTask Process(string filePath, CancellationToken cancellationToken);
}