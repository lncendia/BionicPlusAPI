using Microsoft.AspNetCore.Mvc;

namespace PaymentService.Models.Robokassa
{
    public class ResultModel
    {
        public string OutSum { get; set; }
        public string InvId { get; set; }
        public string EMail { get; set; }
        public string SignatureValue { get; set; }
        public string Shp_planId { get; set; }
        public string Shp_userId { get; set; }
    }
}
