namespace Songerr.Services
{
    public interface ISongerrService
    {
        ValueTask<string> DownloadFirstVideoAsMp3(string videoTitle);
        ValueTask<string> GetMp3BasedOnUrl(string videoTitle);
    }

}
