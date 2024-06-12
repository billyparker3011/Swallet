using Lottery.Core.Enums;

namespace Lottery.Core.Models.Enums
{
    public class CategoryModel
    {
        public Category Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public Region Region { get; set; }
        public int OrderBy { get; set; }
    }
}
