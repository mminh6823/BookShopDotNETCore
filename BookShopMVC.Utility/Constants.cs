using Microsoft.Extensions.Hosting;

namespace BookStoreMVC.Utility
{
    public static class Constants
    {
        public static class Prices
        {
            private static Random _random = new Random();
            public static decimal Shipping = 25000m + (decimal)_random.NextDouble() * (50000m - 25000m);
            public const decimal Vat = 0.05m;
        }

        public enum OrderStatus
        {
            Pending,
            Processing,
            Approved,
            Completed,
            Cancelled
        }

        public static int ImageWidth = 390;
        public static int ImageHeight = 595;
        public static int ImageMaxSizeKB = 500; 
        public static string ImageExtension = ".jpg";
    }
}
