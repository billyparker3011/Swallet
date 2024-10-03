using System.Xml.Serialization;

namespace Lottery.Core.Partners.Models.Bti
{
    [XmlRoot(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class ValidationTokenDataModel
    {
        [XmlElement(ElementName = "status_code")]
        public string StatusCode { get; set; }

        [XmlElement(ElementName = "currency")]
        public string Currency { get; set; }

        [XmlElement(ElementName = "member_id")]
        public int MemberId { get; set; }

        [XmlElement(ElementName = "member_name")]
        public string MemberName { get; set; }

        [XmlElement(ElementName = "language")]
        public string Language { get; set; }

        [XmlElement(ElementName = "Sport")]
        public Sport Sport { get; set; }
    }

    public class Sport
    {
        [XmlAttribute(AttributeName = "MinBet")]
        public decimal MinBet { get; set; }

        [XmlAttribute(AttributeName = "MaxBet")]
        public decimal MaxBet { get; set; }

        [XmlElement(ElementName = "Branch")]
        public List<Branch> Branches { get; set; }
    }

    public class Branch
    {
        [XmlAttribute(AttributeName = "ID")]
        public long Id { get; set; }

        [XmlAttribute(AttributeName = "MinBet")]
        public decimal MinBet { get; set; }

        [XmlAttribute(AttributeName = "MaxBet")]
        public decimal MaxBet { get; set; }
    }
}
