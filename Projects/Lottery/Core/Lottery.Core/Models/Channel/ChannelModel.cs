namespace Lottery.Core.Models.Channel
{
    public class ChannelModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int RegionId { get; set; }
        public List<int> DayOfWeeks { get; set; }
    }
}
