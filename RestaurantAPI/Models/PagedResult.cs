namespace RestaurantAPI.Models
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = [];
        public int TotalPages { get; set; }
        public int ItemsFrom {  get; set; }
        public int ItemsTo { get; set; }
        public int ItemsCount { get; set; }

        public PagedResult(List<T> items, int itemsCount, int pageSize, int pageNumber)
        {
            Items = items;
            ItemsCount = itemsCount;
            ItemsFrom = pageSize * (pageNumber - 1) + 1;
            ItemsTo = ItemsFrom + pageSize - 1;
            TotalPages = (int)Math.Ceiling(itemsCount / (double)pageSize);
        }
    }
}
