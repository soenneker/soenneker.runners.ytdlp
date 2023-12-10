using System.Threading.Tasks;

namespace Soenneker.Runners.ytdlp.Utils.Abstract;

public interface IDownloadUtil
{
    ValueTask<string> Download();
}
