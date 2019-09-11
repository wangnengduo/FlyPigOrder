using Ctrip.Common;
using Ctrip.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ctrip.Request
{
    public class HotelAvailRequest : IBaseRequest<HotelAvailReponse>
    {
        [RequsetNode("OTA_HotelAvailRQ")]
        public OTA_HotelAvailRQ OTA_HotelAvailRQ { get; set; }

        public string Method => "OTA_HotelAvail";

        public string Url => "/Hotel/OTA_HotelAvail.asmx";

        public HotelAvailRequest() { this.OTA_HotelAvailRQ = new OTA_HotelAvailRQ(); }
    }
    public class OTA_HotelAvailRQ
    {
        [RequsetNode("OTA_HotelAvailRQ", "Version", false)]
        public string Version { get; set; }
        [RequsetNode("OTA_HotelAvailRQ", "TimeStamp", false)]
        public string TimeStamp { get; set; }
        [RequsetNode("AvailRequestSegments")]
        public AvailRequestSegments AvailRequestSegments { get; set; }


        public OTA_HotelAvailRQ()
        {
            this.Version = "2.0";   //预付房型可定检查使用2.0版本
            this.TimeStamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff+08:00");
            this.AvailRequestSegments = new AvailRequestSegments();
        }
    }
    public class AvailRequestSegments
    {
        [RequsetNode("AvailRequestSegment")]
        public List<AvailRequestSegment> AvailRequestSegmentList { get; set; }
        public AvailRequestSegments()
        {
            this.AvailRequestSegmentList = new List<AvailRequestSegment>();
        }
    }
    public class AvailRequestSegment
    {
        [RequsetNode("HotelSearchCriteria")]
        public HotelSearchCriteria HotelSearchCriteria { get; set; }

        public AvailRequestSegment() { this.HotelSearchCriteria = new HotelSearchCriteria(); }
    }
    public class HotelSearchCriteria
    {
        [RequsetNode("Criterion")]
        public List<Criterion_Avail> CriterionList { get; set; }
        public HotelSearchCriteria() { this.CriterionList = new List<Criterion_Avail>(); }
    }
    public class Criterion_Avail
    {
        [RequsetNode("HotelRef", "HotelCode", false)]
        public string HotelCode { get; set; }
        /// <summary>
        /// 入住日期
        /// </summary>
        [RequsetNode("StayDateRange", "Start", false)]
        public string StartDate { get; set; }
        /// <summary>
        /// 离店日期
        /// </summary>
        [RequsetNode("StayDateRange", "End", false)]
        public string EndDate { get; set; }
        /// <summary>
        /// 指定价格计划列表
        /// </summary>
        [RequsetNode("RatePlanCandidates")]
        public RatePlanCandidates RatePlanCandidates { get; set; }
        [RequsetNode("RoomStayCandidates")]
        public RoomStayCandidates RoomStayCandidates { get; set; }
        [RequsetNode("TPA_Extensions")]
        public TPA_Extensions_LateArrivalTime TPA_Extensions { get; set; }
        public Criterion_Avail()
        {
            this.RatePlanCandidates = new RatePlanCandidates();
            this.RoomStayCandidates = new RoomStayCandidates();
            this.TPA_Extensions = new TPA_Extensions_LateArrivalTime();
        }
    }
    public class RatePlanCandidates
    {
        [RequsetNode("RatePlanCandidate")]
        public List<RatePlanCandidate_Avail> RatePlanCandidateList { get; set; }
        public RatePlanCandidates() { this.RatePlanCandidateList = new List<RatePlanCandidate_Avail>(); }
    }
    public class RatePlanCandidate_Avail
    {
        [RequsetNode("RatePlanCandidate", "RatePlanCode", false)]
        public string RatePlanCode { get; set; }
        [RequsetNode("RatePlanCandidate", "RatePlanCategory", true)]
        public string RatePlanCategory { get; set; }
        [RequsetNode("RatePlanCandidate", "RatePlanID", true)]
        public string RatePlanID { get; set; }
    }
    public class RoomStayCandidates
    {
        [RequsetNode("RoomStayCandidate")]
        public List<RoomStayCandidate> RoomStayCandidateList { get; set; }
        public RoomStayCandidates() { this.RoomStayCandidateList = new List<RoomStayCandidate>(); }
    }
    public class RoomStayCandidate
    {
        [RequsetNode("RoomStayCandidate", "Quantity", false)]
        public int Quantity { get; set; }
        [RequsetNode("RoomStayCandidate", "BookingCode", true)]
        public string BookingCode { get; set; }
        [RequsetNode("GuestCounts")]
        public GuestCounts GuestCounts { get; set; }
        public RoomStayCandidate() { this.GuestCounts = new GuestCounts(); }
    }
    public class GuestCounts
    {
        [RequsetNode("GuestCounts", "IsPerRoom", true)]
        public bool IsPerRoom { get; set; }
        [RequsetNode("GuestCount")]
        public List<GuestCount> GuestCountList { get; set; }
        public GuestCounts() { this.IsPerRoom = false; this.GuestCountList = new List<GuestCount>(); }
    }
    public class GuestCount
    {
        [RequsetNode("GuestCount", "Count", false)]
        public int Count { get; set; }
    }
    public class TPA_Extensions_LateArrivalTime
    {
        /// <summary>
        /// 最晚到店时间，有可能最晚到店日期为第二天凌晨，格式为 yyyy-MM-dd hh:mm:ss，必填
        /// </summary>
        [RequsetNode("LateArrivalTime", false)]
        public string LateArrivalTime { get; set; }
    }
}
