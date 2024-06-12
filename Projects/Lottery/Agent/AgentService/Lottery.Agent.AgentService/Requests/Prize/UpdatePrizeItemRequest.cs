namespace Lottery.Agent.AgentService.Requests.Prize
{
    public class UpdatePrizeItemRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
    }
}
