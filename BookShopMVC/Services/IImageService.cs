namespace BookShopMVC.Services
{
    public interface IImageService
    {
        void DeleteIfExists(string rootPath, string? imagePath);
    }
}
