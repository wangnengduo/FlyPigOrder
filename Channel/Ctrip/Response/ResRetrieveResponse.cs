using Ctrip.Common;
using Ctrip.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 

namespace Ctrip.Response
{
    [Serializable]
    public class ResRetrieveResponse : BaseResponse
    {
        public HeaderInfo Header { get; set; }
        public List<HotelReservation> OrderList { get; set; }
        public ResRetrieveResponse()
        {
            this.OrderList = new List<HotelReservation>();
        }

        internal override void ParseXml()
        {
            if (base.ResponseXml != null)
            {
                bool suc = true;
                string result = string.Empty;
                XmlParse.SelectSingleNode(base.ResponseXml, "/", (c) =>
                {
                    result = c.InnerText;
                });
                if (string.IsNullOrEmpty(result))
                {
                    suc = false;
                }
                if (suc)
                {
                    base.ResultXML = new System.Xml.XmlDataDocument();
                    result = result.Replace("xmlns=\"http://www.opentravel.org/OTA/2003/05\"", "");
                    base.ResultXML.LoadXml(result);
                    //System.Xml.XmlNamespaceManager mgr = new System.Xml.XmlNamespaceManager(base.ResultXML.NameTable);
                    //mgr.AddNamespace("ns", "http://www.opentravel.org/OTA/2003/05");

                    XmlParse.SelectSingleNode(base.ResultXML, "/Response/Header", (c) =>
                    {
                        this.Header = new HeaderInfo();
                        this.Header.ShouldRecordPerformanceTime = XmlParse.GetAttribute(c, "ShouldRecordPerformanceTime").ToBoolean();
                        string t = XmlParse.GetAttribute(c, "Timestamp");
                        if (t.Count((ch) => ch == ':') == 3)
                        {
                            int i = t.LastIndexOf(':');
                            t = t.Remove(i) + "." + t.Substring(i + 1);
                        }
                        this.Header.Timestamp = t.ToDateTime();
                        this.Header.ReferenceID = XmlParse.GetAttribute(c, "ReferenceID");
                        this.Header.RecentlyTime = XmlParse.GetAttribute(c, "RecentlyTime").ToDateTime();
                        this.Header.AccessCount = XmlParse.GetAttribute(c, "AccessCount");
                        this.Header.CurrentCount = XmlParse.GetAttribute(c, "CurrentCount");
                        this.Header.ResetTime = XmlParse.GetAttribute(c, "ResetTime").ToDateTime();
                        this.Header.ResultCode = XmlParse.GetAttribute(c, "ResultCode");
                        if (this.Header.ResultCode == "Fail")
                        {
                            base.Error = new Error();
                            base.Error.Code = this.Header.ResultCode;
                            base.Error.Type = XmlParse.GetAttribute(c, "ResultNo");
                            base.Error.Message = XmlParse.GetAttribute(c, "ResultMsg");
                        }
                    });
                    XmlParse.SelectSingleNode(base.ResultXML, "/Response/HotelResponse/OTA_ResRetrieveRS/Errors/Error", (c) =>
                    {
                        suc = false;
                        base.Error = new Error();
                        base.Error.Type = XmlParse.GetAttribute(c, "Type");
                        base.Error.Code = XmlParse.GetAttribute(c, "Code");
                        base.Error.Message = c.InnerText;
                    });

                    if (suc)
                    {
                        XmlParse.SelectSingleNode(base.ResultXML, "/Response/HotelResponse/OTA_ResRetrieveRS/ReservationsList", node =>
                        {
                            XmlParse.SelectNodes(node, "HotelReservation", ons =>
                            {

                                foreach (System.Xml.XmlNode on in ons)
                                {
                                    HotelReservation order = new HotelReservation();
                                    order.CreateDateTime = XmlParse.GetAttribute(on, "CreateDateTime").ToDateTime();
                                    order.CreatorID = XmlParse.GetAttribute(on, "CreatorID");
                                    order.LastModifyDateTime = XmlParse.GetAttribute(on, "LastModifyDateTime").ToDateTime();
                                    order.LastModifierID = XmlParse.GetAttribute(on, "LastModifierID");
                                    order.ResStatus = XmlParse.GetAttribute(on, "ResStatus");
                                    order.OrderStatus = XmlParse.GetAttribute(on, "OrderStatus");
                                    XmlParse.SelectNodes(on, "UniqueID", uids =>
                                    {
                                        foreach (System.Xml.XmlNode uid in uids)
                                        {
                                            string type = XmlParse.GetAttribute(uid, "Type");
                                            string id = XmlParse.GetAttribute(uid, "ID");
                                            if (type == "501")
                                            {
                                                order.CtripOrderId = id;
                                                continue;
                                            }
                                        }
                                    });
                                    XmlParse.SelectSingleNode(on, "RoomStays/RoomStay", room =>
                                    {
                                        XmlParse.SelectSingleNode(room, "RoomTypes/RoomType", rt =>
                                        {
                                            order.NumberOfUnits = XmlParse.GetAttribute(rt, "NumberOfUnits").ToInt32();
                                            order.RoomTypeCode = XmlParse.GetAttribute(rt, "RoomTypeCode");
                                        });
                                        XmlParse.SelectSingleNode(room, "RatePlans/RatePlan", plan =>
                                        {
                                            order.RatePlanCode = XmlParse.GetAttribute(plan, "RatePlanCode");
                                            XmlParse.SelectNodes(plan, "AdditionalDetails/AdditionalDetail", adds =>
                                            {
                                                order.AdditionalDetails = new List<AdditionalDetail>();
                                                foreach (System.Xml.XmlNode add in adds)
                                                {
                                                    AdditionalDetail d = new AdditionalDetail();
                                                    d.Type = XmlParse.GetAttribute(add, "Type");
                                                    d.Code = XmlParse.GetAttribute(add, "Code");
                                                    XmlParse.SelectNodes(add, "DetailDescription/Text", tns =>
                                                    {
                                                        d.Texts = new List<string>();
                                                        foreach (System.Xml.XmlNode tn in tns)
                                                        {
                                                            d.Texts.Add(tn.InnerText);
                                                        }
                                                    });
                                                    order.AdditionalDetails.Add(d);
                                                }
                                            });
                                        });
                                        XmlParse.SelectSingleNode(room, "BasicPropertyInfo", info =>
                                        {
                                            XmlParse.SelectSingleNode(info, "Address", c =>
                                            {
                                                XmlParse.SelectSingleNode(c, "AddressLine", a => { order.Address = a.InnerText; });
                                                XmlParse.SelectSingleNode(c, "CityName", a => { order.CityName = a.InnerText; });
                                                XmlParse.SelectSingleNode(c, "PostalCode", a => { order.PostalCode = a.InnerText; });
                                            });
                                            XmlParse.SelectSingleNode(info, "ContactNumbers", con =>
                                            {
                                                XmlParse.SelectNodes(con, "ContactNumber", nums =>
                                                {
                                                    foreach (System.Xml.XmlNode num in nums)
                                                    {
                                                        string type = XmlParse.GetAttribute(num, "PhoneTechType");
                                                        string tel = XmlParse.GetAttribute(num, "PhoneNumber");
                                                        if (type == "Data" || type == "Voice")
                                                        {
                                                            order.Tel = tel;
                                                        }
                                                        else if (type == "Fax")
                                                        {
                                                            order.Fax = tel;
                                                        }
                                                    }
                                                });
                                            });
                                        });

                                    });
                                    XmlParse.SelectSingleNode(on, "BillingInstructionCode", bill =>
                                    {
                                        order.BillingCode = XmlParse.GetAttribute(bill, "BillingCode");
                                    });
                                    XmlParse.SelectSingleNode(on, "ResGuests/ResGuest", guest =>
                                    {
                                        order.ArrivalTime = XmlParse.GetAttribute(guest, "ArrivalTime");
                                        XmlParse.SelectSingleNode(guest, "Profiles/ProfileInfo/Profile/Customer", cus =>
                                        {
                                            XmlParse.SelectNodes(cus, "PersonName", gns =>
                                            {
                                                order.CustomerNames = new List<string>();
                                                foreach (System.Xml.XmlNode gn in gns)
                                                {
                                                    XmlParse.SelectSingleNode(gn, "Surname", name => { order.CustomerNames.Add(name.InnerText); });
                                                }
                                            });
                                            XmlParse.SelectSingleNode(cus, "ContactPerson", con =>
                                            {
                                                order.ContactType = XmlParse.GetAttribute(con, "ContactType");
                                                XmlParse.SelectSingleNode(con, "PersonName/Surname", name => { order.ContactName = name.InnerText; });
                                                XmlParse.SelectSingleNode(con, "Telephone", tel =>
                                                {
                                                    string type = XmlParse.GetAttribute(tel, "PhoneTechType");
                                                    order.ContactNumber = XmlParse.GetAttribute(tel, "PhoneNumber");
                                                    XmlParse.SelectSingleNode(tel, "Email", em => { order.Email = em.InnerText; });
                                                });
                                            });
                                        });
                                        XmlParse.SelectSingleNode(guest, "TPA_Extensions/LateArrivalTime", time => { order.LateArrivalTime = time.InnerText; });

                                    });
                                    XmlParse.SelectSingleNode(on, "ResGlobalInfo", info =>
                                    {
                                        XmlParse.SelectSingleNode(info, "GuestCounts", n =>
                                        {
                                            order.IsPerRoom = XmlParse.GetAttribute(n, "IsPerRoom").ToBoolean();
                                            XmlParse.SelectSingleNode(n, "GuestCount", count =>
                                            {
                                                order.GuestCount = XmlParse.GetAttribute(count, "Count").ToInt32();
                                            });
                                        });
                                        XmlParse.SelectSingleNode(info, "TimeSpan", tn =>
                                        {
                                            order.StartDate = XmlParse.GetAttribute(tn, "Start").ToDateTime();
                                            order.EndDate = XmlParse.GetAttribute(tn, "End").ToDateTime();
                                        });
                                        XmlParse.SelectSingleNode(info, "SpecialRequests/SpecialRequest/Text", text => { order.SpecialRequest = text.InnerText; });
                                        XmlParse.SelectSingleNode(info, "Total", total =>
                                        {
                                            order.AmountBeforeTax = XmlParse.GetAttribute(total, "AmountBeforeTax").ToDecimal();
                                            order.CurrencyCode = XmlParse.GetAttribute(total, "CurrencyCode");
                                            XmlParse.SelectSingleNode(total, "TPA_Extensions", cur =>
                                            {
                                                XmlParse.SelectSingleNode(cur, "OtherCurrency/AmountPercentType", c =>
                                                {
                                                    order.OtherAmount = XmlParse.GetAttribute(c, "Amount").ToDecimal();
                                                    order.OtherCurrencyCode = XmlParse.GetAttribute(c, "CurrencyCode");
                                                });
                                                XmlParse.SelectSingleNode(cur, "NoShowPaid/AmountPercentType", c =>
                                                {
                                                    order.NoShowAmount = XmlParse.GetAttribute(c, "Amount").ToDecimal();
                                                    order.NoShowCurrencyCode = XmlParse.GetAttribute(c, "CurrencyCode");
                                                });
                                            });
                                        });


                                        XmlParse.SelectNodes(info, "HotelReservationIDs/HotelReservationID", ids =>
                                        {
                                            order.HotelReservationIDs = new List<HotelReservationID>();
                                            foreach (System.Xml.XmlNode idn in ids)
                                            {
                                                HotelReservationID oid = new HotelReservationID();
                                                oid.ResID_Type = XmlParse.GetAttribute(idn, "ResID_Type");
                                                oid.ResID_Value = XmlParse.GetAttribute(idn, "ResID_Value");
                                                oid.CancelReason = XmlParse.GetAttribute(idn, "CancelReason");
                                                oid.CancellationDate = XmlParse.GetAttribute(idn, "CancellationDate").ToDateTime();
                                                order.HotelReservationIDs.Add(oid);
                                            }
                                        });

                                    });


                                    XmlParse.SelectSingleNode(on, "TPA_Extensions/DayNightAudit", c => { order.DayNightAudit = c.InnerText; });

                                    XmlParse.SelectSingleNode(on, "TPA_Extensions/OrderTags", info =>
                                    {
                                        order.OrderTags = new List<OrderTag>();

                                        XmlParse.SelectSingleNode(info, "OrderTag", n =>
                                        {
                                            OrderTag tag = new OrderTag();
                                            tag.Code = XmlParse.GetAttribute(n, "Code").ToString();
                                            tag.Value = XmlParse.GetAttribute(n, "Value").ToString();
                                            order.OrderTags.Add(tag);
                                        });

                                    });

                                    this.OrderList.Add(order);
                                }

                            });
                        });
                    }
                }
            }

        }
    }
}
