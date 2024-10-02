namespace Lottery.Core.Models.Navigation
{
    public class NavigationModel
    {
        public int CategoryId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public List<SubNavigationModel> Children { get; set; }
    }
}
